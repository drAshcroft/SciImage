/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using SciImage.Core;
using System;
using System.Collections.Generic;
using System.Text;
using SciImage.Core.Surfaces.Layers;

namespace SciImage.Core.History
{
    public interface IHistoryWorkspace
    {
        Document Document
        {
            get;
            set;
        }

        SciImage.Core.Selection.Selection Selection
        {
            get;
        }

        Layer ActiveLayer
        {
            get;
        }

        int ActiveLayerIndex
        {
            get;
        }
    }
}
