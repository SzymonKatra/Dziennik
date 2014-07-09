﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class GlobalStudentViewModel : ObservableObject, IViewModelExposable<GlobalStudent>
    {
        public GlobalStudentViewModel()
            : this(new GlobalStudent())
        {
        }
        public GlobalStudentViewModel(GlobalStudent globalStudent)
        {
            m_model = globalStudent;
        }

        private GlobalStudent m_model;
        public GlobalStudent Model
        {
            get { return m_model; }
        }

        public int Id
        {
            get { return m_model.Id; }
            set { m_model.Id = value; RaisePropertyChanged("Id"); }
        }
        public string Name
        {
            get { return m_model.Name; }
            set { m_model.Name = value; RaisePropertyChanged("Name"); }
        }
        public string Surname
        {
            get { return m_model.Surname; }
            set { m_model.Surname = value; RaisePropertyChanged("Surname"); }
        }
        public string Email
        {
            get { return m_model.Email; }
            set { m_model.Email = value; RaisePropertyChanged("Email"); }
        }
        public string AdditionalInformation
        {
            get { return m_model.AdditionalInformation; }
            set { m_model.AdditionalInformation = value; RaisePropertyChanged("AdditionalInformation"); }
        }

        public static GlobalStudentViewModel Dummy
        {
            get
            {
                GlobalStudentViewModel result = new GlobalStudentViewModel();

                result.Id = -1;
                result.Name = "----------";
                result.Surname = "----------";
                result.Email = "----------";
                result.AdditionalInformation = string.Empty;

                return result;
            }
        }
    }
}
