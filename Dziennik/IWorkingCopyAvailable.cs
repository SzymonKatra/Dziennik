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
        int CopyDepth { get; }

        void PushCopy();
        void PopCopy(WorkingCopyResult result);
    }
}
