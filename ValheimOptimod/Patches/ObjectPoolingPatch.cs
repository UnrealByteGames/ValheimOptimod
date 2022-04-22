using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace UnrealByte {
    [HarmonyPatch]
    public class ObjectPoolingPatch {

        public static AP_Manager poolManager = null;
        public static List<int> poolsList = new List<int>();
    
        [HarmonyPatch(typeof(ZNetScene), "Awake")]
        public class ZNetScene_Pool {
            static void Postfix(ZNetScene __instance) {
                poolManager = __instance.gameObject.AddComponent<AP_Manager>();
                poolManager.allowCreate = true;
                poolManager.allowModify = true;                    

                Debug.Log("[ValheimOptimod] Creating object pool manager");
                Debug.Log("[ValheimOptimod] " + poolManager);
            }
        }

        [HarmonyPatch(typeof(ZNetScene), "CreateObject")]
        [HarmonyPrefix]
        static bool CreateObject_Prefix(ZNetScene __instance, ref ZDO zdo, ref GameObject __result) {

            __result = null;

            int prefab = zdo.GetPrefab();
            Debug.Log("[ValheimOptimod] ZNetScene.CreateObject: Prefab hash: " + prefab);
            if (prefab == 0) {
                return false;
            }
            GameObject prefab2 = __instance.GetPrefab(prefab);
            if (prefab2 == null) {
                return false;
            }
            Debug.Log("[ValheimOptimod] ZNetScene.CreateObject: Prefab2: " + prefab2.name);

            if (!poolExists(prefab)) {
                //Debug.Log("[ValheimOptimod] ZNetScene.CreateObject: Doesn't exist pool for prefab: " + prefab + ", creating...");
                MF_AutoPool.InitializeSpawn(prefab2, 3, 20, AP_enum.EmptyBehavior.Grow, AP_enum.MaxEmptyBehavior.Fail);
                poolsList.Add(prefab);
            } else {
                //Debug.Log("[ValheimOptimod] ZNetScene.CreateObject: Pool for prefab: " + prefab + ", already exists... Using it!");
            }

            Vector3 position = zdo.GetPosition();
            Quaternion rotation = zdo.GetRotation();
            ZNetView.m_useInitZDO = true;
            ZNetView.m_initZDO = zdo;

            //Add the component to handle the enable/disable
            Debug.Log("[ValheimOptimod] ZNetScene.CreateObject: Checking the component to handle enable/disable...");
            bool hasMWP = false;
            Component[] preFabcomponents = prefab2.GetComponents(typeof(Component));
            foreach (Component component in preFabcomponents) {
                Debug.Log(component.ToString());
                if (component is MonoWrapper_Pool) {
                    Debug.Log("[ValheimOptimod] ZNetScene.CreateObject: Already has the component.");
                    hasMWP = true;
                    continue;
                }
            }
            if (!hasMWP) {
                Debug.Log("[ValheimOptimod] ZNetScene.CreateObject: Adding the component");
                MonoWrapper_Pool mwp = prefab2.AddComponent<MonoWrapper_Pool>();
                mwp.prefab = prefab2;
            }
            prefab2.SetActive(false);

            Debug.Log("[ValheimOptimod] ZNetScene.CreateObject: Prefab: " + prefab2.name + " activestate: " + prefab2.activeSelf);
            //GameObject result = UnityEngine.Object.Instantiate(prefab2, position, rotation);
            GameObject result = MF_AutoPool.Spawn(prefab2, position, rotation);

            if (result != null) {
                Debug.Log("[ValheimOptimod] ZNetScene.CreateObject: Created GameObject " + result.GetInstanceID() + " Name: " + result.name + " activestate: " + result.activeSelf);

                // Result has the MonoWrapper_Pool component?
                Component[] resultFabcomponents = prefab2.GetComponents(typeof(Component));
                foreach (Component component in resultFabcomponents) {
                    Debug.Log(component.ToString());                    
                }

                result.SetActive(true);
                Debug.Log("[ValheimOptimod] ZNetScene.CreateObject: Changed state to: " + result.activeSelf);
            } else Debug.Log("[ValheimOptimod] ZNetScene.CreateObject: Spawned null GameObject " + result);
            if (ZNetView.m_initZDO != null) {
                ZLog.LogWarning(string.Concat("ZDO ", zdo.m_uid, " not used when creating object ", prefab2.name));
                ZNetView.m_initZDO = null;
            }
            ZNetView.m_useInitZDO = false;
            __result = result;

            Debug.Log("[ValheimOptimod] ZNetScene.CreateObject: End");

            return false;
        }

        [HarmonyPatch(typeof(ZNetScene), "RemoveObjects")]
        [HarmonyPrefix]
        static bool RemoveObjects_Prefix(ZNetScene __instance, List<ZDO> currentNearObjects, List<ZDO> currentDistantObjects, Dictionary<ZDO, ZNetView> ___m_instances, List<ZNetView> ___m_tempRemoved) {
            int frameCount = Time.frameCount;
            foreach (ZDO currentNearObject in currentNearObjects) {
                currentNearObject.m_tempRemoveEarmark = frameCount;
            }
            foreach (ZDO currentDistantObject in currentDistantObjects) {
                currentDistantObject.m_tempRemoveEarmark = frameCount;
            }
            ___m_tempRemoved.Clear();
            foreach (ZNetView value in ___m_instances.Values) {
                if (value.GetZDO().m_tempRemoveEarmark != frameCount) {
                    ___m_tempRemoved.Add(value);
                }
            }
            for (int i = 0; i < ___m_tempRemoved.Count; i++) {
                ZNetView zNetView = ___m_tempRemoved[i];
                ZDO zDO = zNetView.GetZDO();
                zNetView.ResetZDO();
                Debug.Log("[ValheimOptimod] ZNetScene.RemoveObjects: Destroyed GameObject " + zNetView.gameObject.GetInstanceID());
                //UnityEngine.Object.Destroy(zNetView.gameObject);
                MF_AutoPool.Despawn(zNetView.gameObject);
                if (!zDO.m_persistent && zDO.IsOwner()) {
                    ZDOMan.instance.DestroyZDO(zDO);
                }
                ___m_instances.Remove(zDO);
            }

            return false;
        }

        /*[HarmonyPatch(typeof(ZNetScene), "Destroy")]
        [HarmonyPrefix]
        static bool Destroy_Prefix(ZNetScene __instance, ref GameObject go, Dictionary<ZDO, ZNetView> ___m_instances) {
            ZNetView component = go.GetComponent<ZNetView>();
            if ((bool)component && component.GetZDO() != null) {
                ZDO zDO = component.GetZDO();
                component.ResetZDO();
                //m_instances.Remove(zDO);
                ___m_instances.Remove(zDO);
                if (zDO.IsOwner()) {
                    ZDOMan.instance.DestroyZDO(zDO);
                }
            }
            Debug.Log("[ValheimOptimod] ZNetScene.Destroy: Destroyed GameObject " + go.GetInstanceID());
            UnityEngine.Object.Destroy(go);
            return false;
        }*/

        //Search RandomFlyingBird.Start

        /*[HarmonyPatch(typeof(RandomFlyingBird), "Start")]
        [HarmonyPrefix]
        static bool RandomFlyingBird_Start_Prefix(RandomFlyingBird __instance, ZNetView ___m_nview, ZSyncAnimation ___m_anim, LODGroup ___m_lodGroup, GameObject ___m_landedModel, GameObject ___m_flyingModel, int ___flapping, Vector3 ___m_spawnPoint, float ___m_randomNoiseTimer, float ___m_randomNoiseIntervalMin, float ___m_randomNoiseIntervalMax, Vector3 ___m_originalLocalRef) {
            Debug.Log("[ValheimOptimod] Bird " + __instance.name + " - " +  __instance.isActiveAndEnabled);
            //Siempre esta activo y enabled, tonce? Tengo que suplantar el "onenable"
            //Debugear para saber donde rompe este metodo.
            ___m_nview = __instance.GetComponent<ZNetView>();
            Debug.Log("[ValheimOptimod] 1");
            ___m_anim = __instance.GetComponentInChildren<ZSyncAnimation>();
            Debug.Log("[ValheimOptimod] 2");
            ___m_lodGroup = __instance.GetComponent<LODGroup>();
            Debug.Log("[ValheimOptimod] 3");
            ___m_landedModel.SetActive(value: true);
            ___m_flyingModel.SetActive(value: true);
            if (___flapping == 0) {
                ___flapping = ZSyncAnimation.GetHash("flapping");
            }
            Debug.Log("[ValheimOptimod] 4");
            ___m_spawnPoint = ___m_nview.GetZDO().GetVec3("spawnpoint", __instance.transform.position);
            if (___m_nview.IsOwner()) {
                ___m_nview.GetZDO().Set("spawnpoint", ___m_spawnPoint);
            }
            Debug.Log("[ValheimOptimod] 5");
            ___m_randomNoiseTimer = UnityEngine.Random.Range(___m_randomNoiseIntervalMin, ___m_randomNoiseIntervalMax);
            if (___m_nview.IsOwner()) {
                My_RandomizeWaypoint(__instance, ground: false);
            }
            Debug.Log("[ValheimOptimod] 6");
            if ((bool)___m_lodGroup) {
                ___m_originalLocalRef = ___m_lodGroup.localReferencePoint;
            }
            Debug.Log("[ValheimOptimod] 7");
            return true;
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(RandomFlyingBird), "RandomizeWaypoint")]
        public static void My_RandomizeWaypoint(object instance, bool ground) {
            // its a stub so it has no initial content
            throw new NotImplementedException("It's a stub");
        }*/

        public static bool poolExists(int prefab) {
            bool exists = false;
            foreach (int kp in poolsList) {
                if (kp == prefab) {
                    exists = true;
                    break;
                }
            }
            return exists;
        }        


    }

    public class MonoWrapper_Pool : MonoBehaviour {

        public GameObject prefab = null;
        
        void OnEnable() {
            Debug.Log("[ValheimOptimod] MonoWrapper_Pool: script was enabled on gameobject " + gameObject.name);            
        }

        void OnDisable() {
            Debug.Log("[ValheimOptimod] MonoWrapper_Pool: script was disabled on gameobject " + gameObject.name);
            /*Component[] preFabcomponents = GetComponents(typeof(Component));
            foreach (Component component in preFabcomponents) {
                Debug.Log(component.ToString());
                if (component is MonoWrapper_Pool) continue;
            }*/
        }

        Component CopyComponent(Component original, GameObject destination) {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            // Copied fields can be restricted with BindingFlags
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields) {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy;
        }


    }

}