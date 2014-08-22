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
using System.Security;

namespace Dziennik.View
{
    /// <summary>
    /// Interaction logic for TypePasswordWindow.xaml
    /// </summary>
    public partial class TypePasswordWindow : Window
    {
        public TypePasswordWindow(TypePasswordViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
            viewModel.GrabSecuredPassword = GrabSecuredPassword;
            viewModel.ClearPasswordInput = ClearPasswordInput;

            GlobalConfig.Dialogs.Register(this, viewModel);

            this.Loaded += TypePasswordWindow_Loaded;
        }

        private void TypePasswordWindow_Loaded(object sender, RoutedEventArgs e)
        {
            password.Focus();
        }

        private SecureString GrabSecuredPassword()
        {
            return password.SecurePassword;
        }
        private void ClearPasswordInput()
        {
            password.Clear();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            ((TypePasswordViewModel)this.DataContext).OkCommand.Execute(null);
        }
    }
}
