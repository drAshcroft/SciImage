using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciImage.Plugins
{
  public   interface IPluginMenuItem
    {
        string MainMenuName { get;  }
        string SubMenuName { get; }
        string Name { get; }
        Image Image { get; }
        double MenuOrder { get; }
        System.Windows.Forms.Keys ShortCutKeys        { get; }

    }
}
