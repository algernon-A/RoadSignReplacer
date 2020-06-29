using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using ICities;
using ColossalFramework.Plugins;


namespace RoadSignReplacer
{
    /// <summary>
    /// Static utility class for managing configuration files.
    /// </summary>
    public static class ConfigurationUtil
    {
        internal static readonly string SettingsFileName = "SignPacks.xml";


        /// <summary>
        /// Returns the path of the current assembly.
        /// </summary>
        private static string AssemblyPath
        {
            get
            {
                // Step through each plugin, looking for a match for this one.
                PluginManager pluginManager = PluginManager.instance;
                IEnumerable<PluginManager.PluginInfo> plugins = pluginManager.GetPluginsInfo();

                foreach (PluginManager.PluginInfo plugin in plugins)
                {
                    try
                    {
                        IUserMod[] instances = plugin.GetInstances<IUserMod>();

                        if (!(instances.FirstOrDefault() is RoadSignMod))
                        {
                            continue;
                        }

                        // Got it!  Return.
                        return plugin.modPath + Path.DirectorySeparatorChar;
                    }
                    catch
                    {
                        // Don't care.
                    }
                }

                // If we got here, we didn't find it.
                Debugging.Message("couldn't find assembly path");
                throw new DllNotFoundException("Road Sign Replacer assembly not found.");
            }
        }


        /// <summary>
        /// Loads an XML configuration file.
        /// </summary>
        /// <returns>Loaded XML configuration file instance (null if failed)</returns>
        internal static ConfigurationFile LoadConfiguration()
        {
            string filePath = AssemblyPath + SettingsFileName;


            try
            {
                // Check to see if configuration file exists.
                if (File.Exists(filePath))
                {
                    // Read it.
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConfigurationFile));
                        if (!(xmlSerializer.Deserialize(reader) is ConfigurationFile configurationFile))
                        {
                            Debugging.Message("couldn't deserialize configuration file");
                        }
                        else
                        {
                            return configurationFile;
                        }
                    }
                }
                else
                {
                    Debugging.Message("no configuration file found");
                }
            }
            catch (Exception e)
            {
                Debugging.Message("exception reading XML configuration file");
                Debugging.LogException(e);
            }

            // If we got here, we failed; return.
            return null;
        }
    }
}