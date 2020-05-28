using UnityEngine;
using ColossalFramework.UI;


namespace RoadSignReplacer
{
    /// <summary>
    /// An individual row in the list of sign packs.
    /// </summary>
    public class UISignPackRow : UIPanel, UIFastListRow
    {
        // Panel components.
        private UIPanel panelBackground;
        private UILabel signPackName;
        public InternalPropPack thisPropPack;


        // Background for each list item.
        public UIPanel background
        {
            get
            {
                if (panelBackground == null)
                {
                    panelBackground = AddUIComponent<UIPanel>();
                    panelBackground.width = width;
                    panelBackground.height = OptionsPanel.rowHeight;
                    panelBackground.relativePosition = Vector2.zero;

                    panelBackground.zOrder = 0;
                }

                return panelBackground;
            }
        }


        /// <summary>
        /// Called when dimensions are changed, including as part of initial setup (required to set correct relative position of label).
        /// </summary>
        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            if (signPackName != null)
            {
                background.width = width;
                signPackName.relativePosition = new Vector3(10f, 6f);
            }
        }


        /// <summary>
        /// Mouse click event handler - updates the selected building to what was clicked.
        /// </summary>
        /// <param name="p">Mouse event parameter</param>
        protected override void OnClick(UIMouseEventParameter p)
        {
            base.OnClick(p);
            OptionsPanel.Instance.UpdateSelectedSignPack(thisPropPack);
        }


        /// <summary>
        /// Generates and displays a building row.
        /// </summary>
        /// <param name="data">Object to list</param>
        /// <param name="isRowOdd">If the row is an odd-numbered row (for background banding)</param>
        public void Display(object data, bool isRowOdd)
        {
            // Perform initial setup for new rows.
            if (signPackName == null)
            {
                isVisible = true;
                canFocus = true;
                isInteractive = true;
                width = parent.width;
                height = OptionsPanel.rowHeight;

                signPackName = AddUIComponent<UILabel>();
                signPackName.width = 200;
                signPackName.relativePosition = new Vector3(10f, 6f);
            }

            // Set selected signpack.
            thisPropPack = data as InternalPropPack;
            signPackName.text = thisPropPack.propPack.name;

            // Set initial background as deselected state.
            Deselect(isRowOdd);
        }


        /// <summary>
        /// Highlights the selected row.
        /// </summary>
        /// <param name="isRowOdd">If the row is an odd-numbered row (for background banding)</param>
        public void Select(bool isRowOdd)
        {
            background.backgroundSprite = "ListItemHighlight";
            background.color = new Color32(255, 255, 255, 255);
        }


        /// <summary>
        /// Unhighlights the (un)selected row.
        /// </summary>
        /// <param name="isRowOdd">If the row is an odd-numbered row (for background banding)</param>
        public void Deselect(bool isRowOdd)
        {
            if (isRowOdd)
            {
                // Lighter background for odd rows.
                background.backgroundSprite = "UnlockingItemBackground";
                background.color = new Color32(0, 0, 0, 128);
            }
            else
            {
                // Darker background for even rows.
                background.backgroundSprite = null;
            }
        }
    }
}