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

            Dialogs = new DialogService(windowViewModelMappings);
        }
    }
}
