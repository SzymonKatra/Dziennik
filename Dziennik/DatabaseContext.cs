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
            ExceptionIfNotExistsAlways,
            ExceptionIfNotExistsNotNull,
        }
        public enum AssingRelationsGlobalCollectionCheckingModes
        {
            Disable,
            ExceptionIfNotExistsAlways,
            ExceptionIfNotExistsNotNull,
            AddToGlobalIfNotExists,
        }
        private class CachedProperty
        {
            public PropertyInfo Info { get; set; }
            public object[] Attributes { get; set; }
            public bool IgnoreFromSearchRelations { get; set; }
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

            public string Name { get; set; }
            public IEnumerable<IModelExposable<ModelBase>> Collection { get; set; }
            public List<RelationProperties> Properties { get; set; }

            public RelationPair()
            {
                Properties = new List<RelationProperties>();
            }
        }
        private class InversePropertyOwnerPairs
        {
            /// <summary>
            /// OBE - ObservableCollectionExtended
            /// </summary>
            public string SubscribeOwnerOBEMethodName { get; set; }
            public string ChildOwnerPropertyName { get; set; }
            public IModelExposable<ModelBase> Owner { get; set; }

            private IModelExposable<ModelBase> m_property;
            public IModelExposable<ModelBase> Property
            {
                get { return m_property; }
                set { m_property = value; m_collection = null; }
            }

            private IEnumerable<IModelExposable<ModelBase>> m_collection;
            public IEnumerable<IModelExposable<ModelBase>> Collection
            {
                get { return m_collection; }
                set { m_collection = value; m_property = null; }
            }

            public bool IsCollectionValid
            {
                get { return m_property == null; }
            }
        }

        #region Properties
        private RestoreRelationsGlobalCollectionCheckingModes m_restoreRelationsGlobalCollectionChecking = RestoreRelationsGlobalCollectionCheckingModes.ExceptionIfNotExistsAlways;
        public RestoreRelationsGlobalCollectionCheckingModes RestoreRelationsGlobalCollectionChecking
        {
            get { return m_restoreRelationsGlobalCollectionChecking; }
            set { m_restoreRelationsGlobalCollectionChecking = value; }
        }

        private AssingRelationsGlobalCollectionCheckingModes m_assignRelationsGlobalCollectionChecking = AssingRelationsGlobalCollectionCheckingModes.ExceptionIfNotExistsAlways;
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
        private List<RelationPair> m_relations = new List<RelationPair>();
        private List<InversePropertyOwnerPairs> m_inverseProperties = new List<InversePropertyOwnerPairs>();
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

            ClearAndReflectViewModel(m_viewModel);
            RestoreRelations();
            AssignInverseProperties();
        }
        protected void Save(Stream stream)
        {
            ClearAndReflectViewModel(m_viewModel);
            AssignIdentifiers(m_viewModel.Model);
            AssignRelations();

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

        protected void RestoreRelations()
        {
            foreach (var relation in m_relations)
            {
                foreach (var relationProperties in relation.Properties)
                {
                    ulong? val = (ulong?)relationProperties.Owner.Model.GetType().GetProperty(relationProperties.ModelPropertyIdName).GetValue(relationProperties.Owner.Model, null);
                    if (val == null && m_restoreRelationsGlobalCollectionChecking == RestoreRelationsGlobalCollectionCheckingModes.ExceptionIfNotExistsNotNull)
                        continue;
                    
                    IModelExposable<ModelBase> result = relation.Collection.FirstOrDefault(x => x.Model.Id == val);
                    if (m_restoreRelationsGlobalCollectionChecking == RestoreRelationsGlobalCollectionCheckingModes.ExceptionIfNotExistsAlways && result == null)
                    {
                        throw new InvalidOperationException("Specified ViewModel not found in global collection. Relationship: " + relation.Name);
                    }

                    relationProperties.RelatedProperty.SetValue(relationProperties.Owner, result, null);
                }
            }
        }
        protected void AssignRelations()
        {
            foreach (var relation in m_relations)
            {
                foreach (var relationProperties in relation.Properties)
                {
                    if (m_assignRelationsGlobalCollectionChecking != AssingRelationsGlobalCollectionCheckingModes.Disable)
                    {
                        if ((m_assignRelationsGlobalCollectionChecking == AssingRelationsGlobalCollectionCheckingModes.ExceptionIfNotExistsNotNull && relationProperties.Related != null) && !relation.Collection.Contains(relationProperties.Related))
                        {
                            if (m_assignRelationsGlobalCollectionChecking == AssingRelationsGlobalCollectionCheckingModes.AddToGlobalIfNotExists)
                            {
                                if (relation.Collection is ICollection<IModelExposable<ModelBase>>)
                                {
                                    ((ICollection<IModelExposable<ModelBase>>)relation.Collection).Add(relationProperties.Related);
                                }
                                else throw new InvalidOperationException("Mode is AddToGlobalIfNotExists but global collection is not type of ICollection<IModelExposable<ModelBase>>");
                            }
                            else // must be ExceptionIfNotExistsAlways
                            {
                                throw new InvalidOperationException("Global collection doesn't contains related ViewModel in relationship: " + relation.Name);
                            }
                        }
                    }

                    PropertyInfo modelIdPropertyInfo = relationProperties.Owner.Model.GetType().GetProperty(relationProperties.ModelPropertyIdName);
                    modelIdPropertyInfo.SetValue(relationProperties.Owner.Model, (relationProperties.Related == null ? null : relationProperties.Related.Model.Id), null);
                }
            }
        }
        protected void AssignInverseProperties()
        {
            foreach (var inverseProperty in m_inverseProperties)
            {
                if(inverseProperty.IsCollectionValid)
                {
                    foreach (var item in inverseProperty.Collection)
                    {
                        PropertyInfo ownerPropertyName = item.GetType().GetProperty(inverseProperty.ChildOwnerPropertyName);
                        ownerPropertyName.SetValue(item, inverseProperty.Owner, null);
                    }
                }
                else
                {
                    PropertyInfo ownerPropertyName = inverseProperty.Property.GetType().GetProperty(inverseProperty.ChildOwnerPropertyName);
                    ownerPropertyName.SetValue(inverseProperty.Property, inverseProperty.Owner, null);
                }

                inverseProperty.Owner.GetType().GetMethod(inverseProperty.SubscribeOwnerOBEMethodName, BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance).Invoke(inverseProperty.Owner, null);
            }
        }

        private void ClearAndReflectViewModel(IModelExposable<ModelBase> viewModel)
        {
            m_relations.Clear();
            m_inverseProperties = new List<InversePropertyOwnerPairs>();
            ReflectViewModel(viewModel, new Dictionary<string, RelationPair>());
        }
        private void ReflectViewModel(IModelExposable<ModelBase> viewModel, Dictionary<string, RelationPair> availableRelationPairs)
        {
            Type type = viewModel.GetType();

            CachedProperty[] properties = GetCachedProperties(type);

            Dictionary<string, RelationPair> addedRelationPairs = new Dictionary<string, RelationPair>();

            //first search in properties and find collections
            foreach (CachedProperty property in properties)
            {
                if (property.IgnoreFromSearchRelations) continue;

                object propVal = property.Info.GetValue(viewModel, null);
                object[] attributes = property.Attributes;

                foreach (object attrib in attributes)
                {
                    if (attrib is DatabaseRelationPropertyAttribute)
                    {
                        DatabaseRelationPropertyAttribute propAttribVal = (DatabaseRelationPropertyAttribute)attrib;

                        if (!availableRelationPairs.ContainsKey(propAttribVal.RelationName)) throw new InvalidOperationException("Specified relation not found: " + propAttribVal.RelationName);

                        availableRelationPairs[propAttribVal.RelationName].Properties.Add(new RelationPair.RelationProperties(viewModel, (IModelExposable<ModelBase>)propVal, property.Info, propAttribVal.ModelPropertyIdName));
                    }
                    else if (attrib is DatabaseRelationCollectionAttribute)
                    {
                        DatabaseRelationCollectionAttribute collAttribVal = (DatabaseRelationCollectionAttribute)attrib;

                        if (availableRelationPairs.ContainsKey(collAttribVal.RelationName)) throw new InvalidOperationException("Found another yet relation with the same name: " + collAttribVal.RelationName);
                        addedRelationPairs.Add(collAttribVal.RelationName, new RelationPair()
                        {
                            Name = collAttribVal.RelationName,
                            Collection = (IEnumerable<IModelExposable<ModelBase>>)propVal
                        });
                    }
                    else if (attrib is DatabaseInversePropertyOwnerAttribute)
                    {
                        DatabaseInversePropertyOwnerAttribute ipAttribVal = (DatabaseInversePropertyOwnerAttribute)attrib;
                        InversePropertyOwnerPairs pair = new InversePropertyOwnerPairs();
                        pair.ChildOwnerPropertyName = ipAttribVal.OwnerPropertyInChildName;
                        pair.Owner = viewModel;
                        pair.SubscribeOwnerOBEMethodName = ipAttribVal.SubscribeObservableCollectionExtendedMethodName;
                        if (propVal is IEnumerable<IModelExposable<ModelBase>>)
                        {
                            pair.Collection = (IEnumerable<IModelExposable<ModelBase>>)propVal;
                        }
                        else if (propVal is IModelExposable<ModelBase>)
                        {
                            pair.Property = (IModelExposable<ModelBase>)propVal;
                        }
                        else throw new InvalidOperationException("Inverse property owner must be of type IModelExposable<ModelBase> or IEnumerable<IModelExposable<ModelBase>>");
                        m_inverseProperties.Add(pair);
                    }
                }
            }

            //second add found collections to cache
            foreach (var kvp in addedRelationPairs)
            {
                availableRelationPairs.Add(kvp.Key, kvp.Value);
            }

            //third search inside rest properties
            foreach (CachedProperty property in properties)
            {
                if (property.IgnoreFromSearchRelations) continue;
                
                object propVal = property.Info.GetValue(viewModel, null);

                if (propVal is IEnumerable<IModelExposable<ModelBase>>)
                {
                    if (propVal is string) continue; // to prevent foreach'ing string

                    IEnumerable<IModelExposable<ModelBase>> coll = (IEnumerable<IModelExposable<ModelBase>>)propVal;
                    foreach (IModelExposable<ModelBase> item in coll)
                    {
                        ReflectViewModel(item, availableRelationPairs);
                    }
                }
                else if (propVal is IModelExposable<ModelBase>) ReflectViewModel((IModelExposable<ModelBase>)propVal, availableRelationPairs);
            }

            //fourth remove collections from cache and add them to global relations list
            foreach (var toRemoveKvp in addedRelationPairs)
            {
                m_relations.Add(toRemoveKvp.Value);
                availableRelationPairs.Remove(toRemoveKvp.Key);
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
                    cachedProperties[i].IgnoreFromSearchRelations = false;
                    foreach (var attrib in cachedProperties[i].Attributes)
                    {
                        if(attrib is DatabaseIgnoreSearchRelationsAttribute)
                        {
                            cachedProperties[i].IgnoreFromSearchRelations = true;
                            break;
                        }
                    }
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