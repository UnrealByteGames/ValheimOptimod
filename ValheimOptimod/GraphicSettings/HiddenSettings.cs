using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace UnrealByte {
    class HiddenSettings {
        public static void OnReload() {
            ApplyShadowQuality();
            ApplyQualitySettings();
            ApplyStartupSettings();
            Game_Start();
        }

        [HarmonyPatch(typeof(Settings), "ApplyShadowQuality")]
        [HarmonyPostfix]
        private static void ApplyShadowQuality() {
            if (ValheimOptimod.printValues.Value) {
                Debug.Log("----Shadow settings----");
                Debug.Log($"shadowResolution: {QualitySettings.shadowResolution} to {(ShadowResolution)ValheimOptimod.shadowResolution.Value}");
                Debug.Log($"shadowCascades: {QualitySettings.shadowCascades} to {ValheimOptimod.shadowCascades.Value}");
                Debug.Log($"shadowDistance: {QualitySettings.shadowDistance} to {ValheimOptimod.shadowDistance.Value}");
                Debug.Log($"shadows: {QualitySettings.shadows} to {(ShadowQuality)ValheimOptimod.shadows.Value}");
            }

            QualitySettings.shadowCascades = ValheimOptimod.shadowCascades.Value;
            QualitySettings.shadowDistance = ValheimOptimod.shadowDistance.Value;
            QualitySettings.shadows = (ShadowQuality)ValheimOptimod.shadows.Value;
            QualitySettings.shadowResolution = (ShadowResolution)ValheimOptimod.shadowResolution.Value;
        }

        [HarmonyPatch(typeof(Settings), "ApplyQualitySettings")]
        [HarmonyPostfix]
        private static void ApplyQualitySettings() {
            if (ValheimOptimod.printValues.Value) {
                Debug.Log("----General settings----");
                Debug.Log($"AnisotropicFiltering: {QualitySettings.anisotropicFiltering} to {(AnisotropicFiltering)ValheimOptimod.anisotropicFiltering.Value}");
                Debug.Log($"MasterTextureLimit: {QualitySettings.masterTextureLimit} to {ValheimOptimod.masterTextureLimit.Value}");
                if (SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Vulkan)
                    Debug.Log($"MaxQueuedFrames: {QualitySettings.maxQueuedFrames} to {ValheimOptimod.maxQueuedFrames.Value}");
                else
                    Debug.Log("Vulkan detected, not applying MaxQueuedFrames setting");
                Debug.Log($"ParticleRaycastBudget: {QualitySettings.particleRaycastBudget} to {ValheimOptimod.particleRaycastBudget.Value}");
                Debug.Log($"PixelLightCount: {QualitySettings.pixelLightCount} to {ValheimOptimod.pixelLightCount.Value}");
                Debug.Log($"RealtimeReflectionProbes: {QualitySettings.realtimeReflectionProbes} to {ValheimOptimod.realtimeReflectionProbes.Value}");
                Debug.Log($"SkinWeights: {QualitySettings.skinWeights} to {(SkinWeights)ValheimOptimod.skinWeights.Value}");
                Debug.Log($"SoftParticles: {QualitySettings.softParticles} to {ValheimOptimod.softParticles.Value}");
                Debug.Log($"SoftVegetation: {QualitySettings.softVegetation} to {ValheimOptimod.softVegetation.Value}");
                Debug.Log($"LodBias: {QualitySettings.lodBias} to {ValheimOptimod.lodBias.Value}");
            }

            QualitySettings.anisotropicFiltering = (AnisotropicFiltering)ValheimOptimod.anisotropicFiltering.Value;
            QualitySettings.lodBias = ValheimOptimod.lodBias.Value;
            QualitySettings.masterTextureLimit = ValheimOptimod.masterTextureLimit.Value;
            if(SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Vulkan)
                QualitySettings.maxQueuedFrames = ValheimOptimod.maxQueuedFrames.Value;
            QualitySettings.particleRaycastBudget = ValheimOptimod.particleRaycastBudget.Value;
            QualitySettings.pixelLightCount = ValheimOptimod.pixelLightCount.Value;
            QualitySettings.realtimeReflectionProbes = ValheimOptimod.realtimeReflectionProbes.Value;
            QualitySettings.skinWeights = (SkinWeights)ValheimOptimod.skinWeights.Value;
            QualitySettings.softParticles = ValheimOptimod.softParticles.Value;
            QualitySettings.softVegetation = ValheimOptimod.softVegetation.Value;
        }

        [HarmonyPatch(typeof(Game), "Start")]
        [HarmonyPostfix]
        private static void Game_Start() {
            Debug.Log($"MaxFPS: {Application.targetFrameRate} to {ValheimOptimod.maxFps.Value}");
            Application.targetFrameRate = ValheimOptimod.maxFps.Value;
        }

        [HarmonyPatch(typeof(Settings), "ApplyStartupSettings")]
        [HarmonyPostfix]
        private static void ApplyStartupSettings() {
            Debug.Log($"vSyncCount: {QualitySettings.vSyncCount} to {ValheimOptimod.vSyncCount.Value}");
            QualitySettings.vSyncCount = ValheimOptimod.vSyncCount.Value;
        }
    }
}
