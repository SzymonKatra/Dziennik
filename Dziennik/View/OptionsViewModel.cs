using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Dziennik.CommandUtils;

namespace Dziennik.View
{
    public class OptionsViewModel : ObservableObject
    {
        private RelayCommand m_closeCommand;
        public ICommand CloseCommand
        {
            get { return m_closeCommand; }
        }

        public OptionsViewModel()
        {
            m_closeCommand = new RelayCommand(Close);
        }

        private void Close(object e)
        {
            ActionDialogViewModel dialogViewModel = new ActionDialogViewModel((d, p) =>
            {
                GlobalConfig.Notifier.SaveRegistry();
            }
            , null, "Zapisywanie ustawień do rejestru...");
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);

            if (e == null) // if window is closed by X icon, e will be not null
            {
                GlobalConfig.Dialogs.Close(this);
            }
        }
    }
}
