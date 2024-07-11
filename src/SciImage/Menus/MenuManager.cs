using SciImage.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SciImage.Plugins.Actions;
using SciImage.Plugins.Effects;

namespace SciImage.Menus
{
    public class MenuManager
    {

        public void LoadSettings()
        {

        }
        public void SaveSettings()
        {
            MostRecentFiles.SaveMruList();
        }


        private MostRecentFiles _MostRecentFiles = null;
        public static int defaultMostRecentFilesMax = 8;

        public MostRecentFiles MostRecentFiles
        {
            get { return _MostRecentFiles; }
        }
        //public event ButtonClickedEvent ButtonClicked;

        private List<SciMenuItem> _Menus = new List<SciMenuItem>();

        public List<SciMenuItem> Menus
        {
            get { return _Menus; }
        }

        public bool SetEnableSubMenu(string actionType, bool enabled)
        {
            //if (Menus.Contains(actionType))
            //{
            //    Menus[actionType].Enabled = enabled;
            //    return true;
            //}

            ////in case of an imcomplete action name
            //foreach (KeyValuePair<string, SciMenuItem> kvp in Menus)
            //{
            //    if (kvp.Key.Contains(actionType))
            //    {
            //        kvp.Value.Enabled = enabled;
            //        return true;
            //    }
            //}

            ////in case of a bad capitalization
            //foreach (KeyValuePair<string, SciMenuItem> kvp in Menus)
            //{
            //    if (kvp.Key.ToLower().Contains(actionType.ToLower()))
            //    {
            //        kvp.Value.Enabled = enabled;
            //        return true;
            //    }
            //}
            return false;
        }

        private string ConvertShortcutKeyToString(Keys ShortCutKey)
        {
            string displayString = "";
            if ((ShortCutKey & Keys.Control) == Keys.Control)
                displayString = "Ctrl+";
            if ((ShortCutKey & Keys.Alt) == Keys.Alt)
                displayString += "Alt+";
            if ((ShortCutKey & Keys.Shift) == Keys.Shift)
                displayString += "Shift+";
            displayString += ShortCutKey.ToString();
            displayString = displayString.Replace(", Control", "");
            displayString = displayString.Replace(", Alt", "");
            displayString = displayString.Replace(", Shift", "");
            displayString = displayString.Replace("Subtract", "(-)");
            displayString = displayString.Replace("Add", "(+)");
            return displayString;
        }

        private SciMenuItem GetMenuItem(string MenuName)
        {
            foreach (SciMenuItem pmi in _Menus)
            {
                if (pmi.Name == MenuName)
                {
                    _Menus.Remove(pmi);
                    return pmi;
                }
            }
            return null;
        }

        void MenuItem_Click(object sender, EventArgs e)
        {
            SciMenuItem pmi = (SciMenuItem)sender;
            if (pmi.MenuAction != null)
            {
                //if (ButtonClicked != null)
                //{
                //    ButtonClicked(pmi.MenuAction);
                //}
                //else 
                {
                    if (pmi.MenuAction.BaseType == typeof(PluginAction))
                        ActionFactory.PerformAction(pmi.MenuAction);
                    else
                        EffectFactory.RunEffect(pmi.MenuAction.ToString());
                }
            }
        }

