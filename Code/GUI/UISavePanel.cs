using System.Text;
using UnityEngine;
using ColossalFramework.UI;


namespace RoadSignReplacer
{
    /// <summary>
    /// Panel for saving settings.
    /// </summary>
    public class UISavePanel : UIPanel
    {
        // Panel components
        private UIButton applyButton;

        // Sign pack selections.
        public PropPack currentSpeedSelection;
        public PropPack appliedSpeedPack;
        public PropPack currentSignSelection;
        public PropPack appliedSignPack;


        /// <summary>
        /// Create the panel; called by Unity just before any of the Update methods is called for the first time.
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Generic setup.
            isVisible = true;
            canFocus = true;
            isInteractive = true;
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Horizontal;
            autoLayoutPadding.top = 5;
            autoLayoutPadding.left = 5;
            autoLayoutPadding.right = 5;
            builtinKeyNavigation = true;
            clipChildren = true;

            // Apply button.
            applyButton = UIUtils.CreateButton(this, 200);
            //applyButton.relativePosition = new Vector3(marginPadding, 70);
            applyButton.text = "Apply";
            applyButton.tooltip = "Applies settings";

            // Apply button event handler.
            applyButton.eventClick += (c, p) =>
            {
                ReplaceRoadProps();
            };

            // Defaults.
            appliedSignPack = Selections.defaultSignPack.propPack;
            appliedSpeedPack = Selections.defaultSpeedPack.propPack;
        }


        /// <summary>
        /// Replace road props.
        /// </summary>
        public void ReplaceRoadProps()
        {
            // Flags.
            bool replacingSpeed = false, replacingSigns = false;


            // Don't do anything if no selection.
            if (currentSpeedSelection == null && currentSignSelection == null)
            {
                Debug.Log("Road Sign Replacer: no selection.");
                return;
            }

            // Get speed limit prop prefabs, if there's a valid new current selection.
            if (currentSpeedSelection != null && !currentSpeedSelection.name.Equals(appliedSpeedPack.name))
            {
                Debug.Log("Road Sign Replacer - replacing speed signs with " + currentSpeedSelection.name);
                replacingSpeed = true;
            }

            // Get general prop prefabs, if there's a valid new current selection.
            if (currentSignSelection != null && !currentSignSelection.name.Equals(appliedSignPack.name))
            {
                Debug.Log("Road Sign Replacer - replacing signs with " + currentSignSelection.name);
                replacingSigns = true;
            }

            // Find all roads and iterate.
            NetInfo[] networks = Resources.FindObjectsOfTypeAll<NetInfo>();

            // Iterate through all roads.
            foreach (NetInfo netInfo in networks)
            {
                // Iterate through all lanes.
                foreach (NetInfo.Lane lane in netInfo.m_lanes)
                {
                    // Only interested in lanes with props.
                    if (lane?.m_laneProps?.m_props == null)
                    {
                        continue;
                    }

                    // Iterate through all props in lane.
                    foreach (NetLaneProps.Prop laneProp in lane.m_laneProps.m_props)
                    {
                        // Only interested in props.
                        if (laneProp == null)
                        {
                            continue;
                        }

                        // Get currently applied lane angle - we'll need to refer back to this after we start changing it.
                        float laneAngle = laneProp.m_angle;

                        // Update any props.
                        if (laneProp.m_prop != null)
                        {
                            UpdateSign(ref laneProp.m_prop, laneProp, laneAngle, replacingSpeed, replacingSigns);
                        }

                        // Update any final props.
                        if (laneProp.m_finalProp != null)
                        {
                            UpdateSign(ref laneProp.m_finalProp, laneProp, laneAngle, replacingSpeed, replacingSigns);
                        }
                    }
                }
            }

            if (replacingSpeed)
            {
                // Update applied pack.
                appliedSpeedPack = currentSpeedSelection;

                // Update current selection dictionary.
                foreach (PropReplacement replacement in currentSpeedSelection.propReplacements)
                {
                    Selections.currentSigns[replacement.targetName] = replacement;
                }

                // Save to configuration file.
                SettingsFile settingsFile = Configuration<SettingsFile>.Load();
                settingsFile.speedPackName = currentSpeedSelection.name;
                Configuration<SettingsFile>.Save();
            }

            if (replacingSigns)
            {
                // Update applied pack.
                appliedSignPack = currentSignSelection;

                // Update current selection dictionary.
                foreach (PropReplacement replacement in currentSignSelection.propReplacements)
                {
                    Selections.currentSigns[replacement.targetName] = replacement;
                }

                // Save to configuration file.
                SettingsFile settingsFile = Configuration<SettingsFile>.Load();
                settingsFile.signPackName = currentSignSelection.name;
                Configuration<SettingsFile>.Save();
            }

        }


