using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciImage.PaintForms
{
    public interface ILayerForm
    {
        void SuspendLayerPreviewUpdates();
        void ResumeLayerPreviewUpdates();
        void SetupNewDocument(DocumentWorkspace newDocument);
    }
}
