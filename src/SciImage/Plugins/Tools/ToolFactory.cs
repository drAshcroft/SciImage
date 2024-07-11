using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SciImage.Plugins.Tools.DefaultTools;
using SciImage.Plugins.Tools.Events;

using SciImage.SciResources;

using SciImage.SystemLayer.System;

namespace SciImage.Plugins.Tools
{
    public class ToolFactory
    {
        static ToolFactory()
        {
            _ToolFactory = new ToolFactory();
           _ToolFactory.FindTools();
        }

        private static ToolFactory _ToolFactory = null;

        public static ToolFactory Factory
        {
            get { return _ToolFactory; }
        }

        //keep this from being created outside of the factory
        private ToolFactory()
        {

        }

        public static readonly Type StaticToolType = typeof(PencilTool);

        private  Type[] tools;

        public  Tool CreateTool(Type toolType, DocumentWorkspace dc)
        {
            ConstructorInfo ci = toolType.GetConstructor(new Type[] { typeof(DocumentWorkspace) });
            Tool tool = (Tool)ci.Invoke(new object[] { dc });
            return tool;
        }

        public  Type[] Tools
        {
            get
            {
                return (Type[])tools.Clone();
            }
        }

        private  List<ToolMenuInfo> _ToolInfos = null;

        public  List<ToolMenuInfo> ToolInfos
        {
            get
            {
                return _ToolInfos;
            }
        }

        public event ToolClickedEventHandler ToolClicked;

        public void OnToolClick(object sender, Type toolType)
        {
            if (ToolClicked != null)
            {
                ToolClicked(sender, new ToolClickedEventArgs(toolType));
            }
            if (DocumentManager.Manager.ActiveDocumentWorkspace != null)
            {
                DocumentManager.Manager.ActiveDocumentWorkspace.Focus();
                DocumentManager.Manager.ActiveDocumentWorkspace.SetToolFromType(toolType);
            }
        }

        public void LoadDefaultToolType()
        {
            string defaultToolTypeName = Settings.CurrentUser.GetString(SettingNames.DefaultToolTypeName, ToolFactory.Factory.DefaultToolType.Name);

            ToolMenuInfo[] tis = ToolFactory.Factory.ToolInfos.ToArray();
            ToolMenuInfo ti = Array.Find(
                tis,
                delegate (ToolMenuInfo check)
                {
                    if (string.Compare(defaultToolTypeName, check.ToolType.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });

            if (ti == null)
            {
                DefaultToolType = ToolFactory.StaticToolType;
            }
            else
            {
                DefaultToolType = ti.ToolType;
            }
        }

        private Type defaultToolTypeChoice = typeof( SciImage.Plugins.Tools.DefaultTools.PencilTool);

        public  Type DefaultToolType
        {
            get
            {
                return defaultToolTypeChoice;
            }

            set
            {
                defaultToolTypeChoice = value;
                Settings.CurrentUser.SetString(SettingNames.DefaultToolTypeName, value.Name);
            }
        }

        public  void FindTools()
        {
            List<Assembly> assemblies = new List<Assembly>();
            string homeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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
                assemblies.Clear();
                foreach (string filePath in filePaths)
                {
                    Assembly pluginAssembly = null;

                    try
                    {
                        pluginAssembly = Assembly.LoadFrom(filePath);
                        assemblies.Add(pluginAssembly);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print("Exception while loading " + filePath + ": " + ex.ToString());
                    }
                }

            }

            List<Type> foundToolsList = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    try
                    {
                        if (typeof(Tool).IsAssignableFrom(type))
                        {
                            foundToolsList.Add(type);
                        }
                    }
                    catch
                    {
                    }
                }
            }

            _ToolInfos = new List<ToolMenuInfo>();
            tools = foundToolsList.ToArray();
            foreach (Type toolType in foundToolsList)
            {
                try
                {
                    using (Tool tool = CreateTool(toolType, null))
                    {
                        _ToolInfos.Add( tool.Info);
                       
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
            }
            _ToolInfos.Sort((a, b) => a.Order.CompareTo(b.Order));
        }
    }
}