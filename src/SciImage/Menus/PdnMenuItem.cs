/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using SciImage.Core;
using SciImage.SystemLayer;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SciImage.Forms;
using SciImage.SciResources;
using SciImage.SystemLayer.System;

namespace SciImage.Menus
{
    public class SciMenuItem
        : ToolStripMenuItem
    {
        private string textResourceName = null;
        private const char noMnemonicChar = (char)0;
        private const char mnemonicPrefix = '&';
        private bool iconsLoaded = false;
        private bool namesLoaded = false;

        private Keys registeredHotKey = Keys.None;

        private Type _MenuAction = null;
        public Type MenuAction
        {
            get { return _MenuAction; }
            set { _MenuAction = value; }
        }

        public new Keys ShortcutKeys
        {
            get
            {
                return base.ShortcutKeys;
            }

            set
            {
                if (ShortcutKeys != Keys.None)
                {
                    SciBaseForm.UnregisterFormHotKey(ShortcutKeys, OnShortcutKeyPressed);
                }

                SciBaseForm.RegisterFormHotKey(value, OnShortcutKeyPressed);

                base.ShortcutKeys = value;
            }
        }

        public bool HasMnemonic
        {
            get
            {
                return (Mnemonic != noMnemonicChar);
            }
        }

        public char Mnemonic
        {
            get
            {
                if (string.IsNullOrEmpty(this.Text))
                {
                    return noMnemonicChar;
                }

                int mnemonicPrefixIndex = this.Text.IndexOf(mnemonicPrefix);

                if (mnemonicPrefixIndex >= 0 && mnemonicPrefixIndex < this.Text.Length - 1)
                {
                    return this.Text[mnemonicPrefixIndex + 1];
                }
                else
                {
                    return noMnemonicChar;
                }
            }
        }

        public SciMenuItem(string name, Image image, EventHandler eventHandler)
            : base(name, image, eventHandler)
        {
            Constructor();
        }

        public SciMenuItem()
        {
            Constructor();
        }

        private void Constructor()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {

        }

        private bool OnShortcutKeyPressed(Keys keys)
        {
            PerformClick();
            return true;
        }

        private bool OnAccessHotKeyPressed(Keys keys)
        {
            ShowDropDown();
            return true;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (this.registeredHotKey != Keys.None)
            {
                SciBaseForm.UnregisterFormHotKey(this.registeredHotKey, OnAccessHotKeyPressed);
            }

            char mnemonic = this.Mnemonic;

            if (mnemonic != noMnemonicChar && !IsOnDropDown)
            {
                Keys hotKey = Utility.LetterOrDigitCharToKeys(mnemonic);
                SciBaseForm.RegisterFormHotKey(Keys.Alt | hotKey, OnAccessHotKeyPressed);
            }

            base.OnTextChanged(e);
        }

        public void LoadNames()
        {
            string baseName = this.Name;
            foreach (ToolStripItem item in this.DropDownItems)
            {
                string itemNameBase = (baseName + "." + item.Name).Trim();
                if (itemNameBase.EndsWith("."))
                    itemNameBase = itemNameBase.Trim().Substring(0, itemNameBase.Length - 1);
                string itemNameText = itemNameBase + ".Text";

                string text = SciResources.SciResources.GetString(itemNameText);

                if (text != null && text != itemNameText)
                {
                    item.Text = text;
                }

                SciMenuItem pmi = item as SciMenuItem;
                if (pmi != null)
                {
                    pmi.textResourceName = itemNameText;
                    pmi.LoadNames();
                }
            }

            this.namesLoaded = true;
        }

        public void SetIcon(string imageName)
        {
            this.ImageTransparentColor = Utility.TransparentKey;
            this.Image = SciResources.SciResources.GetImageResource(imageName).Reference;
        }
        public void SetIcon(Image image)
        {
            this.ImageTransparentColor = Utility.TransparentKey;
            this.Image = image;
        }

        public void SetIcon(ImageResource image)
        {
            this.ImageTransparentColor = Utility.TransparentKey;
            this.Image = image.Reference;
        }

        public void LoadIcons()
        {
            Type ourType = this.GetType();

            FieldInfo[] fields = ourType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (FieldInfo fi in fields)
            {
                if (fi.FieldType.IsSubclassOf(typeof(SciMenuItem)) ||
                    fi.FieldType == typeof(SciMenuItem))
                {
                    string iconFileName = "Icons." + fi.Name[0].ToString().ToUpper() + fi.Name.Substring(1) + "Icon.png";
                    SciMenuItem mi = (SciMenuItem)fi.GetValue(this);
                    Stream iconStream = SciResources.SciResources.GetResourceStream(iconFileName);

                    if (iconStream != null)
                    {
                        iconStream.Dispose();
                        mi.SetIcon(iconFileName);
                    }
                    else
                    {
                        Tracing.Ping(iconFileName + " not found");
                    }
                }
            }

            this.iconsLoaded = true;
        }

    }
}
