using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using System.Windows.Input;
using Dziennik.CommandUtils;
using System.ComponentModel;
using System.Diagnostics;
using Dziennik.Controls;

namespace Dziennik.View
{
    public sealed class AddGroupViewModel : ObservableObject, IDataErrorInfo
    {
        public AddGroupViewModel(ObservableCollection<GlobalStudentViewModel> globalStudentCollection)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);
            m_selectStudentsCommand = new RelayCommand(SelectStudents);
            m_showGlobalSubjectsListCommand = new RelayCommand(ShowGlobalSubjectsList);
            m_showSchedulesListCommand = new RelayCommand(ShowSchedulesList);

            m_globalStudentCollection = globalStudentCollection;

            foreach (var gStudent in m_globalStudentCollection)
            {
                m_selectedStudents.Add(gStudent.Number);
            }
            m_selectedStudentsInput = SelectionParser.Create(m_selectedStudents);
        }

        private SchoolGroupViewModel m_result = new SchoolGroupViewModel();
        public SchoolGroupViewModel Result
        {
            get { return m_result; }
        }

        private ObservableCollection<GlobalStudentViewModel> m_globalStudentCollection;

        private bool m_nameInputValid = false;
        private string m_nameInput;
        public string NameInput
        {
            get { return m_nameInput; }
            set { m_nameInput = value; RaisePropertyChanged("NameInput"); }
        }

        //public List<LessonHourViewModel> AvailableHours
        //{
        //    get
        //    {
        //        //List<int> result = new List<int>();

        //        //foreach (var item in GlobalConfig.GlobalDatabase.ViewModel.Hours.Hours)
        //        //{
        //        //    result.Add(item.Number);
        //        //}

        //        return GlobalConfig.GlobalDatabase.ViewModel.Hours.Hours;
        //    }
        //}

        private List<int> m_selectedStudents = new List<int>();
        private bool m_selectedStudentsInputValid = false;
        private string m_selectedStudentsInput;
        public string SelectedStudentsInput
        {
            get { return m_selectedStudentsInput; }
            set { m_selectedStudentsInput = value; RaisePropertyChanged("SelectedStudentsInput"); }
        }

        private bool m_renumberFromOne = true;
        public bool RenumberFromOne
        {
            get { return m_renumberFromOne; }
            set { m_renumberFromOne = value; RaisePropertyChanged("RenumberFromOne"); }
        }

        private RelayCommand m_okCommand;
        public ICommand OkCommand
        {
            get { return m_okCommand; }
        }

        private RelayCommand m_cancelCommand;
        public ICommand CancelCommand
        {
            get { return m_cancelCommand; }
        }

        private RelayCommand m_selectStudentsCommand;
        public ICommand SelectStudentsCommand
        {
            get { return m_selectStudentsCommand; }
        }

        private RelayCommand m_showGlobalSubjectsListCommand;
        public ICommand ShowGlobalSubjectsListCommand
        {
            get { return m_showGlobalSubjectsListCommand; }
        }

        private RelayCommand m_showSchedulesListCommand;
        public ICommand ShowSchedulesListCommand
        {
            get { return m_showSchedulesListCommand; }
        }

        private void Ok(object param)
        {
            if (m_selectedStudents.Count <= 0)
            {
                if (GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_NoAddedStudents") + Environment.NewLine + GlobalConfig.GetStringResource("lang_DoYouWantToContinue"), MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;
            }

            if (!m_renumberFromOne) m_selectedStudents.Sort();

            int index = 1;

            m_result.Name = m_nameInput;
            foreach (int selStudent in m_selectedStudents)
            {
                StudentInGroupViewModel studentInGroup = new StudentInGroupViewModel();
                studentInGroup.GlobalStudent = m_globalStudentCollection.First(x => x.Number == selStudent);
                studentInGroup.Number = (m_renumberFromOne ? index++ : selStudent);

                m_result.Students.Add(studentInGroup);
            }

            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object param)
        {
            return m_nameInputValid && m_selectedStudentsInputValid;
        }
        private void Cancel(object e)
        {
            m_result = null;

            if (e == null)
            {
                GlobalConfig.Dialogs.Close(this);
            }
        }
        private void SelectStudents(object param)
        {
            SelectStudentsViewModel dialogViewModel = new SelectStudentsViewModel(m_globalStudentCollection, m_selectedStudents);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if(dialogViewModel.Result)
            {
                SelectedStudentsInput = dialogViewModel.ResultSelectionString;
            }
        }
        private void ShowGlobalSubjectsList(object param)
        {
            GlobalSubjectsListViewModel dialogViewModel = new GlobalSubjectsListViewModel(m_result.GlobalSubjects);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }
        private void ShowSchedulesList(object param)
        {
            SchedulesListViewModel dialogViewModel = new SchedulesListViewModel(m_result.Schedules);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }

        public string Error
        {
            get { return string.Empty; }
        }
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "NameInput": return ValidateNameInput();
                    case "SelectedStudentsInput": return ValidateSelectedStudentsInput();
                }

                return string.Empty;
            }
        }

        public string ValidateNameInput()
        {
            m_nameInputValid = false;

            if (string.IsNullOrWhiteSpace(m_nameInput))
            {
                m_okCommand.RaiseCanExecuteChanged();
                return GlobalConfig.GetStringResource("lang_TypeGroupName");
            }

            m_nameInputValid = true;
            m_okCommand.RaiseCanExecuteChanged();
            return string.Empty;
        }
        public string ValidateSelectedStudentsInput()
        {
            m_selectedStudentsInputValid = false;

            string error;
            List<int> tempColl = SelectionParser.Parse(m_selectedStudentsInput, out error);
            if (tempColl == null)
            {
                m_okCommand.RaiseCanExecuteChanged();
                return error;
            }
            m_selectedStudents = tempColl;

            foreach (int selStudent in m_selectedStudents)
            {
                if (m_globalStudentCollection.FirstOrDefault((gs) => { return gs.Number == selStudent; }) == null)
                {
                    m_okCommand.RaiseCanExecuteChanged();
                    return string.Format(GlobalConfig.GetStringResource("lang_StudentNumberNotExistsFormat"), selStudent);
                }
            }

            m_selectedStudentsInputValid = true;
            m_okCommand.RaiseCanExecuteChanged();
            return string.Empty;
        }
       
    }
}
