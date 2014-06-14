using System;
using System.Collections.ObjectModel;
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

namespace Dziennik.WPFControls
{
    /// <summary>
    /// Interaction logic for MarksControl.xaml
    /// </summary>
    public partial class MarksControl : UserControl
    {
        public MarksControl()
        {
            InitializeComponent();
        }

        private static readonly DependencyProperty MarksSourceProperty = DependencyProperty.Register("MarksSource", typeof(ObservableCollection<Mark>), typeof(MarksControl), new PropertyMetadata(null));
        public ObservableCollection<Mark> MarksSource
        {
            get { return (ObservableCollection<Mark>)GetValue(MarksSourceProperty); }
            set { SetValue(MarksSourceProperty, value); }
        }
    }
}
