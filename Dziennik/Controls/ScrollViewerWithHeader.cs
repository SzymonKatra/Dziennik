using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Dziennik.Controls
{
    public class ScrollViewerWithHeader : ScrollViewer
    {
        public ScrollViewerWithHeader()
        {
        }

        public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.Register("HeaderContent", typeof(object), typeof(ScrollViewerWithHeader), new PropertyMetadata(null));
        public object HeaderContent
        {
            get { return GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }
    }
}
