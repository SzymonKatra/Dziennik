using System;
using System.Collections.ObjectModel;
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
using Dziennik.ViewModel;

namespace Dziennik
{
    /// <summary>
    /// Interaction logic for BoundMarksControl.xaml
    /// </summary>
    public partial class BoundMarksControl : UserControl
    {
        public BoundMarksControl()
        {
            InitializeComponent();
        }

        public static DependencyProperty MarksSourceProperty = DependencyProperty.Register("MarksSource", typeof(ObservableCollection<MarkViewModel>), typeof(BoundMarksControl), new PropertyMetadata(null));

        public ObservableCollection<MarkViewModel> MarksSource
        {
            get { return (ObservableCollection<MarkViewModel>)GetValue(MarksSourceProperty); }
            set { SetValue(MarksSourceProperty, value); }
        }
    }
}
