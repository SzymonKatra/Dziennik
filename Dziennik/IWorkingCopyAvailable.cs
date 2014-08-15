using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dziennik
{
    public enum WorkingCopyResult
    {
        Ok,
        Cancel,
    }

    public interface IWorkingCopyAvailable
    {
        void StartWorkingCopy();
        void EndWorkingCopy(WorkingCopyResult result);
    }
}
