/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using SciImage.SciResources;

namespace SciImage.Plugins.Tools
{
    public class ToolMenuInfo
    {
        private string name;
        private string helpText;
        private ImageResource image;
        private bool skipIfActiveOnHotKey;
        private char hotKey;
        private Type toolType;
        private ToolBarConfigItems toolBarConfigItems;

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public string HelpText
        {
            get
            {
                return this.helpText;
            }
        }

        public ImageResource Image
        {
            get
            {
                return this.image;
            }
        }

        public bool SkipIfActiveOnHotKey
        {
            get
            {
                return this.skipIfActiveOnHotKey;
            }
        }

        public char HotKey
        {
            get
            {
                return this.hotKey;
            }
        }

        public Type ToolType
        {
            get
            {
                return this.toolType;
            }
        }

        public int Order { get; private  set; }

        public ToolBarConfigItems ToolBarConfigItems
        {
            get
            {
                return this.toolBarConfigItems;
            }
        }

        public override bool Equals(object obj)
        {
            ToolMenuInfo rhs = obj as ToolMenuInfo;

            if (rhs == null)
            {
                return false;
            }

            return (this.name == rhs.name) &&
                   (this.helpText == rhs.helpText) &&
                   (this.hotKey == rhs.hotKey) &&
                   (this.skipIfActiveOnHotKey == rhs.skipIfActiveOnHotKey) &&
                   (this.toolType == rhs.toolType);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public ToolMenuInfo(
            string name, 
            string helpText, 
            ImageResource image, 
            char hotKey, 
            bool skipIfActiveOnHotKey, 
            int order,
            ToolBarConfigItems toolBarConfigItems, 
            Type toolType)
        {
            this.name = name;
            this.helpText = helpText;
            this.image = image;
            this.hotKey = hotKey;
            this.skipIfActiveOnHotKey = skipIfActiveOnHotKey;
            this.toolBarConfigItems = toolBarConfigItems;
            this.toolType = toolType;
            this.Order = order;
        }
    }
}
