using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.CommandUtils;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Dziennik.ViewModel;

namespace Dziennik.View
{
    public sealed class NoticesListViewModel : ObservableObject
    {
        public NoticesListViewModel()
        {
            m_addNoticeCommand = new RelayCommand(AddNotice);
            m_editNoticeCommand = new RelayCommand(EditNotice);
        }

        private RelayCommand m_addNoticeCommand;
        public ICommand AddNoticeCommand
        {
            get { return m_addNoticeCommand; }
        }

        private RelayCommand m_editNoticeCommand;
        public ICommand EditNoticeCommand
        {
            get { return m_editNoticeCommand; }
        }

        private NoticeViewModel m_selectedNotice;
        public NoticeViewModel SelectedNotice
        {
            get { return m_selectedNotice; }
            set { m_selectedNotice = value; RaisePropertyChanged("SelectedNotice"); }
        }

        private void AddNotice(object e)
        {
            NoticeViewModel notice = new NoticeViewModel();
            EditNoticeViewModel dialogViewModel = new EditNoticeViewModel(notice, true);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditNoticeViewModel.EditNoticeResult.Ok)
            {
                GlobalConfig.GlobalDatabase.ViewModel.Notices.Add(notice);
            }
            if (dialogViewModel.Result != EditNoticeViewModel.EditNoticeResult.Cancel) GlobalConfig.GlobalDatabaseAutoSaveCommand.Execute(null);
        }
        private void EditNotice(object e)
        {
            m_selectedNotice.PushCopy();
            EditNoticeViewModel dialogViewModel = new EditNoticeViewModel(m_selectedNotice);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if(dialogViewModel.Result == EditNoticeViewModel.EditNoticeResult.RemoveNotice)
            {
                m_selectedNotice.PopCopy(WorkingCopyResult.Ok);
                GlobalConfig.GlobalDatabase.ViewModel.Notices.Remove(m_selectedNotice);
                SelectedNotice = null;
            }
            else if(dialogViewModel.Result == EditNoticeViewModel.EditNoticeResult.Ok)
            {
                m_selectedNotice.PopCopy(WorkingCopyResult.Ok);
            }
            else if (dialogViewModel.Result == EditNoticeViewModel.EditNoticeResult.Cancel)
            {
                m_selectedNotice.PopCopy(WorkingCopyResult.Cancel);
            }
            if (dialogViewModel.Result != EditNoticeViewModel.EditNoticeResult.Cancel) GlobalConfig.GlobalDatabaseAutoSaveCommand.Execute(null);
        }
    }
}
