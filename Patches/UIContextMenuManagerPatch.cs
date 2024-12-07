namespace InstantFishing.Patches
{
    public class UIContextMenuManagerPatch
    {
        public static void Create(UIContextMenuManager __instance, string menuName, bool destroyOnHide)
        {
            if (InstantFishingConfig.EnableMenu?.Value == true &&
                menuName == "ContextSystem")
            {
                __instance.currentMenu.AddButton(idLang: OmegaUI.__(ja: "Instant Fishing 設定", en: "Instant Fishing Config"), action: delegate()
                {
                    OmegaUI.OpenWidget<InstantFishingWidgetMain>();
                }, hideAfter: true);
                return;
            }
        }
    }
}