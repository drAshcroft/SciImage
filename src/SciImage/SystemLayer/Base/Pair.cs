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
    public static class Pair
    {
        public static Pair<T, U> Create<T, U>(T first, U second)
        {
            return new Pair<T, U>(first, second);
        }
    }
}
