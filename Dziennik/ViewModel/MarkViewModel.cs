using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;
using System.Globalization;
using System.Xml.Linq;

namespace Dziennik.ViewModel
{
    public sealed class MarkViewModel : ObservableObject, IViewModelExposable<Mark>
    {
        public MarkViewModel()
            : this(new Mark())
        {
        }
        public MarkViewModel(Mark mark)
        {
            m_model = mark;
        }

        private Mark m_model;
        public Mark Model
        {
            get { return m_model; }
        }

        public decimal Value
        {
            get { return m_model.Value; }
            set
            {
                m_model.Value = value;
                OnPropertyChanged("Value");
                OnPropertyChanged("IsValueValid");
                OnPropertyChanged("DisplayedMark");
                OnPropertyChanged("ToolTipFormatted");
            }
        }
        public string Note
        {
            get { return m_model.Note; }
            set
            {
                m_model.Note = value;
                OnPropertyChanged("Note");
                OnPropertyChanged("IsValueValid");
                OnPropertyChanged("DisplayedMark");
                OnPropertyChanged("ToolTipFormatted");
            }
        }
        public DateTime AddDate
        {
            get { return m_model.AddDate; }
            set { m_model.AddDate = value; OnPropertyChanged("AddDate"); OnPropertyChanged("ToolTipFormatted"); }
        }
        public DateTime LastChangeDate
        {
            get { return m_model.LastChangeDate; }
            set { m_model.LastChangeDate = value; OnPropertyChanged("LastChangeDate"); OnPropertyChanged("ToolTipFormatted"); }
        }
        public string Description
        {
            get { return m_model.Description; }
            set { m_model.Description = value; OnPropertyChanged("Description"); OnPropertyChanged("ToolTipFormatted"); }
        }

        public bool IsValueValid
        {
            get
            {
                return string.IsNullOrWhiteSpace(Note);
            }
        }
        public string DisplayedMark
        {
            get
            {
                if (IsValueValid)
                {
                    return this.Value.ToString(CultureInfo.InvariantCulture);
                }
                else return this.Note;
            }
        }
        public string ToolTipFormatted
        {
            get
            {
                return string.Format("{5}: {1}{0}Opis: {2}{0}Data dodania: {3}{0}Ostatnia zmiana: {4}",
                                     Environment.NewLine,
                                     this.DisplayedMark,
                                     this.Description,
                                     this.AddDate.ToString(GlobalConfig.DateTimeFormat),
                                     this.LastChangeDate.ToString(GlobalConfig.DateTimeFormat),
                                     (IsValueValid ? "Ocena" : "Uwaga"));
            }
        }

        private const string XML_VALUE = "Value";
        private const string XML_NOTE = "Note";
        private const string XML_ADDDATE = "AddDate";
        private const string XML_LASTCHANGEDATE = "LastChangeDate";
        private const string XML_DESCRIPTION = "Description";
        public XElement ToXml(string elementName)
        {
            XElement root = new XElement(elementName);

            root.Add(new XElement(XML_VALUE, m_model.Value));
            root.Add(new XElement(XML_NOTE, m_model.Note));
            root.Add(new XElement(XML_ADDDATE, m_model.AddDate.ToBinary()));
            root.Add(new XElement(XML_LASTCHANGEDATE, m_model.LastChangeDate.ToBinary()));
            root.Add(new XElement(XML_DESCRIPTION, m_model.Description));

            return root;
        }
        public MarkViewModel ParseXml(XElement element)
        {
            XElement root = element;

            MarkViewModel result = new MarkViewModel();

            result.m_model.Value = decimal.Parse(root.Element(XML_VALUE).ValueOrDefault("0"), CultureInfo.InvariantCulture);
            result.m_model.Note = root.Element(XML_NOTE).ValueOrDefault(string.Empty);
            result.m_model.AddDate = DateTime.FromBinary(long.Parse(root.Element(XML_ADDDATE).ValueOrDefault("0")));
            result.m_model.LastChangeDate = DateTime.FromBinary(long.Parse(root.Element(XML_LASTCHANGEDATE).ValueOrDefault("0")));
            result.m_model.Description = root.Element(XML_DESCRIPTION).ValueOrDefault(string.Empty);

            return result;
        }
    }
}
