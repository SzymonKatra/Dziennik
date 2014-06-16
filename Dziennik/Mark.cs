using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dziennik
{
    public class Mark : ObservableObject
    {
        private decimal m_value;
        public decimal Value
        {
            get { return m_value; }
            set { m_value = value; OnPropertyChanged("Value"); }
        }

        private DateTime m_addDate;
        public DateTime AddDate
        {
            get { return m_addDate; }
            set { m_addDate = value; OnPropertyChanged("AddDate"); }
        }

        private DateTime m_lastChangeDate;
        public DateTime LastChangeDate
        {
            get { return m_lastChangeDate; }
            set { m_lastChangeDate = value; OnPropertyChanged("LastChangeDate"); }
        }

        private string m_description;
        public string Description
        {
            get { return m_description; }
            set { m_description = value; OnPropertyChanged("Description"); }
        }

        //private int m_weight;
        //public int Weight
        //{
        //    get { return m_weight; }
        //    set { m_weight = value; OnPropertyChanged("Weight"); }
        //}
    }
}
