/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System.Windows.Forms;

namespace SciImage.SystemLayer.ScanningAndCapture
{
    public interface IWiaInterface
    {
        bool IsComponentAvailable
        {
            get;
        }

        bool CanPrint
        {
            get;
        }

        bool CanScan
        {
            get;
        }

        void Print(Control owner, string fileName);

        ScanResult Scan(Control owner, string fileName);
    }
}
