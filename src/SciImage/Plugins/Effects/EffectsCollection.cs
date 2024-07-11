/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using SciImage.Core;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SystemLayer.Base;

namespace SciImage.Plugins.Effects
{
    public sealed class EffectsCollection
    {
        private Assembly[] assemblies;
        private List<Type> effects;
        private List<Type> effectArrays;

        // Assembly, TypeName, Exception[]
        private List<Triple<Assembly, Type, Exception>> loaderExceptions =
            new List<Triple<Assembly, Type, Exception>>();

        private void AddLoaderException(Triple<Assembly, Type, Exception> loaderException)
        {
            lock (this)
            {
                this.loaderExceptions.Add(loaderException);
            }
        }

        public Triple<Assembly, Type, Exception>[] GetLoaderExceptions()
        {
            lock (this)
            {
                return this.loaderExceptions.ToArray();
            }
        }

        public EffectsCollection(List<Assembly> assemblies)
        {
            this.assemblies = assemblies.ToArray();
            this.effects = null;
        }

        public EffectsCollection(List<Type> effects)
        {
            this.assemblies = null;
            this.effects = new List<Type>(effects);
        }

        public Type[] Effects
        {
            get
            {
                lock (this)
                {
                    if (this.effects == null)
                    {
                        List<Triple<Assembly, Type, Exception>> errors = new List<Triple<Assembly, Type, Exception>>();
                        this.effects = GetEffectsFromAssemblies(this.assemblies, errors);

                        for (int i = 0; i < errors.Count; ++i)
                        {
                            AddLoaderException(errors[i]);
                        }
                    }
                }

                return this.effects.ToArray();
            }
        }
        public Type[] EffectArrays
        {
            get
            {
                lock (this)
                {
                    if (this.effectArrays  == null)
                    {
                        List<Triple<Assembly, Type, Exception>> errors = new List<Triple<Assembly, Type, Exception>>();
                        this.effectArrays  = GetEffectArraysFromAssemblies(this.assemblies, errors);

                        for (int i = 0; i < errors.Count; ++i)
                        {
                            AddLoaderException(errors[i]);
                        }
                    }
                }

                return this.effectArrays.ToArray();
            }

        }

        private static Version GetAssemblyVersionFromType(Type type)
        {
            try
            {
                Assembly assembly = type.Assembly;
                AssemblyName assemblyName = new AssemblyName(assembly.FullName);
                return assemblyName.Version;
            }

            catch (Exception)
            {
                return new Version(0, 0, 0, 0);
            }
        }

        private static bool CheckForGuidOnType(Type type, Guid guid)
        {
            try
            {
                object[] attributes = type.GetCustomAttributes(typeof(GuidAttribute), true);

                foreach (GuidAttribute guidAttr in attributes)
                {
                    if (new Guid(guidAttr.Value) == guid)
                    {
                        return true;
                    }
                }
            }

            catch (Exception)
            {
            }

            return false;
        }

        private static bool CheckForAnyGuidOnType(Type type, Guid[] guids)
        {
            foreach (Guid guid in guids)
            {
                if (CheckForGuidOnType(type, guid))
                {
                    return true;
                }
            }

            return false;
        }

        private static string UnstableMessage
        {
            get
            {
                return "This is an unstable plugin";
            }
        }

        private static string BuiltInMessage
        {
            get
            {
                return "This plugin does not need to be loaded";
            }
        }
       
        private static Dictionary<string, List<Triple<string, Version, string>>> indexedBlockedEffects;

        private static List<Type> GetEffectsFromAssemblies(Assembly[] assemblies, IList<Triple<Assembly, Type, Exception>> errorsResult)
        {
            List<Type> effects = new List<Type>();

            foreach (Assembly assembly in assemblies)
            {
                GetEffectsFromAssembly(assembly, effects, errorsResult);
            }

            List<Type> removeUs = new List<Type>();

            foreach (Type removeThisType in removeUs)
            {
                effects.Remove(removeThisType);
            }

            return effects;
        }

        private static List<Type> GetEffectArraysFromAssemblies(Assembly[] assemblies, IList<Triple<Assembly, Type, Exception>> errorsResult)
        {
            List<Type> effectArrays = new List<Type>();

            foreach (Assembly assembly in assemblies)
            {
                GetEffectArraysFromAssembly(assembly, effectArrays, errorsResult);
            }

            List<Type> removeUs = new List<Type>();

            foreach (Type removeThisType in removeUs)
            {
                effectArrays.Remove(removeThisType);
            }

            return effectArrays;
        }

        private static void GetEffectsFromAssembly(Assembly assembly, IList<Type> effectsResult, IList<Triple<Assembly, Type, Exception>> errorsResult)
        {
            try
            {
                Type[] types = GetTypesFromAssembly(assembly, errorsResult);

                foreach (Type type in types)
                {
                    try
                    {
                        if ((type.IsSubclassOf(typeof(Effect)) || (type.BaseType.ToString() == "SciImage.Effects.Effect")) && !type.IsAbstract && !Utility.IsObsolete(type, false))
                        {
                            effectsResult.Add(type);
                        }
                    }
                    catch { }
                }
            }

            catch (ReflectionTypeLoadException)
            {
            }
        }
        private static bool IsInterfaceImplemented(Type derivedType, Type interfaceType)
        {
            return -1 != Array.IndexOf<Type>(derivedType.GetInterfaces(), interfaceType);
        }
        private static void GetEffectArraysFromAssembly(Assembly assembly, IList<Type> effectsResult, IList<Triple<Assembly, Type, Exception>> errorsResult)
        {
            try
            {
                Type[] types = GetTypesFromAssembly(assembly, errorsResult);

                foreach (Type type in types)
                {
                    //if (type.IsSubclassOf(typeof(Effect)) && !type.IsAbstract && !Utility.IsObsolete(type, false))
                    if (IsInterfaceImplemented(type, typeof(IEffectTypeArray)) && !type.IsAbstract)
                    {
                        effectsResult.Add(type);
                    }
                }
            }

            catch (ReflectionTypeLoadException)
            {
            }
        }

        private static Type[] GetTypesFromAssembly(Assembly assembly, IList<Triple<Assembly, Type, Exception>> errorsResult)
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }

            catch (ReflectionTypeLoadException rex)
            {
                List<Type> typesList = new List<Type>();
                Type[] rexTypes = rex.Types;

                foreach (Type rexType in rexTypes)
                {
                    if (rexType != null)
                    {
                        typesList.Add(rexType);
                    }
                }

                foreach (Exception loadEx in rex.LoaderExceptions)
                {
                    // Set Type to null, and the error dialog will look at the exception, see it is a TypeLoadException,
                    // and use the TypeName property from there.
                    errorsResult.Add(Triple.Create<Assembly, Type, Exception>(assembly, null, loadEx));
                }

                types = typesList.ToArray();
            }

            return types;
        }
    }
}
