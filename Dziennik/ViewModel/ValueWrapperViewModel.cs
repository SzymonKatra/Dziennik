using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;

namespace Dziennik.ViewModel
{
    public sealed class ValueWrapperViewModel<T> : ViewModelBase<ValueWrapperViewModel<T>, ValueWrapper<T>>
    {
        public ValueWrapperViewModel(T value)
            : this(new ValueWrapper<T>(value))
        {
        }
        public ValueWrapperViewModel()
            : this(new ValueWrapper<T>())
        {
        }
        public ValueWrapperViewModel(ValueWrapper<T> viewModel)
            : base(viewModel)
        {
        }

        public T Value
        {
            get { return Model.Value; }
            set { Model.Value = value; RaisePropertyChanged("Value"); }
        }

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.Value);

            CopyStack.Push(pack);
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();
            if (result == WorkingCopyResult.Cancel)
            {
                this.Value = (T)pack.Read();
            }
        }
    }
}
