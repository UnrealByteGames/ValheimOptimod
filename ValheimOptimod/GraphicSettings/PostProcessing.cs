using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.PostProcessing;
using UnityStandardAssets.ImageEffects;

namespace UnrealByte {
    public static class PostProcessing {
        private static PostProcessingBehaviour postProcessingBehaviour;
        private static CameraEffects cameraEffects;

        public static void OnReload() {
            SetBloom();
            SetSunShafts(null);
            SetSSAO();
            SetMotionBlur();
            SetAntiAliasing();
            SetCA();
            PostApplySettings();
        }

        [HarmonyPatch(typeof(CameraEffects), "Awake")]
        [HarmonyPrefix]
        private static void Awake(CameraEffects __instance) {
            postProcessingBehaviour = __instance.GetComponent<PostProcessingBehaviour>();
        }

        [HarmonyPatch(typeof(CameraEffects), "ApplySettings")]
        [HarmonyPostfix]
        private static void PostApplySettings() {
            if (postProcessingBehaviour.profile.colorGrading == null)
                return;

            ColorGradingModel.Settings settings = postProcessingBehaviour.profile.colorGrading.settings;

            if (ValheimOptimod.printValues.Value) {
                Debug.Log("----Misc----");
                Debug.Log($"GammaCorrection: {(settings.colorWheels.linear.gamma).ToString("0.")} to {ValheimOptimod.gammaCorrection.Value}");
            }

            settings.colorWheels.linear.gamma = ValheimOptimod.gammaCorrection.Value;

            postProcessingBehaviour.profile.colorGrading.settings = settings;

            ApplyFogSettings();

            postProcessingBehaviour.enabled = !ValheimOptimod.disablePostProcessing.Value;
        }

        private static void ApplyFogSettings() {
            if (SceneManager.GetActiveScene().name == "start")
                return;

            if (ValheimOptimod.printValues.Value) {
                Debug.Log($"FogColor: {RenderSettings.fogColor} to {ValheimOptimod.fogColor.Value}");
                Debug.Log($"FogDensity: {RenderSettings.fogDensity} to {ValheimOptimod.fogDensity.Value}");
                Debug.Log($"FogStartDistance: {RenderSettings.fogStartDistance} to {ValheimOptimod.fogStartDistance.Value}");
                Debug.Log($"FogEndDistance: {RenderSettings.fogEndDistance} to {ValheimOptimod.fogEndDistance.Value}");
                Debug.Log($"FogMode: {RenderSettings.fogMode} to {(FogMode)ValheimOptimod.fogMode.Value}");
                Debug.Log($"Fog: {postProcessingBehaviour.profile.fog.enabled} to {ValheimOptimod.fog.Value}");
            }

            RenderSettings.fogColor = ValheimOptimod.fogColor.Value;
            RenderSettings.fogDensity = ValheimOptimod.fogDensity.Value;
            RenderSettings.fogStartDistance = ValheimOptimod.fogStartDistance.Value;
            RenderSettings.fogEndDistance = ValheimOptimod.fogEndDistance.Value;
            RenderSettings.fogMode = (FogMode)ValheimOptimod.fogMode.Value;
            postProcessingBehaviour.profile.fog.enabled = ValheimOptimod.fog.Value;
        }

