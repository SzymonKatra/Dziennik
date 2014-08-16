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
        public GlobalSubjectsListViewModel(ObservableCollection<GlobalSubjectViewModel> subjects)
        {
            m_addSubjectCommand = new RelayCommand(AddSubject);
            m_autoAddSubjectsClipboardCommand = new RelayCommand(AutoAddSubjectsClipboard);
            m_editSubjectCommand = new RelayCommand(EditSubject);

            m_subjects = subjects;
        }

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
            EditGlobalSubjectViewModel dialogViewModel = new EditGlobalSubjectViewModel(subject, m_subjects, true);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditGlobalSubjectViewModel.EditGlobalSubjectResult.Ok)
            {
                m_subjects.Add(subject);
            }
        }
        private void AutoAddSubjectsClipboard(object e)
        {
            if (GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_DoYouWantToContinue"), MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;
            int currentNumber = GetNextSubjectNumber(m_subjects);
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
                        subject.Number = currentNumber++;
                        subject.Name = line;
                        m_subjects.Add(subject);

                        ++added;
                    }

                    GlobalConfig.MessageBox(this, string.Format(GlobalConfig.GetStringResource("lang_AddedSubjectsFormat"), added), MessageBoxSuperPredefinedButtons.OK);
                }
            }
            catch
            {
                GlobalConfig.MessageBox(this, string.Format("{1}{0}{2}", Environment.NewLine, GlobalConfig.GetStringResource("lang_ErrorOccurredWhileAddingSubjects"), GlobalConfig.GetStringResource("lang_CheckClipboardForValidList")), MessageBoxSuperPredefinedButtons.OK);
            }
        }
        private void EditSubject(object e)
        {
            m_selectedSubject.PushCopy();
            EditGlobalSubjectViewModel dialogViewModel = new EditGlobalSubjectViewModel(m_selectedSubject, m_subjects);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditGlobalSubjectViewModel.EditGlobalSubjectResult.Remove)
            {
                m_selectedSubject.PopCopy(WorkingCopyResult.Ok);
                m_subjects.Remove(m_selectedSubject);
                SelectedSubject = null;
            }
            else if (dialogViewModel.Result == EditGlobalSubjectViewModel.EditGlobalSubjectResult.Ok)
            {
                m_selectedSubject.PopCopy(WorkingCopyResult.Ok);
            }
            else if(dialogViewModel.Result== EditGlobalSubjectViewModel.EditGlobalSubjectResult.Cancel)
            {
                m_selectedSubject.PopCopy(WorkingCopyResult.Cancel);
            }
        }

        public static int GetNextSubjectNumber(IEnumerable<GlobalSubjectViewModel> subjects)
        {
            int highestNumber = 0;
            foreach (GlobalSubjectViewModel sub in subjects)
            {
                if (sub.Number > highestNumber) highestNumber = sub.Number;
            }

            return highestNumber + 1;
        }
    }
}
