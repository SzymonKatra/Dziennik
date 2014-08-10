using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit;
using System.Windows.Controls;

namespace Dziennik.Controls
{
    public class ColorPickerEx : ColorPicker
    {
        private Button m_advanced;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_advanced = GetTemplateChild("PART_ColorModeButton") as Button;
            if (m_advanced != null)
            {
                switch (this.ColorMode)
                {
                    case Xceed.Wpf.Toolkit.ColorMode.ColorCanvas: m_advanced.Content = GlobalConfig.GetStringResource("lang_SimpleMode"); break;
                    case Xceed.Wpf.Toolkit.ColorMode.ColorPalette: m_advanced.Content = GlobalConfig.GetStringResource("lang_AdvancedMode"); break;
                }
                m_advanced.Click += (sender, e) =>
                {
                    switch(this.ColorMode)
                    {
                        case Xceed.Wpf.Toolkit.ColorMode.ColorCanvas: m_advanced.Content = GlobalConfig.GetStringResource("lang_SimpleMode"); break;
                        case Xceed.Wpf.Toolkit.ColorMode.ColorPalette: m_advanced.Content = GlobalConfig.GetStringResource("lang_AdvancedMode"); break;
                    }
                };
                //m_advanced.Content = "default/advanced";
                //m_advanced.Unchecked += (sender, e) =>
                //{
                //    m_advanced.Content = "default/advanced";//uncheck togglebutton, content return to default

                //};
                //m_advanced.Checked += (sender, e) =>
                //{
                //    m_advanced.Content = "standard";//checked, get into advanced mode, changed content into standard

                //};
            }
        }
    }
}
