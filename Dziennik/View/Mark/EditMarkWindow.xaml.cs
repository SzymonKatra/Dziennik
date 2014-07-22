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
using Dziennik.ViewModel;

namespace Dziennik.View
{
    /// <summary>
    /// Interaction logic for EditMarkWindow.xaml
    /// </summary>
    public partial class EditMarkWindow : Window
    {
        public EditMarkWindow(EditMarkViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            GlobalConfig.Dialogs.Register(this, viewModel);
        }
    }
}
