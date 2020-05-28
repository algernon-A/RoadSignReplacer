using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ICities;
using ColossalFramework.UI;
using ColossalFramework.Packaging;


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


        // Data objects.
        public static List<InternalPropPack> speedPacks;
        public static List<InternalPropPack> signPacks;
        private static ConfigurationFile configurationFile;

        // Panel components.
        private UIFastList speedPackSelection;
        private UIFastList signPackSelection;
        private UISavePanel savePanel;

        // Selected items.
        private InternalPropPack selectedSpeedPack;
        private InternalPropPack selectedSignPack;

        // Instance reference.
        private static OptionsPanel _instance;
        public static OptionsPanel Instance => _instance;


        /// <summary>
        /// Options panel constructor.
        /// </summary>
        /// <param name="helper">UIHelperBase parent</param>
        public OptionsPanel(UIHelperBase helper)
        {
            // Load settings.
            configurationFile = Configuration<ConfigurationFile>.Load(true);

            if (configurationFile == null)
            {
                Debug.Log("Road Sign Replacer: configuration file not found.");
                return;
            }

            if (configurationFile.propPacks == null)
            {
                Debug.Log("Road Sign Replacer: no valid records found in configuration file.");
                return;
            }

            // Set instance.
            _instance = this;

            if (Selections.currentSigns == null)
            {
                Selections.Setup();
            }

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
                                Debug.Log("Road Sign Replacer: workshop subscription " + workshopPrefix + " not found.");
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
            speedPackSelection.rowsData = ToFastList(speedPacks);

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
            signPackSelection.rowsData = ToFastList(signPacks);

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
            // Load the file.
            SettingsFile settingsFile = Configuration<SettingsFile>.Load();

            // Apply general sign pack setting, if any.
            if (settingsFile.signPackName != null && settingsFile.signPackName != "Vanilla")
            {
                selectedSignPack = signPacks.Find(pack => pack.propPack.name.Equals(settingsFile.signPackName));

                if (selectedSignPack == null)
                {
                    Debug.Log("Road Sign Replacer: couldn't find configured general sign pack '" + settingsFile.signPackName + "'.");
                }

                UpdateSelectedSignPack(selectedSignPack);
            }

            // Apply speed sign pack setting, if any.
            if (settingsFile.speedPackName != null && settingsFile.speedPackName != "Vanilla")
            {
                selectedSpeedPack = speedPacks.Find(pack => pack.propPack.name.Equals(settingsFile.speedPackName));

                if (selectedSpeedPack == null)
                {
                    Debug.Log("Road Sign Replacer: couldn't find configured speed sign pack '" + settingsFile.speedPackName + "'.");
                }

                UpdateSelectedSignPack(selectedSpeedPack);
            }

            // Update options panel selection.
            signPackSelection.FindPack(selectedSignPack == null ? "Vanilla" : selectedSignPack.propPack.name);
            speedPackSelection.FindPack(selectedSpeedPack == null ? "Vanilla" : selectedSpeedPack.propPack.name);

            // Perform actual replacement if we're in-game.
            if (Loading.InGame)
            {
                savePanel.ReplaceRoadProps();
            }
        }
    }
}