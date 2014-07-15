using System;
using System.Collections.Generic;
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
using Dziennik.Controls;
using Dziennik.ViewModel;

namespace Dziennik.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DatabaseMain database = new DatabaseMain();
            database.SchoolClass = new SchoolClassViewModel();
            GlobalStudentViewModel gs = new GlobalStudentViewModel();
            database.SchoolClass.Students.Add(gs);
            SchoolGroupViewModel gr = new SchoolGroupViewModel();
            gr.Students.Add(new StudentInGroupViewModel() { GlobalStudent = gs });
            gr.Students.Add(new StudentInGroupViewModel() { GlobalStudent = gs });
            database.SchoolClass.Groups.Add(gr);
            database.ReflectToAssignId(database.SchoolClass.Model);

            InitializeComponent();

            MainViewModel viewModel = new MainViewModel();

            GlobalConfig.Main = viewModel;
            this.DataContext = viewModel;

            GlobalConfig.Dialogs.Register(this, viewModel);

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)this.DataContext).Init();
        }
    }
}
