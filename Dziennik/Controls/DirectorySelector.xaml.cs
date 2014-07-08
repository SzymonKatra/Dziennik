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
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace Dziennik.Controls
{
    /// <summary>
    /// Interaction logic for DirectorySelector.xaml
    /// </summary>
    public partial class DirectorySelector : UserControl
    {
        public static readonly DependencyProperty SelectedPathProperty = DependencyProperty.Register("SelectedPath", typeof(string), typeof(DirectorySelector), new PropertyMetadata(null));
        public string SelectedPath
        {
            get { return (string)GetValue(SelectedPathProperty); }
            set { SetValue(SelectedPathProperty, value); }
        }

        public static readonly DependencyProperty IsTextBoxReadOnlyProperty = DependencyProperty.Register("IsTextBoxReadOnly", typeof(bool), typeof(DirectorySelector), new PropertyMetadata(false));
        public bool IsTextBoxReadOnly
        {
            get { return (bool)GetValue(IsTextBoxReadOnlyProperty); }
            set { SetValue(IsTextBoxReadOnlyProperty, value); }
        }

        public DirectorySelector()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = textBox.Text;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) textBox.Text = dialog.SelectedPath;

            e.Handled = true;
        }
    }
}
