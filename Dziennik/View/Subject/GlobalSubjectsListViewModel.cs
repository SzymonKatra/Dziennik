using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using System.Windows.Input;
using Dziennik.CommandUtils;
using System.Windows;
using Dziennik.Controls;

namespace Dziennik.View
{
    public sealed class GlobalSubjectsListViewModel : ObservableObject
    {
        public GlobalSubjectsListViewModel(ObservableCollection<GlobalSubjectViewModel> subjects, ICommand autoSaveCommand)
        {
            m_addSubjectCommand = new RelayCommand(AddSubject);
            m_autoAddSubjectsClipboardCommand = new RelayCommand(AutoAddSubjectsClipboard);
            m_editSubjectCommand = new RelayCommand(EditSubject);

            m_autoSaveCommand = autoSaveCommand;

            m_subjects = subjects;
        }

        private ICommand m_autoSaveCommand;
        private RelayCommand m_addSubjectCommand;
        public ICommand AddSubjectCommand
        {
            get { return m_addSubjectCommand; }
        }
        private RelayCommand m_autoAddSubjectsClipboardCommand;
        public ICommand AutoAddSubjectsClipboardCommand
        {
            get { return m_autoAddSubjectsClipboardCommand; }
        }
        private RelayCommand m_editSubjectCommand;
        public ICommand EditSubjectCommand
        {
            get { return m_editSubjectCommand; }
        }

        private ObservableCollection<GlobalSubjectViewModel> m_subjects;
        public ObservableCollection<GlobalSubjectViewModel> Subjects
        {
            get { return m_subjects; }
            set { m_subjects = value; RaisePropertyChanged("Subjects"); }
        }

        private GlobalSubjectViewModel m_selectedSubject;
        public GlobalSubjectViewModel SelectedSubject
        {
            get { return m_selectedSubject; }
            set { m_selectedSubject = value; RaisePropertyChanged("SelectedSubject"); }
        }

        private void AddSubject(object e)
        {
            GlobalSubjectViewModel subject = new GlobalSubjectViewModel();
            subject.Number = GetNextSubjectId();
            EditGlobalSubjectViewModel dialogViewModel = new EditGlobalSubjectViewModel(subject, true);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditGlobalSubjectViewModel.EditGlobalSubjectResult.Ok)
            {
                m_subjects.Add(subject);
            }
            if (dialogViewModel.Result != EditGlobalSubjectViewModel.EditGlobalSubjectResult.Cancel) m_autoSaveCommand.Execute(null);
        }
        private void AutoAddSubjectsClipboard(object e)
        {
            if (MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Czy chcesz kontynuować?", "Dziennik", MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;
            TypeSubjectCategoryViewModel dialogViewModel = new TypeSubjectCategoryViewModel();
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result != TypeSubjectCategoryViewModel.TypeSubjectCategoryResult.Ok) return;
            string category = dialogViewModel.Category;
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

                        GlobalSubjectViewModel subject = new GlobalSubjectViewModel();
                        subject.Number = GetNextSubjectId();
                        subject.Category = category;
                        subject.Name = line;
                        m_subjects.Add(subject);

                        ++added;
                    }

                    MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Dodano " + added + " tematów", "Dziennik", MessageBoxSuperPredefinedButtons.OK);
                }
            }
            catch
            {
                MessageBoxSuper.ShowBox(GlobalConfig.Dialogs.GetWindow(this), "Wystąpił błąd podczas dodawania tematów" + Environment.NewLine + "Sprawdź czy schowek zawiera prawidłowy format listy", "Dziennik", MessageBoxSuperPredefinedButtons.OK);
            }
        }
        private void EditSubject(object e)
        {
            EditGlobalSubjectViewModel dialogViewModel = new EditGlobalSubjectViewModel(m_selectedSubject);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditGlobalSubjectViewModel.EditGlobalSubjectResult.Remove)
            {
                m_subjects.Remove(m_selectedSubject);
                SelectedSubject = null;
            }
            if (dialogViewModel.Result != EditGlobalSubjectViewModel.EditGlobalSubjectResult.Cancel) m_autoSaveCommand.Execute(null);
        }

        private int GetNextSubjectId()
        {
            return (m_subjects.Count > 0 ? m_subjects[m_subjects.Count - 1].Number + 1 : 1);
        }
    }
}
