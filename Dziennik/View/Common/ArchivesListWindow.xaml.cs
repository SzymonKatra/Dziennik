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
    /// Interaction logic for ArchivesListWindow.xaml
    /// </summary>
    public partial class ArchivesListWindow : Window
    {
        public ArchivesListWindow(ArchivesListViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            GlobalConfig.Dialogs.Register(this, viewModel);

            this.Loaded += ArchivesListWindow_Loaded;
        }

        private void ArchivesListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (dataGrid.Items.Count > 0) dataGrid.ScrollIntoView(dataGrid.Items[dataGrid.Items.Count - 1]);
        }
    }
}
