using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Dziennik.Model;
using System.Windows.Media;

namespace Dziennik.ViewModel
{
    public sealed class MarksCategoryViewModel : ObservableObject, IModelExposable<MarksCategory>
    {
        public MarksCategoryViewModel()
            : this(new MarksCategory())
        {
        }
        public MarksCategoryViewModel(MarksCategory model)
        {
            m_model = model;
        }

        private MarksCategory m_model;
        public MarksCategory Model
        {
            get { return m_model; }
        }

        public string Name
        {
            get { return m_model.Name; }
            set { m_model.Name = value; RaisePropertyChanged("Name"); }
        }
        public System.Windows.Media.Color Color
        {
            get
            {
                if (m_model.Color == null) m_model.Color = new Dziennik.Model.Color();
                return System.Windows.Media.Color.FromArgb(m_model.Color.A, m_model.Color.R, m_model.Color.G, m_model.Color.B);
            }
            set
            {
                if (m_model.Color == null) m_model.Color = new Dziennik.Model.Color();

                m_model.Color.A = value.A;
                m_model.Color.R = value.R;
                m_model.Color.G = value.G;
                m_model.Color.B = value.B;

                RaisePropertyChanged("Color");
                RaisePropertyChanged("Brush");
            }
        }
        public System.Windows.Media.SolidColorBrush Brush
        {
            get { return new SolidColorBrush(this.Color); }
        }
    }
}
