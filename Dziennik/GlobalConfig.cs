using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Dziennik.View;

namespace Dziennik
{
    public static class GlobalConfig
    {
        public static readonly int DecimalRoundingPoints = 2;
        public static readonly string DateTimeFormat = "dd.MM.yyyy HH:mm";

        public static readonly DialogService Dialogs;

        static GlobalConfig()
        {
            Dictionary<Type, Func<object, Window>> windowViewModelMappings = new Dictionary<Type, Func<object, Window>>();

            windowViewModelMappings.Add(typeof(EditMarkViewModel), (vm) => { return new EditMarkWindow((EditMarkViewModel)vm); });
            windowViewModelMappings.Add(typeof(EditStudentViewModel), (vm) => { return new EditStudentWindow((EditStudentViewModel)vm); });
            windowViewModelMappings.Add(typeof(ActionDialogViewModel), (vm) => { return new ActionDialogWindow((ActionDialogViewModel)vm); });
            windowViewModelMappings.Add(typeof(GlobalStudentsListViewModel), (vm) => { return new GlobalStudentsListWindow((GlobalStudentsListViewModel)vm); });
            windowViewModelMappings.Add(typeof(AddGroupViewModel), (vm) => { return new AddGroupWindow((AddGroupViewModel)vm); });
            windowViewModelMappings.Add(typeof(SelectStudentsViewModel), (vm) => { return new SelectStudentsWindow((SelectStudentsViewModel)vm); });
            windowViewModelMappings.Add(typeof(EditGroupViewModel), (vm) => { return new EditGroupWindow((EditGroupViewModel)vm); });

            Dialogs = new DialogService(windowViewModelMappings);
        }
    }
}
