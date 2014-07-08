using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using Dziennik.CommandUtils;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using Dziennik.Model;

namespace Dziennik.View
{
    public sealed class SchoolClassControlViewModel : ObservableObject
    {
        public SchoolClassControlViewModel(MainViewModel dialogOwnerViewModel)
            : this(dialogOwnerViewModel, new SchoolClassViewModel())
        {
        }
        public SchoolClassControlViewModel(MainViewModel dialogOwnerViewModel, SchoolClassViewModel viewModel)
        {
            m_addMarkCommand = new RelayCommand<ObservableCollection<MarkViewModel>>(AddMark);
            m_editMarkCommand = new RelayCommand<ObservableCollection<MarkViewModel>>(EditMark);
            m_autoSaveCommand = new RelayCommand(AutoSave);
            m_saveCommand = new RelayCommand(Save);
            m_showGlobalStudentsListCommand = new RelayCommand(ShowGlobalStudentsList);
            m_addGroupCommand = new RelayCommand(AddGroup);
            m_editGroupCommand = new RelayCommand(EditGroup, CanEditGroup);

            m_ownerViewModel = dialogOwnerViewModel;
            m_viewModel = viewModel;
        }

        private MainViewModel m_ownerViewModel;
        public MainViewModel OwnerViewModel
        {
            get { return m_ownerViewModel; }
        }

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
            set { m_selectedGroup = value; OnPropertyChanged("SelectedGroup"); m_editGroupCommand.RaiseCanExecuteChanged(); }
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

        private RelayCommand m_autoSaveCommand;
        public ICommand AutoSaveCommand
        {
            get { return m_autoSaveCommand; }
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

        private RelayCommand m_editGroupCommand;
        public ICommand EditGroupCommand
        {
            get { return m_editGroupCommand; }
        }

        private void AddMark(ObservableCollection<MarkViewModel> param)
        {
            MarkViewModel mark = new MarkViewModel();
            EditMarkViewModel dialogViewModel = new EditMarkViewModel(mark);
            dialogViewModel.IsAddingMode = true;
            GlobalConfig.Dialogs.ShowDialog(m_ownerViewModel, dialogViewModel);
            if (dialogViewModel.Result == EditMarkViewModel.EditMarkResult.Ok)
            {
                param.Add(mark);
            }
            if (dialogViewModel.Result != EditMarkViewModel.EditMarkResult.Cancel) m_autoSaveCommand.Execute(null);
        }
        private void EditMark(ObservableCollection<MarkViewModel> param)
        {
            EditMarkViewModel dialogViewModel = new EditMarkViewModel(m_selectedMark);
            GlobalConfig.Dialogs.ShowDialog(m_ownerViewModel, dialogViewModel);
            if (dialogViewModel.Result == EditMarkViewModel.EditMarkResult.RemoveMark)
            {
                param.Remove(m_selectedMark);
            }
            if (dialogViewModel.Result != EditMarkViewModel.EditMarkResult.Cancel) m_autoSaveCommand.Execute(null);
        }
        private void AutoSave(object param)
        {
            if (GlobalConfig.Notifier.AutoSave) m_saveCommand.Execute(null);
        }
        private void Save(object param)
        {
            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                Directory.CreateDirectory(Path.GetDirectoryName(m_viewModel.Path));
                using (FileStream stream = new FileStream(m_viewModel.Path, System.IO.FileMode.Create))
                {
                    m_viewModel.Serialize(stream);
                }
            }
            , null, "Zapisywanie...");
            GlobalConfig.Dialogs.ShowDialog(m_ownerViewModel, dialogViewModel);
        }
        private void ShowGlobalStudentsList(object param)
        {
            GlobalStudentsListViewModel dialogViewModel = new GlobalStudentsListViewModel(m_viewModel.Students);
            dialogViewModel.NeedSave += (s, e) => { m_autoSaveCommand.Execute(null); };
            GlobalConfig.Dialogs.ShowDialog(m_ownerViewModel, dialogViewModel);
        }
        private void AddGroup(object param)
        {
            AddGroupViewModel dialogViewModel = new AddGroupViewModel(m_viewModel.Students);
            GlobalConfig.Dialogs.ShowDialog(m_ownerViewModel, dialogViewModel);
            if (dialogViewModel.Result != null)
            {
                m_viewModel.Groups.Add(dialogViewModel.Result);
                SelectedGroup = dialogViewModel.Result;
                m_autoSaveCommand.Execute(null);
            }
        }
        private void EditGroup(object param)
        {
            EditGroupViewModel dialogViewModel = new EditGroupViewModel(m_selectedGroup, m_viewModel.Students);
            GlobalConfig.Dialogs.ShowDialog(m_ownerViewModel, dialogViewModel);
            if (dialogViewModel.Result == EditGroupViewModel.EditGroupResult.RemoveGroup)
            {
                m_viewModel.Groups.Remove(m_selectedGroup);
                SelectedGroup = null;
                m_autoSaveCommand.Execute(null);
            }
        }
        private bool CanEditGroup(object param)
        {
            return m_selectedGroup != null;
        }
    }
}
