using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.ViewModel;
using Dziennik.CommandUtils;
using System.Windows.Input;

namespace Dziennik.View
{
    public sealed class SelectGlobalSubjectViewModel : ObservableObject
    {
        public enum SelectedGlobalSubjectResult
        {
            Ok,
            Cancel,
        }

        public class SubjectCategory
        {
            public string Name { get; set; }

            public SubjectCategory(string name)
            {
                Name = name;
            }
        }

        public SelectGlobalSubjectViewModel(ObservableCollection<GlobalSubjectViewModel> subjects)
        {
            m_okCommand = new RelayCommand(Ok);
            m_cancelCommand = new RelayCommand(Cancel);

            m_subjects = subjects;

            m_categories = new ObservableCollection<string>();
            foreach (GlobalSubjectViewModel sub in subjects)
            {
                string result = m_categories.FirstOrDefault(x => CheckCategory(x, sub.Category));
                if (result == null) m_categories.Add((sub.Category == null ? string.Empty : sub.Category));
            }
            m_categories.Sort();
        }

        private SelectedGlobalSubjectResult m_result = SelectedGlobalSubjectResult.Cancel;
        public SelectedGlobalSubjectResult Result
        {
            get { return m_result; }
        }

        private ObservableCollection<GlobalSubjectViewModel> m_subjects;

        private ObservableCollection<string> m_categories;
        private ObservableCollection<string> Categories
        {
            get { return m_categories; }
            set { m_categories = value; RaisePropertyChanged("Categories"); }
        }

        private string m_selectedCategory;
        public string SelectedCategory
        {
            get { return m_selectedCategory; }
            set { m_selectedCategory = value; RaisePropertyChanged("SelectedCategory"); UpdateDisplayedSubjects(); }
        }

        private ObservableCollection<GlobalSubjectViewModel> m_displayedSubjects;
        public ObservableCollection<GlobalSubjectViewModel> DisplayedSubjects
        {
            get { return m_displayedSubjects; }
            set { m_displayedSubjects = value; RaisePropertyChanged("DisplayedSubjects"); }
        }

        private GlobalSubjectViewModel m_selectedSubject;
        public GlobalSubjectViewModel SelectedSubject
        {
            get { return m_selectedSubject; }
            set { m_selectedSubject = value; RaisePropertyChanged("SelectedSubject"); }
        }

        private RelayCommand m_okCommand;
        public ICommand OkCommand
        {
            get { return m_okCommand; }
        }
        private RelayCommand m_cancelCommand;
        public ICommand CancelCommand
        {
            get { return m_cancelCommand; }
        }

        private void Ok(object e)
        {
            m_result = SelectedGlobalSubjectResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private void Cancel(object e)
        {
            m_result = SelectedGlobalSubjectResult.Cancel;
            GlobalConfig.Dialogs.Close(this);
        }

        private void UpdateDisplayedSubjects()
        {
            m_displayedSubjects.Clear();
            if (m_selectedCategory == null) return;

            foreach (GlobalSubjectViewModel subject in m_subjects)
            {
                if (CheckCategory(subject.Category, m_selectedCategory)) m_displayedSubjects.Add(subject);
            }
        }

        private bool CheckCategory(string category, string categoryToCompare)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return string.IsNullOrWhiteSpace(categoryToCompare);
            }
            else return category == categoryToCompare;
        }
    }
}
