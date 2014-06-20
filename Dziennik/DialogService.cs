using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Dziennik
{
    public class DialogService
    {
        public DialogService()
            : this(new Dictionary<Type, Func<object, Window>>())
        {
        }
        public DialogService(Dictionary<Type, Func<object, Window>> windowViewModelMappings)
        {
            m_windowViewModelMappings = windowViewModelMappings;
        }

        private Dictionary<Type, Func<object, Window>> m_windowViewModelMappings;
        public Dictionary<Type, Func<object, Window>> WindowViewModelMappings
        {
            get { return m_windowViewModelMappings; }
            set { m_windowViewModelMappings = value; }
        }

        private Dictionary<object, Window> m_registeredViews = new Dictionary<object, Window>();
        private Dictionary<object, Window> m_openedDialogs = new Dictionary<object, Window>();

        public void Register(Window view, object viewModel, bool unregisterOnClose = true)
        {
            m_registeredViews.Add(viewModel, view);
            if (unregisterOnClose) view.Closed += view_Closed;
        }
        public void Unregister(Window view)
        {
            m_registeredViews.Remove(m_registeredViews.First((kvp) => { return (kvp.Value == view); }));
        }

        public bool? ShowDialog(object ownerViewModel, object viewModel)
        {
            if (!m_registeredViews.ContainsKey(ownerViewModel)) throw new ArgumentException("Owner not registered");

            Type viewModelType = viewModel.GetType();

            if (!m_windowViewModelMappings.ContainsKey(viewModelType)) throw new ArgumentException("No function to create view attached to this type of ViewModel");

            Window dialogView = m_windowViewModelMappings[viewModelType](viewModel);
            dialogView.Owner = m_registeredViews[ownerViewModel];

            m_openedDialogs.Add(viewModel, dialogView);

            bool? result = dialogView.ShowDialog();

            m_openedDialogs.Remove(viewModel);

            return result;
        }
        public void CloseDialog(object viewModel)
        {
            if (!m_openedDialogs.ContainsKey(viewModel)) throw new ArgumentException("No opened views for this ViewModel");

            m_openedDialogs[viewModel].Close();
        }

        private void view_Closed(object sender, EventArgs e)
        {
            Window view = sender as Window;
            if (view == null) return;

            view.Closed -= view_Closed;
            Unregister(view);
        }
    }
}
