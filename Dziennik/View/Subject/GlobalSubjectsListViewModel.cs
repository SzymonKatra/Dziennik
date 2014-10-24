using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Dziennik.ViewModel;
using System.Windows.Input;
using Dziennik.CommandUtils;
using System.Windows;
using Dziennik.Controls;

namespace Dziennik.View
{
    public sealed class GlobalSubjectsListViewModel : ObservableObject
    {
        public GlobalSubjectsListViewModel(ObservableCollection<GlobalSubjectViewModel> subjects, IEnumerable<GlobalSubjectViewModel> availableSubjects)
        {
            m_addSubjectCommand = new RelayCommand(AddSubject);
            m_autoAddSubjectsClipboardCommand = new RelayCommand(AutoAddSubjectsClipboard);
            m_editSubjectCommand = new RelayCommand(EditSubject);
            m_addFromAnotherGroupCommand = new RelayCommand(AddFromAnotherGroup);

            m_subjects = subjects;
            m_availableSubjects = new List<GlobalSubjectViewModel>(availableSubjects);
        }

        private RelayCommand m_addSubjectCommand;

        private List<GlobalSubjectViewModel> m_availableSubjects; 

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
        private RelayCommand m_addFromAnotherGroupCommand;
        public ICommand AddFromAnotherGroupCommand
        {
            get { return m_addFromAnotherGroupCommand; }
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
            EditGlobalSubjectViewModel dialogViewModel = new EditGlobalSubjectViewModel(subject, m_subjects, GetMinNumber(), GetMaxNumber() + 1, true);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditGlobalSubjectViewModel.EditGlobalSubjectResult.Ok)
            {
                GlobalSubjectViewModel found = m_subjects.FirstOrDefault(x => x.Number == subject.Number);
                if (found != null)
                {
                    int index = m_subjects.IndexOf(found);
                    for (int i = index; i < m_subjects.Count; i++)
                    {
                        m_subjects[i].Number++;
                    }

                    m_subjects.Insert(index, subject);
                    m_availableSubjects.Insert(index, subject);
                }
                else
                {
                    m_subjects.Add(subject);
                    m_availableSubjects.Add(subject);
                }
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
                        m_availableSubjects.Add(subject);

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
            bool isAvailable = m_availableSubjects.Contains(m_selectedSubject);
            int oldNumber = m_selectedSubject.Number;

            m_selectedSubject.PushCopy();
            EditGlobalSubjectViewModel dialogViewModel = new EditGlobalSubjectViewModel(m_selectedSubject, m_subjects, GetMinNumber(), GetMaxNumber());
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditGlobalSubjectViewModel.EditGlobalSubjectResult.Remove)
            {
                if (!isAvailable)
                {
                    m_selectedSubject.PopCopy(WorkingCopyResult.Cancel);
                    GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_CannotRemoveRealizedSubject"), MessageBoxSuperPredefinedButtons.OK);
                }
                else
                {
                    m_selectedSubject.PopCopy(WorkingCopyResult.Ok);
                    int index = m_subjects.IndexOf(m_selectedSubject);
                    int currentNumber = m_selectedSubject.Number;
                    m_subjects.Remove(m_selectedSubject);
                    for (int i = index; i < m_subjects.Count; i++)
                    {
                        m_subjects[i].Number=currentNumber;
                        currentNumber++;
                    }
                    SelectedSubject = null;
                }
            }
            else if (dialogViewModel.Result == EditGlobalSubjectViewModel.EditGlobalSubjectResult.Ok)
            {
                m_selectedSubject.PopCopy(WorkingCopyResult.Ok);

                if (oldNumber != m_selectedSubject.Number)
                {
                    if (!isAvailable)
                    {
                        m_selectedSubject.Number = oldNumber;
                        GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_CannotMoveRealizedSubject"), MessageBoxSuperPredefinedButtons.OK);
                        return;
                    }

                    GlobalSubjectViewModel temp = m_selectedSubject;

                    int newIndex = m_subjects.IndexOf(m_subjects.First(x => x.Number == temp.Number));
                    m_subjects.Remove(temp);
                    m_subjects.Insert(newIndex, temp);

                    int currentNumber = temp.Number + 1;
                    for (int i = newIndex + 1; i < m_subjects.Count; i++)
                    {
                        m_subjects[i].Number = currentNumber;
                        currentNumber++;
                    }
                }
            }
            else if (dialogViewModel.Result == EditGlobalSubjectViewModel.EditGlobalSubjectResult.Cancel)
            {
                m_selectedSubject.PopCopy(WorkingCopyResult.Cancel);
            }
        }
        private void AddFromAnotherGroup(object param)
        {
            SelectGroupViewModel dialogViewModel = new SelectGroupViewModel(GlobalConfig.Main.OpenedSchoolClasses);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == SelectGroupViewModel.SelectGroupResult.Cancel || dialogViewModel.SelectedGroup == null) return;

            if (GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_DoYouWantToContinue"), MessageBoxSuperPredefinedButtons.YesNo) != MessageBoxSuperButton.Yes) return;

            int currentNumber = GetNextSubjectNumber(m_subjects);
            int added = 0;
            foreach (var item in dialogViewModel.SelectedGroup.GlobalSubjects)
            {
                GlobalSubjectViewModel newSubject = new GlobalSubjectViewModel
                {
                    Number = currentNumber,
                    Name = item.Name
                };
                currentNumber++;
                m_subjects.Add(newSubject);
                m_availableSubjects.Add(newSubject);
                added++;
            }

            GlobalConfig.MessageBox(this, string.Format(GlobalConfig.GetStringResource("lang_AddedSubjectsFormat"), added), MessageBoxSuperPredefinedButtons.OK);
        }

        private int GetMinNumber()
        {
            if (m_availableSubjects.Count > 0)
                return m_availableSubjects[0].Number;

            return GetNextSubjectNumber(m_subjects);
        }
        private int GetMaxNumber()
        {
            if (m_availableSubjects.Count > 0)
                return m_availableSubjects[m_availableSubjects.Count - 1].Number;

            return GetNextSubjectNumber(m_subjects);
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
