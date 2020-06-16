using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ICities;
using ColossalFramework.Plugins;


namespace RoadSignReplacer
{
    /// <summary>
    /// Utility class for managing configuration files.
    /// </summary>
    public static class ConfigurationUtil
    {
        /// <summary>
        /// Returns the path of the current assembly.
        /// </summary>
        public static string AssemblyPath
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
    }
}