using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;
using System.IO;

namespace Dziennik.ViewModel
{
    public class DatabaseGlobal : DatabaseContext<GlobalSchoolViewModel, GlobalSchool>
    {
        public DatabaseGlobal()
        {
            ViewModel = new GlobalSchoolViewModel();

            base.AssignRelationsGlobalCollectionChecking = AssingRelationsGlobalCollectionCheckingModes.ExceptionIfNotExistsNotNull;
            base.RestoreRelationsGlobalCollectionChecking = RestoreRelationsGlobalCollectionCheckingModes.ExceptionIfNotExistsNotNull;
        }

        private string m_path;
        public string Path
        {
            get { return m_path; }
            set { m_path = value; }
        }

        public static DatabaseGlobal Load(string path)
        {
            DatabaseGlobal database = new DatabaseGlobal();

            database.m_path = path;

            using (FileStream stream = new FileStream(database.m_path, FileMode.OpenOrCreate))
            {
                database.Load(stream);
            }

            return database;
        }
        public void Save()
        {
            using (FileStream stream = new FileStream(m_path, FileMode.OpenOrCreate))
            {
                base.Save(stream);
            }
        }
    }
}
