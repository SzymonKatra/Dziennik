﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using Dziennik.CommandUtils;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Dziennik.View
{
    public sealed class SchoolClassControlViewModel : ObservableObject
    {
        public SchoolClassControlViewModel(object dialogOwnerViewModel)
            : this(dialogOwnerViewModel, new SchoolClassViewModel())
        {
        }
        public SchoolClassControlViewModel(object dialogOwnerViewModel, SchoolClassViewModel viewModel)
        {
            m_addMarkCommand = new RelayCommand<ObservableCollection<MarkViewModel>>(AddMark);
            m_editMarkCommand = new RelayCommand<ObservableCollection<MarkViewModel>>(EditMark);
            m_saveCommand = new RelayCommand(Save);
            m_showGlobalStudentsListCommand = new RelayCommand(ShowGlobalStudentsList);
            m_addGroupCommand = new RelayCommand(AddGroup);

            m_dialogOwnerViewModel = dialogOwnerViewModel;
            m_viewModel = viewModel;

            m_viewModel.Name = "Klasa";

            GlobalStudentViewModel s;

            s = new GlobalStudentViewModel();
            s.Id = 3;
            s.Name = "aaaaaaaaaaaaaaaa";
            s.Surname = "bbb";
            s.Email = "eaaa";
            viewModel.Students.Add(s);
            

            s = new GlobalStudentViewModel();
            s.Id = 1;
            s.Name = "ccc";
            s.Surname = "ddd";
            s.Email = "eccc";
            viewModel.Students.Add(s);

            s = new GlobalStudentViewModel();
            s.Id = 4;
            s.Name = "ggg";
            s.Surname = "hhh";
            s.Email = "eggg";
           
            viewModel.Students.Add(s);

            s = new GlobalStudentViewModel();
            s.Id = 2;
            s.Name = "eee";
            s.Surname = "fff";
            s.Email = "eeee";
            viewModel.Students.Add(s);

            viewModel.Students.ModelCollection.Sort((x, y) => { return x.Id.CompareTo(y.Id); });
            viewModel.Students.ResynchronizeWithModel();

            SchoolGroupViewModel grp = new SchoolGroupViewModel();
            grp.Name = "fafa";

            StudentInGroupViewModel sg = new StudentInGroupViewModel();
            sg.GlobalId = 1;
            sg.Id = 1;
            sg.FirstSemester.Marks.Add(new MarkViewModel() { Value = 4M });
            sg.SecondSemester.Marks.Add(new MarkViewModel() { Value = 4.5M });
            sg.FirstSemester.Marks.Add(new MarkViewModel() { Value = 2.5M });

            grp.Students.Add(sg);

            sg = new StudentInGroupViewModel();
            sg.GlobalId = 2;
            sg.Id = 2;
            sg.FirstSemester.Marks.Add(new MarkViewModel() { Value = 2M });
            sg.SecondSemester.Marks.Add(new MarkViewModel() { Value = 5.5M });
            sg.FirstSemester.Marks.Add(new MarkViewModel() { Value = 4.5M });

            grp.Students.Add(sg);
            viewModel.Groups.Add(grp);

            //m_selectedGroup = grp;
        }

        private object m_dialogOwnerViewModel;

        private SchoolClassViewModel m_viewModel;
        public SchoolClassViewModel ViewModel
        {
            get { return m_viewModel; }
            set { m_viewModel = value; OnPropertyChanged("ViewModel"); }
        }

        private StudentInGroupViewModel m_selectedStudent;
        public StudentInGroupViewModel SelectedStudent
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

        private SchoolGroupViewModel m_selectedGroup;
        public SchoolGroupViewModel SelectedGroup
        {
            get { return m_selectedGroup; }
            set { m_selectedGroup = value; OnPropertyChanged("SelectedGroup"); }
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

        private RelayCommand m_showGlobalStudentsListCommand;
        public ICommand ShowGlobalStudentsListCommand
        {
            get { return m_showGlobalStudentsListCommand; }
        }

        private RelayCommand m_addGroupCommand;
        public ICommand AddGroupCommand
        {
            get { return m_addGroupCommand; }
        }

        private void AddMark(ObservableCollection<MarkViewModel> param)
        {
            MarkViewModel mark = new MarkViewModel();
            EditMarkViewModel dialogViewModel = new EditMarkViewModel(mark);
            dialogViewModel.IsAddingMode = true;
            GlobalConfig.Dialogs.ShowDialog(m_dialogOwnerViewModel, dialogViewModel);
            if (dialogViewModel.Result == EditMarkViewModel.EditMarkResult.Ok)
            {
                param.Add(mark);
            }
            if (dialogViewModel.Result != EditMarkViewModel.EditMarkResult.Cancel) m_saveCommand.Execute(null);
        }
        private void EditMark(ObservableCollection<MarkViewModel> param)
        {
            EditMarkViewModel dialogViewModel = new EditMarkViewModel(m_selectedMark);
            GlobalConfig.Dialogs.ShowDialog(m_dialogOwnerViewModel, dialogViewModel);
            if (dialogViewModel.Result == EditMarkViewModel.EditMarkResult.RemoveMark)
            {
                param.Remove(m_selectedMark);
            }
            if (dialogViewModel.Result != EditMarkViewModel.EditMarkResult.Cancel) m_saveCommand.Execute(null);
        }
        private void Save(object param)
        {
            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                //TODO: saving
            }
            , null, "Zapisywanie...");
            GlobalConfig.Dialogs.ShowDialog(m_dialogOwnerViewModel, dialogViewModel);
        }
        private void ShowGlobalStudentsList(object param)
        {
            GlobalStudentsListViewModel dialogViewModel = new GlobalStudentsListViewModel(m_viewModel.Students);
            dialogViewModel.NeedSave += (s, e) => { m_saveCommand.Execute(null); };
            GlobalConfig.Dialogs.ShowDialog(m_dialogOwnerViewModel, dialogViewModel);
        }
        private void AddGroup(object param)
        {
            AddGroupViewModel dialogViewModel = new AddGroupViewModel(m_viewModel.Students);
            GlobalConfig.Dialogs.ShowDialog(m_dialogOwnerViewModel, dialogViewModel);
            if (dialogViewModel.Result != null)
            {
                m_viewModel.Groups.Add(dialogViewModel.Result);
                SelectedGroup = dialogViewModel.Result;
                m_saveCommand.Execute(null);
            }
        }
    }
}
