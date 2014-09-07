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
using Dziennik.View;
using Dziennik.ViewModel;
using System.Collections.ObjectModel;

namespace Dziennik.Controls
{
    /// <summary>
    /// Interaction logic for SchoolDayControl.xaml
    /// </summary>
    public partial class SchoolDayControl : UserControl
    {
        public SchoolDayControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SchoolClassesProperty = DependencyProperty.Register("SchoolClasses", typeof(ObservableCollection<SchoolClassViewModel>), typeof(SchoolDayControl), new PropertyMetadata(null));
        public ObservableCollection<SchoolClassViewModel> SchoolClasses
        {
            get { return (ObservableCollection<SchoolClassViewModel>)GetValue(SchoolClassesProperty); }
            set { SetValue(SchoolClassesProperty, value); }
        }

        public static readonly DependencyProperty DayProperty = DependencyProperty.Register("Day", typeof(EditGlobalScheduleViewModel.SchoolDayItem), typeof(SchoolDayControl), new PropertyMetadata(null));
        public EditGlobalScheduleViewModel.SchoolDayItem Day
        {
            get { return (EditGlobalScheduleViewModel.SchoolDayItem)GetValue(DayProperty); }
            set { SetValue(DayProperty, value); }
        }
    }
}
