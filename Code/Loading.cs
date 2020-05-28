using UnityEngine;
using ICities;


namespace RoadSignReplacer
{
    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public class Loading : LoadingExtensionBase
    {
        public static bool InGame => _inGame;
        private static bool _inGame = false;


        /// <summary>
        /// Called by the game when level loading is complete.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            // Don't do anything if not in game.
            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame)
            {
                return;
            }

            UnityEngine.Debug.Log("Road Sign Replacer: version v" + RoadSignMod.Version + " loading.");

            base.OnLevelLoaded(mode);
            _inGame = true;

            // Load and apply configuration file settings.
            OptionsPanel.Instance.LoadConfiguration();

            // Create dictionary if it doesn't already exist.
            if (Selections.currentSigns == null)
            {
                Selections.Setup();
            }

            Debug.Log("Road Sign Replacer finished loading.");
        }
    }
}