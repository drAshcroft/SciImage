/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using SciImage.Plugins.Tools.Infos;

namespace SciImage.Plugins.Tools.ToolConfigs
{
    public interface ISelectionDrawModeConfig
    {
       // event EventHandler SelectionDrawModeInfoChanged;

        SelectionDrawModeInfo SelectionDrawModeInfo
        {
            get;
            set;
        }

        void PerformSelectionDrawModeInfoChanged();
    }
}