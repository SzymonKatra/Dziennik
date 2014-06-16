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
    public class EditMarkEventArgs : RoutedEventArgs
    {
        private Mark m_value;
        public Mark Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public EditMarkEventArgs(Mark value)
            : base(MarksControl.EditMarkEvent)
        {
            m_value = value;
        }
    }
    public delegate void EditMarkEventHandler(object sender, EditMarkEventArgs e);

    /// <summary>
    /// Interaction logic for MarksControl.xaml
    /// </summary>
    public partial class MarksControl : UserControl
    {
        public MarksControl()
        {
            InitializeComponent();
            AddHandler(TextBlock.MouseLeftButtonDownEvent, new MouseButtonEventHandler(TextBlock_MouseLeftButtonDown), true); //because adding handler via xaml doesn't catch double click
        }

        public static readonly DependencyProperty MarksSourceProperty = DependencyProperty.Register("MarksSource", typeof(ObservableCollection<Mark>), typeof(MarksControl), new FrameworkPropertyMetadata(null));
        public ObservableCollection<Mark> MarksSource
        {
            get { return (ObservableCollection<Mark>)GetValue(MarksSourceProperty); }
            set { SetValue(MarksSourceProperty, value); }
        }

        //public static readonly DependencyProperty CommandAddMarkProperty = DependencyProperty.Register("CommandAddMark", typeof(ICommand), typeof(MarksControl), new FrameworkPropertyMetadata(null));
        //public ICommand CommandAddMark
        //{
        //    get { return (ICommand)GetValue(CommandAddMarkProperty); }
        //    set { SetValue(CommandAddMarkProperty, value); }
        //}

        public static readonly RoutedEvent AddMarkEvent = EventManager.RegisterRoutedEvent("AddMark", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MarksControl));
        public event RoutedEventHandler AddMark
        {
            add { AddHandler(AddMarkEvent, value); }
            remove { RemoveHandler(AddMarkEvent, value); }
        }

        public static readonly RoutedEvent EditMarkEvent = EventManager.RegisterRoutedEvent("EditMark", RoutingStrategy.Bubble, typeof(EditMarkEventHandler), typeof(MarksControl));
        public event EditMarkEventHandler EditMark
        {
            add { AddHandler(EditMarkEvent, value); }
            remove { RemoveHandler(EditMarkEvent, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(AddMarkEvent));
        }
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (listBox.SelectedItem is Mark)
                {
                    RaiseEvent(new EditMarkEventArgs((Mark)listBox.SelectedItem));
                }
            }

            e.Handled = true;
        }
    }
}
