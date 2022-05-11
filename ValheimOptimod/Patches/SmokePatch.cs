using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace UnrealByte {
    public class SmokePatch {

        public static void OnReload() {

        }

        /*[HarmonyPatch(typeof(Smoke), "Awake")]
        class Patch_Smoke {
            static bool Prefix(Smoke __instance) {
                if (ValheimOptimod.smoke.Value == 0) __instance.gameObject.SetActive(false);
                return false;
            }
        }*/

        [HarmonyPatch(typeof(SmokeSpawner), "Spawn")]
        [HarmonyPrefix]
        static bool SmokeSpawner_Prefix(SmokeSpawner __instance) {
            if (ValheimOptimod.smoke.Value == 0) __instance.gameObject.SetActive(false);
            return false;
        }
    }
}