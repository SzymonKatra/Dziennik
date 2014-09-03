using System;
using System.Collections.Generic;
using System.Collections;
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
    /// Interaction logic for AddMarksSetWindow.xaml
    /// </summary>
    public partial class AddMarksSetWindow : Window
    {
        public AddMarksSetWindow(AddMarksSetViewModel viewModel)
        {
            InitializeComponent();

            this.Width = SystemParameters.PrimaryScreenWidth * 0.5;
            this.Height = SystemParameters.PrimaryScreenHeight * 0.7;

            this.DataContext = viewModel;

            GlobalConfig.Dialogs.Register(this, viewModel);

            dataGrid.CellEditEnding += (s, e) => { e.Cancel = true; };
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if (dataGrid.SelectedIndex < 0) return;
                do
                {
                    if (dataGrid.Items.Count - 1 > dataGrid.SelectedIndex)
                    {
                        var uiElement = e.OriginalSource as UIElement;
                        if (uiElement != null)
                        {
                            uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                        }
                        dataGrid.SelectedIndex++;
                    }
                }
                while (!(dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex) as DataGridRow).IsEnabled);

                e.Handled = true;
            }
        }

        private IEnumerable<DataGridRow> GetDataGridRows()
        {
            var itemsSource = dataGrid.ItemsSource as IEnumerable;
            if (null == itemsSource) yield return null;
            foreach (var item in itemsSource)
            {
                var row = dataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
        }
    }
}
