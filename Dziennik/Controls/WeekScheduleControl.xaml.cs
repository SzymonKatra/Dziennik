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
using Dziennik.ViewModel;
using System.ComponentModel;

namespace Dziennik.Controls
{
    /// <summary>
    /// Interaction logic for WeekScheduleControl.xaml
    /// </summary>
    public partial class WeekScheduleControl : UserControl
    {
        public WeekScheduleControl()
        {
            InitializeComponent();

        }

        public static readonly DependencyProperty WeekScheduleProperty = DependencyProperty.Register("WeekSchedule", typeof(WeekScheduleViewModel), typeof(WeekScheduleControl), new PropertyMetadata(null));
        public WeekScheduleViewModel WeekSchedule
        {
            get { return (WeekScheduleViewModel)GetValue(WeekScheduleProperty); }
            set { SetValue(WeekScheduleProperty, value); }
        }

        public static readonly DependencyProperty AvailableProperty = DependencyProperty.Register("Available", typeof(List<int>), typeof(WeekScheduleControl), new PropertyMetadata(null));
        public List<int> Available
        {
            get { return (List<int>)GetValue(AvailableProperty); }
            set { SetValue(AvailableProperty, value); }
        }
    }
}
