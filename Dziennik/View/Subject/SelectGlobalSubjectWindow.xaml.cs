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
    /// Interaction logic for SelectGlobalSubjectWindow.xaml
    /// </summary>
    public partial class SelectGlobalSubjectWindow : Window
    {
        public SelectGlobalSubjectWindow(SelectGlobalSubjectViewModel viewModel)
        {
            InitializeComponent();

            this.Width = SystemParameters.PrimaryScreenWidth * 0.65;
            this.Height = SystemParameters.PrimaryScreenHeight * 0.45;

            this.DataContext = viewModel;

            GlobalConfig.Dialogs.Register(this, viewModel);
        }
    }
}
