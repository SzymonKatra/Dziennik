﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Dziennik.WPFControls;
using Dziennik.CommandUtils;
using Dziennik.ViewModel;
using System.ComponentModel;

namespace Dziennik.WindowViewModel
{
    public class EditMarkEventArgs : EventArgs
    {
        private MarkViewModel m_mark;
        public MarkViewModel Mark
        {
            get { return m_mark; }
            set { m_mark = value; }
        }

        private bool m_save;
        public bool Save
        {
            get { return m_save; }
            set { m_save = value; }
        }

        public EditMarkEventArgs(MarkViewModel mark)
        {
            m_mark = mark;
        }
    }

    public sealed class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            m_addMarkCommand = new RelayCommand<ObservableCollection<MarkViewModel>>(AddMark);
            m_editMarkCommand = new RelayCommand<object>(EditMark);

            StudentViewModel s;

            s = new StudentViewModel();
            s.Id = 3;
            s.Name = "aaaaaaaaaaaaaaaa";
            s.Surname = "bbb";
            s.Email = "eaaa";
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 5M });
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 3.5M });
            m_students.Add(s);

            s = new StudentViewModel();
            s.Id = 1;
            s.Name = "ccc";
            s.Surname = "ddd";
            s.Email = "eccc";
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 3M });
            s.SecondSemester.Marks.Add(new MarkViewModel() { Value = 2.5M });
            m_students.Add(s);

            s = new StudentViewModel();
            s.Id = 4;
            s.Name = "ggg";
            s.Surname = "hhh";
            s.Email = "eggg";
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 4M });
            s.SecondSemester.Marks.Add(new MarkViewModel() { Value = 4.5M });
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 2.5M });
            m_students.Add(s);

            s = new StudentViewModel();
            s.Id = 2;
            s.Name = "eee";
            s.Surname = "fff";
            s.Email = "eeee";
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 1M });
            s.FirstSemester.Marks.Add(new MarkViewModel() { Value = 6M });
            s.SecondSemester.Marks.Add(new MarkViewModel() { Value = 6M });
            m_students.Add(s);

            m_students = new ObservableCollection<StudentViewModel>(m_students.OrderBy(x => x.Id));
        }

        private ObservableCollection<StudentViewModel> m_students = new ObservableCollection<StudentViewModel>();
        public ObservableCollection<StudentViewModel> Students
        {
            get { return m_students; }
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
        private RelayCommand<object> m_editMarkCommand;
        public ICommand EditMarkCommand
        {
            get { return m_editMarkCommand; }
        }

        public event EventHandler<EditMarkEventArgs> EditMarkDialog;

        public void AddMark(ObservableCollection<MarkViewModel> e)
        {
            MarkViewModel mark = new MarkViewModel();
            EditMarkEventArgs eResult = new EditMarkEventArgs(mark);
            OnEditMarkDialog(eResult);
            if (eResult.Save)
            {
                mark.AddDate = DateTime.Now;
                e.Add(mark);
            }
        }
        public void EditMark(object e)
        {
            OnEditMarkDialog(new EditMarkEventArgs(m_selectedMark));
        }

        private void OnEditMarkDialog(EditMarkEventArgs e)
        {
            EventHandler<EditMarkEventArgs> handler = EditMarkDialog;
            if (handler != null) handler(this, e);
        }
    }
}