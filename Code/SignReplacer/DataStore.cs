using System.Collections.Generic;
using ColossalFramework.Packaging;


namespace RoadSignReplacer
{
    internal static class DataStore
    {
        // Data objects.
        internal static List<InternalPropPack> speedPacks;
        internal static List<InternalPropPack> signPacks;


        internal static void Setup()
        {
            if (Selections.currentSigns == null)
            {
                Selections.Setup();
            }

            // Load settings.
            ConfigurationFile configurationFile = ConfigurationUtil.LoadConfiguration();

            // Build lists of packs in relevant selection categories.
            signPacks = new List<InternalPropPack>();
            speedPacks = new List<InternalPropPack>();

            // Add inbuilt vanilla packs.
            signPacks.Add(Selections.defaultSignPack);
            speedPacks.Add(Selections.defaultSpeedPack);

            // Iterate through each prop pack loaded from the settings file.
            foreach (PropPack propPack in configurationFile.propPacks)
            {
                // Create new package references.
                InternalPropPack newSpeedPack = new InternalPropPack();
                InternalPropPack newSignPack = new InternalPropPack();

                newSpeedPack.category = (int)SignTypes.speed;
                newSignPack.category = (int)SignTypes.general;

                newSpeedPack.propPack = new PropPack();
                newSignPack.propPack = new PropPack();
                newSpeedPack.propPack.name = propPack.name;
                newSignPack.propPack.name = propPack.name;
                newSpeedPack.propPack.propReplacements = new List<PropReplacement>();
                newSignPack.propPack.propReplacements = new List<PropReplacement>();

                // Iterate through each replacement in the pack.
                foreach (PropReplacement replacement in propPack.propReplacements)
                {
                    // Check that prop pack is subscribed on the workshop.
                    int prefixIndex = replacement.replacementName.IndexOf(".");

                    if (prefixIndex > 0)
                    {
                        string workshopPrefix = replacement.replacementName.Substring(0, prefixIndex);

                        if (!string.IsNullOrEmpty(workshopPrefix))
                        {
                            if (PackageManager.GetPackage(workshopPrefix) == null)
                            {
                                Debugging.Message("workshop subscription " + workshopPrefix + " not found");
                                continue;
                            }
                        }
                    }

                    // Sort into speed signs and other signs.
                    switch (replacement.targetName)
                    {
                        case "30 Speed Limit":
                        case "40 Speed Limit":
                        case "50 Speed Limit":
                        case "60 Speed Limit":
                        case "100 Speed Limit":
                            newSpeedPack.propPack.propReplacements.Add(replacement);
                            break;
                        default:
                            newSignPack.propPack.propReplacements.Add(replacement);
                            break;
                    }
                }

                // Add generated speed sign pack to list if it has at least one sign in it.
                if (newSpeedPack.propPack.propReplacements.Count > 0)
                {
                    speedPacks.Add(newSpeedPack);
                }

                // Add generated general sign pack to list if it has at least one sign in it.
                if (newSignPack.propPack.propReplacements.Count > 0)
                {
                    signPacks.Add(newSignPack);
                }
            }
        }
    }
}