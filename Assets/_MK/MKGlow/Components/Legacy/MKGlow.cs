//////////////////////////////////////////////////////
// MK Glow 	    	    	                        //
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de                            //
// Copyright © 2020 All rights reserved.            //
//////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.Rendering;

namespace MK.Glow.Legacy
{
	#if UNITY_2018_3_OR_NEWER
        [ExecuteAlways]
    #else
        [ExecuteInEditMode]
    #endif
    [DisallowMultipleComponent]
    [ImageEffectAllowedInSceneView]
    [RequireComponent(typeof(Camera))]
	public class MKGlow : MonoBehaviour
	{
        #if UNITY_EDITOR && !UNITY_CLOUD_BUILD
        public bool showEditorMainBehavior = true;
		public bool showEditorBloomBehavior;
		public bool showEditorLensSurfaceBehavior;
		public bool showEditorLensFlareBehavior;
		public bool showEditorGlareBehavior;
        #endif

        //Main
        public bool allowGeometryShaders = true;
        public bool allowComputeShaders = true;
        public RenderPriority renderPriority = RenderPriority.Balanced;
        public DebugView debugView = DebugView.None;
        public Quality quality = Quality.High;
        public AntiFlickerMode antiFlickerMode = AntiFlickerMode.Balanced;
        public Workflow workflow = Workflow.Threshold;
        public LayerMask selectiveRenderLayerMask = -1;
        [Range(-1f, 1f)]
        public float anamorphicRatio;
        [Range(0f, 1f)]
        public float lumaScale = 0.5f;
        [Range(0f, 1f)]
		public float blooming;

        //Bloom
        [MinMaxRange(0, 10)]
        public MinMaxRange bloomThreshold = new MinMaxRange(1.25f, 10f);
        [Range(1f, 10f)]
		public float bloomScattering = 7f;
		public float bloomIntensity = 1f;

        //LensSurface
        public bool allowLensSurface;
		public Texture2D lensSurfaceDirtTexture;
		public float lensSurfaceDirtIntensity = 2.5f;
		public Texture2D lensSurfaceDiffractionTexture;
		public float lensSurfaceDiffractionIntensity = 2.0f;

        //LensFlare
        public bool allowLensFlare;
        public LensFlareStyle lensFlareStyle = LensFlareStyle.Average;
        [Range(0f, 25f)]
		public float lensFlareGhostFade = 10.0f;
		public float lensFlareGhostIntensity = 1.0f;
        [MinMaxRange(0, 10)]
		public MinMaxRange lensFlareThreshold = new MinMaxRange(1.3f, 10f);
        [Range(0f, 8f)]
		public float lensFlareScattering = 5f;
		public Texture2D lensFlareColorRamp;
        [Range(-100f, 100f)]
		public float lensFlareChromaticAberration = 53f;
        [Range(1, 4)]
		public int lensFlareGhostCount = 3;
        [Range(-1f, 1f)]
		public float lensFlareGhostDispersal = 0.6f;
        [Range(0f, 25f)]
		public float lensFlareHaloFade = 2f;
		public float lensFlareHaloIntensity = 1.0f;
        [Range(0f, 1f)]
		public float lensFlareHaloSize = 0.4f;

        //Glare
        public bool allowGlare;
        [Range(0.0f, 1.0f)]
        public float glareBlend = 0.33f;
        public float glareIntensity = 1f;
        [Range(0.0f, 360.0f)]
        public float glareAngle;
        [MinMaxRange(0, 10)]
        public MinMaxRange glareThreshold = new MinMaxRange(1.25f, 10f);
        [Range(1, 4)]
        public int glareStreaks = 4;
        public GlareStyle glareStyle = GlareStyle.DistortedCross;
        [Range(0.0f, 4.0f)]
        public float glareScattering = 2f;
        //Sample0
        [Range(0f, 10f)]
        public float glareSample0Scattering = 5f;
        [Range(0f, 360f)]
        public float glareSample0Angle;
        public float glareSample0Intensity = 1f;
        [Range(-5f, 5f)]
        public float glareSample0Offset;
        //Sample1
        [Range(0f, 10f)]
        public float glareSample1Scattering = 5f;
        [Range(0f, 360f)]
        public float glareSample1Angle = 45f;
        public float glareSample1Intensity = 1f;
        [Range(-5f, 5f)]
        public float glareSample1Offset;
        //Sample0
        [Range(0f, 10f)]
        public float glareSample2Scattering = 5f;
        [Range(0f, 360f)]
        public float glareSample2Angle = 90f;
        public float glareSample2Intensity = 1f;
        [Range(-5f, 5f)]
        public float glareSample2Offset;
        //Sample0
        [Range(0f, 10f)]
        public float glareSample3Scattering = 5f;
        [Range(0f, 360f)]
        public float glareSample3Angle = 135f;
        public float glareSample3Intensity = 1f;
        [Range(-5f, 5f)]
        public float glareSample3Offset;

        private Effect _effect;

        private RenderTarget _source, _destination;

		private Camera renderingCamera;

		private void Start()
		{
			if (!renderingCamera) renderingCamera = GetComponent<Camera>();
		}

		/// <summary>
        /// Load some mobile optimized settings
        /// </summary>
        [ContextMenu("Load Preset For Mobile")]
        private void LoadMobilePreset()
        {
            bloomScattering = 5f;
            renderPriority = RenderPriority.Performance;
            quality = Quality.Low;
            allowGlare = false;
            allowLensFlare = false;
            lensFlareScattering = 5;
            allowLensSurface = false;
        }

        /// <summary>
        /// Load some quality optimized settings
        /// </summary>
        [ContextMenu("Load Preset For Quality")]
        private void LoadQualityPreset()
        {
            bloomScattering = 7f;
            renderPriority = RenderPriority.Quality;
            quality = Quality.High;
            allowGlare = false;
            allowLensFlare = false;
            lensFlareScattering = 6;
            allowLensSurface = false;
        }

		public void OnEnable()
		{
            _effect = new Effect();
			_effect.Enable(RenderPipeline.Legacy);

            enabled = Compatibility.IsSupported;
		}

		public void OnDisable() => _effect.Disable();

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if(workflow == Workflow.Selective && (GraphicsSettings.renderPipelineAsset || PipelineProperties.xrEnabled))
            {
                Graphics.Blit(source, destination);
                return;
            }

            _source.renderTexture = source;
            _destination.renderTexture = destination;
            SettingsLegacy settings = this;
            CameraDataLegacy cameraData = renderingCamera;
			_effect.Build(_source, _destination, settings, null, cameraData, renderingCamera, false);

            Graphics.Blit(source, destination, _effect.renderMaterialNoGeometry, _effect.currentRenderIndex);
            _effect.AfterCompositeCleanup();
        }
	}
}