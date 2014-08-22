using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;
using Dziennik.CommandUtils;
using System.Security;

namespace Dziennik.View
{
    public sealed class TypePasswordViewModel
    {
        public enum TypePasswordResult
        {
            Ok,
            CloseApplication,
        }

        public delegate SecureString GrabSecuredPasswordDelegate();

        public TypePasswordViewModel()
        {
            m_okCommand = new RelayCommand(Ok);
            m_closeCommand = new RelayCommand<CancelEventArgs>(Close);
            m_closeApplicationCommand = new RelayCommand(CloseAppliation);
        }

        private bool m_allowClose = false;

        private TypePasswordResult m_result = TypePasswordResult.CloseApplication;
        public TypePasswordResult Result
        {
            get { return m_result; }
        }

        private RelayCommand m_okCommand;
        public ICommand OkCommand
        {
            get { return m_okCommand; }
        }

        private RelayCommand<CancelEventArgs> m_closeCommand;
        public ICommand CloseCommand
        {
            get { return m_closeCommand; }
        }

        private RelayCommand m_closeApplicationCommand;
        public ICommand CloseApplicationCommand
        {
            get { return m_closeApplicationCommand; }
        }

        public GrabSecuredPasswordDelegate GrabSecuredPassword;
        public Action ClearPasswordInput;

        private void Ok(object param)
        {
            if (GlobalConfig.Notifier.Password != null)
            {
                SecureString password = GrabSecuredPassword();

                if (!PasswordEncryption.Compare(password, GlobalConfig.Notifier.Password))
                {
                    GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_BadPassword"), Controls.MessageBoxSuperPredefinedButtons.OK);
                    ClearPasswordInput();
                    return;
                }

                m_allowClose = true;
                GlobalConfig.MessageBox(this, GlobalConfig.GetStringResource("lang_SuccessfullyUnlocked"), Controls.MessageBoxSuperPredefinedButtons.OK);
            }

            m_result = TypePasswordResult.Ok;
            GlobalConfig.Dialogs.Close(this);
        }
        private void Close(CancelEventArgs param)
        {
            param.Cancel = !m_allowClose;
        }
        private void CloseAppliation(object param)
        {
            m_result = TypePasswordResult.CloseApplication;
            m_allowClose = true;
            GlobalConfig.Dialogs.Close(this);
        }
    }
}
