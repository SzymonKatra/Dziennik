using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using Dziennik.CommandUtils;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Dziennik.View;

namespace Dziennik.Controls
{
    public sealed class SchoolClassControlViewModel : ObservableObject
    {
        public SchoolClassControlViewModel(object dialogOwnerViewModel)
            : this(dialogOwnerViewModel, new SchoolClassViewModel())
        {
        }
        public SchoolClassControlViewModel(object dialogOwnerViewModel, SchoolClassViewModel viewModel)
        {
            m_dialogOwnerViewModel = dialogOwnerViewModel;
            m_viewModel = viewModel;

            m_viewModel.Name = "Klasa";

            m_addMarkCommand = new RelayCommand<ObservableCollection<MarkViewModel>>(AddMark);
            m_editMarkCommand = new RelayCommand<ObservableCollection<MarkViewModel>>(EditMark);
            m_saveCommand = new RelayCommand(Save);
            m_addStudentCommand = new RelayCommand(AddStudent);
            m_editStudentCommand = new RelayCommand(EditStudent);

            StudentViewModel s;

            s = new StudentViewModel();
            s.Id = 3;
            s.Name = "aaaaaaaaaaaaaaaa";
            s.Surname = "bbb";
            s.Email = "eaaa";
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 5M });
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 3.5M });
            viewModel.Students.Add(s);

            s = new StudentViewModel();
            s.Id = 1;
            s.Name = "ccc";
            s.Surname = "ddd";
            s.Email = "eccc";
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 3M });
            s.SecondSemester.Marks.Add(new MarkViewModel() { Value = 2.5M });
            viewModel.Students.Add(s);

            s = new StudentViewModel();
            s.Id = 4;
            s.Name = "ggg";
            s.Surname = "hhh";
            s.Email = "eggg";
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 4M });
            s.SecondSemester.Marks.Add(new MarkViewModel() { Value = 4.5M });
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 2.5M });
            viewModel.Students.Add(s);

            s = new StudentViewModel();
            s.Id = 2;
            s.Name = "eee";
            s.Surname = "fff";
            s.Email = "eeee";
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 1M });
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 6M });
            s.SecondSemester.Marks.Add(new MarkViewModel() { Value = 6M });
            viewModel.Students.Add(s);

            viewModel.Students.ModelCollection.Sort((x, y) => { return x.Id.CompareTo(y.Id); });
            viewModel.Students.ResynchronizeWithModel();
        }

        private object m_dialogOwnerViewModel;

        private SchoolClassViewModel m_viewModel;
        public SchoolClassViewModel ViewModel
        {
            get { return m_viewModel; }
            set { m_viewModel = value; OnPropertyChanged("ViewModel"); }
        }

        private StudentViewModel m_selectedStudent;
        public StudentViewModel SelectedStudent
        {
            get { return m_selectedStudent; }
            set { m_selectedStudent = value; OnPropertyChanged("SelectedStudent"); }
        }

        private MarkViewModel m_selectedMark;
        public MarkViewModel SelectedMark
        {
            get { return m_selectedMark; }
            set { m_selectedMark = value; OnPropertyChanged("SelectedMark"); }
        }

        private RelayCommand<ObservableCollection<MarkViewModel>> m_addMarkCommand;
        public ICommand AddMarkCommand
        {
            get { return m_addMarkCommand; }
        }

        private RelayCommand<ObservableCollection<MarkViewModel>> m_editMarkCommand;
        public ICommand EditMarkCommand
        {
            get { return m_editMarkCommand; }
        }

        private RelayCommand m_saveCommand;
        public ICommand SaveCommand
        {
            get { return m_saveCommand; }
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

        private void AddMark(ObservableCollection<MarkViewModel> e)
        {
            MarkViewModel mark = new MarkViewModel();
            EditMarkViewModel dialogViewModel = new EditMarkViewModel(mark);
            dialogViewModel.IsAddingMode = true;
            GlobalConfig.Dialogs.ShowDialog(m_dialogOwnerViewModel, dialogViewModel);
            if (dialogViewModel.Result == EditMarkViewModel.EditMarkResult.Ok)
            {
                e.Add(mark);
            }
            if (dialogViewModel.Result != EditMarkViewModel.EditMarkResult.Cancel) m_saveCommand.Execute(null);
        }
        private void EditMark(ObservableCollection<MarkViewModel> e)
        {
            EditMarkViewModel dialogViewModel = new EditMarkViewModel(m_selectedMark);
            GlobalConfig.Dialogs.ShowDialog(m_dialogOwnerViewModel, dialogViewModel);
            if (dialogViewModel.Result == EditMarkViewModel.EditMarkResult.RemoveMark)
            {
                e.Remove(m_selectedMark);
            }
            if (dialogViewModel.Result != EditMarkViewModel.EditMarkResult.Cancel) m_saveCommand.Execute(null);
        }
        private void Save(object e)
        {
            Console.WriteLine("SchoolClassControlViewModel.Save()");
            //TODO: saving

            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel(SaveWorker, null, "Zapisywanie...");
            GlobalConfig.Dialogs.ShowDialog(m_dialogOwnerViewModel, dialogViewModel);
            Console.WriteLine("Zapisano");
        }
        private void AddStudent(object e)
        {
            StudentViewModel student = new StudentViewModel();
            student.Id = m_viewModel.Students[m_viewModel.Students.Count - 1].Id + 1;

            EditStudentViewModel dialogViewModel = new EditStudentViewModel(student);
            dialogViewModel.IsAddingMode = true;
            GlobalConfig.Dialogs.ShowDialog(m_dialogOwnerViewModel, dialogViewModel);

            if (dialogViewModel.Result == EditStudentViewModel.EditStudentResult.Ok)
            {
                m_viewModel.Students.Add(student);
                //m_viewModel.Students.Insert(student.Id, student);
                //for (int i = student.Id + 1; i < m_viewModel.Students.Count; i++)
                //{
                //    m_viewModel.Students[i].Id++;
                //}
            }
            if (dialogViewModel.Result != EditStudentViewModel.EditStudentResult.Cancel) m_saveCommand.Execute(null);
        }
        private void EditStudent(object e)
        {
            EditStudentViewModel dialogViewModel = new EditStudentViewModel(m_selectedStudent);
            GlobalConfig.Dialogs.ShowDialog(m_dialogOwnerViewModel, dialogViewModel);
            if(dialogViewModel.Result == EditStudentViewModel.EditStudentResult.RemoveStudentCompletly)
            {
                int index = m_viewModel.Students.IndexOf(m_selectedStudent);
                if (index < 0) return;
                for (int i = index + 1; i < m_viewModel.Students.Count; i++)
                {
                    m_viewModel.Students[i].Id--;
                }

                m_viewModel.Students.RemoveAt(index);
            }
            if (dialogViewModel.Result != EditStudentViewModel.EditStudentResult.Cancel) m_saveCommand.Execute(null);
        }

        private void SaveWorker(ActionDialogViewModel dialog, object parameter)
        {
            System.Threading.Thread.Sleep(1000);
            dialog.Content = "dupa";
            System.Threading.Thread.Sleep(30000);
        }
    }
}
