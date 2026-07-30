using MelonLoader;
using HarmonyLib;
using UnityEngine;
using Il2CppHUD;
using Il2CppView_Access;
using AFUtils;

[assembly: MelonInfo(typeof(HideNametags.Core), "HideNametags", "1.0.2", "Klehrik", null)]
[assembly: MelonGame("Videocult", "Airframe")]
[assembly: MelonAdditionalDependencies("AFUtils")]

namespace HideNametags;

public class Core : MelonMod
{
    public static bool enabled = false;
    public static WorldSpaceLabels WSLabels;
    public static MelonLogger.Instance Logger => Melon<Core>.Logger;

    public override void OnInitializeMelon()
    {
        var option = new ActionMenu.Option(
            () =>
            {
                enabled = !enabled;

                var transform = GameObject.Find("Camera_View 0")?.transform.Find("HUDCanvas(Clone)")?.Find("OtherPlayersTracker");
                if (transform != null)
                {
                    transform.gameObject.SetActive(!enabled);
                }
            }
        );

        ActionMenu.RegisterForCollection(
            () =>
            {
                ActionMenu.AddOption(option, enabled ? "show nametags" : "hide nametags");
            }
        );
    }

    public override void OnUpdate()
    {
        if (enabled)
        {
            if (WSLabels != null)
            {
                foreach (var label in WSLabels.active)
                {
                    if (label.type == HUD_Access.WorldSpaceLabelType.Player)
                    {
                        label.Sleep();
                    }
                }
            }
        }
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        enabled = false;
        WSLabels = null;
    }

    [HarmonyPatch(typeof(WorldSpaceLabels), nameof(WorldSpaceLabels.Construct))]
    public class WorldSpaceLabelsPatch
    {
        static void Postfix(WorldSpaceLabels __instance)
        {
            WSLabels = __instance;
        }
    }
}