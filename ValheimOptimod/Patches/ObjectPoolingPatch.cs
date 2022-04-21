using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace UnrealByte {
    public class ObjectPoolingPatch {

        public static AP_Manager poolManager = null;
    
        [HarmonyPatch]
        class ZNetScene_Patches {

            [HarmonyPatch(typeof(ZNetScene), "Awake")]
            public class ZNetScene_Pool {
                static void Postfix(ZNetScene __instance) {
                    poolManager = __instance.gameObject.AddComponent<AP_Manager>();
                    poolManager.allowCreate = true;
                    poolManager.allowModify = true;
                    //MF_AutoPool.InitializeSpawn()

                    Debug.Log("ValheimOptimod: Creating object pool");
                    Debug.Log("ValheimOptimod: " + poolManager);
                }
            }

            [HarmonyPatch(typeof(ZNetScene), "CreateObject")]
            static bool Prefix(ZNetScene __instance, ref ZDO zdo, ref GameObject __result) {

                __result = null;                

                int prefab = zdo.GetPrefab();
                if (prefab == 0) {
                    return false;
                }
                //GameObject prefab2 = GetPrefab(prefab);
                GameObject prefab2 = __instance.GetPrefab(prefab);
                if (prefab2 == null) {
                    return false;
                }
                Vector3 position = zdo.GetPosition();
                Quaternion rotation = zdo.GetRotation();
                ZNetView.m_useInitZDO = true;
                ZNetView.m_initZDO = zdo;
                GameObject result = UnityEngine.Object.Instantiate(prefab2, position, rotation);
                if (ZNetView.m_initZDO != null) {
                    ZLog.LogWarning(string.Concat("ZDO ", zdo.m_uid, " not used when creating object ", prefab2.name));
                    ZNetView.m_initZDO = null;
                }
                ZNetView.m_useInitZDO = false;
                __result = result;

                return false;
            }


        }

    }
}