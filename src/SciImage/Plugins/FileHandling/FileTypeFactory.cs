/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SciImage
{
    /// <summary>
    /// Provides static method and properties for obtaining all the FileType objects
    /// responsible for loading and saving Document instances. Loads FileType plugins
    /// too.
    /// </summary>
    public sealed class FileTypeFactory
    {
       
        private static FileTypeCollection collection;
       
        public static FileTypeCollection GetFileTypes()
        {
            if (collection == null)
            {
                collection = LoadFileTypes();
            }

            return collection;
        }

        static  FileTypeFactory ()
        {
            GetFileTypes ();
        }

        private static FileTypeCollection LoadFileTypes()
        {
            List<Type> LoadedTypes = new List<Type>();
            List<Assembly> assemblies = new List<Assembly>();

            
            assemblies.Add(Assembly.GetAssembly(typeof(FileType  )));

            // TARGETDIR\Effects\*.dll
            string homeDir = Assembly.GetExecutingAssembly().CodeBase ;
            homeDir+="\\FileTypes\\";
            // System.IO.Path.GetDirectoryName(PluginPath);

            bool dirExists;

            try
            {
                dirExists = Directory.Exists(homeDir);
            }
            catch
            {
                dirExists = false;
            }

            if (dirExists)
            {
                string fileSpec = "*.dll";
                string[] filePaths = Directory.GetFiles(homeDir, fileSpec);

                foreach (string filePath in filePaths)
                {
                    Assembly pluginAssembly = null;

                    try
                    {
                        pluginAssembly = Assembly.LoadFrom(filePath);
                        assemblies.Add(pluginAssembly);
                    }
                    catch
                    {
                        //Tracing.Ping("Exception while loading " + filePath + ": " + ex.ToString());
                    }
                }

            }
            
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {

                    //System.Diagnostics.Debug.Print(type.ToString());
                    try
                    {
                        if (typeof(FileType ).IsAssignableFrom(type))
                        {
                            LoadedTypes.Add( type );
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return new FileTypeCollection(LoadedTypes);
            
        }
    }
}
