/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) Rick Brewster, Tom Jackson, and past contributors.            //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SciImage.SystemLayer.System;

namespace SciImage.SciResources
{
    /// <summary>
    /// A few utility functions specific to SciImage.exe
    /// </summary>
    public static class SciInfo
    {
        private static Icon appIcon;
        public static Icon AppIcon
        {
            get
            {
                try
                {
                    if (appIcon == null)
                    {
                        Stream stream = SciResources.GetResourceStream("Icons.SciImage.ico");
                        appIcon = new Icon(stream);
                        stream.Close();
                    }
                }
                catch { }
                return appIcon;
            }
        }

        /// <summary>
        /// Gets the full path to where user customization files should be stored.
        /// </summary>
        /// <returns>
        /// User data files should include settings or customizations that don't go into data files such as *.PDN.
        /// An example of a user data file is a color palette.
        /// </returns>
        public static string UserDataPath
        {
            get
            {
                string myDocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string userDataDirName = SciResources.GetString("SystemLayer.UserDataDirName");
                string userDataPath = Path.Combine(myDocsPath, userDataDirName);
                return userDataPath;
            }
        }

        private static StartupTestType startupTest = StartupTestType.None;
        public static StartupTestType StartupTest
        {
            get 
            {
                return startupTest; 
            }

            set 
            {
                startupTest = value; 
            }
        }

        private static bool isTestMode = false;
        public static bool IsTestMode
        {
            get
            {
                return isTestMode;
            }

            set
            {
                isTestMode = value;
            }
        }

        public static DateTime BuildTime
        {
            get
            {
                Version version = GetVersion();

                DateTime time = new DateTime(2000, 1, 1, 0, 0, 0);
                time = time.AddDays(version.Build);
                time = time.AddSeconds(version.Revision * 2);

                return time;
            }
        }

        private static string GetAppConfig()
        {
            object[] attributes = typeof(SciInfo).Assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
            AssemblyConfigurationAttribute aca = (AssemblyConfigurationAttribute)attributes[0];
            return aca.Configuration;
        }

        private static readonly bool isFinalBuild = GetIsFinalBuild();

        private static bool GetIsFinalBuild()
        {
            return !(GetAppConfig().IndexOf("Final") == -1);
        }

        public static bool IsFinalBuild
        {
            get
            {
                return isFinalBuild;
            }
        }

        public static bool IsDebugBuild
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        // Pre-release builds expire after this many days. (debug+"final" also equals expiration)
        public const int BetaExpireTimeDays = 30;

        public static DateTime ExpirationDate
        {
            get
            {
                if (SciInfo.IsFinalBuild && !IsDebugBuild)
                {
                    return DateTime.MaxValue;
                }
                else
                {
                    return SciInfo.BuildTime + new TimeSpan(BetaExpireTimeDays, 0, 0, 0);
                }
            }
        }

