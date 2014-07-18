using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class DatabaseRelationPropertyAttribute : DatabaseRelationAttribute
    {
        public DatabaseRelationPropertyAttribute(string relationName, string modelPropertyIdName)
            : base(relationName)
        {
            m_modelPropertyIdName = modelPropertyIdName;
        }

        private string m_modelPropertyIdName;
        public string ModelPropertyIdName
        {
            get { return m_modelPropertyIdName; }
            set { m_modelPropertyIdName = value; }
        }
    }
}
