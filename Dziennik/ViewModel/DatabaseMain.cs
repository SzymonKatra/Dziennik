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
    public class DatabaseMain : DatabaseContext<SchoolClassViewModel, SchoolClass>
    {
        public DatabaseMain()
        {
            base.AssignRelationsGlobalCollectionChecking = AssingRelationsGlobalCollectionCheckingModes.ExceptionIfNotExistsNotNull;
            base.RestoreRelationsGlobalCollectionChecking = RestoreRelationsGlobalCollectionCheckingModes.ExceptionIfNotExistsNotNull;
        }

        private string m_path;
        public string Path
        {
            get { return m_path; }
            set { m_path = value; }
        }

        public static DatabaseMain Load(string path)
        {
            DatabaseMain database = new DatabaseMain();

            database.m_path = path;

            using (FileStream stream = new FileStream(database.m_path, FileMode.OpenOrCreate))
            {
                database.Load(stream);

                stream.Close();
            }

            return database;
        }
        public void Save()
        {
            using (FileStream stream = new FileStream(m_path, FileMode.OpenOrCreate))
            {
                base.Save(stream);

                stream.Close();
            }
        }
    }
}