        public List<SciMenuItem> CreateMenus()
        {
            Dictionary<string, List<IPluginMenuItem>> MenuStructure = ActionFactory.CreateMenuStructure();
            var AllEffects = EffectFactory.AllEffects();

            var hashTitles = new HashSet<string>();
            List<IPluginMenuItem> MenuItems = new List<IPluginMenuItem>();
            foreach (KeyValuePair<string, List<IPluginMenuItem>> kvp in MenuStructure)
            {
                List<IPluginMenuItem> laa = kvp.Value;
                foreach (IPluginMenuItem ale in laa)
                {
                    hashTitles.Add(ale.MainMenuName);
                    MenuItems.Add(ale);
                }
            }
            foreach (var effect in AllEffects)
            {
                hashTitles.Add(effect.MainMenuName);
                MenuItems.Add(effect);
            }

            MenuItems.Sort((a, b) => a.MenuOrder.CompareTo(b.MenuOrder));


            List<string> unorderedTitles = new List<string>();
            unorderedTitles.AddRange(hashTitles);
            List<string> OrderedTitles = new List<string>();
            string[] ordered = new string[] { "File", "Edit", "View", "Image", "Layers", "Adjustments" };
            string help = "";
            foreach (var title in ordered)
            {
                for (int i = 0; i < unorderedTitles.Count; i++)
                {
                    if (title == unorderedTitles[i])
                    {
                        OrderedTitles.Add(unorderedTitles[i]);
                        unorderedTitles.RemoveAt(i);
                        break;
                    }
                    if ("Help" == unorderedTitles[i])
                    {
                        help = unorderedTitles[i];
                        unorderedTitles.RemoveAt(i);
                    }
                }
            }

            foreach (var title in unorderedTitles)
                if (title != "")
                    OrderedTitles.Add(title);

            if (help != "")
                OrderedTitles.Add(help);


            Dictionary<string, List<object>> MainMenu = new Dictionary<string, List<object>>();
            foreach (var menuTitle in OrderedTitles)
            {
                if (menuTitle != "")
                    MainMenu.Add(menuTitle, new List<object>());
            }

            foreach (var menu in MainMenu.Keys)
            {
                hashTitles = new HashSet<string>();
                foreach (var menuItem in MenuItems)
                {
                    if (menuItem.MainMenuName == menu && menuItem.SubMenuName != "" && menuItem.SubMenuName != null)
                    {
                        hashTitles.Add(menuItem.SubMenuName);
                    }
                }

                unorderedTitles = new List<string>();
                unorderedTitles.AddRange(hashTitles);
                unorderedTitles.Sort();

                Dictionary<string, List<IPluginMenuItem>> subMenus = new Dictionary<string, List<IPluginMenuItem>>();
                foreach (var title in unorderedTitles)
                {
                    subMenus.Add(title, new List<IPluginMenuItem>());
                }


                var singles = new List<IPluginMenuItem>();
                foreach (var menuItem in MenuItems)
                {
                    if (menuItem.MainMenuName == menu)
                    {
                        if (hashTitles.Contains(menuItem.SubMenuName))
                        {
                            subMenus[menuItem.SubMenuName].Add(menuItem);
                        }
                        else
                        {
                            singles.Add(menuItem);
                        }
                    }
                }

                MainMenu[menu].AddRange(singles);

                // Dictionary<string, List<IPluginMenuItem>> subMenus2 = new Dictionary<string, List<IPluginMenuItem>>();
                foreach (KeyValuePair<string, List<IPluginMenuItem>> kvp in subMenus)
                {
                    kvp.Value.Sort((a, b) => a.MenuOrder.CompareTo(b.MenuOrder));
                }

                if (subMenus.Count > 0)
                    MainMenu[menu].Add(subMenus);
            }

            List<SciMenuItem> menus = new List<SciMenuItem>();
            foreach (KeyValuePair<string, List<Object>> kvp in MainMenu)
            {
                var m = new SciMenuItem(kvp.Key, null, null);
                m.Name = kvp.Key;
                m.Text = kvp.Key;
                foreach (var mItem in kvp.Value)
                {
                    if (mItem is IPluginMenuItem)
                    {
                        var pmi = (IPluginMenuItem)mItem;
                        var t = new SciMenuItem
                        {
                            Name = pmi.Name,
                            Text = pmi.Name,
                            Image = pmi.Image,
                            MenuAction = pmi.GetType()
                        };
                        if (pmi.ShortCutKeys != Keys.F9)
                        {
                            t.ShortcutKeys = pmi.ShortCutKeys;
                            t.ShortcutKeyDisplayString = ConvertShortcutKeyToString(pmi.ShortCutKeys);
                        }
                        t.Click += new EventHandler(MenuItem_Click);
                        m.DropDownItems.Add(t);
                    }
                    else
                    {
                        Dictionary<string, List<IPluginMenuItem>> subMenus = (Dictionary<string, List<IPluginMenuItem>>)mItem;
                        foreach (KeyValuePair<string, List<IPluginMenuItem>> kvp2 in subMenus)
                        {
                            var m2 = new SciMenuItem(kvp2.Key, null, null);
                            m2.Text = kvp2.Key;
                            m2.Name = kvp2.Key;
                            foreach (var mItem2 in kvp2.Value)
                            {
                                if (mItem2 is IPluginMenuItem)
                                {
                                    var pmi = (IPluginMenuItem)mItem2;
                                    var t = new SciMenuItem
                                    {
                                        Name = pmi.Name,
                                        Text = pmi.Name,
                                        Image = pmi.Image,
                                        MenuAction = pmi.GetType()
                                    };
                                    if (pmi.ShortCutKeys != Keys.F9)
                                    {
                                        t.ShortcutKeys = pmi.ShortCutKeys;
                                        t.ShortcutKeyDisplayString = ConvertShortcutKeyToString(pmi.ShortCutKeys);
                                    }
                                    t.Click += new EventHandler(MenuItem_Click);
                                    m2.DropDownItems.Add(t);
                                }
                            }
                            m2.LoadNames();
                            m2.LoadIcons();
                            m.DropDownItems.Add(m2);
                        }
                    }
                }
                m.LoadNames();
                m.LoadIcons();
                menus.Add(m);
            }
            _Menus = menus;




            return _Menus;



        }


        private static MenuManager _menuManager = null;
        private MenuManager()
        { }

        public static MenuManager MainMenu
        {
            get { return _menuManager; }
        }

        static MenuManager()
        {
            _menuManager = new MenuManager();

        }

    }
}
