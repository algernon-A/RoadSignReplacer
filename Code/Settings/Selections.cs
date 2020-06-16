using System.Collections.Generic;


namespace RoadSignReplacer
{
    /// <summary>
    /// Categories for each defined sign.
    /// </summary>
    public enum SignTypes
    {
        speed,
        general
    }


    /// <summary>
    /// Internal prop pack data structure - same as the file structure but with added sign category.
    /// </summary>
    public class InternalPropPack
    {
        public PropPack propPack;
        public int category;
    }


    /// <summary>
    /// Class to store and handle selection information.
    /// </summary>
    public static class Selections
    {
        // Default Vanilla sign names.
        public static string[] DefaultNames =
        {
            "30 Speed Limit",
            "40 Speed Limit",
            "50 Speed Limit",
            "60 Speed Limit",
            "100 Speed Limit",
            "Stop Sign",
            "No Parking Sign",
            "No Right Turn Sign",
            "No Left Turn Sign",
            "Motorway Sign",
            "Street Name Sign"
        };

        // Matching sign types (categories) for the above list.
        public static int[] DefaultTypes =
        {
            (int)SignTypes.speed,
            (int)SignTypes.speed,
            (int)SignTypes.speed,
            (int)SignTypes.speed,
            (int)SignTypes.speed,
            (int)SignTypes.general,
            (int)SignTypes.general,
            (int)SignTypes.general,
            (int)SignTypes.general,
            (int)SignTypes.general,
            (int)SignTypes.general
        };

        // Matching rotations for the above list.
        public static float[] DefaultRotations =
        {
            0f,
            0f,
            0f,
            0f,
            0f,
            0f,
            0f,
            0f,
            0f,
            0f,
            0f
        };

        // Dictionary to store currently active replacements.
        public static Dictionary<string, PropReplacement> currentSigns;

        // Default packs, not read from XML but generated here.
        public static InternalPropPack defaultSignPack;
        public static InternalPropPack defaultSpeedPack;


        /// <summary>
        /// Initialises the selection records.
        /// </summary>
        public static void Setup()
        {
            // Create default prop packs.
            defaultSignPack = new InternalPropPack();
            defaultSpeedPack = new InternalPropPack();

            defaultSignPack.category = (int)SignTypes.general;
            defaultSpeedPack.category = (int)SignTypes.speed;

            defaultSignPack.propPack = new PropPack();
            defaultSignPack.propPack.name = "Vanilla";
            defaultSignPack.propPack.propReplacements = new List<PropReplacement>();

            defaultSpeedPack.propPack = new PropPack();
            defaultSpeedPack.propPack.name = "Vanilla";
            defaultSpeedPack.propPack.propReplacements = new List<PropReplacement>();

            // Create dictionary for currently active replacements.
            currentSigns = new Dictionary<string, PropReplacement>();

            // Populate default prop packs and dictionary using default data.
            for (int i = 0; i < DefaultNames.Length; i ++)
            {
                // Generate new prop replacement record.
                PropReplacement defaultReplacement = new PropReplacement();

                // Set values.
                defaultReplacement.targetName = DefaultNames[i];
                defaultReplacement.replacementName = DefaultNames[i];
                defaultReplacement.rotation = DefaultRotations[i];

                // Add to dictionary.
                currentSigns.Add(DefaultNames[i], defaultReplacement);

                // Add to relevant prop pack.
                if (DefaultTypes[i] == (int)SignTypes.speed)
                {
                    defaultSpeedPack.propPack.propReplacements.Add(defaultReplacement);
                }
                else
                {
                    defaultSignPack.propPack.propReplacements.Add(defaultReplacement);
                }
            }
        }
    }
}