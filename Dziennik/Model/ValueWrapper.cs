using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik.Model
{
    [Serializable]
    public class ValueWrapper<T> : ModelBase
    {
        public ValueWrapper()
        {
        }
        public ValueWrapper(T value)
        {
            this.Value = value;
        }

        public T Value { get; set; }
    }
}
