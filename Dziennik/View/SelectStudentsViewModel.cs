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

            public int Id
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
                RaisePropertyChanged("Id");
                RaisePropertyChanged("Name");
                RaisePropertyChanged("Surname");
                RaisePropertyChanged("Email");
                RaisePropertyChanged("AdditionalInformation");
            }
        }

        public SelectStudentsViewModel(ObservableCollection<GlobalStudentViewModel> toSelect, List<int> initialSelection)
        {
            ObservableCollection<Selection> innerCopy = new ObservableCollection<Selection>();
            foreach(GlobalStudentViewModel global in toSelect)
            {
                innerCopy.Add(new Selection() { Global = global });
            }
            Initialize(innerCopy, initialSelection);
        }
        public SelectStudentsViewModel(ObservableCollection<StudentInGroupViewModel> toSelect, List<int> initialSelection)
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
                    Selection sel = toSelect.FirstOrDefault((x) => { return x.Id == already; });
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
                    if (sel.Selected) selected.Add(sel.Id);
                }

                return selected;
            }
        }
        public string ResultSelectionString
        {
            get
            {
                List<int> selected = ResultSelection;

                string result = string.Empty;

                int startRange = -1;
                int endRange = -1;

                foreach (int sel in selected)
                {
                    if (endRange < 0)
                    {
                        startRange = endRange = sel;
                    }
                    else
                    {
                        if (sel == endRange + 1)
                        {
                            endRange++;
                        }
                        else
                        {
                            if (startRange != endRange)
                            {
                                result += startRange.ToString();
                                result += '-';
                                result += endRange.ToString();
                                result += "; ";
                            }
                            else
                            {
                                result += endRange.ToString();
                                result += "; ";
                            }

                            startRange = endRange = sel;
                        }
                    }
                }

                if (endRange >= 0)
                {
                    if (startRange != endRange)
                    {
                        result += startRange.ToString();
                        result += '-';
                        result += endRange.ToString();
                        result += "; ";
                    }
                    else
                    {
                        result += endRange.ToString();
                        result += "; ";
                    }
                }

                return result;
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
        private void Cancel(object param)
        {
            m_result = false;
            GlobalConfig.Dialogs.Close(this);
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

        public static List<int> ParseSelection(string input)
        {
            string errorTemp;
            return ParseSelection(input, out errorTemp);
        }
        public static List<int> ParseSelection(string input, out string error)
        {
            List<int> result = new List<int>();
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(input)) return result;

            try
            {
                string toParse = input.Replace(" ", "");
                while (toParse[toParse.Length - 1] == ',' || toParse[toParse.Length - 1] == ';') toParse = toParse.Remove(toParse.Length - 1);
                if (string.IsNullOrWhiteSpace(toParse)) return result;

                string[] tokens = input.Split(',', ';');

                foreach (string item in tokens)
                {
                    if (string.IsNullOrWhiteSpace(item)) continue;
                    int valResult;
                    if (int.TryParse(item, out valResult))
                    {
                        result.Add(valResult);
                        continue;
                    }

                    string[] rangeStr = item.Split('-');
                    if (rangeStr.Length != 2)
                    {
                        error = "Nieprawidłowy format. Zakresy oddziel jednym myślnikiem(-)";
                        return null;
                    }

                    int minRange;
                    int maxRange;
                    if (!int.TryParse(rangeStr[0], out minRange) || !int.TryParse(rangeStr[1], out maxRange))
                    {
                        error = "Niedozwolone znaki";
                        return null;
                    }

                    for (int i = minRange; i <= maxRange; i++) result.Add(i);
                }
            }
            catch
            {
                Debug.Assert(true, "Exception in SelectStudentsViewModel.ParseSelection");
            }

            return result;
        }
    }
}
