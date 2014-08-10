using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit;

namespace Dziennik.Controls
{
    public class NonEditableIntegerUpDown : IntegerUpDown
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TextBox.IsReadOnly = true;
        }
    }
}
