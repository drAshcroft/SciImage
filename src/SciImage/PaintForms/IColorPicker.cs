using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciImage.PaintForms
{
   public interface IColorPicker
    {
        UserControls.ColorPickers.PaletteCollection PaletteCollection
        {
            get; set;
        }

        UserControls.ColorPickers.WhichUserColor WhichUserColor { get; set; }

       void  _PrimaryColorChanged(object sender, EventArgs e);
     void    _SecondaryColorChanged(object sender, EventArgs e);
    }
}
