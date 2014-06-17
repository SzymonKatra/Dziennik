using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public class MarkViewModel : ObservableObject, IViewModelExposable<Mark>
    {
        public MarkViewModel()
            : this(new Mark())
        {
        }
        public MarkViewModel(Mark mark)
        {
            m_model = mark;
        }

        private Mark m_model;
        public Mark Model
        {
            get { return m_model; }
        }

        public decimal Value
        {
            get { return m_model.Value; }
            set { m_model.Value = value; OnPropertyChanged("Value"); }
        }
        public DateTime AddDate
        {
            get { return m_model.AddDate; }
            set { m_model.AddDate = value; OnPropertyChanged("AddDate"); }
        }
        public DateTime LastChangeDate
        {
            get { return m_model.LastChangeDate; }
            set { m_model.LastChangeDate = value; OnPropertyChanged("LastChangeDate"); }
        }
        public string Description
        {
            get { return m_model.Description; }
            set { m_model.Description = value; OnPropertyChanged("Description"); }
        }
    }
}
