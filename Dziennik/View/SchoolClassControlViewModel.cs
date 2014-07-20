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
            : this(new DatabaseMain())
        {
        }
        public SchoolClassControlViewModel(DatabaseMain database)
        {
            m_addMarkCommand = new RelayCommand<ObservableCollection<MarkViewModel>>(AddMark);
            m_editMarkCommand = new RelayCommand<ObservableCollection<MarkViewModel>>(EditMark);
            m_editEndingMarkCommand = new RelayCommand<string>(EditEndingMark);
            m_autoSaveCommand = new RelayCommand(AutoSave);
            m_saveCommand = new RelayCommand(Save);

            m_database = database;
        }

        private DatabaseMain m_database;
        public DatabaseMain Database
        {
            get { return m_database; }
        }

        public SchoolClassViewModel ViewModel
        {
            get { return m_database.ViewModel; }
            set { m_database.ViewModel = value; RaisePropertyChanged("ViewModel"); }
        }

        private SchoolGroupViewModel m_selectedGroup;
        public SchoolGroupViewModel SelectedGroup
        {
            get { return m_selectedGroup; }
            set { m_selectedGroup = value; RaisePropertyChanged("SelectedGroup"); }
        }

        private StudentInGroupViewModel m_selectedStudent;
        public StudentInGroupViewModel SelectedStudent
        {
            get { return m_selectedStudent; }
            set { m_selectedStudent = value; RaisePropertyChanged("SelectedStudent"); }
        }

        private MarkViewModel m_selectedMark;
        public MarkViewModel SelectedMark
        {
            get { return m_selectedMark; }
            set { m_selectedMark = value; RaisePropertyChanged("SelectedMark"); }
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

        private RelayCommand<string> m_editEndingMarkCommand;
        public ICommand EditEndingMarkCommand
        {
            get { return m_editEndingMarkCommand; }
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
        private void EditEndingMark(string param)
        {
            decimal initialMark;
            decimal averageMark;

            if (param == "half")
            {
                initialMark = m_selectedStudent.HalfEndingMark;
                averageMark = m_selectedStudent.FirstSemester.AverageMark;
            }
            else
            {
                initialMark = m_selectedStudent.YearEndingMark;
                averageMark = m_selectedStudent.AverageMarkAll;
            }

            EditEndingMarkViewModel dialogViewModel = new EditEndingMarkViewModel(initialMark, averageMark);
            GlobalConfig.Dialogs.ShowDialog(GlobalConfig.Main, dialogViewModel);

            if (dialogViewModel.Result == EditEndingMarkViewModel.EditEndingMarkResult.Ok)
            {
                if (param == "half")
                {
                    m_selectedStudent.HalfEndingMark = dialogViewModel.Mark;
                }
                else
                {
                    m_selectedStudent.YearEndingMark = dialogViewModel.Mark;
                }
            }
            if (dialogViewModel.Result != EditEndingMarkViewModel.EditEndingMarkResult.Cancel) m_autoSaveCommand.Execute(null);
        }
        private void AutoSave(object param)
        {
            if (GlobalConfig.Notifier.AutoSave) m_saveCommand.Execute(null);
        }
        private void Save(object param)
        {
            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                m_database.Save();
            }
            , null, "Zapisywanie...");
            GlobalConfig.Dialogs.ShowDialog((param == null ? GlobalConfig.Main : param), dialogViewModel);
        }
        
    }
}
