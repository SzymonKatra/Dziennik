using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Dziennik.CommandUtils;
using Dziennik.ViewModel;

namespace Dziennik.View
{
    public class OptionsViewModel : ObservableObject
    {
        public OptionsViewModel(ObservableCollection<SchoolClassControlViewModel> openedSchoolClasses)
        {
            m_closeCommand = new RelayCommand(Close);
            m_editClassCommand = new RelayCommand(EditClass, CanEditClass);
            m_addClassCommand = new RelayCommand(AddClass);
            m_editCalendarCommand = new RelayCommand(EditCalendar, CanEditCalendar);
            m_addCalendarCommand = new RelayCommand(AddCalendar);
            m_showCalendarsListCommand = new RelayCommand(ShowCalendarsList);
            m_editMarksCategoryCommand = new RelayCommand(EditMarksCategory, CanEditMarksCategory);
            m_addMarksCategoryCommand = new RelayCommand(AddMarksCategory);
            m_showNoticesListCommand = new RelayCommand(ShowNoticesList);

            m_openedSchoolClasses = openedSchoolClasses;
        }

        private RelayCommand m_closeCommand;
        public ICommand CloseCommand
        {
            get { return m_closeCommand; }
        }
        private RelayCommand m_editClassCommand;
        public ICommand EditClassCommand
        {
            get { return m_editClassCommand; }
        }
        private RelayCommand m_addClassCommand;
        public ICommand AddClassCommand
        {
            get { return m_addClassCommand; }
        }
        private RelayCommand m_editCalendarCommand;
        public ICommand EditCalendarCommand
        {
            get { return m_editCalendarCommand; }
        }

        private RelayCommand m_addCalendarCommand;
        public ICommand AddCalendarCommand
        {
            get { return m_addCalendarCommand; }
        }

        private RelayCommand m_showCalendarsListCommand;
        public ICommand ShowCalendarsListCommand
        {
            get { return m_showCalendarsListCommand; }
        }

        private RelayCommand m_editMarksCategoryCommand;
        public ICommand EditMarksCategoryCommand
        {
            get { return m_editMarksCategoryCommand; }
        }

        private RelayCommand m_addMarksCategoryCommand;
        public ICommand AddMarksCategoryCommand
        {
            get { return m_addMarksCategoryCommand; }
        }

        private RelayCommand m_showNoticesListCommand;
        public ICommand ShowNoticesListCommand
        {
            get { return m_showNoticesListCommand; }
        }

        private ObservableCollection<SchoolClassControlViewModel> m_openedSchoolClasses;
        public ObservableCollection<SchoolClassControlViewModel> OpenedSchoolClasses
        {
            get { return m_openedSchoolClasses; }
            set { m_openedSchoolClasses = value; RaisePropertyChanged("OpenedSchoolClasses"); }
        }

        private SchoolClassControlViewModel m_selectedClass;
        public SchoolClassControlViewModel SelectedClass
        {
            get { return m_selectedClass; }
            set { m_selectedClass = value; RaisePropertyChanged("SelectedClass"); m_editClassCommand.RaiseCanExecuteChanged(); }
        }

        private CalendarViewModel m_selectedCalendar;
        public CalendarViewModel SelectedCalendar
        {
            get { return m_selectedCalendar; }
            set { m_selectedCalendar = value; RaisePropertyChanged("SelectedCalendar"); m_editCalendarCommand.RaiseCanExecuteChanged(); }
        }

        private MarksCategoryViewModel m_selectedMarksCategory;
        public MarksCategoryViewModel SelectedMarksCategory
        {
            get { return m_selectedMarksCategory; }
            set { m_selectedMarksCategory = value; RaisePropertyChanged("SelectedMarksCategory"); m_editMarksCategoryCommand.RaiseCanExecuteChanged(); }
        }