        [HarmonyPatch(typeof(CameraEffects), "SetSunShafts")]
        [HarmonyPrefix]
        private static void SetSunShafts(CameraEffects __instance) {
            if (__instance != null && cameraEffects == null)
                cameraEffects = __instance;

            SunShafts component = cameraEffects.GetComponent<SunShafts>();

            if (component == null)
                return;

            if (ValheimOptimod.printValues.Value) {
                Debug.Log("----SunShafts settings----");
                Debug.Log($"SunShaftsResolution: {component.resolution} to {(SunShafts.SunShaftsResolution)ValheimOptimod.sunShaftsResolution.Value}");
                Debug.Log($"SunShaftsScreenBlendMode: {component.screenBlendMode} to {(SunShafts.ShaftsScreenBlendMode)ValheimOptimod.sunShaftsScreenBlendMode.Value}");
                Debug.Log($"SunShaftsMaxRadius: {component.maxRadius} to {ValheimOptimod.sunShaftsMaxRadius.Value}");
                Debug.Log($"SunShaftsBlurRadius: {component.sunShaftBlurRadius} to {ValheimOptimod.sunShaftsBlurRadius.Value}");
                Debug.Log($"SunShaftsBlurIterations: {component.radialBlurIterations} to {ValheimOptimod.sunShaftsRadialBlurIterations.Value}");
                Debug.Log($"SunShaftsIntensity: {component.sunShaftIntensity} to {ValheimOptimod.sunShaftsIntensity.Value}");
            }

            component.resolution = (SunShafts.SunShaftsResolution)ValheimOptimod.sunShaftsResolution.Value;
            component.screenBlendMode = (SunShafts.ShaftsScreenBlendMode)ValheimOptimod.sunShaftsScreenBlendMode.Value;
            component.maxRadius = ValheimOptimod.sunShaftsMaxRadius.Value;
            component.sunShaftBlurRadius = ValheimOptimod.sunShaftsBlurRadius.Value;
            component.radialBlurIterations = ValheimOptimod.sunShaftsRadialBlurIterations.Value;
            component.sunShaftIntensity = ValheimOptimod.sunShaftsIntensity.Value;
        }

        [HarmonyPatch(typeof(CameraEffects), "SetBloom")]
        [HarmonyPrefix]
        private static void SetBloom() {
            if (postProcessingBehaviour.profile.bloom == null)
                return;

            BloomModel.Settings settings = postProcessingBehaviour.profile.bloom.settings;

            if (ValheimOptimod.printValues.Value) {
                Debug.Log("----Bloom settings----");
                Debug.Log($"BloomIntensity: {settings.bloom.intensity} to {ValheimOptimod.bloomIntensity.Value}");
                Debug.Log($"BloomAntiFlicker: {settings.bloom.antiFlicker} to {ValheimOptimod.bloomAntiFlicker.Value}");
                Debug.Log($"BloomRadious: {settings.bloom.radius} to {ValheimOptimod.bloomRadious.Value}");
                Debug.Log($"BloomSoftKnee: {settings.bloom.softKnee} to {ValheimOptimod.bloomSoftKnee.Value}");
                Debug.Log($"LensDirtIntensity: {settings.lensDirt.intensity} to {ValheimOptimod.lensDirtIntensity.Value}");
            }

            settings.bloom.intensity = ValheimOptimod.bloomIntensity.Value;
            settings.bloom.antiFlicker = ValheimOptimod.bloomAntiFlicker.Value;
            settings.bloom.radius = ValheimOptimod.bloomRadious.Value;
            settings.bloom.softKnee = ValheimOptimod.bloomSoftKnee.Value;
            settings.lensDirt.intensity = ValheimOptimod.lensDirtIntensity.Value;

            postProcessingBehaviour.profile.bloom.settings = settings;
        }

