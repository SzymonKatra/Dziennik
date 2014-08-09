using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.View
{
    public sealed class NoActionDialogParameter
    {
        private NoActionDialogParameter()
        {
        }

        public static readonly NoActionDialogParameter Instance = new NoActionDialogParameter();
    }
}
