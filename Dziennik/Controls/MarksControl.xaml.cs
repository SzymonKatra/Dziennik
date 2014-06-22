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
using Dziennik.ViewModel;

namespace Dziennik.Controls
{
    public class EditMarkEventArgs : RoutedEventArgs
    {
        private MarkViewModel m_value;
        public MarkViewModel Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public EditMarkEventArgs(MarkViewModel value)
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

        public static readonly DependencyProperty MarksSourceProperty = DependencyProperty.Register("MarksSource", typeof(ObservableCollection<MarkViewModel>), typeof(MarksControl), new PropertyMetadata(null));
        public ObservableCollection<MarkViewModel> MarksSource
        {
            get { return (ObservableCollection<MarkViewModel>)GetValue(MarksSourceProperty); }
            set { SetValue(MarksSourceProperty, value); }
        }

        public static readonly DependencyProperty SelectedMarkProperty = DependencyProperty.Register("SelectedMark", typeof(MarkViewModel), typeof(MarksControl), new PropertyMetadata(null));
        public MarkViewModel SelectedMark
        {
            get { return (MarkViewModel)GetValue(SelectedMarkProperty); }
            set { SetValue(SelectedMarkProperty, value); }
        }

        public static readonly DependencyProperty CommandAddMarkProperty = DependencyProperty.Register("CommandAddMark", typeof(ICommand), typeof(MarksControl), new PropertyMetadata(null));
        public ICommand CommandAddMark
        {
            get { return (ICommand)GetValue(CommandAddMarkProperty); }
            set { SetValue(CommandAddMarkProperty, value); }
        }
        public static readonly DependencyProperty CommandParametrAddMarkProperty = DependencyProperty.Register("CommandParametrAddMark", typeof(object), typeof(MarksControl), new PropertyMetadata(null));
        public object CommandParametrAddMark
        {
            get { return GetValue(CommandParametrAddMarkProperty); }
            set { SetValue(CommandParametrAddMarkProperty, value); }
        }

        public static readonly DependencyProperty CommandEditMarkProperty = DependencyProperty.Register("CommandEditMark", typeof(ICommand), typeof(MarksControl), new PropertyMetadata(null));
        public ICommand CommandEditMark
        {
            get { return (ICommand)GetValue(CommandEditMarkProperty); }
            set { SetValue(CommandEditMarkProperty, value); }
        }
        public static readonly DependencyProperty CommandParametrEditMarkProperty = DependencyProperty.Register("CommandParametrEditMark", typeof(object), typeof(MarksControl), new PropertyMetadata(null));
        public object CommandParametrEditMark
        {
            get { return GetValue(CommandParametrEditMarkProperty); }
            set { SetValue(CommandParametrEditMarkProperty, value); }
        }

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
            if (CommandAddMark != null) CommandAddMark.Execute(CommandParametrAddMark);
        }
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (listBox.SelectedItem is MarkViewModel)
                {
                    RaiseEvent(new EditMarkEventArgs((MarkViewModel)listBox.SelectedItem));
                    if (CommandEditMark != null) CommandEditMark.Execute(CommandParametrEditMark);
                }
            }

            e.Handled = true;
        }

        private void listBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ((ListBox)sender).SelectedItem = null;
        }
    }
}
