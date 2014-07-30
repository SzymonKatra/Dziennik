using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class DatabaseIgnoreSearchRelationsAttribute : System.Attribute
    {
    }
}
