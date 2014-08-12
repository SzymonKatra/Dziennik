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
    /// Interaction logic for NoticesListWindow.xaml
    /// </summary>
    public partial class NoticesListWindow : Window
    {
        public NoticesListWindow(NoticesListViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            GlobalConfig.Dialogs.Register(this, viewModel);
        }
    }
}
