using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Dziennik.ViewModel;
using Dziennik.CommandUtils;
using System.Windows.Input;

namespace Dziennik.View
{
    public sealed class SelectStudentsViewModel : ObservableObject
    {
        public sealed class Selection : ObservableObject
        {
            private bool m_selected = false;
            public bool Selected
            {
                get { return m_selected; }
                set { m_selected = value; RaisePropertyChanged("Selected"); }
            }

            private GlobalStudentViewModel m_global;
            public GlobalStudentViewModel Global
            {
                get { return m_global; }
                set { m_global = value; m_inGroup = null; AllChanged(); }
            }

            private StudentInGroupViewModel m_inGroup;
            public StudentInGroupViewModel InGroup
            {
                get { return m_inGroup; }
                set { m_inGroup = value; m_global = null; AllChanged(); }
            }

            public int Number
            {
                get
                {
                    return (m_global != null ? m_global.Number : m_inGroup.Number);
                }
            }
            public string Name
            {
                get
                {
                    return (m_global != null ? m_global.Name : m_inGroup.GlobalStudent.Name);
                }
            }
            public string Surname
            {
                get
                {
                    return (m_global != null ? m_global.Surname : m_inGroup.GlobalStudent.Surname);
                }
            }
            public string Email
            {
                get
                {
                    return (m_global != null ? m_global.Email : m_inGroup.GlobalStudent.Email);
                }
            }
            public string AdditionalInformation
            {
                get
                {
                    return (m_global != null ? m_global.AdditionalInformation : m_inGroup.GlobalStudent.AdditionalInformation);
                }
            }

            private void AllChanged()
            {
                RaisePropertyChanged("Global");
                RaisePropertyChanged("InGroup");
                RaisePropertyChanged("Number");
                RaisePropertyChanged("Name");
                RaisePropertyChanged("Surname");
                RaisePropertyChanged("Email");
                RaisePropertyChanged("AdditionalInformation");
            }
        }

        public SelectStudentsViewModel(IEnumerable<GlobalStudentViewModel> toSelect, List<int> initialSelection)
        {
            ObservableCollection<Selection> innerCopy = new ObservableCollection<Selection>();
            foreach(GlobalStudentViewModel global in toSelect)
            {
                innerCopy.Add(new Selection() { Global = global });
            }
            Initialize(innerCopy, initialSelection);
        }
        public SelectStudentsViewModel(IEnumerable<StudentInGroupViewModel> toSelect, List<int> initialSelection)
        {
            ObservableCollection<Selection> innerCopy = new ObservableCollection<Selection>();
            foreach (StudentInGroupViewModel inGroup in toSelect)
            {
                innerCopy.Add(new Selection() { InGroup = inGroup });
            }
            Initialize(innerCopy, initialSelection);
        }
        public SelectStudentsViewModel(ObservableCollection<Selection> toSelect, List<int> initialSelection)
        {
            Initialize(toSelect, initialSelection);
        }
        private void Initialize(ObservableCollection<Selection> toSelect, List<int> initialSelection)
        {
            m_okCommand = new RelayCommand(Ok);
            m_cancelCommand = new RelayCommand(Cancel);
            m_selectAllCommand = new RelayCommand(SelectAll);
            m_clearAllCommand = new RelayCommand(ClearAll);
            m_uncheckOtherSelectionsCommand = new RelayCommand<Selection>(UncheckOtherSelections, CanUncheckOtherSelections);

            m_toSelect = toSelect;
            if (initialSelection != null)
            {
                foreach (int already in initialSelection)
                {
                    Selection sel = toSelect.FirstOrDefault((x) => { return x.Number == already; });
                    if (sel != null) sel.Selected = true;
                }
            }
        }

        private bool m_result = false;
        public bool Result
        {
            get { return m_result; }
        }
        public List<int> ResultSelection
        {
            get
            {
                List<int> selected = new List<int>();

                foreach (Selection sel in m_toSelect)
                {
                    if (sel.Selected) selected.Add(sel.Number);
                }

                return selected;
            }
        }
        public string ResultSelectionString
        {
            get
            {
                return SelectionParser.Create(ResultSelection);
            }
        }

        private bool m_singleSelection = false;
        public bool SingleSelection
        {
            get { return m_singleSelection; }
            set
            {
                m_singleSelection = value;

                int selectedCount = m_toSelect.Count((x) => { return x.Selected; });
                if (selectedCount > 1)
                {
                    foreach (Selection sel in m_toSelect) sel.Selected = false;
                }
            }
        }

        private ObservableCollection<Selection> m_toSelect;
        public ObservableCollection<Selection> ToSelect
        {
            get { return m_toSelect; }
            set { m_toSelect = value; RaisePropertyChanged("ToSelect"); }
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

        private RelayCommand m_selectAllCommand;
        public ICommand SelectAllCommand
        {
            get { return m_selectAllCommand; }
        }

        private RelayCommand m_clearAllCommand;
        public ICommand ClearAllCommand
        {
            get { return m_clearAllCommand; }
        }

        private RelayCommand<Selection> m_uncheckOtherSelectionsCommand;
        public ICommand UncheckOtherSelectionsCommand
        {
            get { return m_uncheckOtherSelectionsCommand; }
        }

        private void Ok(object param)
        {
            m_result = true;
            GlobalConfig.Dialogs.Close(this);
        }
        private void Cancel(object e)
        {
            m_result = false;
            if (e == null)
            {
                GlobalConfig.Dialogs.Close(this);
            }
        }
        private void SelectAll(object param)
        {
            foreach (Selection sel in m_toSelect) sel.Selected = true;
        }
        private void ClearAll(object param)
        {
            foreach (Selection sel in m_toSelect) sel.Selected = false;
        }
        private void UncheckOtherSelections(Selection param)
        {
            foreach (Selection sel in m_toSelect)
            {
                if (sel == param) continue;
                sel.Selected = false;
            }
        }
        private bool CanUncheckOtherSelections(Selection param)
        {
            return m_singleSelection;
        }
    }
}
