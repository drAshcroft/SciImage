/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) Rick Brewster, Tom Jackson, and past contributors.            //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

namespace SciImage.SciResources
{
    /// <summary>
    /// Contains strings that must be the same no matter what locale the UI is running with.
    /// </summary>
    public static class InvariantStrings
    {
        // {0} is "All Rights Reserved"
        // Legal has advised that's the only part of this string that should be localizable.
        public const string CopyrightFormat = 
            "Copyright © 2007 Rick Brewster, Tom Jackson, and past contributors. Portions Copyright © 2007 Microsoft Corporation. {0}";

        public const string FeedbackEmail = "mono-paint-port@groups.google.com";

        public const string WebsiteUrl = "";

        public const string WebsitePageHelpMenu = "";

        public const string ForumPageHelpPage = "";

        public const string PluginsPageHelpPage = "";

        public const string TutorialsPageHelpPage = "";

        public const string DonatePageHelpMenu = "";

        public const string DonateUrlSetup = "";

        public const string ExpiredPage = "";

        public const string EffectsSubDir = "Effects";

        public const string FileTypesSubDir = "FileTypes";

        public const string DllExtension = ".dll";

        // Fallback strings are used in case the resources file is unavailable.
        public const string CrashLogHeaderTextFormatFallback =
            @"This text file was created because Mono Paint crashed.
Please e-mail this file to {0} so we can diagnose and fix the problem.
";
        public const string SearchEngineHelpMenu = "";
        public const string CrashlogEmail = "";

        public const string StartupUnhandledErrorFormatFallback =
            "There was an unhandled error, and Mono Paint must be closed. Refer to the file '{0}', which has been placed on your desktop, for more information.";
    }
}
