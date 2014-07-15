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

            foreach (SchoolGroupViewModel group in database.SchoolClass.Groups)
            {
                foreach (StudentInGroupViewModel student in group.Students)
                {
                    if (student.Model.GlobalStudentId != null)
                    {
                        student.GlobalStudent = database.SchoolClass.Students.FirstOrDefault(x => x.Model.Id == student.Model.GlobalStudentId);
                        Debug.Assert(student.GlobalStudent == null, "DatabaseMain.Load - GlobalStudent id not found");
                    }
                }
            }

            return database;
        }
        public void Save()
        {
            ReflectToAssignId(SchoolClass.Model);

            foreach (SchoolGroupViewModel group in SchoolClass.Groups)
            {
                foreach (StudentInGroupViewModel student in group.Students)
                {
                    student.Model.GlobalStudentId = (student.GlobalStudent == null ? null : student.GlobalStudent.Model.Id);
                }
            }

            using (FileStream stream = new FileStream(m_path, FileMode.OpenOrCreate))
            {
                stream.Write(BitConverter.GetBytes(m_currentId), 0, sizeof(ulong)); // save current id
                byte[] jsonBytes = Encoding.UTF8.GetBytes(JSON.ToJSON(SchoolClass.Model));
                stream.Write(BitConverter.GetBytes(jsonBytes.Length), 0, sizeof(int)); // save json data length
                stream.Write(jsonBytes, 0, jsonBytes.Length); // save json data

                stream.Close();
            }
        }
        //private Dictionary<Type, PropertyInfo[]> m_reflectionCache = new Dictionary<Type, PropertyInfo[]>();

        protected class RelationPair
        {
            private IEnumerable<IModelExposable<ModelBase>> m_collection;
            public IEnumerable<IModelExposable<ModelBase>> Collection
            {
                get { return m_collection; }
                set { m_collection = value; }
            }

            private IModelExposable<ModelBase> m_property;
            public IModelExposable<ModelBase> Property
            {
                get { return m_property; }
                set { m_property = value; }
            }
        }
        private Dictionary<string, RelationPair> m_relationPairs = new Dictionary<string, RelationPair>();
        public void ReflectToAssignId(ModelBase model)
        {
            if (model.Id == null) AssignId(model);

            Type type = model.GetType();

            //System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            //if (!m_reflectionCache.ContainsKey(type)) m_reflectionCache.Add(type, type.GetProperties());
            PropertyInfo[] properties = type.GetProperties();
            //PropertyInfo[] properties = m_reflectionCache[type];
            //sw.Stop();
            //Console.WriteLine(sw.Elapsed);
            foreach (PropertyInfo property in properties)
            {
                object propVal = property.GetValue(model, null);

                if(propVal is ModelBase)
                {
                    ReflectToAssignId((ModelBase)propVal);
                }
                else if (propVal is IEnumerable)
                {
                    IEnumerable coll = (IEnumerable)propVal;
                    foreach (object item in coll)
                    {
                        if (item is ModelBase) ReflectToAssignId((ModelBase)item);
                    }
                }
            }
        }
        public void ReflectToAssignRelations(IModelExposable<ModelBase> viewModel)
        {
            Type type = viewModel.GetType();

            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object propVal = property.GetValue(viewModel, null);
                object[] attributes = property.GetCustomAttributes(false);

                bool isCollection;
                if (propVal is IEnumerable)
                    isCollection = true;
                else if (propVal is IModelExposable<ModelBase>)
                    isCollection = false;
                else continue;

                foreach (object attrib in attributes)
                {
                    if(attrib is DatabaseRelationAttribute)
                    {
                        DatabaseRelationAttribute attribVal = (DatabaseRelationAttribute)attrib;

                        RelationPair pair;
                        if (!m_relationPairs.ContainsKey(attribVal.RelationName))
                        {
                            pair = new RelationPair();
                            m_relationPairs.Add(attribVal.RelationName, pair);
                        }
                        else pair = m_relationPairs[attribVal.RelationName];

                        if (isCollection)
                            pair.Collection = (IEnumerable<IModelExposable<ModelBase>>)propVal;
                        else pair.Property = (IModelExposable<ModelBase>)propVal;
                    }
                }
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