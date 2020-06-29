using ICities;


namespace RoadSignReplacer
{
    /// <summary>
    /// The base mod class for instantiation by the game.
    /// </summary>
    public class RoadSignMod : IUserMod
    {
        public static string ModName => "Road Sign Replacer";

        public static string Version => "0.2.1";
        public string Name => ModName + " " + Version;

        public string Description => Translations.Translate("RSR_DESC");


        /// <summary>
        /// Called by the game when the mod options panel is setup.
        /// </summary>
        public void OnSettingsUI(UIHelperBase helper)
        {
            // Load settings file.
            SettingsUtils.LoadSettings();

            // Create options panel.
            OptionsPanel optionsPanel = new OptionsPanel(helper);
        }
    }
}
