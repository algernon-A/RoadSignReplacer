using System.Collections.Generic;
using UnityEngine;


namespace RoadSignReplacer
{
    /// <summary>
    /// Actual sign replacer class.
    /// </summary>
    public static class SignReplacer
    {
        // Currently applied packs.
        private static PropPack appliedSignPack;
        private static PropPack appliedSpeedPack;


        /// <summary>
        /// Replace road props.
        /// </summary>
        public static void ReplaceRoadProps(PropPack signSelection, PropPack speedSelection)
        {
            // Flags.
            bool replacingSpeed = false, replacingSigns = false;


            // Set applied packs to default if they're not already set.
            if (appliedSignPack == null)
            {
                appliedSignPack = Selections.defaultSignPack.propPack;
            }
            if (appliedSpeedPack == null)
            {
                appliedSpeedPack = Selections.defaultSpeedPack.propPack;
            }

            // Don't do anything if no selection.
            if (speedSelection == null && signSelection == null)
            {
                Debugging.Message("no selection");
                return;
            }

            // Get speed limit prop prefabs, if there's a valid new current selection.
            if (speedSelection != null && !speedSelection.name.Equals(appliedSpeedPack.name))
            {
                Debugging.Message("replacing speed signs with " + speedSelection.name);
                replacingSpeed = true;
            }

            // Get general prop prefabs, if there's a valid new current selection.
            if (signSelection != null && !signSelection.name.Equals(appliedSignPack.name))
            {
                Debugging.Message("replacing signs with " + signSelection.name);
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
                            UpdateSign(ref laneProp.m_prop, laneProp, laneAngle, replacingSigns ? signSelection : null, replacingSpeed ? speedSelection : null);
                        }

                        // Update any final props.
                        if (laneProp.m_finalProp != null)
                        {
                            UpdateSign(ref laneProp.m_finalProp, laneProp, laneAngle, replacingSigns ? signSelection : null, replacingSpeed ? speedSelection : null);
                        }
                    }
                }
            }

            if (replacingSpeed)
            {
                // Update applied pack.
                appliedSpeedPack = speedSelection;

                // Update current selection dictionary.
                foreach (PropReplacement replacement in speedSelection.propReplacements)
                {
                    Selections.currentSigns[replacement.targetName] = replacement;
                }
            }

            if (replacingSigns)
            {
                // Update applied pack.
                appliedSignPack = signSelection;

                // Update current selection dictionary.
                foreach (PropReplacement replacement in signSelection.propReplacements)
                {
                    Selections.currentSigns[replacement.targetName] = replacement;
                }
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
        /// <param name="signSelection">General sign pack selection to apply (null if none)</param>
        /// <param name="speedSelection">Speed sign pack selection to apply (null if none)</param>
        private static void UpdateSign(ref PropInfo prop, NetLaneProps.Prop lane, float laneAngle, PropPack signSelection, PropPack speedSelection)
        {
            // Revert and replace general signs, if we're doing that.
            if (signSelection != null)
            {
                RevertSign(DataStore.signPacks, ref prop, lane, ref laneAngle);
                ReplaceSign(signSelection, ref prop, lane, laneAngle);
            }

            // Revert and replace speed limit signs, if we're doing that.
            if (speedSelection != null)
            {
                RevertSign(DataStore.speedPacks, ref prop, lane, ref laneAngle);
                ReplaceSign(speedSelection, ref prop, lane, laneAngle);
            }
        }


        /// <summary>
        /// Replaces a given road prop.
        /// </summary>
        /// <param name="newPack">Prop pack to be applied</param>
        /// <param name="prop">Exisiting prop</param>
        /// <param name="lane">Parent LaneProp</param>
        /// <param name="laneAngle">Existing angle for props in prop lane</param>
        private static void ReplaceSign(PropPack newPack, ref PropInfo prop, NetLaneProps.Prop lane, float laneAngle)
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
                        Debugging.Message("couldn't find replacement for " + prop.name);
                    }

                    // All done here.  Once we've got a match there's no point carrying on looping.
                    break;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="packList">Relevant list of prop packs (e.g. sign or speed)</param>
        /// <param name="prop">Existing prop</param>
        /// <param name="lane">Parent LaneProp</param>
        /// <param name="laneAngle">Existing angle for props in prop lane - modified to remove the offset of the reverted prop</param>
        private static void RevertSign(List<InternalPropPack> packList, ref PropInfo prop, NetLaneProps.Prop lane, ref float laneAngle)
        {
            // Iterate through each prop in the packlist.

            foreach (InternalPropPack intPropPack in packList)
            {
                foreach (PropReplacement propReplacement in intPropPack.propPack.propReplacements)
                    {
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
                            Debugging.Message("couldn't revert to vanilla for " + prop.name);
                        }

                        // All done here.  Once we've got a match there's no point carrying on looping.
                        break;
                    }
                }
            }
        }
    }
}