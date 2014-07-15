using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class DatabaseRelationAttribute : System.Attribute
    {
        public DatabaseRelationAttribute(string relationName)
        {
            m_relationName = relationName;
        }

        private string m_relationName;
        public string RelationName
        {
            get { return m_relationName; }
            set { m_relationName = value; }
        }
    }
}
