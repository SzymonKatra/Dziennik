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

            m_globalStudentCollection = globalStudentCollection;
        }

        private SchoolGroupViewModel m_result = null;
        public SchoolGroupViewModel Result
        {
            get { return m_result; }
        }

        private ObservableCollection<GlobalStudentViewModel> m_globalStudentCollection;

        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; OnPropertyChanged("Name"); }
        }

        private List<int> m_selectedStudents = new List<int>();
        private bool m_selectedStudentsInputValid = false;
        private string m_selectedStudentsInput;
        public string SelectedStudentsInput
        {
            get { return m_selectedStudentsInput; }
            set { m_selectedStudentsInput = value; OnPropertyChanged("SelectedStudentsInput"); }
        }

        private bool m_renumberFromOne = true;
        public bool RenumberFromOne
        {
            get { return m_renumberFromOne; }
            set { m_renumberFromOne = value; OnPropertyChanged("RenumberFromOne"); }
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

        private void Ok(object param)
        {
            if (m_selectedStudents.Count <= 0)
            {
                if (MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Uwaga! Nie dodano żadnego ucznia do grupy." + Environment.NewLine + "Czy chcesz kontynuować?", "Dziennik", MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;
            }

            if (!m_renumberFromOne) m_selectedStudents.Sort();

            int index = 1;

            m_result = new SchoolGroupViewModel();

            m_result.Name = m_name;
            foreach (int selStudent in m_selectedStudents)
            {
                StudentInGroupViewModel studentInGroup = new StudentInGroupViewModel();
                studentInGroup.GlobalId = selStudent;
                studentInGroup.Id = (m_renumberFromOne ? index++ : selStudent);

                m_result.Students.Add(studentInGroup);
            }

            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object param)
        {
            return m_selectedStudentsInputValid;
        }
        private void Cancel(object param)
        {
            m_result = null;

            GlobalConfig.Dialogs.Close(this);
        }

        public string Error
        {
            get { return string.Empty; }
        }
        public string this[string columnName]
        {
            get
            {
                switch(columnName)
                {
                    case "SelectedStudentsInput": return ValidateSelectedStudentsInput();
                }

                return string.Empty;
            }
        }

        public string ValidateSelectedStudentsInput()
        {
            m_selectedStudentsInputValid = false;
            m_selectedStudents.Clear();

            if (string.IsNullOrWhiteSpace(m_selectedStudentsInput))
            {
                m_selectedStudentsInputValid = true;
                return string.Empty;
            }

            try
            {
                string toParse = m_selectedStudentsInput.Replace(" ", "");
                
                string[] tokens = m_selectedStudentsInput.Split(',', ';');

                foreach (string item in tokens)
                {
                    int result;
                    if (int.TryParse(item, out result))
                    {
                        m_selectedStudents.Add(result);
                        continue;
                    }

                    string[] rangeStr = item.Split('-');
                    if (rangeStr.Length != 2) return "Nieprawidłowy format. Zakresy oddziel jednym myślnikiem(-)";

                    int minRange;
                    int maxRange;
                    if (!int.TryParse(rangeStr[0], out minRange) || !int.TryParse(rangeStr[1], out maxRange)) return "Niedozwolone znaki";

                    for (int i = minRange; i <= maxRange; i++) m_selectedStudents.Add(i);
                }
            }
            catch
            {
                Debug.Assert(true, "Exception in AddGroupViewModel.ValidateSelectedStudentsInput");
            }

            foreach (int selStudent in m_selectedStudents)
            {
                if (m_globalStudentCollection.FirstOrDefault((gs) => { return gs.Id == selStudent; }) == null)
                {
                    return string.Format("Ucznia o numerze {0} nie ma w bazie", selStudent);
                }
            }

            m_selectedStudentsInputValid = true;
            return string.Empty;
        }
    }
}
