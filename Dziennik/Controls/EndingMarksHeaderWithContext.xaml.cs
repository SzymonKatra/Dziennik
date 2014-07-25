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
    /// Interaction logic for EndingMarksHeaderWithContext.xaml
    /// </summary>
    public partial class EndingMarksHeaderWithContext : UserControl
    {
        public EndingMarksHeaderWithContext()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CommandPutAllEndingMarksProperty = DependencyProperty.Register("CommandPutAllEndingMarks", typeof(ICommand), typeof(EndingMarksHeaderWithContext), new PropertyMetadata(null));
        public ICommand CommandPutAllEndingMarks
        {
            get { return (ICommand)GetValue(CommandPutAllEndingMarksProperty); }
            set { SetValue(CommandPutAllEndingMarksProperty, value); }
        }

        public static readonly DependencyProperty CommandPutNotAllEndingMarksProperty = DependencyProperty.Register("CommandPutNotAllEndingMarks", typeof(ICommand), typeof(EndingMarksHeaderWithContext), new PropertyMetadata(null));
        public ICommand CommandPutNotAllEndingMarks
        {
            get { return (ICommand)GetValue(CommandPutNotAllEndingMarksProperty); }
            set { SetValue(CommandPutNotAllEndingMarksProperty, value); }
        }

        public static readonly DependencyProperty CommandCancelAllEndingMarksProperty = DependencyProperty.Register("CommandCancelAllEndingMarks", typeof(ICommand), typeof(EndingMarksHeaderWithContext), new PropertyMetadata(null));
        public ICommand CommandCancelAllEndingMarks
        {
            get { return (ICommand)GetValue(CommandCancelAllEndingMarksProperty); }
            set { SetValue(CommandCancelAllEndingMarksProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterAllProperty = DependencyProperty.Register("CommandParameterAll", typeof(object), typeof(EndingMarksHeaderWithContext), new PropertyMetadata(null));
        public object CommandParameterAll
        {
            get { return GetValue(CommandParameterAllProperty); }
            set { SetValue(CommandParameterAllProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(EndingMarksHeaderWithContext), new PropertyMetadata(null));
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}
