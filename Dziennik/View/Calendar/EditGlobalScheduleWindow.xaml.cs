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
    /// Interaction logic for EditGlobalScheduleWindow.xaml
    /// </summary>
    public partial class EditGlobalScheduleWindow : Window
    {
        public EditGlobalScheduleWindow(EditGlobalScheduleViewModel viewModel)
        {
            InitializeComponent();

            this.Width = SystemParameters.PrimaryScreenWidth * 0.7;
            this.Height = SystemParameters.PrimaryScreenHeight * 0.7;

            this.DataContext = viewModel;

            GlobalConfig.Dialogs.Register(this, viewModel);
        }
    }
}
