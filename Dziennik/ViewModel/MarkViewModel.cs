﻿using System;
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
        public MarkViewModel(Mark model) : base(model)
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

        public static string GetValidDisplayedMark(decimal value)
        {
            if (value < 1M || value > 6M) return string.Empty;
            decimal truncated = decimal.Truncate(value);
            string result = truncated.ToString(CultureInfo.InvariantCulture);
            if (value - truncated == 0.5M) result += "+";
            return result;
        }

        public override void CopyDataTo(MarkViewModel viewModel)
        {
            viewModel.Value = this.Value;
            viewModel.Note = this.Note;
            viewModel.Weight = this.Weight;
            viewModel.AddDate = this.AddDate;
            viewModel.LastChangeDate = this.LastChangeDate;
            viewModel.Description = this.Description;
            viewModel.Category = this.Category;
        }
    }
}
