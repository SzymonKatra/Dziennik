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
    /// Interaction logic for RemoveButtonControl.xaml
    /// </summary>
    public partial class RemoveButtonControl : UserControl
    {
        public RemoveButtonControl()
        {
            InitializeComponent();

            this.FreeSpaceContentMargin = new Thickness(10, 5, 10, 5);
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(RemoveButtonControl), new PropertyMetadata(null));
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(RemoveButtonControl), new PropertyMetadata(null));
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty FreeSpaceContentProperty = DependencyProperty.Register("FreeSpaceContent", typeof(object), typeof(RemoveButtonControl), new PropertyMetadata(null));
        public object FreeSpaceContent
        {
            get { return GetValue(FreeSpaceContentProperty); }
            set { SetValue(FreeSpaceContentProperty, value); }
        }

        public static readonly DependencyProperty FreeSpaceContentMarginProperty = DependencyProperty.Register("FreeSpaceContentMargin", typeof(Thickness), typeof(RemoveButtonControl), new PropertyMetadata(null));
        public Thickness FreeSpaceContentMargin
        {
            get { return (Thickness)GetValue(FreeSpaceContentMarginProperty); }
            set { SetValue(FreeSpaceContentMarginProperty, value); }
        }
    }
}
