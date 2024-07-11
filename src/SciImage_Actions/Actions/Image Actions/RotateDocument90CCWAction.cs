using System.Collections.Generic;
using SciImage;
using SciImage.Core.History.HistoryMementos;
using SciImage.Plugins.Actions;

namespace SciImage_Actions.Actions.Image_Actions
{
    public sealed class RotateDocument90CCWAction
        : PluginAction
    {
        public override  string Name
        {
            get
            {
                return "Rotate CounterClockwise 90 Degrees ";
            }
        }
        public override System.Drawing.Image Image
        {
            get { return null; }
        }
        public override string MainMenuName
        {
            get { return "Image"; }
        }
        public override string SubMenuName
        {
            get { return ""; }
        }

        public override System.Windows.Forms.Keys ShortCutKeys
        {
            get {
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G);
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 3; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 1; }
        }
        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {
            if (documentWorkspace == null)
            {
                return ActionDisplayOptions.Visible;
            }
            if (documentWorkspace.ActiveLayer.Surface!=null)
            {
                return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;
            }
            return ActionDisplayOptions.Hidden;
        }
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            RotateDocumentHelper rdh = new RotateDocumentHelper();
            HistoryMemento hm = rdh.OnExecute(ActiveDocumentWorkspace, RotateDocumentHelper.RotateType.CounterClockwise90, TargetLayerIndex);
            if (OptionalHistoryRecord == null)
                ActiveDocumentWorkspace.History.PushNewMemento(hm);
            else
                OptionalHistoryRecord.Add(hm);
            return true;
        }

        public RotateDocument90CCWAction()
            : base(ActionFlags.None)
        {
        }

    }
}
