using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Dziennik.CommandUtils;
using System.ComponentModel;
using Dziennik.ViewModel;
using System.Collections.ObjectModel;

namespace Dziennik.View
{
    public class AddMarksSetViewModel : ObservableObject, IDataErrorInfo
    {
        public enum AddMarksSetResult
        {
            Ok,
            Cancel,
        }

        public enum SemesterType
        {
            First,
            Second,
        }

        public class StudentAddMarkPair : ObservableObject, IDataErrorInfo
        {
            public StudentAddMarkPair(StudentInGroupViewModel student)
            {
                m_student = student;
            }

            private StudentInGroupViewModel m_student;
            public StudentInGroupViewModel Student
            {
                get { return m_student; }
                set { m_student = value; RaisePropertyChanged("Student"); }
            }

            public bool IsRemoved
            {
                get { return m_student.IsRemoved; }
            }

            private decimal m_value;
            public decimal Value
            {
                get { return m_value; }
                private set
                {
                    m_value = value;
                    RaisePropertyChanged("Value");
                    RaisePropertyChanged("IsValueValid");
                }
            }
            private string m_note;
            public string Note
            {
                get { return m_note; }
                private set
                {
                    m_note = value;
                    RaisePropertyChanged("Note");
                    RaisePropertyChanged("IsValueValid");
                }
            }
            public bool IsValueValid
            {
                get
                {
                    return string.IsNullOrWhiteSpace(m_note);
                }
            }
            private bool m_inputValid;
            public bool InputValid
            {
                get { return m_inputValid; }
                private set { m_inputValid = value; RaisePropertyChanged("InputValid"); }
            }
            private string m_input;
            public string Input
            {
                get { return m_input; }
                set
                {
                    if (IsRemoved) return;
                    m_input = value; RaisePropertyChanged("Input");
                }
            }
            private bool m_isInputNull;
            public bool IsInputNull
            {
                get { return m_isInputNull; }
                private set { m_isInputNull = value; RaisePropertyChanged("IsInputNull"); }
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
                        case "Input": return ValidateInput();
                    }
                    return string.Empty;
                }
            }
            public string ValidateInput()
            {
                if(string.IsNullOrWhiteSpace(m_input))
                {
                    IsInputNull = true;
                    InputValid = true;
                    return string.Empty;
                }
                IsInputNull = false;

                string errorResult;
                decimal result;

                errorResult = EditMarkViewModel.GlobalValidateValue(m_input, out result);
                if (string.IsNullOrEmpty(errorResult))
                {
                    Value = result;
                }
                else
                {
                    errorResult = EditMarkViewModel.GlobalValidateNote(m_input);
                    if(string.IsNullOrEmpty(errorResult))
                    {
                        Note = m_input;
                    }
                    else
                    {
                        InputValid = false;
                        return "error";
                    }
                }

                InputValid = true;
                return string.Empty;
            }
        }

        public AddMarksSetViewModel(IEnumerable<StudentInGroupViewModel> students, SemesterType semester)
        {
            m_okCommand = new RelayCommand(Ok, CanOk);
            m_cancelCommand = new RelayCommand(Cancel);

            m_availableCategories = new ObservableCollection<MarksCategoryViewModel>(GlobalConfig.GlobalDatabase.ViewModel.MarksCategories);
            m_availableCategories.Insert(0, EditMarkViewModel.NoSelectionMarksCategory);

            m_selectedCategory = EditMarkViewModel.NoSelectionMarksCategory;

            m_students = new ObservableCollection<StudentAddMarkPair>();
            foreach (StudentInGroupViewModel student in students)
            {
                StudentAddMarkPair pair = new StudentAddMarkPair(student);
                pair.PropertyChanged += pair_PropertyChanged;
                pair.ValidateInput();
                m_students.Add(pair);
            }
            m_semester = semester;
            //m_weightInput = m_weight.ToString();
        }

        private AddMarksSetResult m_result = AddMarksSetResult.Cancel;
        public AddMarksSetResult Result
        {
            get { return m_result; }
        }

        private SemesterType m_semester;
        public SemesterType Semester
        {
            get { return m_semester; }
            set { m_semester = value; RaisePropertyChanged("Semester"); }
        }

        private ObservableCollection<StudentAddMarkPair> m_students;
        public ObservableCollection<StudentAddMarkPair> Students
        {
            get { return m_students; }
        }

        private string m_description;
        public string Description
        {
            get { return m_description; }
            set { m_description = value; RaisePropertyChanged("Description"); }
        }

        //private bool m_weightInputValid = false;
        private int m_weight = 1;
        public int Weight
        {
            get { return m_weight; }
            set { m_weight = value; RaisePropertyChanged("Weight"); }
        }
        //private string m_weightInput;
        //public string WeightInput
        //{
        //    get { return m_weightInput; }
        //    set { m_weightInput = value; RaisePropertyChanged("WeightInput"); }
        //}

        private ObservableCollection<MarksCategoryViewModel> m_availableCategories;
        public ObservableCollection<MarksCategoryViewModel> AvailableCategories
        {
            get { return m_availableCategories; }
        }

        private MarksCategoryViewModel m_selectedCategory;
        public MarksCategoryViewModel SelectedCategory
        {
            get { return m_selectedCategory; }
            set
            {
                m_selectedCategory = value; RaisePropertyChanged("SelectedCategory");
                Weight = m_selectedCategory.DefaultWeight;
            }
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

        private void Ok(object e)
        {
            List<int> nullStudents = new List<int>();
            foreach (StudentAddMarkPair pair in m_students)
            {
                if (pair.IsInputNull) nullStudents.Add(pair.Student.Number);
            }

            if (nullStudents.Count > 0)
            {
                string mboxtext = string.Format(GlobalConfig.GetStringResource("lang_NullInputStudentsFormat"), nullStudents.Count);
                mboxtext += Environment.NewLine;
                mboxtext += GlobalConfig.GetStringResource("lang_TheyAre");
                mboxtext += ' ';
                mboxtext += nullStudents[0];
                for (int i = 1; i < nullStudents.Count; i++)
                {
                    mboxtext += GlobalConfig.GetStringResource("lang_TheyAreSeparator");
                    mboxtext += nullStudents[i];
                }
                mboxtext += Environment.NewLine;
                mboxtext += GlobalConfig.GetStringResource("lang_DoYouWantToContinue");

                if (GlobalConfig.MessageBox(this, mboxtext, Controls.MessageBoxSuperPredefinedButtons.YesNo) != Controls.MessageBoxSuperButton.Yes) return;
            }

            DateTime nowDate = DateTime.Now; // must be the same for all students
            foreach (StudentAddMarkPair pair in m_students)
            {
                if (!pair.IsInputNull)
                {
                    SemesterViewModel semester = (m_semester == SemesterType.First ? pair.Student.FirstSemester : pair.Student.SecondSemester);

                    MarkViewModel mark = new MarkViewModel();
                    mark.AddDate = mark.LastChangeDate = nowDate;
                    mark.Description = m_description;
                    mark.Weight = m_weight;
                    mark.Category = (m_selectedCategory == EditMarkViewModel.NoSelectionMarksCategory ? null : m_selectedCategory);

                    if (pair.IsValueValid)
                    {
                        mark.Value = pair.Value;
                    }
                    else // note valid
                    {
                        mark.Note = pair.Note;
                    }

                    semester.Marks.Add(mark);
                }
            }

            m_result = AddMarksSetResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanOk(object e)
        {
            foreach (StudentAddMarkPair pair in m_students)
            {
                if (!pair.InputValid) return false;
            }
            return true;
            //return m_weightInputValid;
        }
        private void Cancel(object e)
        {
            m_result = AddMarksSetResult.Cancel;
            GlobalConfig.Dialogs.Close(this);
        }

        private void pair_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "InputValid")
            {
                m_okCommand.RaiseCanExecuteChanged();
            }
        }

        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                //switch(columnName)
                //{
                //    //case "WeightInput": return ValidateWeightInput();
                //}

                return string.Empty;
            }
        }

        //private string ValidateWeightInput()
        //{
        //    m_weightInputValid = false;

        //    int result;

        //    string errorResult = EditMarkViewModel.GlobalValidateWeightInput(m_weightInput, out result);
        //    if (!string.IsNullOrEmpty(errorResult))
        //    {
        //        m_okCommand.RaiseCanExecuteChanged();
        //        return errorResult;
        //    }

        //    m_weight = result;
        //    m_weightInputValid = true;
        //    m_okCommand.RaiseCanExecuteChanged();

        //    return string.Empty;
        //}
    }
}
