using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SciImage
{
    public interface  IControlHoldingForm
    {
        Control MainUserControl { get; }
        Form HostForm { get; }
    }
}
