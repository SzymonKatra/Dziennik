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
using System.Windows.Shapes;

namespace Dziennik.View
{
    /// <summary>
    /// Interaction logic for ActionDialogWindow.xaml
    /// </summary>
    public partial class ActionDialogWindow : Window
    {
        public ActionDialogWindow(ActionDialogViewModel viewModel)
        {    
            InitializeComponent();

            this.DataContext = viewModel;
            viewModel.PropertyChanged += viewModel_PropertyChanged;

            m_viewModel = viewModel;

            GlobalConfig.Dialogs.Register(this, viewModel);
        }

        private ActionDialogViewModel m_viewModel;

        private Action Empty = new Action(() => { });
        private void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Content")
            {
                textBlock.Text = m_viewModel.Content;
                textBlock.Dispatcher.Invoke(Empty, System.Windows.Threading.DispatcherPriority.Render);
            }
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            m_viewModel.DoWorkCommand.Execute(null);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            m_viewModel.PropertyChanged -= viewModel_PropertyChanged;
            m_viewModel = null;
        }
    }
}