        [HarmonyPatch(typeof(CameraEffects), "SetSSAO")]
        [HarmonyPostfix]
        private static void SetSSAO() {
            if (postProcessingBehaviour.profile.ambientOcclusion == null)
                return;

            AmbientOcclusionModel.Settings settings = postProcessingBehaviour.profile.ambientOcclusion.settings;

            if (ValheimOptimod.printValues.Value) {
                Debug.Log("----Ambient Oclusion settings----");
                Debug.Log($"SSAOintensity: {settings.intensity} to {ValheimOptimod.SSAOintensity.Value}");
                Debug.Log($"SSAOhighPrecision: {settings.highPrecision} to {ValheimOptimod.SSAOhighPrecision.Value}");
                Debug.Log($"SSAOambientOnly: {settings.ambientOnly} to {ValheimOptimod.SSAOambientOnly.Value}");
                Debug.Log($"SSAOdownSampling: {settings.downsampling} to {ValheimOptimod.SSAOdownSampling.Value}");
                Debug.Log($"SSAOfarDistance: {settings.farDistance} to {ValheimOptimod.SSAOfarDistance.Value}");
                Debug.Log($"SSAOsampleCount: {settings.sampleCount} to {(AmbientOcclusionModel.SampleCount)ValheimOptimod.SSAOsampleCount.Value}");
            }

            settings.intensity = ValheimOptimod.SSAOintensity.Value;
            settings.highPrecision = ValheimOptimod.SSAOhighPrecision.Value;
            settings.ambientOnly = ValheimOptimod.SSAOambientOnly.Value;
            settings.downsampling = ValheimOptimod.SSAOdownSampling.Value;
            settings.farDistance = ValheimOptimod.SSAOfarDistance.Value;
            settings.sampleCount = (AmbientOcclusionModel.SampleCount)ValheimOptimod.SSAOsampleCount.Value;

            postProcessingBehaviour.profile.ambientOcclusion.settings = settings;
        }

        [HarmonyPatch(typeof(CameraEffects), "SetMotionBlur")]
        [HarmonyPostfix]
        private static void SetMotionBlur() {
            if (postProcessingBehaviour.profile.motionBlur == null)
                return;

            MotionBlurModel.Settings settings = postProcessingBehaviour.profile.motionBlur.settings;

            if (ValheimOptimod.printValues.Value) {
                Debug.Log("----MotionBlur settings----");
                Debug.Log($"FrameBlending: {settings.frameBlending} to {ValheimOptimod.MBframeBlending.Value}");
                Debug.Log($"SampleCount: {settings.sampleCount} to {ValheimOptimod.MBsampleCount.Value}");
                Debug.Log($"ShutterAngle: {settings.shutterAngle} to {ValheimOptimod.MBshutterAngle.Value}");
            }

            settings.frameBlending = ValheimOptimod.MBframeBlending.Value;
            settings.sampleCount = ValheimOptimod.MBsampleCount.Value;
            settings.shutterAngle = ValheimOptimod.MBshutterAngle.Value;

            postProcessingBehaviour.profile.motionBlur.settings = settings;
        }

        [HarmonyPatch(typeof(CameraEffects), "SetAntiAliasing")]
        [HarmonyPostfix]
        private static void SetAntiAliasing() {
            if (postProcessingBehaviour.profile.antialiasing == null)
                return;

            AntialiasingModel.Settings settings = postProcessingBehaviour.profile.antialiasing.settings;

            if (ValheimOptimod.printValues.Value) {
                Debug.Log("----AntiAliasing settings----");
                Debug.Log($"Method: {settings.method} to {(AntialiasingModel.Method)ValheimOptimod.AAmethod.Value}");
            }

            settings.method = (AntialiasingModel.Method)ValheimOptimod.AAmethod.Value;

            postProcessingBehaviour.profile.antialiasing.settings = settings;
        }

        [HarmonyPatch(typeof(CameraEffects), "SetCA")]
        [HarmonyPostfix]
        private static void SetCA() {
            if (postProcessingBehaviour.profile.chromaticAberration == null)
                return;

            ChromaticAberrationModel.Settings settings = postProcessingBehaviour.profile.chromaticAberration.settings;

            if (ValheimOptimod.printValues.Value) {
                Debug.Log("----ChromaticAberration settings----");
                Debug.Log($"Intensity: {settings.intensity} to {ValheimOptimod.CAintensity.Value}");
            }

            settings.intensity = ValheimOptimod.CAintensity.Value;

            postProcessingBehaviour.profile.chromaticAberration.settings = settings;
        }
    }
}
