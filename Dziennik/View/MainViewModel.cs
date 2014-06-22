using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Dziennik.Controls;
using Dziennik.CommandUtils;
using Dziennik.ViewModel;
using System.ComponentModel;

namespace Dziennik.View
{
    public sealed class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            m_saveAllCommand = new RelayCommand(SaveAll);
            m_closeTabCommand = new RelayCommand<SchoolClassControlViewModel>(CloseTab);

            m_openedSchoolClasses.Add(new SchoolClassControlViewModel(this));
            m_openedSchoolClasses.Add(new SchoolClassControlViewModel(this));
            m_openedSchoolClasses.Add(new SchoolClassControlViewModel(this));
        }

        private ObservableCollection<SchoolClassControlViewModel> m_openedSchoolClasses = new ObservableCollection<SchoolClassControlViewModel>();
        public ObservableCollection<SchoolClassControlViewModel> OpenedSchoolClasses
        {
            get { return m_openedSchoolClasses; }
            set { m_openedSchoolClasses = value; OnPropertyChanged("OpenedSchoolClasses"); }
        }

        private RelayCommand m_saveAllCommand;
        public ICommand SaveAllCommand
        {
            get { return m_saveAllCommand; }
        }
        private RelayCommand<SchoolClassControlViewModel> m_closeTabCommand;
        public ICommand CloseTabCommand
        {
            get { return m_closeTabCommand; }
        }

        private void SaveAll(object e)
        {
            foreach (SchoolClassControlViewModel tab in m_openedSchoolClasses) tab.SaveCommand.Execute(null);
        }
        private void CloseTab(SchoolClassControlViewModel e)
        {
            e.SaveCommand.Execute(null);
            m_openedSchoolClasses.Remove(e);
        }
    }
}
