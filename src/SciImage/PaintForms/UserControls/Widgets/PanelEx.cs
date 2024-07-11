/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;
using SciImage.SystemLayer.Forms;

namespace SciImage.PaintForms.UserControls.Widgets
{
    public class PanelEx : 
        ScrollPanel
    {
        private bool hideHScroll = false;

        public bool HideHScroll
        {
            get
            {
                return this.hideHScroll;
            }

            set
            {
                this.hideHScroll = value;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.hideHScroll)
            {
                UI.SuspendControlPainting(this);
            }

            base.OnSizeChanged(e);

            if (this.hideHScroll)
            {
                UI.HideHorizontalScrollBar(this);
                UI.ResumeControlPainting(this);
                Invalidate(true);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            //base.OnMouseWheel(e);
        }
    }
}
