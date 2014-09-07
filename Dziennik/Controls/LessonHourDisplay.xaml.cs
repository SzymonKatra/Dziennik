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

namespace Dziennik.Controls
{
    /// <summary>
    /// Interaction logic for LessonHourDisplay.xaml
    /// </summary>
    public partial class LessonHourDisplay : UserControl
    {
        public LessonHourDisplay()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty HourProperty = DependencyProperty.Register("Hour", typeof(LessonHourViewModel), typeof(LessonHourDisplay), new PropertyMetadata(null));
        public LessonHourViewModel Hour
        {
            get { return (LessonHourViewModel)GetValue(HourProperty); }
            set { SetValue(HourProperty, value); }
        }
    }
}
