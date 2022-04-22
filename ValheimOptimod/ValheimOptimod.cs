using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace UnrealByte {
    [BepInPlugin("com.unrealbyte.valheimoptimod", "Valheim Optimod", "0.2.0")]
    [BepInProcess("valheim.exe")]
    public class ValheimOptimod : BaseUnityPlugin {
        private readonly Harmony harmony = new Harmony("com.unrealbyte.valheimoptimod");

		#region Settings
		public static ConfigEntry<bool> modEnabled { get; private set; }
		public static ConfigEntry<bool> printValues { get; private set; }
		public static ConfigEntry<int> anisotropicFiltering { get; private set; }
		public static ConfigEntry<int> masterTextureLimit { get; private set; }
		public static ConfigEntry<int> maxQueuedFrames { get; private set; }
		public static ConfigEntry<int> particleRaycastBudget { get; private set; }
		public static ConfigEntry<int> pixelLightCount { get; private set; }
		public static ConfigEntry<bool> realtimeReflectionProbes { get; private set; }
		public static ConfigEntry<int> skinWeights { get; private set; }
		public static ConfigEntry<bool> softParticles { get; private set; }
		public static ConfigEntry<bool> softVegetation { get; private set; }
		public static ConfigEntry<float> lodBias { get; private set; }
		public static ConfigEntry<int> shadowResolution { get; private set; }
		public static ConfigEntry<int> shadowCascades { get; private set; }
		public static ConfigEntry<float> shadowDistance { get; private set; }
		public static ConfigEntry<int> shadows { get; private set; }
		public static ConfigEntry<bool> disablePostProcessing { get; private set; }
		public static ConfigEntry<float> lensDirtIntensity { get; private set; }
		public static ConfigEntry<float> sunShaftsIntensity { get; private set; }
		public static ConfigEntry<int> sunShaftsResolution { get; private set; }
		public static ConfigEntry<int> sunShaftsScreenBlendMode { get; private set; }
		public static ConfigEntry<float> sunShaftsMaxRadius { get; private set; }
		public static ConfigEntry<float> sunShaftsBlurRadius { get; private set; }
		public static ConfigEntry<int> sunShaftsRadialBlurIterations { get; private set; }
		public static ConfigEntry<bool> bloomAntiFlicker { get; private set; }
		public static ConfigEntry<float> bloomRadious { get; private set; }
		public static ConfigEntry<float> bloomSoftKnee { get; private set; }
		public static ConfigEntry<float> bloomIntensity { get; private set; }
		public static ConfigEntry<Color> gammaCorrection { get; private set; }
		public static ConfigEntry<float> SSAOintensity { get; private set; }
		public static ConfigEntry<bool> SSAOhighPrecision { get; private set; }
		public static ConfigEntry<bool> SSAOambientOnly { get; private set; }
		public static ConfigEntry<bool> SSAOdownSampling { get; private set; }
		public static ConfigEntry<float> SSAOfarDistance { get; private set; }
		public static ConfigEntry<int> SSAOsampleCount { get; private set; }
		public static ConfigEntry<float> MBframeBlending { get; private set; }
		public static ConfigEntry<int> MBsampleCount { get; private set; }
		public static ConfigEntry<float> MBshutterAngle { get; private set; }
		public static ConfigEntry<int> AAmethod { get; private set; }
		public static ConfigEntry<float> CAintensity { get; private set; }
		public static ConfigEntry<int> maxFps { get; private set; }
		public static ConfigEntry<int> vSyncCount { get; private set; }
		public static ConfigEntry<string> reloadKey { get; private set; }
		public static ConfigEntry<bool> fog { get; private set; }
		public static ConfigEntry<Color> fogColor { get; private set; }
		public static ConfigEntry<float> fogDensity { get; private set; }
		public static ConfigEntry<float> fogStartDistance { get; private set; }
		public static ConfigEntry<float> fogEndDistance { get; private set; }
		public static ConfigEntry<int> fogMode { get; private set; }
		public static ConfigEntry<int> smoke { get; private set; }
		public static ConfigEntry<int> activeZones { get; private set; }
		public static ConfigEntry<int> loadedZones { get; private set; }
		public static ConfigEntry<int> generatedZones { get; private set; }

		#endregion

		void Awake() {

			Debug.Log("[ValheimOptimod] - System Specs");
			Debug.Log("[ValheimOptimod] - Processors: " + SystemInfo.processorCount + " " + SystemInfo.processorType + " " + SystemInfo.processorFrequency);
			Debug.Log("[ValheimOptimod] - Memory: " + SystemInfo.systemMemorySize);
			Debug.Log("[ValheimOptimod] - Graphics Mem: " + SystemInfo.graphicsMemorySize);
			Debug.Log("[ValheimOptimod] - SO: " + SystemInfo.operatingSystem);
			LoadValues();

			if (!modEnabled.Value)
				return;

			//harmony.PatchAll(typeof(HiddenSettings));
			//harmony.PatchAll(typeof(PostProcessing));
			//harmony.PatchAll(typeof(Main));
			harmony.PatchAll();

			Config.ConfigReloaded += OnReloaded;
		}

		private void OnReloaded(object sender, System.EventArgs e) {
			PostProcessing.OnReload();
			HiddenSettings.OnReload();
		}

		private void LoadValues() {
			//ModConfig
			modEnabled = Config.Bind("General", "Enabled", true, "Enable this mod");
			printValues = Config.Bind("General", "PrintDefaultValues", false, "Whether to print the game's graphics configuration before the mod loads.");
			reloadKey = Config.Bind("General", "ReloadKey", "home", "Key used to reload mod config file.");
			//General graphics config
			anisotropicFiltering = Config.Bind("General", "AnisotropicFiltering", 2, "Set Anisotropic Filtering. 0 - Disabled, 1 - Enabled, 2 - Forced");
			masterTextureLimit = Config.Bind("General", "MasterTextureLimit", 0, "A texture size limit applied to all textures. 1 will halve texture size, 2 will quarter it, and so on. 0 disables this setting.");
			maxQueuedFrames = Config.Bind("General", "MaxQueuedFrames", 2, "Incrising it can smother the fps a little, but can introduce input lag. Ignored when using Vulkan");
			particleRaycastBudget = Config.Bind("General", "ParticleRaycastBudget", 4096, "Budget for how many ray casts can be performed per frame for approximate collision testing.");
			pixelLightCount = Config.Bind("General", "PixelLightCount", 8, "The maximum number of pixel lights that should affect any object.");
			realtimeReflectionProbes = Config.Bind("General", "RealtimeReflectionProbes", true, "Enables realtime reflection probes. If disabled, there will be no reflections in the game.");
			skinWeights = Config.Bind("General", "SkinWeights", 2, "The maximum number of bone weights that can affect a vertex, for all skinned meshes in the project. 0 - One bone, 1 - Two bones, 2 - Four bones, 3 - Unlimited");
			softParticles = Config.Bind("General", "SoftParticles", true, "Whether soft blending should be used for particles");
			softVegetation = Config.Bind("General", "SoftVegetation", true, "If enabled, vegetation will have smoothed edges; if disabled all plants will have hard edges but are rendered roughly twice as fast.");
			lodBias = Config.Bind("General", "LodBias", 5f, "Global multiplier for the LOD's switching distance. A larger value leads to a longer view distance before a lower resolution LOD is picked.");
			vSyncCount = Config.Bind("General", "vSyncCount", 0, "Set if vSync shoud be used. 0 is disabled and 1 is enabled. You also can use 2, 3 or 4 to enable vSync but cut the max FPS. Example:if your screen is 60Hz using 2 as vSync will max fps at 30, if is set to 3 it will be 20 fps and if vSync is 4 it will be at max 15.");
			maxFps = Config.Bind("General", "MaxFPS", -1, "Set the target fps. Use -1 to set the FPS based on screen refresh rate. This setting only works if vSyncCount is 0.");

			//Shadows
			shadowResolution = Config.Bind("Shadows", "ShadowResolution", 2, "0 - Low, 1 - Medium, 2 - High, 3 - VeryHigh");
			shadowCascades = Config.Bind("Shadows", "ShadowCascades", 4, "Number of cascades to use for directional light shadows.");
			shadowDistance = Config.Bind("Shadows", "ShadowDistance", 150f, "Shadow drawing distance.");
			shadows = Config.Bind("Shadows", "Shadows", 1, "Shadow quality: 0 - Disabled, 1 - Only hard shadows, 2 - All");

			//PostProcessing
			disablePostProcessing = Config.Bind("PostProcessing", "DisablePostProcessing", false, "Disable all post-processing effects. (Includes SSAO and antialiasing)");
			fog = Config.Bind("PostProcessing", "Fog", true, "");
			sunShaftsResolution = Config.Bind("PostProcessing", "SunShaftsResolution", 1, "Changes SunShafts resolution. 0 - Low, 1 - Normal, 2 - High");
			sunShaftsScreenBlendMode = Config.Bind("PostProcessing", "SunShaftsScreenBlendMode", 0, "Changes SunShafts blending metod. 0 - Screen, 1 - Add (fastest)");
			bloomAntiFlicker = Config.Bind("PostProcessing", "BloomAntiFlicker", true, "Reduces flashing noise with an additional filter");
			SSAOdownSampling = Config.Bind("PostProcessing", "AOdownSampling", false, "Reduces the effect quality to gain some performance");
			SSAOfarDistance = Config.Bind("PostProcessing", "AOfarDistance", 150f, "How far the effect is applied in the screen");
			SSAOsampleCount = Config.Bind("PostProcessing", "AOsampleCount", 10, "Lowest = 3, Low = 6, Medium = 10, High = 16");
			SSAOhighPrecision = Config.Bind("PostProcessing", "AOhighPrecision", false, "Increase effect quality at cost of performance");
			AAmethod = Config.Bind("PostProcessing", "AA_Method", 1, "Fxaa - 0, Taa - 1");

			//Flavors
			bloomIntensity = Config.Bind("Flavor", "BloomIntensity", 0.3f, "Strength of the Bloom filter");
			bloomRadious = Config.Bind("Flavor", "BloomRadious", 5f, "Changes extent of bloom veiling effects");
			lensDirtIntensity = Config.Bind("Flavor", "LensDirtIntensity", 10.4f, "Amount of bloom lens dirtiness");
			sunShaftsIntensity = Config.Bind("Flavor", "SunShaftsIntensity", 1f, "The brightness of the sun shafts");
			gammaCorrection = Config.Bind("Flavor", "GammaCorrection", new Color(1f, 1f, 1f, 0f), "Hexadecimal color to apply over screen. Format RRGGBBAA, with AA being the intensity.");
			SSAOintensity = Config.Bind("Flavor", "AOintensity", 1f, "Adjust the degree of darkness Ambient Occlusion produces.");
			SSAOambientOnly = Config.Bind("Flavor", "AOambientOnly", false, "Make the Ambient Occlusion effect only affect ambient lighting.");
			CAintensity = Config.Bind("Flavor", "ChromaticAberrationIntensity", 0.15f, "Strength of chromatic aberrations");
			fogColor = Config.Bind("Flavor", "FogColor", new Color(0.487f, 0.570f, 0.669f, 1f), "The color of the fog.");
			fogDensity = Config.Bind("Flavor", "FogDensity", 0.1f, "The density of the exponential fog.");
			fogStartDistance = Config.Bind("Flavor", "FogStartDistance", 300f, "The starting distance of linear fog.");
			fogEndDistance = Config.Bind("Flavor", "FogEndDistance", 900f, "The ending distance of linear fog.");

			//Advanced
			sunShaftsMaxRadius = Config.Bind("Advanced", "SunShaftsMaxRadius", 0.4f, "How far the sun shafts go");
			sunShaftsBlurRadius = Config.Bind("Advanced", "SunShaftsBlurRadius", 3f, "The radius over which pixel colours are combined during blurring.");
			sunShaftsRadialBlurIterations = Config.Bind("Advanced", "SunShaftsRadialBlurIterations", 3, "Increase to get smoother blurring at performance cost or decrease to get a fast and sharper effect");
			bloomSoftKnee = Config.Bind("Advanced", "BloomSoftKnee", 0.7f, "Makes transition between under/over-threshold gradual (0 = hard threshold, 1 = soft threshold)");
			MBframeBlending = Config.Bind("Advanced", "MotionBlurFrameBlending", 0f, "");
			MBsampleCount = Config.Bind("Advanced", "MotionBlurSampleCount", 10, "Set the value for the amount of sample points. Less is faster, more is smother");
			MBshutterAngle = Config.Bind("Advanced", "MotionBlurShutterAngle", 150f, "Set the angle of the rotary shutter. Larger values give longer exposure and a stronger blur effect.");
			fogMode = Config.Bind("Flavor", "FogMode", 2, "1 - Linear, 2 - Exponential, 3 - ExponentialSquared");
			smoke = Config.Bind("Flavor", "Smoke", 1, "1=True, 0=False");

			//Zones
			generatedZones = Config.Bind("Zones", "GeneratedZones", 5, "Default 5. Minimum value is LoadedZones");
			loadedZones = Config.Bind("Zones", "LoadedZones", 3, "Default 3. Minimum value is ActiveZones");
			activeZones = Config.Bind("Zones", "ActiveZones", 2, "Default 2. Minimum value is 1");
			
		}
	}
}