/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) Rick Brewster, Tom Jackson, and past contributors.            //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace SciImage.SystemLayer.Base.PropertySystem
{
    public abstract class PropertyCollectionRule
        : ICloneable<PropertyCollectionRule>
    {
        private PropertyCollection owner;

        protected PropertyCollection Owner
        {
            get
            {
                return this.owner;
            }
        }

        public PropertyCollectionRule()
        {
        }

        public void Initialize(PropertyCollection owner)
        {
            if (this.owner != null)
            {
                throw new InvalidOperationException("Already initialized");
            }

            this.owner = owner;

            OnInitialized();
        }

        public bool IsInitialized
        {
            get
            {
                return (this.owner != null);
            }
        }

        public void VerifyInitialized()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("This rule was never initialized into a PropertyCollection");
            }
        }

        public abstract PropertyCollectionRule Clone();

        object ICloneable.Clone()
        {
            return Clone();
        }

        protected abstract void OnInitialized();
    }
}
