using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dziennik.CommandUtils;
using System.ComponentModel;

namespace Dziennik.Controls
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl, IDataErrorInfo
    {
        public NumericUpDown()
        {
            m_upCommand = new RelayCommand(Up);
            m_downCommand = new RelayCommand(Down);

            InitializeComponent();

            ValueInput = Value.ToString();
        }

        private RelayCommand m_upCommand;
        public ICommand UpCommand
        {
            get { return m_upCommand; }
        }

        private RelayCommand m_downCommand;
        public ICommand DownCommand
        {
            get { return m_downCommand; }
        }

        public static readonly DependencyProperty ButtonChangeOnlyProperty = DependencyProperty.Register("ButtonChangeOnly", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(null));
        public bool ButtonChangeOnly
        {
            get { return (bool)GetValue(ButtonChangeOnlyProperty); }
            set { SetValue(ButtonChangeOnlyProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(NumericUpDown), new PropertyMetadata(new PropertyChangedCallback((s, e) =>
        {
            ((NumericUpDown)s).ValueInput = e.NewValue.ToString();
        })));
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(int), typeof(NumericUpDown), new PropertyMetadata());
        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(int), typeof(NumericUpDown), new PropertyMetadata());
        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        private static readonly DependencyProperty ValueInputProperty = DependencyProperty.Register("ValueInput", typeof(string), typeof(NumericUpDown), new PropertyMetadata(null));
        public string ValueInput
        {
            get { return (string)GetValue(ValueInputProperty); }
            set { SetValue(ValueInputProperty, value); }
        }

        public string Error
        {
            get { return string.Empty; }
        }
        public string this[string columnName]
        {
            get
            {
                switch(columnName)
                {
                    case "ValueInput": return ValidateValueInput();
                }

                return string.Empty;
            }
        }
        private string ValidateValueInput()
        {
            int result;
            if (!int.TryParse(ValueInput, out result))
            {
                Value = MinValue;
                return string.Empty;
            }

            if (result < MinValue)
            {
                Value = MinValue;
                return string.Empty;
            }
            else if (result > MaxValue)
            {
                Value = MaxValue;
                return string.Empty;
            }

            Value = result;
            return string.Empty;
        }
        private void Up(object e)
        {
            if (Value < MaxValue) Value++;
        }
        private void Down(object e)
        {
            if (Value > MinValue) Value--;
        }
    }
}
