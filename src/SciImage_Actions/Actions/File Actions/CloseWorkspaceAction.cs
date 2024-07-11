/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using SciImage;
using SciImage.Core;
using SciImage.Core.History.HistoryMementos;
using SciImage.Forms;
using SciImage.PaintForms.UserControls;
using SciImage.PaintForms.UserControls.Buttons;
using SciImage.Plugins.Actions;
using SciImage.SciResources;

namespace SciImage_Actions.Actions.File_Actions
{
    public sealed class CloseWorkspaceAction
        : PluginAction
    {
        public override ActionDisplayOptions CheckIfEnabled(DocumentWorkspace documentWorkspace)
        {

            return ActionDisplayOptions.Visible | ActionDisplayOptions.Enabled;
        }
        public override string Name
        {
            get
            {
                return "Close Document";
            }
        }
        public override System.Drawing.Image Image
        {
            get { return null; }
        }
        public override string MainMenuName
        {
            get { return "File"; }
        }
        public override string SubMenuName
        {
            get { return ""; }
        }
        public override System.Windows.Forms.Keys ShortCutKeys
        {
            get
            {
                return (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W);
            }
        }
        public override int MenuSubGroupIndex
        {
            get { return 1; }
        }
        public override int SuggestedMenuOrder
        {
            get { return 5; }
        }
        public override bool PerformAction( List<HistoryMemento> OptionalHistoryRecord, int TargetLayerIndex)
        {
            if (FormsManager.BaseForm == null)
            {
                throw new ArgumentNullException("appWorkspace");
            }

            DocumentWorkspace dw;

           
            dw = ActiveDocumentWorkspace;
            

            if (dw != null)
            {
                if (dw.Document == null)
                {
                    DocumentManager.Manager.RemoveDocumentWorkspace(dw);
                }
                else if (!dw.Document.Dirty)
                {
                    DocumentManager.Manager.RemoveDocumentWorkspace(dw);
                }
                else
                {
                    DocumentManager.Manager.ActiveDocumentWorkspace = dw;

                    TaskButton saveTB = new TaskButton(
                        SciImage.SciResources.SciResources.GetImageResource("Icons.MenuFileSaveIcon.png").Reference,
                        "&Save",
                        "Save Image before exiting");

                    TaskButton dontSaveTB = new TaskButton(
                        SciImage.SciResources.SciResources.GetImageResource("Icons.MenuFileCloseIcon.png").Reference,
                        "Do Not Save",
                        "Discard Image");

                    TaskButton cancelTB = new TaskButton(
                        SciImage.SciResources.SciResources.GetImageResource("Icons.CancelIcon.png").Reference,
                        "Cancel Close",
                        "Do not close Program");

                    string title = "There are unsaved changed what would you like to do.";
                    string introTextFormat = "You have not saved {0} . ";
                    string introText = string.Format(introTextFormat, dw.GetFriendlyName());

                    Image thumb = DocumentManager.Manager.GetDocumentWorkspaceThumbnail(dw);

                    if (thumb == null)
                    {
                        thumb = new Bitmap(32, 32);
                    }

                    Bitmap taskImage = new Bitmap(thumb.Width + 2, thumb.Height + 2, PixelFormat.Format32bppArgb);

                    using (Graphics g = Graphics.FromImage(taskImage))
                    {
                        g.Clear(Color.Transparent);

                        g.DrawImage(
                            thumb, 
                            new Rectangle(1, 1, thumb.Width, thumb.Height), 
                            new Rectangle(0, 0, thumb.Width, thumb.Height), 
                            GraphicsUnit.Pixel);

                        Utility.DrawDropShadow1px(g, new Rectangle(0, 0, taskImage.Width, taskImage.Height));
                    }

                    Form mainForm = FormsManager.BaseForm.FindForm();
                    if (mainForm != null)
                    {
                        SciBaseForm asPDF = mainForm as SciBaseForm;

                        if (asPDF != null)
                        {
                            asPDF.RestoreWindow();
                        }
                    }

                    Icon warningIcon;
                    ImageResource warningIconImageRes = SciImage.SciResources.SciResources.GetImageResource("Icons.WarningIcon.png");

                    if (warningIconImageRes != null)
                    {
                        Image warningIconImage = warningIconImageRes.Reference;
                        warningIcon = Utility.ImageToIcon(warningIconImage, false);
                    }
                    else
                    {
                        warningIcon = null;
                    }                     

                    TaskButton clickedTB = TaskDialog.Show(
                        FormsManager.BaseForm,
                        warningIcon,
                        title,
                        taskImage,
                        false,
                        introText,
                        new TaskButton[] { saveTB, dontSaveTB, cancelTB },
                        saveTB,
                        cancelTB,
                        340);                        

                    if (clickedTB == saveTB)
                    {
                        if (dw.DoSave())
                        {

                            DocumentManager.Manager.RemoveDocumentWorkspace(dw);
                        }
                        else
                        {
                            
                        }
                    }
                    else if (clickedTB == dontSaveTB)
                    {

                        DocumentManager.Manager.RemoveDocumentWorkspace(dw);
                    }
                    else
                    {
                        
                    }
                }
            }

            Utility.GCFullCollect();
            return true ;
        }

        public CloseWorkspaceAction()
        {
        }

      
    }
}