        /// <summary>
        /// Updates a given roadsign (revert and/or replace).
        /// </summary>
        /// <param name="prop">Exisiting prop</param>
        /// <param name="lane">Parent LaneProp</param>
        /// <param name="laneAngle">Existing angle for props in prop lane</param>
        /// <param name="replaceSpeed">True if speed signs are being replaced, false otherwise</param>
        /// <param name="replaceSign">True if general signs are being replaced, false otherwise</param>
        private void UpdateSign(ref PropInfo prop, NetLaneProps.Prop lane, float laneAngle, bool replaceSpeed, bool replaceSign)
        {
            // Revert and replace general signs, if we're doing that.
            if (replaceSign)
            {
                RevertSign(appliedSignPack, ref prop, lane, ref laneAngle);
                ReplaceSign(currentSignSelection, ref prop, lane, laneAngle);
            }

            // Revert and replace speed limit signs, if we're doing that.
            if (replaceSpeed)
            {
                RevertSign(appliedSpeedPack, ref prop, lane, ref laneAngle);
                ReplaceSign(currentSpeedSelection, ref prop, lane, laneAngle);
            }
        }


        /// <summary>
        /// Replaces a given road prop.
        /// </summary>
        /// <param name="newPack">Prop pack to be applied</param>
        /// <param name="prop">Exisiting prop</param>
        /// <param name="lane">Parent LaneProp</param>
        /// <param name="laneAngle">Existing angle for props in prop lane</param>
        private void ReplaceSign(PropPack newPack, ref PropInfo prop, NetLaneProps.Prop lane, float laneAngle)
        {
            // Iterate through each prop in the replacement pack.
            foreach (PropReplacement propReplacement in newPack.propReplacements)
            {
                if (prop.name.Equals(propReplacement.targetName))
                {
                    // Got a name match with this prop; load the new prefab.
                    PropInfo replacementProp = PrefabCollection<PropInfo>.FindLoaded(propReplacement.replacementName);

                    if (replacementProp != null)
                    {
                        // Valid replacement found - change the prop and adjust the angle.
                        prop = replacementProp;
                        lane.m_angle = laneAngle + propReplacement.rotation;
                    }
                    else
                    {
                        Debug.Log("Road Sign Replacer: couldn't find replacement for " + prop.name);
                    }

                    // All done here.  Once we've got a match there's no point carrying on looping.
                    break;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="appliedPack">Currently applied prop pack</param>
        /// <param name="prop">Existing prop</param>
        /// <param name="lane">Parent LaneProp</param>
        /// <param name="laneAngle">Existing angle for props in prop lane - modified to remove the offset of the reverted prop</param>
        private void RevertSign(PropPack appliedPack, ref PropInfo prop, NetLaneProps.Prop lane, ref float laneAngle)
        {
            foreach (PropReplacement propReplacement in appliedPack.propReplacements)
            {
                // Iterate through each prop in currently applied prop pack.
                if (prop.name.Equals(propReplacement.replacementName))
                {
                    // Got a name match with this prop; load the new prefab.
                    PropInfo originalProp = PrefabCollection<PropInfo>.FindLoaded(propReplacement.targetName);

                    if (originalProp != null)
                    {
                        // Replace prop and reset lane angle.
                        prop = originalProp;
                        laneAngle -= propReplacement.rotation;
                    }
                    else
                    {
                        Debug.Log("Road Sign Replacer: couldn't revert to original for " + prop.name);
                    }

                    // All done here.  Once we've got a match there's no point carrying on looping.
                    break;
                }
            }
        }
    }
}