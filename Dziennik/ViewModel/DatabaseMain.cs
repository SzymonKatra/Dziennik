using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Dziennik.Model;
using fastJSON;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace Dziennik.ViewModel
{
    public class DatabaseMain
    {
        private string m_path;
        public string Path
        {
            get { return m_path; }
            set { m_path = value; }
        }

        private bool m_globalRelationsCollectionChecking = true;
        public bool GlobalRelationsCollectionChecking
        {
            get { return m_globalRelationsCollectionChecking; }
            set { m_globalRelationsCollectionChecking = value; }
        }

        private ulong m_currentId = 1;
        public void AssignId(ModelBase model)
        {
            model.Id = m_currentId;
            m_currentId++; //in new line for clarity
        }
        public void AssignId(IModelExposable<ModelBase> viewModel)
        {
            AssignId(viewModel.Model);
        }

        public static DatabaseMain Load(string path)
        {
            DatabaseMain database = new DatabaseMain();

            database.m_path = path;

            using (FileStream stream = new FileStream(database.m_path, FileMode.OpenOrCreate))
            {
                byte[] buffer = new byte[sizeof(ulong)];

                stream.Read(buffer, 0, sizeof(ulong)); // current id
                database.m_currentId = BitConverter.ToUInt64(buffer, 0);

                stream.Read(buffer, 0, sizeof(int)); // json data length
                int len = BitConverter.ToInt32(buffer, 0);

                buffer = new byte[len];
                stream.Read(buffer, 0, len);

                string json = Encoding.UTF8.GetString(buffer);
                database.SchoolClass = new SchoolClassViewModel((SchoolClass)JSON.ToObject(json, typeof(SchoolClass)));
            }

            database.RestoreRelations(database.SchoolClass);

            //foreach (SchoolGroupViewModel group in database.SchoolClass.Groups)
            //{
            //    foreach (StudentInGroupViewModel student in group.Students)
            //    {
            //        if (student.Model.GlobalStudentId != null)
            //        {
            //            student.GlobalStudent = database.SchoolClass.Students.FirstOrDefault(x => x.Model.Id == student.Model.GlobalStudentId);
            //            Debug.Assert(student.GlobalStudent == null, "DatabaseMain.Load - GlobalStudent id not found");
            //        }
            //    }
            //}

            return database;
        }
        public void Save()
        {
            AssignIdentifiers(SchoolClass.Model);
            AssignRelations(SchoolClass);
            //foreach (SchoolGroupViewModel group in SchoolClass.Groups)
            //{
            //    foreach (StudentInGroupViewModel student in group.Students)
            //    {
            //        student.Model.GlobalStudentId = (student.GlobalStudent == null ? null : student.GlobalStudent.Model.Id);
            //    }
            //}

            using (FileStream stream = new FileStream(m_path, FileMode.OpenOrCreate))
            {
                stream.Write(BitConverter.GetBytes(m_currentId), 0, sizeof(ulong)); // save current id
                byte[] jsonBytes = Encoding.UTF8.GetBytes(JSON.ToJSON(SchoolClass.Model));
                stream.Write(BitConverter.GetBytes(jsonBytes.Length), 0, sizeof(int)); // save json data length
                stream.Write(jsonBytes, 0, jsonBytes.Length); // save json data

                stream.Close();
            }
        }

        protected void AssignIdentifiers(object modelObj)
        {
            if (!(modelObj is ModelBase)) return;
            ModelBase model = (ModelBase)modelObj;

            if (model.Id == null) AssignId(model);

            Type type = model.GetType();

            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.GetGetMethod().IsStatic) continue;

                object propVal = property.GetValue(model, null);

                if (propVal is IEnumerable)
                {
                    IEnumerable coll = (IEnumerable)propVal;
                    foreach (object item in coll)
                    {
                        AssignIdentifiers(item);
                    }
                }
                else AssignIdentifiers(propVal);
            }
        }

        private class RelationPair
        {
            public class ViewModelsPair
            {
                public IModelExposable<ModelBase> Owner { get; set; } // view model to which model assign xxxId property
                public IModelExposable<ModelBase> Related { get; set; } // related view model
                public PropertyInfo RelatedProperty { get; set; }

                public ViewModelsPair()
                    : this(null, null, null)
                {
                }
                public ViewModelsPair(IModelExposable<ModelBase> owner, IModelExposable<ModelBase> related, PropertyInfo relatedProperty)
                {
                    Owner = owner;
                    Related = related;
                    RelatedProperty = relatedProperty;
                }
            }

            public IEnumerable<IModelExposable<ModelBase>> Collection { get; set; }
            public List<ViewModelsPair> Properties { get; set; }
            public string ModelPropertyIdName { get; set; }

            public RelationPair()
            {
                Properties = new List<ViewModelsPair>();
            }
        }
        private Dictionary<string, RelationPair> m_relations = new Dictionary<string, RelationPair>();
        protected void RestoreRelations(object viewModelObj)
        {
            m_relations.Clear();
            MakeRelationPairs(viewModelObj);

            foreach (var kvp in m_relations)
            {
                foreach (var viewModels in kvp.Value.Properties)
                {
                    IModelExposable<ModelBase> result = kvp.Value.Collection.FirstOrDefault(x => x.Model.Id == (ulong?)viewModels.Owner.Model.GetType().GetProperty(kvp.Value.ModelPropertyIdName).GetValue(viewModels.Owner.Model, null));
                    if (m_globalRelationsCollectionChecking && result == null) throw new InvalidOperationException("Specified ViewModel not found in global collection. Relationship: " + kvp.Key);

                    viewModels.RelatedProperty.SetValue(viewModels.Owner, result, null);
                }
            }
        }
        protected void AssignRelations(object viewModelObj)
        {
            m_relations.Clear();
            MakeRelationPairs(viewModelObj);

            foreach (var kvp in m_relations)
            {
                foreach (var viewModels in kvp.Value.Properties)
                {
                    if (m_globalRelationsCollectionChecking)
                    {
                        if (!kvp.Value.Collection.Contains(viewModels.Related)) throw new InvalidOperationException("Global collection doesn't contains related ViewModel in relationship: " + kvp.Key);
                    }

                    PropertyInfo modelIdPropertyInfo = viewModels.Owner.Model.GetType().GetProperty(kvp.Value.ModelPropertyIdName);
                    modelIdPropertyInfo.SetValue(viewModels.Owner.Model, viewModels.Related.Model.Id, null);
                }
            }
        }
        private void MakeRelationPairs(object viewModelObj)
        {
            if (!(viewModelObj is IModelExposable<ModelBase>)) return;
            IModelExposable<ModelBase> viewModel = (IModelExposable<ModelBase>)viewModelObj;

            Type type = viewModel.GetType();

            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if(property.GetGetMethod().IsStatic) continue;
                object propVal = property.GetValue(viewModel, null);
                object[] attributes = property.GetCustomAttributes(false);

                foreach (object attrib in attributes)
                {
                    if(attrib is DatabaseRelationAttribute)
                    {               
                        DatabaseRelationAttribute baseAttribVal = (DatabaseRelationAttribute)attrib;

                        if (!m_relations.ContainsKey(baseAttribVal.RelationName)) m_relations.Add(baseAttribVal.RelationName, new RelationPair());
                        RelationPair pair = m_relations[baseAttribVal.RelationName];

                        if(baseAttribVal is DatabaseRelationPropertyAttribute)
                        {
                            DatabaseRelationPropertyAttribute propAttribVal = (DatabaseRelationPropertyAttribute)baseAttribVal;
                            pair.Properties.Add(new RelationPair.ViewModelsPair(viewModel, (IModelExposable<ModelBase>)propVal, property));
                            pair.ModelPropertyIdName = propAttribVal.ModelPropertyIdName;
                        }
                        else if (baseAttribVal is DatabaseRelationCollectionAttribute)
                        {
                            if (pair.Collection != null) throw new InvalidOperationException("Relation " + baseAttribVal.RelationName + " already has paired collection");

                            DatabaseRelationCollectionAttribute collAttribVal = (DatabaseRelationCollectionAttribute)baseAttribVal;
                            pair.Collection = (IEnumerable<IModelExposable<ModelBase>>)propVal;
                        }
                    }
                }

                if (propVal is IEnumerable)
                {
                    IEnumerable coll = (IEnumerable)propVal;
                    foreach (object item in coll)
                    {
                        MakeRelationPairs(item);
                    }
                }
                else MakeRelationPairs(propVal);
            }
        }

        public SchoolClassViewModel SchoolClass { get; set; }
    }
}

/*
 * FILE STRUCTURE:
 * 8 bytes - [Current Id]
 * 4 bytes - [Length of JSON]
 * [Length of JSON] bytes - json data
*/