﻿using System;
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
using System.Windows.Shapes;

namespace Dziennik.View
{
    /// <summary>
    /// Interaction logic for OverdueSubjectsListWindow.xaml
    /// </summary>
    public partial class OverdueSubjectsListWindow : Window
    {
        public OverdueSubjectsListWindow(OverdueSubjectsListViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            GlobalConfig.Dialogs.Register(this, viewModel);
        }

        private void Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            ((OverdueSubjectsListViewModel)this.DataContext).RealizeSelectedSubjectCommand.Execute(null);
        }
    }
}
