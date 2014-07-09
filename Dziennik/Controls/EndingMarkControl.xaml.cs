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
using System.ComponentModel;

namespace Dziennik.Controls
{
    /// <summary>
    /// Interaction logic for EndingMarkControl.xaml
    /// </summary>
    public partial class EndingMarkControl : UserControl, INotifyPropertyChanged
    {
        public EndingMarkControl()
        {
            InitializeComponent();
            
            AddHandler(TextBlock.MouseLeftButtonDownEvent, new MouseButtonEventHandler(TextBlock_MouseLeftButtonDown), true); //because adding handler via xaml doesn't catch double click
            listBox.Items.Add(new object()); // dummy element, need for pretty selection
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty EndingMarkProperty = DependencyProperty.Register("EndingMark", typeof(decimal), typeof(EndingMarkControl), new PropertyMetadata(0M));
        public decimal EndingMark
        {
            get { return (decimal)GetValue(EndingMarkProperty); }
            set { SetValue(EndingMarkProperty, value); RaisePropertyChanged("DisplayedText"); }
        }
        public string DisplayedText
        {
            get
            {
                if (EndingMark == 0M) return "Wystaw";
                return EndingMark.ToString();
            }
        }

        public static readonly DependencyProperty CommandEditMarkProperty = DependencyProperty.Register("CommandEditMark", typeof(ICommand), typeof(EndingMarkControl), new PropertyMetadata(null));
        public ICommand CommandEditMark
        {
            get { return (ICommand)GetValue(CommandEditMarkProperty); }
            set { SetValue(CommandEditMarkProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterEditMarkProperty = DependencyProperty.Register("CommandParameterEditMark", typeof(object), typeof(EndingMarkControl), new PropertyMetadata(null));
        public object CommandParameterEditMark
        {
            get { return GetValue(CommandParameterEditMarkProperty); }
            set { SetValue(CommandParameterEditMarkProperty, value); }
        }

        public static readonly RoutedEvent EditMarkEvent = EventManager.RegisterRoutedEvent("EditMark", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EndingMarkControl));
        public event RoutedEventHandler EditMark
        {
            add { AddHandler(EditMarkEvent, value); }
            remove { RemoveHandler(EditMarkEvent, value); }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                RaiseEvent(new RoutedEventArgs(EditMarkEvent));
                if (CommandEditMark != null) CommandEditMark.Execute(CommandParameterEditMark);
            }

            e.Handled = true;
        }

        private void listBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ((ListBox)sender).SelectedItem = null;
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
