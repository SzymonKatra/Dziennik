using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    public interface IViewModelShallowCopyable<VM>
    {
        void ShallowCopyDataTo(VM viewModel);
    }
}
