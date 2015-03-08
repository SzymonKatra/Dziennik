using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dziennik.Model;
using System.Globalization;
using System.Xml.Linq;

namespace Dziennik.ViewModel
{
    public sealed class MarkViewModel : ViewModelBase<MarkViewModel, Mark>
    {
        public MarkViewModel()
            : this(new Mark())
        {
        }
        public MarkViewModel(Mark model)
            : base(model)
        {
            m_category = GlobalConfig.GlobalDatabase.ViewModel.MarksCategories.FirstOrDefault(x => x.Model.Id == Model.GlobalCategoryId);
        }

        public decimal Value
        {
            get { return Model.Value; }
            set
            {
                Model.Value = value;
                RaisePropertyChanged("Value");
                RaisePropertyChanged("IsValueValid");
                RaisePropertyChanged("DisplayedMark");
                RaisePropertyChanged("ToolTipFormatted");
            }
        }
        public string Note
        {
            get { return Model.Note; }
            set
            {
                Model.Note = value;
                RaisePropertyChanged("Note");
                RaisePropertyChanged("IsValueValid");
                RaisePropertyChanged("DisplayedMark");
                RaisePropertyChanged("ToolTipFormatted");
            }
        }
        public int Weight
        {
            get { return Model.Weight; }
            set { Model.Weight = value; RaisePropertyChanged("Weight"); RaisePropertyChanged("DisplayedWeight"); }
        }
        public DateTime AddDate
        {
            get { return Model.AddDate; }
            set { Model.AddDate = value; RaisePropertyChanged("AddDate"); RaisePropertyChanged("ToolTipFormatted"); }
        }
        public DateTime LastChangeDate
        {
            get { return Model.LastChangeDate; }
            set { Model.LastChangeDate = value; RaisePropertyChanged("LastChangeDate"); RaisePropertyChanged("ToolTipFormatted"); }
        }
        public string Description
        {
            get { return Model.Description; }
            set { Model.Description = value; RaisePropertyChanged("Description"); RaisePropertyChanged("ToolTipFormatted"); }
        }
        private MarksCategoryViewModel m_category;
        public MarksCategoryViewModel Category
        {
            get { return m_category; }
            set
            {
                m_category = value;
                Model.GlobalCategoryId = (m_category == null ? null : m_category.Model.Id);
                RaisePropertyChanged("Category");
                RaisePropertyChanged("ToolTipFormatted");
            }
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
                    return GetValidDisplayedMark(this.Value);
                }
                else return this.Note;
            }
        }
        public string DisplayedWeight
        {
            get
            {
                if (IsValueValid)
                {
                    return this.Weight.ToString();
                }
                else return string.Empty;
            }
        }
        public string ToolTipFormatted
        {
            get
            {
                return string.Format("{6}: {1}{0}Waga: {2}{0}Opis: {3}{0}Kategoria: {4}{0}Data dodania: {5}{0}Ostatnia zmiana: {6}",
                                     Environment.NewLine,
                                     this.DisplayedMark,
                                     this.Weight,
                                     this.Description,
                                     (this.Category == null ? string.Empty : this.Category.Name),
                                     this.AddDate.ToString(GlobalConfig.DateTimeFormat),
                                     this.LastChangeDate.ToString(GlobalConfig.DateTimeFormat),
                                     (IsValueValid ? "Ocena" : "Uwaga"));
            }
        }

        public static string GetValidDisplayedMark(decimal value)
        {
            //if (value < 1M || value > 6M) return string.Empty;
            decimal truncated = decimal.Truncate(value);
            string result = truncated.ToString(CultureInfo.InvariantCulture);
            if (value - truncated == 0.5M) result += "+";
            return result;
        }

        protected override void OnPushCopy()
        {
            ObjectsPack pack = new ObjectsPack();
            pack.Write(this.Value);
            pack.Write(this.Note);
            pack.Write(this.Weight);
            pack.Write(this.AddDate);
            pack.Write(this.LastChangeDate);
            pack.Write(this.Description);
            pack.Write(this.Category);

            CopyStack.Push(pack);
        }
        protected override void OnPopCopy(WorkingCopyResult result)
        {
            ObjectsPack pack = CopyStack.Pop();

            if (result == WorkingCopyResult.Cancel)
            {
                this.Value = (decimal)pack.Read();
                this.Note = (string)pack.Read();
                this.Weight = (int)pack.Read();
                this.AddDate = (DateTime)pack.Read();
                this.LastChangeDate = (DateTime)pack.Read();
                this.Description = (string)pack.Read();
                this.Category = (MarksCategoryViewModel)pack.Read();
            }
        }
    }
}
