using SciImage.Core.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciImage.PaintForms
{
    public interface IHistoryForm
    {
        HistoryStack HistoryStack { set; }
        void NewActiveDocument(DocumentWorkspace activeDocument);
    }
}
