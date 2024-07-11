using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using SciImage.Core.History.HistoryMementos;

namespace SciImage.Plugins.Actions
{

    public abstract class PluginAction : IPluginMenuItem
    {
        public DocumentWorkspace ActiveDocumentWorkspace
        {
            get
            {
                return DocumentManager.Manager.ActiveDocumentWorkspace;
            }
            set
            {
                DocumentManager.Manager.ActiveDocumentWorkspace = value;
            }
        }

        private ActionFlags actionFlags;
        public ActionFlags ActionFlags
        {
            get
            {
                return this.actionFlags;
            }
        }

        public abstract ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace);
        /// <summary>
        /// This is the heading that shows in the top of the page
        /// </summary>
        public abstract string MainMenuName
        { get; }
        /// <summary>
        /// If this belongs to a submenu, name it here
        /// </summary>
        public abstract string SubMenuName
        { get; }


        /// <summary>
        /// Inorder to get everything into their little boxes, give the index here
        /// </summary>
        public abstract int MenuSubGroupIndex
        { get; }

        public double OrderParam
        {
            get { return MenuSubGroupIndex + .01 * SuggestedMenuOrder; }
        }

        public abstract int SuggestedMenuOrder
        { get; }

        public  double MenuOrder
        {
            get
            {
                return .1 * SuggestedMenuOrder +  MenuSubGroupIndex;
            }
        }

        /// <summary>
        /// This is the same that will be used in the scripting and macro dialog
        /// </summary>
        public abstract string Name
        { get; }

        /// <summary>
        /// If a menu image is required, place it here
        /// </summary>
        public abstract Image Image
        { get; }


        /// <summary>
        /// Assign any shortcut keys here.  This can cause conflicts, so check the already used
        /// keys manually.  A second application will overwrite the first so do not use common 
        /// keys such as ctrl+c...
        /// </summary>
        public abstract Keys ShortCutKeys
        { get; }



        /// <summary>
        /// Implement this to provide an action. If OptionHistoryList is initiallized then all
        /// the history momentos will be returned in order to make a compound event.  Otherwise
        /// you just let the action perform its own event.
        /// 
        /// </summary>
        /// <param name="appWorkspace" > The active application workspace</param>
        /// 
        /// <param name="OptionalHistoryRecord" >  If you wish to build a complex history momento, then you must send in a list, otherwise leave this null and the tool will do its own history work</param>
        /// <param name="TargetLayerIndex" >  This is the desired blayer index,  if you wish to edit the currently active blayer, use -1</param>
        /// <returns>If a history object is needed, then this will return a HistoryMemento object that will be placed onto the HistoryStack.</returns>
        public abstract bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex);

        public PluginAction(ActionFlags actionFlags)
        {
            this.actionFlags = actionFlags;
        }
        public PluginAction()
        {
        }

        public static PluginAction CreatePluginAction(Type Action)
        {


            ConstructorInfo ci = Action.GetConstructor(Type.EmptyTypes);
            PluginAction PA = (PluginAction)ci.Invoke(null);

            return PA;

        }
    }
}