        public static bool IsExpired
        {
            get
            {
		//return false;

                if (!SciInfo.IsFinalBuild || SciInfo.IsDebugBuild)
                {
                    if (DateTime.Now > SciInfo.ExpirationDate)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Checks if the build is expired, and displays a dialog box that takes the user to
        /// the Paint.NET website if necessary.
        /// </summary>
        /// <returns>true if the user should be allowed to continue, false if the build has expired</returns>
        public static bool HandleExpiration(IWin32Window owner)
        {
            if (IsExpired)
            {
                string expiredMessage = SciResources.GetString("ExpiredDialog.Message");

                DialogResult result = MessageBox.Show(expiredMessage, SciInfo.GetProductName(true),
                    MessageBoxButtons.OKCancel);

                if (result == DialogResult.OK)
                {
                    string expiredRedirect = InvariantStrings.ExpiredPage;
                    SciInfo.LaunchWebSite(owner, expiredRedirect);
                }

                return false;
            }

            return true;
        }

        public static string GetApplicationDir()
        {
            string appPath = Application.StartupPath;
            return appPath;
        }

        /// <summary>
        /// For final builds, returns a string such as "Paint.NET v2.6"
        /// For non-final builds, returns a string such as "Paint.NET v2.6 Beta 2"
        /// </summary>
        /// <returns></returns>
        public static string GetProductName()
        {
            return GetProductName(!IsFinalBuild);
        }

        public static string GetProductName(bool withTag)
        {
            string bareProductName = GetBareProductName();
            string productNameFormat = SciResources.GetString("Application.ProductName.Format");
            string tag;

            if (withTag)
            {
                string tagFormat = SciResources.GetString("Application.ProductName.Tag.Format");
                tag = string.Format(tagFormat, GetAppConfig());
            }
            else
            {
                tag = string.Empty;
            }

            string version = GetVersion().ToString(2);

            string productName = string.Format(
                productNameFormat,
                bareProductName,
                version,
                tag);

            return productName;
        }

        /// <summary>
        /// Returns the bare product name, e.g. "Paint.NET"
        /// </summary>
        public static string GetBareProductName()
        {
            return SciResources.GetString("Application.ProductName.Bare");
        }

        private static string copyrightString = null;
        public static string GetCopyrightString()
        {
            if (copyrightString == null)
            {
                string format = InvariantStrings.CopyrightFormat;
                string allRightsReserved = SciResources.GetString("Application.Copyright.AllRightsReserved");
                copyrightString = string.Format(CultureInfo.CurrentCulture, format, allRightsReserved);
            }

            return copyrightString;
        }

        public static Version GetVersion()
        {
            return new Version(Application.ProductVersion);
        }

        private static string GetConfigurationString()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
            AssemblyConfigurationAttribute aca = (AssemblyConfigurationAttribute)attributes[0];
            return aca.Configuration;
        }

        /// <summary>
        /// Returns a full version string of the form: ApplicationConfiguration + BuildType + BuildVersion
        /// i.e.: "Beta 2 Debug build 1.0.*.*"
        /// </summary>
        /// <returns></returns>
        public static string GetVersionString()
        {
            string buildType =
#if DEBUG
                "Debug";
#else
                "Release";
#endif
                
            string versionFormat = SciResources.GetString("SciImage.SciResource.SciInfo.VersionString.Format");
            string versionText = string.Format(
                versionFormat, 
                GetConfigurationString(), 
                buildType, 
                Application.ProductVersion);

            return versionText;
        }

        /// <summary>
        /// Returns a version string that is presentable without the Paint.NET name. example: "version 2.5 Beta 5"
        /// </summary>
        /// <returns></returns>
        public static string GetFriendlyVersionString()
        {
            Version version = SciInfo.GetVersion();
            string versionFormat = SciResources.GetString("SciImage.SciResource.SciInfo.FriendlyVersionString.Format");
            string configFormat = SciResources.GetString("SciImage.SciResource.SciInfo.FriendlyVersionString.ConfigWithSpace.Format");
            string config = string.Format(configFormat, GetConfigurationString());
            string configText;
            
            if (SciInfo.IsFinalBuild)
            {
                configText = string.Empty;
            }
            else
            {
                configText = config;
            }

            string versionText = string.Format(versionFormat, version.ToString(2), configText);
            return versionText;
        }

        /// <summary>
        /// Returns the application name, with the version string. i.e., "Paint.NET v2.5 (Beta 2 Debug build 1.0.*.*)"
        /// </summary>
        /// <returns></returns>
        public static string GetFullAppName()
        {
            string fullAppNameFormat = SciResources.GetString("SciImage.SciResource.SciInfo.FullAppName.Format");
            string fullAppName = string.Format(fullAppNameFormat, SciInfo.GetProductName(false), GetVersionString());
            return fullAppName;
        }

        /// <summary>
        /// For final builds, this returns PdnInfo.GetProductName() (i.e., "Paint.NET v2.2")
        /// For non-final builds, this returns GetFullAppName()
        /// </summary>
        /// <returns></returns>
        public static string GetAppName()
        {
            if (SciInfo.IsFinalBuild && !SciInfo.IsDebugBuild)
            {
                return SciInfo.GetProductName(false);
            }
            else
            {
                return GetFullAppName();
            }
        }

        public static void LaunchWebSite(IWin32Window owner)
        {
            LaunchWebSite(owner, null);
        }

        public static void LaunchWebSite(IWin32Window owner, string page)
        {
            string webSite = InvariantStrings.WebsiteUrl;

            Uri baseUri = new Uri(webSite);
            Uri uri;

            if (page == null)
            {
                uri = baseUri;
            }
            else
            {
                uri = new Uri(baseUri, page);
            }

            string url = uri.ToString();

            if (url.IndexOf("@") == -1)
            {
                OpenUrl(owner, url);
            }
        }

        public static bool OpenUrl(IWin32Window owner, string url)
        {
            bool result = Shell.LaunchUrl(owner, url);

            if (!result)
            {
                string message = SciResources.GetString("LaunchLink.Error");
                MessageBox.Show(owner, message, SciInfo.GetBareProductName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }

        public static string GetNgenPath()
        {
            return GetNgenPath(false);
        }

        public static string GetNgenPath(bool force32bit)
        {
            string fxDir;

            if (UIntPtr.Size == 8 && !force32bit)
            {
                fxDir = "Framework64";
            }
            else
            {
                fxDir = "Framework";
            }

            string fxPathBase = @"%WINDIR%\Microsoft.NET\" + fxDir + @"\v";
            string fxPath = fxPathBase + Environment.Version.ToString(3) + @"\";
            string fxPathExp = System.Environment.ExpandEnvironmentVariables(fxPath);
            string ngenExe = Path.Combine(fxPathExp, "ngen.exe");

            return ngenExe;
        }
    }
}
