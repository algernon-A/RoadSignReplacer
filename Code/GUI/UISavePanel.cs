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
        public PropPack currentSignSelection;


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
            applyButton.eventClick += (control, clickEvent) =>
            {
                SignReplacer.ReplaceRoadProps(currentSignSelection, currentSpeedSelection);
            };
        }
    }
}