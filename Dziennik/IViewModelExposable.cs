using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="M">Model type</typeparam>
    public interface IViewModelExposable<out M>
    {
        M Model { get; }
    }
}
