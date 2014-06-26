using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using Dziennik.Model;
using Dziennik.CommandUtils;
using System.Windows.Input;
using System.Windows;
using Dziennik.Controls;

namespace Dziennik.View
{
    public sealed class GlobalStudentsListViewModel : ObservableObject
    {
        public GlobalStudentsListViewModel(ObservableCollection<GlobalStudentViewModel> students)
        {
            m_addStudentCommand = new RelayCommand(AddStudent);
            m_editStudentCommand = new RelayCommand(EditStudent);
            m_autoAddStudentsClipboardCommand = new RelayCommand(AutoAddStudentsClipboard);

            m_students = students;
        }

        private RelayCommand m_addStudentCommand;
        public ICommand AddStudentCommand
        {
            get { return m_addStudentCommand; }
        }

        private RelayCommand m_editStudentCommand;
        public ICommand EditStudentCommand
        {
            get { return m_editStudentCommand; }
        }

        private RelayCommand m_autoAddStudentsClipboardCommand;
        public ICommand AutoAddStudentsClipboardCommand
        {
            get { return m_autoAddStudentsClipboardCommand; }
        }

        private ObservableCollection<GlobalStudentViewModel> m_students;
        public ObservableCollection<GlobalStudentViewModel> Students
        {
            get { return m_students; }
            set { m_students = value; OnPropertyChanged("Students"); }
        }

        private GlobalStudentViewModel m_selectedStudent;
        public GlobalStudentViewModel SelectedStudent
        {
            get { return m_selectedStudent; }
            set { m_selectedStudent = value; OnPropertyChanged("SelectedStudent"); }
        }

        public event EventHandler NeedSave;

        private void AddStudent(object e)
        {
            GlobalStudentViewModel student = new GlobalStudentViewModel();
            student.Id = GetNextStudentId();

            EditStudentViewModel dialogViewModel = new EditStudentViewModel(student);
            dialogViewModel.IsAddingMode = true;
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);

            if (dialogViewModel.Result == EditStudentViewModel.EditStudentResult.Ok)
            {
                m_students.Add(student);
                //m_viewModel.Students.Insert(student.Id, student);
                //for (int i = student.Id + 1; i < m_viewModel.Students.Count; i++)
                //{
                //    m_viewModel.Students[i].Id++;
                //}
            }
            if (dialogViewModel.Result != EditStudentViewModel.EditStudentResult.Cancel) OnNeedSave(EventArgs.Empty);
        }
        private void EditStudent(object e)
        {
            EditStudentViewModel dialogViewModel = new EditStudentViewModel(m_selectedStudent);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditStudentViewModel.EditStudentResult.RemoveStudentCompletly)
            {
                int index = m_students.IndexOf(m_selectedStudent);
                if (index < 0) return;
                for (int i = index + 1; i < m_students.Count; i++)
                {
                    m_students[i].Id--;
                }

                m_students.RemoveAt(index);
            }
            if (dialogViewModel.Result != EditStudentViewModel.EditStudentResult.Cancel) OnNeedSave(EventArgs.Empty);
        }
        private void AutoAddStudentsClipboard(object param)
        {
            try
            {
                if (Clipboard.ContainsData(DataFormats.Text))
                {
                    string data = Clipboard.GetText();
                    data = data.Replace("\r", "");
                    data = data.Replace("\t", "");

                    string[] lines = data.Split('\n');
                    int added = 0;
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        int nameSurnameSeparatorIndex = line.IndexOf(' ');

                        GlobalStudentViewModel student = new GlobalStudentViewModel();
                        student.Id = GetNextStudentId();
                        student.Surname = line.Substring(0, nameSurnameSeparatorIndex);
                        student.Name = line.Substring(nameSurnameSeparatorIndex + 1);

                        m_students.Add(student);
                        ++added;
                    }

                    MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Dodano " + added + " uczniów", "Dziennik", MessageBoxSuperPredefinedButtons.OK);
                }
            }
            catch
            {
                MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Wystąpił błąd podczas dodawania uczniów" + Environment.NewLine + "Sprawdź czy schowek zawiera prawidłowy format listy", "Dziennik", MessageBoxSuperPredefinedButtons.OK);
            }
        }

        private int GetNextStudentId()
        {
            return (m_students.Count <= 0 ? 1 : m_students[m_students.Count - 1].Id + 1);
        }

        private void OnNeedSave(EventArgs e)
        {
            EventHandler handler = NeedSave;
            if (handler != null) handler(this, e);
        }
    }
}
