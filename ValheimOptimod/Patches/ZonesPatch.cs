using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace UnrealByte {
    public class ZonesPatch {

        //public static int instancesCount = 0;
        //private static int nextUpdate = 5;

        /**
         * 
        The game world is split to "zones" of 64x64 meters. 
        By default the current zone and all adjacent zones consist of the active area. 
        This is the area where things happen and where creatures are visible.

        Around that is the loaded area. Here objects exist in the world but are frozen. 
        Static objects like structures are visible here. Real terrain is visible here.

        Finally there is the distant area that is two zones around the loaded area. 
        Here most objects are instantly destroyed after being generated. Big static objects like trees are visible here.

        */

        [HarmonyPatch(typeof(ZoneSystem), "Awake")]
        public class ZoneSystemActive {
            static void Postfix(ZoneSystem __instance) => Set(__instance);
            static void Set(ZoneSystem obj) {
                obj.m_activeArea = ValheimOptimod.loadedZones.Value - 1;
                obj.m_activeDistantArea = ValheimOptimod.generatedZones.Value - ValheimOptimod.loadedZones.Value;
            }
            public static void Update() {
                if (ZoneSystem.instance) Set(ZoneSystem.instance);
            }
        }

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.InActiveArea), new[] { typeof(Vector2i), typeof(Vector2i) })]
        public class InActiveArea {
            static bool Prefix(Vector2i zone, Vector2i refCenterZone, ref bool __result) {
                var num = ValheimOptimod.activeZones.Value - 1;
                __result = zone.x >= refCenterZone.x - num && zone.x <= refCenterZone.x + num && zone.y <= refCenterZone.y + num && zone.y >= refCenterZone.y - num;
                return false;
            }
        }

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.OutsideActiveArea), new[] { typeof(Vector3), typeof(Vector3) })]
        public class OutsideActiveArea {
            static bool Prefix(Vector3 point, Vector3 refPoint, ref bool __result) {
                var num = ValheimOptimod.activeZones.Value - 1;
                var zone = ZoneSystem.instance.GetZone(refPoint);
                var zone2 = ZoneSystem.instance.GetZone(point);
                __result = zone2.x <= zone.x - num || zone2.x >= zone.x + num || zone2.y >= zone.y + num || zone2.y <= zone.y - num;
                return false;
            }
        }

    }
}