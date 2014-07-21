using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Dziennik.Model;
using System.IO;
using fastJSON;

namespace Dziennik
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Base ViewModel type</typeparam>
    public class DatabaseContext<VM, M> where VM : IModelExposable<M> where M : ModelBase
    {
        public enum RestoreRelationsGlobalCollectionCheckingModes
        {
            Disable,
            ExceptionIfNotExists,
        }
        public enum AssingRelationsGlobalCollectionCheckingModes
        {
            Disable,
            ExceptionIfNotExists,
            AddToGlobalIfNotExists,
        }
        private class CachedProperty
        {
            public PropertyInfo Info { get; set; }
            public object[] Attributes { get; set; }
        }
        private class RelationPair
        {
            public class RelationProperties
            {
                public IModelExposable<ModelBase> Owner { get; set; } // view model to which model assign xxxId property
                public IModelExposable<ModelBase> Related { get; set; } // related view model
                public PropertyInfo RelatedProperty { get; set; }
                public string ModelPropertyIdName { get; set; }

                public RelationProperties()
                    : this(null, null, null, null)
                {
                }
                public RelationProperties(IModelExposable<ModelBase> owner, IModelExposable<ModelBase> related, PropertyInfo relatedProperty, string modelPropertyIdName)
                {
                    Owner = owner;
                    Related = related;
                    RelatedProperty = relatedProperty;
                    ModelPropertyIdName = modelPropertyIdName;
                }
            }

            public IEnumerable<IModelExposable<ModelBase>> Collection { get; set; }
            public List<RelationProperties> Properties { get; set; }

            public RelationPair()
            {
                Properties = new List<RelationProperties>();
            }
        }

        #region Properties
        private RestoreRelationsGlobalCollectionCheckingModes m_restoreRelationsGlobalCollectionChecking = RestoreRelationsGlobalCollectionCheckingModes.ExceptionIfNotExists;
        public RestoreRelationsGlobalCollectionCheckingModes RestoreRelationsGlobalCollectionChecking
        {
            get { return m_restoreRelationsGlobalCollectionChecking; }
            set { m_restoreRelationsGlobalCollectionChecking = value; }
        }

        private AssingRelationsGlobalCollectionCheckingModes m_assignRelationsGlobalCollectionChecking = AssingRelationsGlobalCollectionCheckingModes.ExceptionIfNotExists;
        public AssingRelationsGlobalCollectionCheckingModes AssignRelationsGlobalCollectionChecking
        {
            get { return m_assignRelationsGlobalCollectionChecking; }
            set { m_assignRelationsGlobalCollectionChecking = value; }
        }

        private VM m_viewModel;
        public VM ViewModel
        {
            get { return m_viewModel; }
            set { m_viewModel = value; }
        }

        private ulong m_currentId = 1;
        private Dictionary<string, RelationPair> m_relations = new Dictionary<string, RelationPair>();
        private Dictionary<Type, CachedProperty[]> m_reflectionCache = new Dictionary<Type, CachedProperty[]>();
        #endregion

        #region Methods
        public void AssignId(ModelBase model)
        {
            model.Id = m_currentId;
            m_currentId++; //in new line for clarity
        }
        public void AssignId(IModelExposable<ModelBase> viewModel)
        {
            AssignId(viewModel.Model);
        }

        protected void Load(Stream stream)
        {
            Load(stream, (m) => { return (VM)Activator.CreateInstance(typeof(VM), m); });
        }
        protected void Load(Stream stream, Func<M, VM> customCreator)
        {
            byte[] buffer = new byte[sizeof(ulong)];

            stream.Read(buffer, 0, sizeof(ulong)); // current id
            m_currentId = BitConverter.ToUInt64(buffer, 0);

            stream.Read(buffer, 0, sizeof(int)); // json data length
            int len = BitConverter.ToInt32(buffer, 0);

            buffer = new byte[len];
            stream.Read(buffer, 0, len);

            string json = Encoding.UTF8.GetString(buffer);

            m_viewModel = customCreator((M)JSON.ToObject(json, typeof(M)));

            RestoreRelations(m_viewModel);
        }
        protected void Save(Stream stream)
        {
            AssignIdentifiers(m_viewModel.Model);
            AssignRelations(m_viewModel);

            stream.Write(BitConverter.GetBytes(m_currentId), 0, sizeof(ulong)); // save current id
            byte[] jsonBytes = Encoding.UTF8.GetBytes(JSON.ToJSON(m_viewModel.Model));
            stream.Write(BitConverter.GetBytes(jsonBytes.Length), 0, sizeof(int)); // save json data length
            stream.Write(jsonBytes, 0, jsonBytes.Length); // save json data
        }

        protected void AssignIdentifiers(ModelBase model)
        {
            if (model.Id == null) AssignId(model);

            Type type = model.GetType();

            CachedProperty[] properties = GetCachedProperties(type);

            foreach (CachedProperty property in properties)
            {
                object propVal = property.Info.GetValue(model, null);

                if (propVal is IEnumerable)
                {
                    if (propVal is string) continue; // to prevent foreach'ing string

                    IEnumerable coll = (IEnumerable)propVal;
                    foreach (object item in coll)
                    {
                        if (item is ModelBase) AssignIdentifiers((ModelBase)item);
                    }
                }
                else if (propVal is ModelBase) AssignIdentifiers((ModelBase)propVal);
            }
        }

        protected void RestoreRelations(IModelExposable<ModelBase> viewModel)
        {
            m_relations.Clear();
            MakeRelationPairs(viewModel);

            foreach (var kvp in m_relations)
            {
                foreach (var relationProperties in kvp.Value.Properties)
                {
                    IModelExposable<ModelBase> result = kvp.Value.Collection.FirstOrDefault(x => x.Model.Id == (ulong?)relationProperties.Owner.Model.GetType().GetProperty(relationProperties.ModelPropertyIdName).GetValue(relationProperties.Owner.Model, null));
                    if (m_restoreRelationsGlobalCollectionChecking == RestoreRelationsGlobalCollectionCheckingModes.ExceptionIfNotExists && result == null)
                    {
                        throw new InvalidOperationException("Specified ViewModel not found in global collection. Relationship: " + kvp.Key);
                    }

                    relationProperties.RelatedProperty.SetValue(relationProperties.Owner, result, null);
                }
            }
        }
        protected void AssignRelations(IModelExposable<ModelBase> viewModel)
        {
            m_relations.Clear();
            MakeRelationPairs(viewModel);

            foreach (var kvp in m_relations)
            {
                foreach (var relationProperties in kvp.Value.Properties)
                {
                    if (m_assignRelationsGlobalCollectionChecking != AssingRelationsGlobalCollectionCheckingModes.Disable)
                    {
                        if (!kvp.Value.Collection.Contains(relationProperties.Related))
                        {
                            if (m_assignRelationsGlobalCollectionChecking == AssingRelationsGlobalCollectionCheckingModes.AddToGlobalIfNotExists)
                            {
                                if (kvp.Value.Collection is ICollection<IModelExposable<ModelBase>>)
                                {
                                    ((ICollection<IModelExposable<ModelBase>>)kvp.Value.Collection).Add(relationProperties.Related);
                                }
                                else throw new InvalidOperationException("Mode is AddToGlobalIfNotExists but global collection is not type of ICollection<IModelExposable<ModelBase>>");
                            }
                            else // must be ExceptionIfNotExists
                            {
                                throw new InvalidOperationException("Global collection doesn't contains related ViewModel in relationship: " + kvp.Key);
                            }
                        }
                    }

                    PropertyInfo modelIdPropertyInfo = relationProperties.Owner.Model.GetType().GetProperty(relationProperties.ModelPropertyIdName);
                    modelIdPropertyInfo.SetValue(relationProperties.Owner.Model, relationProperties.Related.Model.Id, null);
                }
            }
        }

        private void MakeRelationPairs(IModelExposable<ModelBase> viewModel, Dictionary<string, IEnumerable> availableGlobalCollections)
        {
            Type type = viewModel.GetType();

            CachedProperty[] properties = GetCachedProperties(type);

            foreach (CachedProperty property in properties)
            {
                object propVal = property.Info.GetValue(viewModel, null);
                object[] attributes = property.Attributes;

                foreach (object attrib in attributes)
                {
                    if (attrib is DatabaseRelationAttribute)
                    {
                        DatabaseRelationAttribute baseAttribVal = (DatabaseRelationAttribute)attrib;

                        if (!m_relations.ContainsKey(baseAttribVal.RelationName)) m_relations.Add(baseAttribVal.RelationName, new RelationPair());
                        RelationPair pair = m_relations[baseAttribVal.RelationName];

                        if (baseAttribVal is DatabaseRelationPropertyAttribute)
                        {
                            DatabaseRelationPropertyAttribute propAttribVal = (DatabaseRelationPropertyAttribute)baseAttribVal;
                            pair.Properties.Add(new RelationPair.RelationProperties(viewModel, (IModelExposable<ModelBase>)propVal, property.Info, propAttribVal.ModelPropertyIdName));
                        }
                        else if (baseAttribVal is DatabaseRelationCollectionAttribute)
                        {
                            if (pair.Collection != null) throw new InvalidOperationException("Relation " + baseAttribVal.RelationName + " already has paired collection");

                            DatabaseRelationCollectionAttribute collAttribVal = (DatabaseRelationCollectionAttribute)baseAttribVal;
                            pair.Collection = (IEnumerable<IModelExposable<ModelBase>>)propVal;
                        }
                    }
                }
                //TODO: add to list collecion
                if (propVal is IEnumerable)
                {
                    if (propVal is string) continue; // to prevent foreach'ing string

                    IEnumerable coll = (IEnumerable)propVal;
                    foreach (object item in coll)
                    {
                        if (item is IModelExposable<ModelBase>) MakeRelationPairs((IModelExposable<ModelBase>)item);
                    }
                }
                else if (propVal is IModelExposable<ModelBase>) MakeRelationPairs((IModelExposable<ModelBase>)propVal);
                //TODO: remove from list collection
            }
        }

        private CachedProperty[] GetCachedProperties(Type type)
        {
            if (!m_reflectionCache.ContainsKey(type))
            {
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                CachedProperty[] cachedProperties = new CachedProperty[properties.Length];

                for (int i = 0; i < properties.Length; i++)
                {
                    cachedProperties[i] = new CachedProperty();
                    cachedProperties[i].Info = properties[i];
                    cachedProperties[i].Attributes = properties[i].GetCustomAttributes(false);
                }

                m_reflectionCache.Add(type, cachedProperties);
                return cachedProperties;
            }

            return m_reflectionCache[type];
        }
        public void ClearReflectionCache()
        {
            m_reflectionCache.Clear();
        }
        #endregion
    }
}

/*
 * FILE STRUCTURE:
 * 8 bytes - [Current Id]
 * 4 bytes - [Length of JSON]
 * [Length of JSON] bytes - json data
*/