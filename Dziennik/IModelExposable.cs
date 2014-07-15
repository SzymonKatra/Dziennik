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
    public interface IModelExposable<out M>
    {
        M Model { get; }
    }
}
