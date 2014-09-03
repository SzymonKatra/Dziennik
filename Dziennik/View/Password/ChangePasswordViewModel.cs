using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Dziennik.CommandUtils;
using System.Security;

namespace Dziennik.View
{
    public sealed class ChangePasswordViewModel : ObservableObject
    {
        public class SecuredPasswords
        {
            public SecureString Current { get; set; }
            public SecureString New { get; set; }
            public SecureString RepeatNew { get; set; }
        }
        public delegate SecuredPasswords GrabSecuredPasswordsDelegate();

        public enum ChangePasswordResult
        {
            Ok,
            Cancel,
            RemovedPassword,
        }

        public ChangePasswordViewModel()
        {
            m_okCommand = new RelayCommand(Ok);
            m_cancelCommand = new RelayCommand(Cancel);
            m_removePasswordCommand = new RelayCommand(RemovePassword, CanRemovePassword);
        }

        private ChangePasswordResult m_result = ChangePasswordResult.Cancel;
        public ChangePasswordResult Result
        {
            get { return m_result; }
        }

        public GrabSecuredPasswordsDelegate GrabSecuredPasswords;

        private bool m_wantChangePassword = false;
        public bool WantChangePassword
        {
            get { return m_wantChangePassword; }
            set { m_wantChangePassword = value; RaisePropertyChanged("WantChangePassword"); }
        }

        private int m_blockingMinutes = 0;
        public int BlockingMinutes
        {
            get { return m_blockingMinutes; }
            set { m_blockingMinutes = value; RaisePropertyChanged("BlockingMinutes"); }
        }

        public bool AlreadyHasPassword
        {
            get
            {
                return GlobalConfig.Notifier.Password != null;
            }
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

        private RelayCommand m_removePasswordCommand;
        public ICommand RemovePasswordCommand
        {
            get { return m_removePasswordCommand; }
        }

        public void Ok(object param)
        {
            if (!AskForContinue()) return;

            SecuredPasswords passwords = GrabSecuredPasswords();

            if (AlreadyHasPassword)
            {
                if (!PasswordEncryption.Compare(passwords.Current, GlobalConfig.Notifier.Password))
                {
                    GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_CurrentPasswordNotValid"), Controls.MessageBoxSuperPredefinedButtons.OK);
                    return;
                }
            }

            if (m_wantChangePassword)
            {
                if (!passwords.New.IsEqualTo(passwords.RepeatNew))
                {
                    GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_NewNotEqualsRepeatNew"), Controls.MessageBoxSuperPredefinedButtons.OK);
                    return;
                }

                GlobalConfig.Notifier.Password = PasswordEncryption.Encrypt(passwords.New);
            }

            if(AlreadyHasPassword || m_wantChangePassword) GlobalConfig.Notifier.BlockingMinutes = this.BlockingMinutes;

            m_result = ChangePasswordResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private void Cancel(object param)
        {
            m_result = ChangePasswordResult.Cancel;
            GlobalConfig.Dialogs.Close(this);
        }
        private void RemovePassword(object param)
        {
            if (!AskForContinue()) return;

            SecuredPasswords passwords = GrabSecuredPasswords();

            if (!PasswordEncryption.Compare(passwords.Current, GlobalConfig.Notifier.Password))
            {
                GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_CurrentPasswordNotValid"), Controls.MessageBoxSuperPredefinedButtons.OK);
                return;
            }

            GlobalConfig.Notifier.Password = null;
            GlobalConfig.Notifier.BlockingMinutes = 0;

            m_result = ChangePasswordResult.RemovedPassword;
            GlobalConfig.Dialogs.Close(this);
        }
        private bool CanRemovePassword(object param)
        {
            return AlreadyHasPassword;
        }

        private bool AskForContinue()
        {
            return (GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_DoYouWantToContinue"), Controls.MessageBoxSuperPredefinedButtons.YesNo) == Controls.MessageBoxSuperButton.Yes);
        }
    }
}
