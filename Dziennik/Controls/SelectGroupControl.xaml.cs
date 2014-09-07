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
using Dziennik.View;
using Dziennik.CommandUtils;

namespace Dziennik.Controls
{
    /// <summary>
    /// Interaction logic for SelectGroupControl.xaml
    /// </summary>
    public partial class SelectGroupControl : UserControl
    {
        public SelectGroupControl()
        {
            InitializeComponent();

            m_clearCommand = new RelayCommand(Clear);

            this.Loaded += SelectGroupControl_Loaded;
        }

        void SelectGroupControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeSelections();
        }

        public static readonly DependencyProperty SchoolClassesProperty = DependencyProperty.Register("SchoolClasses", typeof(ObservableCollection<SchoolClassViewModel>), typeof(SelectGroupControl), new PropertyMetadata(null));
        public ObservableCollection<SchoolClassViewModel> SchoolClasses
        {
            get { return (ObservableCollection<SchoolClassViewModel>)GetValue(SchoolClassesProperty); }
            set { SetValue(SchoolClassesProperty, value); }
        }

        public static readonly DependencyProperty SelectedClassProperty = DependencyProperty.Register("SelectedClass", typeof(SchoolClassViewModel), typeof(SelectGroupControl), new PropertyMetadata(null));
        public SchoolClassViewModel SelectedClass
        {
            get { return (SchoolClassViewModel)GetValue(SelectedClassProperty); }
            set { SetValue(SelectedClassProperty, value); }
        }
        public static readonly DependencyProperty SelectedGroupProperty = DependencyProperty.Register("SelectedGroup", typeof(SchoolGroupViewModel), typeof(SelectGroupControl), new PropertyMetadata(null));
        public SchoolGroupViewModel SelectedGroup
        {
            get { return (SchoolGroupViewModel)GetValue(SelectedGroupProperty); }
            set { SetValue(SelectedGroupProperty, value); }
        }

        private void InitializeSelections()
        {
            SchoolClassViewModel ownerClass = (SchoolClasses == null ? null : SchoolClasses.FirstOrDefault(x => x.Groups.Contains(SelectedGroup)));
            if (ownerClass == null)
            {
                comboClasses.SelectedItem = null;
            }
            else
            {
                comboClasses.SelectedItem = ownerClass;
            }
        }

        public static readonly DependencyProperty RoomProperty = DependencyProperty.Register("Room", typeof(string), typeof(SelectGroupControl), new PropertyMetadata(null));
        public string Room
        {
            get { return (string)GetValue(RoomProperty); }
            set { SetValue(RoomProperty, value); }
        }

        private RelayCommand m_clearCommand;
        public ICommand ClearCommand
        {
            get { return m_clearCommand; }
        }

        private void Clear(object param)
        {
            comboGroups.SelectedItem = null;
            comboClasses.SelectedItem = null;
            txtboxRoom.Text = string.Empty;
        }
    }
}
