using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace UnrealByte {
    public class SmokePatch {

        [HarmonyPatch(typeof(Smoke), "Awake")]
        class Patch_Smoke {
            static bool Prefix(Smoke __instance) {
                __instance.gameObject.SetActive(false);
                return false;
            }
        }

        [HarmonyPatch(typeof(SmokeSpawner), "Spawn")]
        class Patch_SmokeSpawner {
            static bool Prefix(SmokeSpawner __instance) {
                __instance.gameObject.SetActive(false);
                return false;
            }
        }        
    }
}