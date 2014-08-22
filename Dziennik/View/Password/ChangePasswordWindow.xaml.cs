using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dziennik.View
{
    /// <summary>
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        public ChangePasswordWindow(ChangePasswordViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
            viewModel.GrabSecuredPasswords = GrabSecuredPasswords;

            GlobalConfig.Dialogs.Register(this, viewModel);
        }

        public ChangePasswordViewModel.SecuredPasswords GrabSecuredPasswords()
        {
            return new ChangePasswordViewModel.SecuredPasswords()
            {
                Current = currentPassword.SecurePassword,
                New = newPassword.SecurePassword,
                RepeatNew = repeatNewPassword.SecurePassword,
            };
        }
    }
}
