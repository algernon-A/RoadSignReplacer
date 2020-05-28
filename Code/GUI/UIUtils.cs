using UnityEngine;
using ColossalFramework.UI;


namespace RoadSignReplacer
{
    public class UIUtils
    {
        // Original utils code by SamsamTS, inherited  here via Ploppable RICO.
        // Altered slightly by algernon for Realistic Population Revisited.
        // SamsamTS's comments:
        // Figuring all this was a pain (no documentation whatsoever)
        // So if your are using it for your mod consider thanking me (SamsamTS)
        // Extended Public Transport UI's code helped me a lot so thanks a lot AcidFire
        //
        // So, thank you, SamsamTS!


        public static UIButton CreateButton(UIComponent parent, float width)
        {
            UIButton button = parent.AddUIComponent<UIButton>();

            button.size = new Vector2(width, 30f);
            button.textScale = 0.9f;
            button.normalBgSprite = "ButtonMenu";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.pressedBgSprite = "ButtonMenuPressed";
            button.disabledBgSprite = "ButtonMenuDisabled";
            button.disabledTextColor = new Color32(128, 128, 128, 255);
            button.canFocus = false;

            return button;
        }
    }
}