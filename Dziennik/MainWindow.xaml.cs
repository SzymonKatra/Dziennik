using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dziennik.WPFControls;

namespace Dziennik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            m_viewModel = new MainViewModel();
            this.DataContext = m_viewModel;
        }

        private MainViewModel m_viewModel;
        public MainViewModel ViewModel
        {
            get { return m_viewModel; }
        }
        
    }
}
