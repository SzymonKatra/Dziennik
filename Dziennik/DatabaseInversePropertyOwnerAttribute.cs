using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    public sealed class DatabaseInversePropertyOwnerAttribute : System.Attribute
    {
        public DatabaseInversePropertyOwnerAttribute(string ownerPropertyInChildName, string subscribeObservableCollectionExtendedMethodName)
        {
            m_ownerPropertyInChildName = ownerPropertyInChildName;
            m_subscribeObservableCollectionExtendedMethodName = subscribeObservableCollectionExtendedMethodName;
        }

        private string m_ownerPropertyInChildName;
        public string OwnerPropertyInChildName
        {
            get { return m_ownerPropertyInChildName; }
            set { m_ownerPropertyInChildName = value; }
        }

        private string m_subscribeObservableCollectionExtendedMethodName;
        public string SubscribeObservableCollectionExtendedMethodName
        {
            get { return m_subscribeObservableCollectionExtendedMethodName; }
            set { m_subscribeObservableCollectionExtendedMethodName = value; }
        }
    }
}
