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
            m_showArchivesListCommand = new RelayCommand(ShowArchivesList, CanShowArchivesList);
            m_changePasswordCommand = new RelayCommand(ChangePassword, CanChangePassword);
            m_editSoundNotificationsCommand = new RelayCommand(EditSoundNotifications);

            m_openedSchoolClasses = openedSchoolClasses;
        }

        private RelayCommand m_closeCommand;
        public ICommand CloseCommand
        {
            get { return m_closeCommand; }
        }

        private RelayCommand m_showArchivesListCommand;
        public ICommand ShowArchivesListCommand
        {
            get { return m_showArchivesListCommand; }
        }

        private RelayCommand m_editSoundNotificationsCommand;
        public ICommand EditSoundNotificationsCommand
        {
            get { return m_editSoundNotificationsCommand; }
        }

        private RelayCommand m_changePasswordCommand;
        public ICommand ChangePasswordCommand
        {
            get { return m_changePasswordCommand; }
        }

        private ObservableCollection<SchoolClassControlViewModel> m_openedSchoolClasses;
        public ObservableCollection<SchoolClassControlViewModel> OpenedSchoolClasses
        {
            get { return m_openedSchoolClasses; }
            set { m_openedSchoolClasses = value; RaisePropertyChanged("OpenedSchoolClasses"); }
        }

        public string DatabasesDirectory
        {
            get { return GlobalConfig.Notifier.DatabasesDirectory; }
            set { GlobalConfig.Notifier.DatabasesDirectory = value; RaisePropertyChanged("DatabasesDirectory"); GlobalConfig.Dialogs.Close(this); }
        }

        private void Close(object e)
        {
            if (e == null) // if window is closed by X icon, e will be not null
            {
                GlobalConfig.Dialogs.Close(this);
            }
        }

        private void SaveRegistry()
        {
            ActionDialogViewModel saveDialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                if (!GlobalConfig.Main.BlockSaving) GlobalConfig.Notifier.SaveRegistry();
            }
            , null, "Zapisywanie ustawień do rejestru...");
            GlobalConfig.Dialogs.ShowDialog(this, saveDialogViewModel);
        }
        
        private void ShowArchivesList(object e)
        {
            ObservableCollection<ArchivesListViewModel.ArchiveInfo> archives = ArchivesListViewModel.LoadArchives(this);
            ArchivesListViewModel dialogViewModel = new ArchivesListViewModel(archives);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == ArchivesListViewModel.ArchivesListResult.Preview)
            {
                GlobalConfig.Main.SaveCommand.Execute(null);
                GlobalConfig.Main.BlockSaving = true;
                GlobalConfig.Main.OriginalDatabasePath = GlobalConfig.Notifier.DatabasesDirectory;
                GlobalConfig.Notifier.DatabasesDirectory = System.IO.Path.GetTempPath() + @"\Dziennik_" + Guid.NewGuid().ToString().Replace('-', '_');
                GlobalConfig.CreateDirectoriesIfNotExists();
                MainViewModel.UnpackArchive(GlobalConfig.Main, dialogViewModel.SelectedArchive.Path, GlobalConfig.Notifier.DatabasesDirectory + @"\" + GlobalConfig.CurrentDatabaseSubdirectory);
                GlobalConfig.Dialogs.Close(this);
            }
            else if (dialogViewModel.Result == ArchivesListViewModel.ArchivesListResult.Restore)
            {
                GlobalConfig.Main.BlockSaving = true;
                GlobalConfig.Main.SaveCommand.Execute(null);
                GlobalConfig.Main.ArchiveDatabaseCommand.Execute(string.Format(GlobalConfig.GetStringResource("lang_BeforeRestoringFromFormat"), dialogViewModel.SelectedArchive.Date.ToString(GlobalConfig.DateTimeWithSecondsFormat)));
                string unpackPath = GlobalConfig.Notifier.DatabasesDirectory + @"\" + GlobalConfig.CurrentDatabaseSubdirectory;
                Ext.ClearDirectory(unpackPath);
                MainViewModel.UnpackArchive(GlobalConfig.Main, dialogViewModel.SelectedArchive.Path, unpackPath);
                GlobalConfig.Main.Reload();
                GlobalConfig.Main.BlockSaving = false;
                GlobalConfig.Dialogs.Close(this);
            }
        }
        private bool CanShowArchivesList(object param)
        {
            return !GlobalConfig.Main.BlockSaving;
        }
        private void ChangePassword(object param)
        {
            ChangePasswordViewModel dialogViewModel = new ChangePasswordViewModel();
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result != ChangePasswordViewModel.ChangePasswordResult.Cancel) SaveRegistry();
        }
        private bool CanChangePassword(object param)
        {
            return !GlobalConfig.Main.BlockSaving;
        }
        private void EditSoundNotifications(object param)
        {
            SoundNotificationViewModel dialogViewModel = new SoundNotificationViewModel();
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
        }
    }
}
