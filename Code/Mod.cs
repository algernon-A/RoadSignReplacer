using ICities;


namespace RoadSignReplacer
{
    /// <summary>
    /// The base mod class for instantiation by the game.
    /// </summary>
    public class RoadSignMod : IUserMod
    {
        public string Name => "Road Sign Replacer";

        public static string Version => "0.2";

        public string Description => "Replaces road signs.  UNDER DEVELOPMENT: feedback and suggestions welcome.";


        /// <summary>
        /// Called by the game when the mod options panel is setup.
        /// </summary>
        public void OnSettingsUI(UIHelperBase helper)
        {
            // Create options panel.
            OptionsPanel optionsPanel = new OptionsPanel(helper);
        }

    }
}
