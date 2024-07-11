using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SciImage.Plugins.Effects.IEffects;
using SciImage.SciResources;
using SciImage.SystemLayer;
using SciImage.SystemLayer.Base;
using SciImage.SystemLayer.System;

namespace SciImage.Plugins.Effects
{
    public class EffectFactory
    {
        public static List<Effect> AllEffects()
        {
            if (EffectCatalog == null)
            {
                GatherEffects();
            }

            List<Effect> newEffects = new List<Effect>();
            foreach (Type type in EffectCatalog.Effects)
            {
                try
                {
                    ConstructorInfo ci = type.GetConstructor(Type.EmptyTypes);
                    Effect effect = (Effect)ci.Invoke(null);
                        newEffects.Add(effect);
                }
                catch (Exception ex)
                {

                    // We don't want a DLL that can't be figured out to cause the app to crash
                    //continue;
                //    AppWorkspace.ReportEffectLoadError(Triple.Create(type.Assembly, type, ex));

                }
            }
            return newEffects;
        }

        private static EffectsCollection EffectCatalog = null;
        
        private static Dictionary<Type, EffectConfigToken> effectTokens = new Dictionary<Type, EffectConfigToken>();

        public static Effect RunEffect(string EffectType)
        {
            Type effectType=null;
            foreach (var effectT in EffectCatalog.Effects)
            {
                if (effectT.ToString().ToLower() == EffectType.ToLower())
                {
                    effectType = effectT;
                    break;
                }
            }
            if (effectType == null)
                return null;

            Effect effect = (Effect)Activator.CreateInstance((Type)effectType);

            RunEffect(effect);
            return effect;
        }

        private static void RunEffect(Effect effect)
        {
            if (effectTokens.ContainsKey(effect.GetType()))
            {
                EffectConfigToken oldToken = (EffectConfigToken)effectTokens[effect.GetType()].Clone();
                effect.EffectToken = oldToken;
            }

            var newLastToken = effect.RunEffect();

            if (effect.dialogResult == DialogResult.OK)
            {
                effectTokens[effect.GetType()] = (EffectConfigToken)newLastToken.Clone();
            }
        }

        private static void ReadDirectory(ref List<Assembly> assemblies, string effectsDir)
        {
            string fileSpec = "*" + InvariantStrings.DllExtension;
            string[] filePaths = Directory.GetFiles(effectsDir, fileSpec);

            Assembly pluginAssembly = null;
            foreach (string filePath in filePaths)
            {


                try
                {
                    pluginAssembly = Assembly.LoadFrom(filePath);
                    assemblies.Add(pluginAssembly);
                }

                catch (Exception ex)
                {
                    Tracing.Ping("Exception while loading " + filePath + ": " + ex.ToString());
                }
            }

            pluginAssembly = null;
            try
            {
                pluginAssembly = Assembly.LoadFrom(SciInfo.GetApplicationDir() + "\\SciImage_Effects.dll");
                assemblies.Add(pluginAssembly);
            }

            catch (Exception ex)
            {

            }

        }

        public static EffectsCollection GatherEffects()
        {
            List<Assembly> assemblies = new List<Assembly>();

            // SciImage.Effects.dll
            // assemblies.Add(Assembly.GetAssembly(typeof(Effect)));
            // assemblies.Add(Assembly.GetExecutingAssembly () );
            // TARGETDIR\Effects\*.dll
            string homeDir = SciInfo.GetApplicationDir();
            string effectsDir = Path.Combine(homeDir, InvariantStrings.EffectsSubDir);
            bool dirExists;

            try
            {
                dirExists = Directory.Exists(effectsDir);
            }
            catch
            {
                dirExists = false;
            }

            //  ReadDirectory(ref assemblies, homeDir);

            if (dirExists)
            {
                ReadDirectory(ref assemblies, effectsDir);
            }

            EffectsCollection ec = new EffectsCollection(assemblies);
            EffectCatalog = ec;
            return ec;
        }

        private Set<Triple<Assembly, Type, Exception>> effectLoadErrors = new Set<Triple<Assembly, Type, Exception>>();

        public void ReportEffectLoadError(Triple<Assembly, Type, Exception> error)
        {
            lock (this.effectLoadErrors)
            {
                if (!this.effectLoadErrors.Contains(error))
                {
                    this.effectLoadErrors.Add(error);
                }
            }
        }

        public static string GetLocalizedEffectErrorMessage(Assembly assembly, Type type, Exception exception)
        {
            IPluginSupportInfo supportInfo;
            string typeName;

            if (type != null)
            {
                typeName = type.FullName;
                supportInfo = PluginSupportInfo.GetPluginSupportInfo(type);
            }
            else if (exception is TypeLoadException)
            {
                TypeLoadException asTlex = exception as TypeLoadException;
                typeName = asTlex.TypeName;
                supportInfo = PluginSupportInfo.GetPluginSupportInfo(assembly);
            }
            else
            {
                supportInfo = PluginSupportInfo.GetPluginSupportInfo(assembly);
                typeName = null;
            }

            return GetLocalizedEffectErrorMessage(assembly, typeName, supportInfo, exception);
        }

        public static string GetLocalizedEffectErrorMessage(Assembly assembly, string typeName, Exception exception)
        {
            IPluginSupportInfo supportInfo = PluginSupportInfo.GetPluginSupportInfo(assembly);
            return GetLocalizedEffectErrorMessage(assembly, typeName, supportInfo, exception);
        }

        private static string GetLocalizedEffectErrorMessage(Assembly assembly, string typeName, IPluginSupportInfo supportInfo, Exception exception)
        {
            string fileName = assembly.Location;
            string shortErrorFormat = SciResources.SciResources.GetString("EffectErrorMessage.ShortFormat");
            string fullErrorFormat = SciResources.SciResources.GetString("EffectErrorMessage.FullFormat");
            string notSuppliedText = SciResources.SciResources.GetString("EffectErrorMessage.InfoNotSupplied");

            string errorText;

            if (supportInfo == null)
            {
                errorText = string.Format(
                    shortErrorFormat,
                    fileName ?? notSuppliedText,
                    typeName ?? notSuppliedText,
                    exception.ToString());
            }
            else
            {
                errorText = string.Format(
                    fullErrorFormat,
                    fileName ?? notSuppliedText,
                    typeName ?? supportInfo.DisplayName ?? notSuppliedText,
                    (supportInfo.Version ?? new Version()).ToString(),
                    supportInfo.Author ?? notSuppliedText,
                    supportInfo.Copyright ?? notSuppliedText,
                    (supportInfo.WebsiteUri == null ? notSuppliedText : supportInfo.WebsiteUri.ToString()),
                    exception.ToString());
            }

            return errorText;
        }

        public IList<Triple<Assembly, Type, Exception>> GetEffectLoadErrors()
        {
            return this.effectLoadErrors.ToArray();
        }

    }
}
