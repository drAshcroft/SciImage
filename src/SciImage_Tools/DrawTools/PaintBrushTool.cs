/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using SciImage;
using SciImage.Plugins.Tools;
using SciImage.Plugins.Tools.DefaultTools;

namespace SciImage_Tools.DrawTools
{
    public class PaintBrushTool
        : PencilTool 
    {
       
        public PaintBrushTool(DocumentWorkspace documentWorkspace)
            : base(documentWorkspace,
                   SciImage.SciResources.SciResources.GetImageResource("Icons.glyphicons-235-brush.png"),
                   "Paint Brush",
                   "Draw",
                   'b',6,
                   false,
                   ToolBarConfigItems.Brush | ToolBarConfigItems.Pen | ToolBarConfigItems.Antialiasing | ToolBarConfigItems.AlphaBlending)
        {
            // initialize any state information you need
            mouseDown = false;
        }
    }
}
