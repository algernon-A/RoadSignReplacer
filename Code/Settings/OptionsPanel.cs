using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ICities;
using ColossalFramework.UI;


namespace RoadSignReplacer
{
    /// <summary>
    /// Class to handle the mod settings options panel.
    /// </summary>
    public class OptionsPanel
    {
        // Constants.
        private const float panelWidth = 751;
        private const float listHeight = 210;
        private const float bottomHeight = 50;
        private const float spacing = 5;
        public const float titleHeight = 40;
        public const float headingHeight = 30;
        public const float rowHeight = 30;

        // Panel components.
        private UIFastList speedPackSelection;
        private UIFastList signPackSelection;
        private UISavePanel savePanel;

        // Selected items.
        internal InternalPropPack selectedSpeedPack;
        internal InternalPropPack selectedSignPack;

        // Instance reference.
        private static OptionsPanel _instance;
        public static OptionsPanel Instance => _instance;


        /// <summary>
        /// Options panel constructor.
        /// </summary>
        /// <param name="helper">UIHelperBase parent</param>
        public OptionsPanel(UIHelperBase helper)
        {
            // Set instance.
            _instance = this;

            // Get root options panel.
            UIScrollablePanel optionsPanel = ((UIHelper)helper).self as UIScrollablePanel;
            optionsPanel.autoLayout = false;

            // Set up panels.
            UILabel speedLabel = optionsPanel.AddUIComponent<UILabel>();
            speedLabel.relativePosition = new Vector3(spacing, titleHeight);
            speedLabel.textScale = 1.3f;
            speedLabel.text = "Installed speed sign replacement packs";

            // Speed sign list panel.
            UIPanel speedPanel = optionsPanel.AddUIComponent<UIPanel>();
            speedPanel.width = panelWidth;
            speedPanel.height = listHeight;
            speedPanel.relativePosition = new Vector3(spacing, titleHeight + headingHeight);

            // Speed sign pack selection list.
            speedPackSelection = UIFastList.Create<UISignPackRow>(speedPanel);
            speedPackSelection.backgroundSprite = "UnlockingPanel";
            speedPackSelection.width = speedPanel.width;
            speedPackSelection.height = speedPanel.height;
            speedPackSelection.canSelect = true;
            speedPackSelection.rowHeight = rowHeight;
            speedPackSelection.autoHideScrollbar = true;
            speedPackSelection.relativePosition = Vector3.zero;
            speedPackSelection.rowsData = new FastList<object>();
            speedPackSelection.selectedIndex = -1;

            // Populate the list.
            speedPackSelection.rowsData = ToFastList(DataStore.speedPacks);

            // Set up panels.
            UILabel signLabel = optionsPanel.AddUIComponent<UILabel>();
            signLabel.relativePosition = new Vector3(spacing, titleHeight + headingHeight + listHeight + headingHeight);
            signLabel.textScale = 1.3f;
            signLabel.text = "Installed general road sign replacement packs";

            // General sign list panel.
            UIPanel generalPanel = optionsPanel.AddUIComponent<UIPanel>();
            generalPanel.width = panelWidth;
            generalPanel.height = listHeight;
            generalPanel.relativePosition = new Vector3(spacing, titleHeight + headingHeight + listHeight + headingHeight + headingHeight);

            // General sign pack selection list.
            signPackSelection = UIFastList.Create<UISignPackRow>(generalPanel);
            signPackSelection.backgroundSprite = "UnlockingPanel";
            signPackSelection.width = generalPanel.width;
            signPackSelection.height = generalPanel.height;
            signPackSelection.canSelect = true;
            signPackSelection.rowHeight = rowHeight;
            signPackSelection.autoHideScrollbar = true;
            signPackSelection.relativePosition = Vector3.zero;
            signPackSelection.rowsData = new FastList<object>();
            signPackSelection.selectedIndex = -1;

            // Populate the list.
            signPackSelection.rowsData = ToFastList(DataStore.signPacks);

            // Bottom panel for buttons.
            savePanel = optionsPanel.AddUIComponent<UISavePanel>();
            savePanel.width = panelWidth;
            savePanel.height = bottomHeight;
            savePanel.relativePosition = new Vector3(spacing, titleHeight + headingHeight + listHeight + headingHeight + headingHeight + listHeight + headingHeight);


            // Set up panels.
            UILabel noteLabel = optionsPanel.AddUIComponent<UILabel>();
            noteLabel.autoSize = false;
            noteLabel.autoHeight = true;
            noteLabel.width = panelWidth;
            noteLabel.textScale = 1.0f;
            noteLabel.wordWrap = true;
            noteLabel.text = "Note that only installed and recognised road sign prop packs are included in these lists.  Please see the mod's Steam Workshop page for a list of currently supported road sign prop packs, and feel free to add suggestions in the discussion on ones that should be added.";
            noteLabel.relativePosition = new Vector3(spacing, titleHeight + headingHeight + listHeight + headingHeight + headingHeight + listHeight + headingHeight + bottomHeight + headingHeight);

            // Load current configuraton.
            LoadConfiguration();
        }


