using System;

namespace SciImage.Plugins.Actions
{
    [Flags]
    public enum ActionFlags
    {
        None = 0,
        KeepToolActive = 1,
        Cancellable = 2,
        ReportsProgress = 4,
    }
}
