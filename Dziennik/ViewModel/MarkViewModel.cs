using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;
using System.Globalization;
using System.Xml.Linq;

namespace Dziennik.ViewModel
{
    public sealed class MarkViewModel : ObservableObject, IModelExposable<Mark>
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
                RaisePropertyChanged("Value");
                RaisePropertyChanged("IsValueValid");
                RaisePropertyChanged("DisplayedMark");
                RaisePropertyChanged("ToolTipFormatted");
            }
        }
        public string Note
        {
            get { return m_model.Note; }
            set
            {
                m_model.Note = value;
                RaisePropertyChanged("Note");
                RaisePropertyChanged("IsValueValid");
                RaisePropertyChanged("DisplayedMark");
                RaisePropertyChanged("ToolTipFormatted");
            }
        }
        public int Weight
        {
            get { return m_model.Weight; }
            set { m_model.Weight = value; }
        }
        public DateTime AddDate
        {
            get { return m_model.AddDate; }
            set { m_model.AddDate = value; RaisePropertyChanged("AddDate"); RaisePropertyChanged("ToolTipFormatted"); }
        }
        public DateTime LastChangeDate
        {
            get { return m_model.LastChangeDate; }
            set { m_model.LastChangeDate = value; RaisePropertyChanged("LastChangeDate"); RaisePropertyChanged("ToolTipFormatted"); }
        }
        public string Description
        {
            get { return m_model.Description; }
            set { m_model.Description = value; RaisePropertyChanged("Description"); RaisePropertyChanged("ToolTipFormatted"); }
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
                return string.Format("{6}: {1}{0}Waga: {2}{0}Opis: {3}{0}Data dodania: {4}{0}Ostatnia zmiana: {5}",
                                     Environment.NewLine,
                                     this.DisplayedMark,
                                     this.Weight,
                                     this.Description,
                                     this.AddDate.ToString(GlobalConfig.DateTimeFormat),
                                     this.LastChangeDate.ToString(GlobalConfig.DateTimeFormat),
                                     (IsValueValid ? "Ocena" : "Uwaga"));
            }
        }
    }
}
