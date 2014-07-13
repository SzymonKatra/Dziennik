using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public class DatabaseMain
    {
        public DatabaseMain(string path)
        {
            m_path = path;

            //TODO: DatabaseMain.ctor(string) - load

            foreach (SchoolGroupViewModel group in SchoolClass.Groups)
            {
                foreach(StudentInGroupViewModel student in group.Students)
                {
                    if (student.Model.GlobalStudentId != null) student.GlobalStudent = SchoolClass.Students.First(x => x.Model.Id == student.Model.GlobalStudentId);
                }
            }
        }

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
        public void AssignId(IViewModelExposable<ModelBase> viewModel)
        {
            AssignId(viewModel.Model);
        }
        public void Save()
        {
            //TODO: DatabaseMain.Save()
        }

        public SchoolClassViewModel SchoolClass { get; set; }
    }
}
