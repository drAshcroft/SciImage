/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System.Drawing;

namespace SciImage.PaintForms.UserControls.Buttons
{
    public sealed class TaskButton
    {
        private static TaskButton cancel = null;
        public static TaskButton Cancel
        {
            get
            {
                if (cancel == null)
                {
                    cancel = new TaskButton(
                        SciResources.SciResources.GetImageResource("Icons.CancelIcon.png").Reference,
                        SciResources.SciResources.GetString("TaskButton.Cancel.ActionText"),
                        SciResources.SciResources.GetString("TaskButton.Cancel.ExplanationText"));
                }

                return cancel;
            }
        }

        private Image image;
        private string actionText;
        private string explanationText;

        public Image Image
        {
            get
            {
                return this.image;
            }
        }

        public string ActionText
        {
            get
            {
                return this.actionText;
            }
        }

        public string ExplanationText
        {
            get
            {
                return this.explanationText;
            }
        }

        public TaskButton(Image image, string actionText, string explanationText)
        {
            this.image = image;
            this.actionText = actionText;
            this.explanationText = explanationText;
        }
    }
}
