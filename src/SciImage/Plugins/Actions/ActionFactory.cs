using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SciImage.PaintForms.UserControls.ProgressBars;

namespace SciImage.Plugins.Actions
{
    public class ActionFactory
    {

        public static bool PerformAction(string ActionType)
        {
            List<IPluginMenuItem> AllActions = ActionFactory.GetAllAvailableActions;
            foreach (IPluginMenuItem ale in AllActions)
            {
                if (ale.GetType().ToString() == ActionType)
                {
                    return PerformAction( (PluginAction)ale);
                }
            }

            foreach (IPluginMenuItem ale in AllActions)
            {
                if (ale.GetType().ToString().Contains(ActionType))
                {
                    return PerformAction( (PluginAction)ale);
                }
            }

            return false;
        }

     
        public  static bool PerformAction( Type ActionType)
        {

            PluginAction performMe = (Actions.PluginAction)Activator.CreateInstance((Type)ActionType);
            return PerformAction( performMe);

        }
        private static bool PerformAction( PluginAction performMe)
        {
             using (new WaitCursorChanger())
            {
                return performMe.PerformAction( null, -1);
            }
        }

        private static List<IPluginMenuItem> LoadedActions = new List<IPluginMenuItem>();

        public static Dictionary<string, List<IPluginMenuItem>> CreateMenuStructure()
        {
            List<IPluginMenuItem> Actions = ActionFactory.GetAllAvailableActions;
            Dictionary<string, List<IPluginMenuItem>> MenuStructure = new Dictionary<string, List<IPluginMenuItem>>();

            foreach (IPluginMenuItem ale in Actions)
            {
                List<IPluginMenuItem> Menu;
                if (MenuStructure.ContainsKey(ale.MainMenuName) == true)
                {
                    Menu = MenuStructure[ale.MainMenuName];
                    MenuStructure.Remove(ale.MainMenuName);
                }
                else
                {
                    Menu = new List<IPluginMenuItem>();

                }
                Menu.Add(ale);
                MenuStructure.Add(ale.MainMenuName, Menu);
            }

            foreach (List<IPluginMenuItem> laa in MenuStructure.Values)
            {
                laa.Sort(delegate (IPluginMenuItem a1, IPluginMenuItem a2)
                    { return a1.MenuOrder.CompareTo(a2.MenuOrder); });
            }

            return MenuStructure;
        }

        public static string[] GetAllAvailableActionNames
        {
            get
            {
                List<string> names = new List<string>();
                foreach (PluginAction ale in LoadedActions)
                    names.Add(ale.Name);
                return names.ToArray();
            }

        }
        public static List<IPluginMenuItem> GetAllAvailableActions
        {
            get
            {
                return LoadedActions;
            }
        }

        static ActionFactory()
        {
            FindActions();
        }

        private static void FindActions()
        {

            List<Assembly> assemblies = new List<Assembly>();


            assemblies.Add(Assembly.GetAssembly(typeof(PluginAction)));

            // TARGETDIR\Effects\*.dll
            string homeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);// System.IO.Path.GetDirectoryName(PluginPath);


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

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {

                    //System.Diagnostics.Debug.Print(type.ToString());
                    try
                    {
                        if (typeof(PluginAction).IsAssignableFrom(type))
                        {
                            LoadedActions.Add(PluginAction.CreatePluginAction(type));
                        }
                    }
                    catch
                    {
                    }
                }
            }

        }
    }
}