        /// <summary>
        /// Called when the prop selection changes to update other panels.
        /// </summary>
        /// <param name="signPack">New signpack to update to</param>
        public void UpdateSelectedSignPack(InternalPropPack signPack)
        {
            // Check type of update (speed sign or general sign) and update accordingly.
            if (signPack.category == (int)SignTypes.speed)
            {
                selectedSpeedPack = signPack;
                savePanel.currentSpeedSelection = signPack.propPack;
            }
            else
            {
                selectedSignPack = signPack;
                savePanel.currentSignSelection = signPack.propPack;
            }
        }


        /// <summary>
        /// Refreshes the sign pack selection list.
        /// </summary>
        public void Refresh()
        {
            // Refresh the sign pack lists.
            speedPackSelection.Refresh();
            signPackSelection.Refresh();

            // Update mod calculations and edit panels.
            UpdateSelectedSignPack(selectedSpeedPack);
            UpdateSelectedSignPack(selectedSignPack);
        }


        /// <summary>
        /// Generates the list of speed sign packs.
        /// </summary>
        /// <returns>FastList of internal prop packs</returns>
        private FastList<object> ToFastList(List<InternalPropPack> packList)
        {
            // Create return list from signPacks array.
            FastList<object> fastList = new FastList<object>();
            fastList.m_buffer = packList.ToArray();
            fastList.m_size = packList.Count();

            return fastList;
        }


        /// <summary>
        /// Loads the configuration file and applies the settings.
        /// </summary>
        public void LoadConfiguration()
        {
            // Load the network file.

            // Apply general sign pack setting, if any.
            if (Settings.signPackName != null && Settings.signPackName != "Vanilla")
            {
                selectedSignPack = DataStore.signPacks.Find(pack => pack.propPack.name.Equals(Settings.signPackName));

                if (selectedSignPack == null)
                {
                    Debugging.Message("couldn't find configured general sign pack '" + Settings.signPackName + "'.");
                }
                else
                {
                    UpdateSelectedSignPack(selectedSignPack);
                }
            }

            // Apply speed sign pack setting, if any.
            if (Settings.speedPackName != null && Settings.speedPackName != "Vanilla")
            {
                selectedSpeedPack = DataStore.speedPacks.Find(pack => pack.propPack.name.Equals(Settings.speedPackName));

                if (selectedSpeedPack == null)
                {
                    Debugging.Message("Road Sign Replacer: couldn't find configured speed sign pack '" + Settings.speedPackName + "'.");
                }
                else
                {
                    UpdateSelectedSignPack(selectedSpeedPack);
                }
            }

            // Update options panel selection.
            signPackSelection.FindPack(selectedSignPack == null ? "Vanilla" : selectedSignPack.propPack.name);
            speedPackSelection.FindPack(selectedSpeedPack == null ? "Vanilla" : selectedSpeedPack.propPack.name);
            
            // Perform actual replacement if we're in-game.
            if (Loading.InGame)
            {
                Debugging.Message("applying settings");

                SignReplacer.ReplaceRoadProps(selectedSignPack?.propPack, selectedSpeedPack?.propPack);
            }
        }
    }
}