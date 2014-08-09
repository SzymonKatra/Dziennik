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
    /// Interaction logic for AddMarksSetHeaderWithContext.xaml
    /// </summary>
    public partial class AddMarksSetHeaderWithContext : UserControl
    {
        public AddMarksSetHeaderWithContext()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CommandAddMarksSetProperty = DependencyProperty.Register("CommandAddMarksSet", typeof(ICommand), typeof(AddMarksSetHeaderWithContext), new PropertyMetadata(null));
        public ICommand CommandAddMarksSet
        {
            get { return (ICommand)GetValue(CommandAddMarksSetProperty); }
            set { SetValue(CommandAddMarksSetProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterAllProperty = DependencyProperty.Register("CommandParameterAll", typeof(object), typeof(AddMarksSetHeaderWithContext), new PropertyMetadata(null));
        public object CommandParameterAll
        {
            get { return GetValue(CommandParameterAllProperty); }
            set { SetValue(CommandParameterAllProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(AddMarksSetHeaderWithContext), new PropertyMetadata(null));
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}
