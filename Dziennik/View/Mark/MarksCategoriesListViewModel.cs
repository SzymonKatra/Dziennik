using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Dziennik.CommandUtils;
using Dziennik.ViewModel;

namespace Dziennik.View
{
    public sealed class MarksCategoriesListViewModel : ObservableObject
    {
        public MarksCategoriesListViewModel(ObservableCollection<MarksCategoryViewModel> categories, ObservableCollection<SchoolClassControlViewModel> openedClasses)
        {
            m_addCategoryCommand = new RelayCommand(AddCategory);
            m_editCategoryCommand = new RelayCommand(EditCategory);

            m_categories = categories;
            m_openedClasses = openedClasses;
        }

        private ObservableCollection<SchoolClassControlViewModel> m_openedClasses;

        private ObservableCollection<MarksCategoryViewModel> m_categories;
        public ObservableCollection<MarksCategoryViewModel> Categories
        {
            get { return m_categories; }
        }

        private MarksCategoryViewModel m_selectedCategory;
        public MarksCategoryViewModel SelectedCategory
        {
            get { return m_selectedCategory; }
            set { m_selectedCategory = value; RaisePropertyChanged("SelectedCategory"); }
        }

        private RelayCommand m_addCategoryCommand;
        public ICommand AddCategoryCommand
        {
            get { return m_addCategoryCommand; }
        }

        private RelayCommand m_editCategoryCommand;
        public ICommand EditCategoryCommand
        {
            get { return m_editCategoryCommand; }
        }

        private void AddCategory(object e)
        {
            MarksCategoryViewModel marksCategory = new MarksCategoryViewModel();
            EditMarksCategoryViewModel dialogViewModel = new EditMarksCategoryViewModel(marksCategory, true);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditMarksCategoryViewModel.EditMarkCategoryResult.Ok)
            {
                GlobalConfig.GlobalDatabase.ViewModel.MarksCategories.Add(marksCategory);
            }
            if (dialogViewModel.Result != EditMarksCategoryViewModel.EditMarkCategoryResult.Cancel) GlobalConfig.GlobalDatabaseAutoSaveCommand.Execute(null);
        }
        private void EditCategory(object e)
        {
            m_selectedCategory.PushCopy();
            EditMarksCategoryViewModel dialogViewModel = new EditMarksCategoryViewModel(m_selectedCategory);
            GlobalConfig.Dialogs.ShowDialog(this, dialogViewModel);
            if (dialogViewModel.Result == EditMarksCategoryViewModel.EditMarkCategoryResult.RemoveCategory)
            {
                m_selectedCategory.PopCopy(WorkingCopyResult.Ok);
                foreach (var schoolClass in m_openedClasses)
                {
                    foreach (var schoolGroup in schoolClass.ViewModel.Groups)
                    {
                        foreach (var student in schoolGroup.Students)
                        {
                            foreach (var mark in student.FirstSemester.Marks)
                            {
                                if (mark.Category == m_selectedCategory) mark.Category = null;
                            }
                            foreach (var mark in student.SecondSemester.Marks)
                            {
                                if (mark.Category == m_selectedCategory) mark.Category = null;
                            }
                        }
                    }
                }

                GlobalConfig.GlobalDatabase.ViewModel.MarksCategories.Remove(m_selectedCategory);
                SelectedCategory = null;
            }
            else if (dialogViewModel.Result == EditMarksCategoryViewModel.EditMarkCategoryResult.Ok)
            {
                m_selectedCategory.PopCopy(WorkingCopyResult.Ok);
            }
            else if (dialogViewModel.Result == EditMarksCategoryViewModel.EditMarkCategoryResult.Cancel)
            {
                m_selectedCategory.PopCopy(WorkingCopyResult.Cancel);
            }

            if (dialogViewModel.Result != EditMarksCategoryViewModel.EditMarkCategoryResult.Cancel) GlobalConfig.GlobalDatabaseAutoSaveCommand.Execute(null);
        }
    }
}
