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
        public SchoolClassControlViewModel()
            : this(new SchoolClassViewModel())
        {
        }
        public SchoolClassControlViewModel(SchoolClassViewModel viewModel)
        {
            m_addMarkCommand = new RelayCommand<ObservableCollection<MarkViewModel>>(AddMark);
            m_editMarkCommand = new RelayCommand<ObservableCollection<MarkViewModel>>(EditMark);
            m_autoSaveCommand = new RelayCommand(AutoSave);
            m_saveCommand = new RelayCommand(Save);

            m_viewModel = viewModel;
        }

        private SchoolClassViewModel m_viewModel;
        public SchoolClassViewModel ViewModel
        {
            get { return m_viewModel; }
            set { m_viewModel = value; OnPropertyChanged("ViewModel"); }
        }

        private SchoolGroupViewModel m_selectedGroup;
        public SchoolGroupViewModel SelectedGroup
        {
            get { return m_selectedGroup; }
            set { m_selectedGroup = value; OnPropertyChanged("SelectedGroup"); }
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

        private void AddMark(ObservableCollection<MarkViewModel> param)
        {
            MarkViewModel mark = new MarkViewModel();
            EditMarkViewModel dialogViewModel = new EditMarkViewModel(mark);
            dialogViewModel.IsAddingMode = true;
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Main, dialogViewModel);
            if (dialogViewModel.Result == EditMarkViewModel.EditMarkResult.Ok)
            {
                param.Add(mark);
            }
            if (dialogViewModel.Result != EditMarkViewModel.EditMarkResult.Cancel) m_autoSaveCommand.Execute(null);
        }
        private void EditMark(ObservableCollection<MarkViewModel> param)
        {
            EditMarkViewModel dialogViewModel = new EditMarkViewModel(m_selectedMark);
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Main, dialogViewModel);
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
            GlobalConfig.Dialogs.ShowDialog((param == null ? GlobalConfig.Main : param), dialogViewModel);
        }
        
    }
}
