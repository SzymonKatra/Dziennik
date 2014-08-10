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
                return System.Windows.Media.Color.FromArgb(m_model.Color.A, m_model.Color.R, m_model.Color.G, m_model.Color.B);
            }
            set
            {
                Dziennik.Model.Color col = new Dziennik.Model.Color();

                col.A = value.A;
                col.R = value.R;
                col.G = value.G;
                col.B = value.B;

                m_model.Color = col;

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
