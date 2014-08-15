using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    public interface IViewModelCopyable<VM>
    {
        void CopyDataTo(VM viewModel);
    }
}