        private void Close(object e)
        {
            if (e == null) // if window is closed by X icon, e will be not null
            {
                GlobalConfig.Dialogs.Close(this);
            }
        }
        private void EditClass(object e)
        {
            SchoolClassControlViewModel tab = m_selectedClass;
            EditClassViewModel dialogViewModel = new EditClassViewModel(tab.ViewModel, tab.AutoSaveCommand);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditClassViewModel.EditClassResult.RemoveClass)
            {
                //GlobalConfig.Database.SchoolClasses.Remove(m_selectedClass.ViewModel.Model);
                SelectedClass = null;
                m_openedSchoolClasses.Remove(tab);
                return;
            }
            if (dialogViewModel.Result != EditClassViewModel.EditClassResult.Cancel) m_selectedClass.AutoSaveCommand.Execute(this);
        }
        private bool CanEditClass(object e)
        {
            return m_selectedClass != null;
        }
        private void AddClass(object e)
        {
            SchoolClassViewModel schoolClass = new SchoolClassViewModel();
            EditClassViewModel dialogViewModel = new EditClassViewModel(schoolClass, new RelayCommand((x) => { }));
            dialogViewModel.IsAddingMode = true;
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditClassViewModel.EditClassResult.Ok)
            {
                //GlobalConfig.Database.SchoolClasses.Add(schoolClass.Model);
                SchoolClassControlViewModel tab = new SchoolClassControlViewModel(new DatabaseMain());
                tab.Database.ViewModel = schoolClass;
                tab.Database.Path = dialogViewModel.Path;
                m_openedSchoolClasses.Add(tab);
                SelectedClass = tab;
            }
            if (dialogViewModel.Result != EditClassViewModel.EditClassResult.Cancel) m_selectedClass.AutoSaveCommand.Execute(this);
        }
        private void EditCalendar(object e)
        {
            m_selectedCalendar.StartWorkingCopy();
            EditCalendarViewModel dialogViewModel = new EditCalendarViewModel(m_selectedCalendar, GlobalConfig.GlobalDatabaseAutoSaveCommand);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if(dialogViewModel.Result == EditCalendarViewModel.EditCalendarResult.Remove)
            {
                m_selectedCalendar.EndWorkingCopy(WorkingCopyResult.Ok);

                foreach (var schoolClass in m_openedSchoolClasses)
                {
                    if (schoolClass.ViewModel.Calendar == m_selectedCalendar) schoolClass.ViewModel.Calendar = null;
                }

                GlobalConfig.GlobalDatabase.ViewModel.Calendars.Remove(m_selectedCalendar);
                SelectedCalendar = null;
            }
            else if (dialogViewModel.Result== EditCalendarViewModel.EditCalendarResult.Ok)
            {
                m_selectedCalendar.EndWorkingCopy(WorkingCopyResult.Ok);
            }
            else if(dialogViewModel.Result == EditCalendarViewModel.EditCalendarResult.Cancel)
            {
                m_selectedCalendar.EndWorkingCopy(WorkingCopyResult.Cancel);
            }
            if (dialogViewModel.Result != EditCalendarViewModel.EditCalendarResult.Cancel)
            {
                GlobalConfig.GlobalDatabaseAutoSaveCommand.Execute(null);
                foreach (var schoolClass in m_openedSchoolClasses)
                {
                    if (schoolClass.ViewModel.Calendar == m_selectedCalendar)
                    {
                        foreach (var group in schoolClass.ViewModel.Groups)
                        {
                            foreach (var student in group.Students)
                            {
                                student.RaiseAttendanceChanged();
                            }
                        }
                    }
                }
            }

        }
        private bool CanEditCalendar(object e)
        {
            return m_selectedCalendar != null;
        }
        private void AddCalendar(object e)
        {
            CalendarViewModel calendar = new CalendarViewModel();
            EditCalendarViewModel dialogViewModel = new EditCalendarViewModel(calendar, GlobalConfig.GlobalDatabaseAutoSaveCommand, true);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditCalendarViewModel.EditCalendarResult.Ok)
            {
                GlobalConfig.GlobalDatabase.ViewModel.Calendars.Add(calendar);
            }
            if (dialogViewModel.Result != EditCalendarViewModel.EditCalendarResult.Cancel) GlobalConfig.GlobalDatabaseAutoSaveCommand.Execute(null);
        }
        private void ShowCalendarsList(object e)
        {
            GlobalCalendarListViewModel dialogViewModel = new GlobalCalendarListViewModel(GlobalConfig.GlobalDatabase.ViewModel.Calendars, m_openedSchoolClasses, GlobalConfig.GlobalDatabaseAutoSaveCommand);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }
        private void EditMarksCategory(object e)
        {
            EditMarksCategoryViewModel dialogViewModel = new EditMarksCategoryViewModel(m_selectedMarksCategory);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if(dialogViewModel.Result == EditMarksCategoryViewModel.EditMarkCategoryResult.RemoveCategory)
            {
                foreach (var schoolClass in m_openedSchoolClasses)
                {
                    foreach (var schoolGroup in schoolClass.ViewModel.Groups)
                    {
                        foreach (var student in schoolGroup.Students)
                        {
                            foreach (var mark in student.FirstSemester.Marks)
                            {
                                if (mark.Category == m_selectedMarksCategory) mark.Category = null;
                            }
                            foreach (var mark in student.SecondSemester.Marks)
                            {
                                if (mark.Category == m_selectedMarksCategory) mark.Category = null;
                            }
                        }
                    }
                }

                GlobalConfig.GlobalDatabase.ViewModel.MarksCategories.Remove(m_selectedMarksCategory);
                SelectedMarksCategory = null;
            }
            if (dialogViewModel.Result != EditMarksCategoryViewModel.EditMarkCategoryResult.Cancel) GlobalConfig.GlobalDatabaseAutoSaveCommand.Execute(null);
        }
        private bool CanEditMarksCategory(object e)
        {
            return m_selectedMarksCategory != null;
        }
        private void AddMarksCategory(object e)
        {
            MarksCategoryViewModel marksCategory = new MarksCategoryViewModel();
            EditMarksCategoryViewModel dialogViewModel = new EditMarksCategoryViewModel(marksCategory, true);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditMarksCategoryViewModel.EditMarkCategoryResult.Ok)
            {
                GlobalConfig.GlobalDatabase.ViewModel.MarksCategories.Add(marksCategory);
            }
            if (dialogViewModel.Result != EditMarksCategoryViewModel.EditMarkCategoryResult.Cancel) GlobalConfig.GlobalDatabaseAutoSaveCommand.Execute(null);
        }
        private void ShowNoticesList(object e)
        {
            NoticesListViewModel dialogViewModel = new NoticesListViewModel();
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }
    }
}
