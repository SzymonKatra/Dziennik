using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.CommandUtils;
using System.Windows.Input;

namespace Dziennik.View
{
    public class InfoDialogViewModel : ObservableObject
    {
        public InfoDialogViewModel()
        {
            m_closeCommand = new RelayCommand(Close);
        }

        private RelayCommand m_closeCommand;
        public ICommand CloseCommand
        {
            get { return m_closeCommand; }
        }

        private void Close(object e)
        {
            GlobalConfig.Dialogs.Close(this);
        }
    }
}
