using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Dziennik.Model;
using System.Windows.Media;

namespace Dziennik.ViewModel
{
    public sealed class MarksCategoryViewModel : ViewModelBase<MarksCategoryViewModel, MarksCategory>
    {
        public MarksCategoryViewModel()
            : this(new MarksCategory())
        {
        }
        public MarksCategoryViewModel(MarksCategory model)
            : base(model)
        {
        }

        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; RaisePropertyChanged("Name"); }
        }
        public System.Windows.Media.Color Color
        {
            get
            {
                if (Model.Color == null) Model.Color = new Dziennik.Model.Color();
                return System.Windows.Media.Color.FromArgb(Model.Color.A, Model.Color.R, Model.Color.G, Model.Color.B);
            }
            set
            {
                if (Model.Color == null) Model.Color = new Dziennik.Model.Color();

                Model.Color.A = value.A;
                Model.Color.R = value.R;
                Model.Color.G = value.G;
                Model.Color.B = value.B;

                RaisePropertyChanged("Color");
                RaisePropertyChanged("Brush");
            }
        }
        public System.Windows.Media.SolidColorBrush Brush
        {
            get { return new SolidColorBrush(this.Color); }
        }

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();

            pack.Write(this.Name);
            pack.Write(this.Color);

            CopyStack.Push(pack);
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();

            if(result == WorkingCopyResult.Cancel)
            {
                this.Name = (string)pack.Read();
                this.Color = (System.Windows.Media.Color)pack.Read();
            }
        }
    }
}
