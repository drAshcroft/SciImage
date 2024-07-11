using SciImage.Plugins.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciImage.PaintForms
{
 public interface IToolPicker
    {
        void Environment_ToolChosen(Type tool);
        void SetTools(ToolMenuInfo[] toolInfos);
        void SelectTool(Type toolType);
    }
}
