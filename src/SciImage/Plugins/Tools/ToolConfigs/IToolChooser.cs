/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace SciImage.Plugins.Tools.ToolConfigs
{
    public interface IToolChooser
    {
        void SelectTool(Type toolType);
        void SelectTool(Type toolType, bool raiseEvent);
        void SetTools(ToolMenuInfo[] toolInfos);
       // event ToolClickedEventHandler ToolClicked;
    }
}
