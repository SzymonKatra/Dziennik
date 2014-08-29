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
using System.Collections.ObjectModel;
using Dziennik.ViewModel;

namespace Dziennik.Controls
{
    /// <summary>
    /// Interaction logic for ListHoursSchedule.xaml
    /// </summary>
    public partial class ListHoursSchedule : UserControl
    {
        public ListHoursSchedule()
        {
            InitializeComponent();
        }

        public event EventHandler<ChangedStateEventArgs> SelectionChanged;

        public static readonly DependencyProperty AvailableHoursProperty = DependencyProperty.Register("AvailableHours", typeof(List<int>), typeof(ListHoursSchedule), new PropertyMetadata(null));
        public List<int> AvailableHours
        {
            get { return (List<int>)GetValue(AvailableHoursProperty); }
            set { SetValue(AvailableHoursProperty, value); }
        }

        public static readonly DependencyProperty SelectedHoursProperty = DependencyProperty.Register("SelectedHours", typeof(ObservableCollection<SelectedHourViewModel>), typeof(ListHoursSchedule), new PropertyMetadata(null));
        public ObservableCollection<SelectedHourViewModel> SelectedHours
        {
            get { return (ObservableCollection<SelectedHourViewModel>)GetValue(SelectedHoursProperty); }
            set { SetValue(SelectedHoursProperty, value); }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        protected virtual void OnSelectionChanged(ChangedStateEventArgs e)
        {
            EventHandler<ChangedStateEventArgs> handler = SelectionChanged;
            if (handler != null) handler(this, e);
        }
    }
}
