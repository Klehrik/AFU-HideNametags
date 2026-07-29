using UnityEngine;
using UnityEngine.InputSystem;
using MelonLoader;
using HarmonyLib;
using Il2CppHUD;
using Il2CppQuantum;
using Il2CppView_Access;

[assembly: MelonInfo(typeof(HideNametags.Core), "HideNametags", "1.0.1", "Klehrik", null)]
[assembly: MelonGame("Videocult", "Airframe")]

namespace HideNametags;

public class Core : MelonMod
{
    public static bool enabled;
    public static WorldSpaceLabels WSLabels;
    public static MelonLogger.Instance Logger => Melon<Core>.Logger;

    public Core()
    {
        enabled = false;
    }

    public override void OnUpdate()
    {
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            enabled = !enabled;
            Logger.Msg("Hidden: " + enabled.ToString());

            var transform = GameObject.Find("Camera_View 0")?.transform.Find("HUDCanvas(Clone)")?.Find("OtherPlayersTracker");
            if (transform != null)
            {
                transform.gameObject.SetActive(!enabled);
            }
        }

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