/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

namespace SciImage.SystemLayer.Base
{
    public static class Triple
    {
        public static Triple<T, U, V> Create<T, U, V>(T first, U second, V third)
        {
            return new Triple<T, U, V>(first, second, third);
        }
    }
}
