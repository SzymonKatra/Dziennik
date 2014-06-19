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
using Dziennik.WindowViewModel;
using Dziennik.ViewModel;

namespace Dziennik.View
{
    /// <summary>
    /// Interaction logic for EditMarkWindow.xaml
    /// </summary>
    public partial class EditMarkWindow : Window
    {
        public EditMarkWindow(MarkViewModel mark)
        {
            InitializeComponent();

            m_viewModel = new EditMarkViewModel(mark);
            m_viewModel.DialogClose += m_viewModel_DialogClose;

            this.DataContext = m_viewModel;
        }

        private EditMarkViewModel m_viewModel;

        private bool m_result;
        public bool Result
        {
            get { return m_result; }
        }

        private void m_viewModel_DialogClose(object sender, EditMarkDialogCloseEventArgs e)
        {
            m_result = e.Save;
            this.Close();
        }
    }
}
