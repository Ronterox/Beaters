//////////////////////////////////////////////////////
// MK Glow Settings Legacy	    	    	        //
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de                            //
// Copyright © 2020 All rights reserved.            //
//////////////////////////////////////////////////////

namespace MK.Glow.Legacy
{
    internal sealed class SettingsLegacy : Settings
    {
        public static implicit operator SettingsLegacy(MKGlow input)
        {
            var settings = new SettingsLegacy
            {
                allowComputeShaders = false,
                allowGeometryShaders = false,
                renderPriority = input.renderPriority,
                debugView = input.debugView,
                quality = input.quality,
                antiFlickerMode = input.antiFlickerMode,
                workflow = input.workflow,
                selectiveRenderLayerMask = input.selectiveRenderLayerMask,
                anamorphicRatio = input.anamorphicRatio,
                lumaScale = input.lumaScale,
                bloomThreshold = input.bloomThreshold,
                bloomScattering = input.bloomScattering,
                bloomIntensity = input.bloomIntensity,
                blooming = input.blooming,
                allowLensSurface = input.allowLensSurface,
                lensSurfaceDirtTexture = input.lensSurfaceDirtTexture,
                lensSurfaceDirtIntensity = input.lensSurfaceDirtIntensity,
                lensSurfaceDiffractionTexture = input.lensSurfaceDiffractionTexture,
                lensSurfaceDiffractionIntensity = input.lensSurfaceDiffractionIntensity,
                allowLensFlare = input.allowLensFlare,
                lensFlareStyle = input.lensFlareStyle,
                lensFlareGhostFade = input.lensFlareGhostFade,
                lensFlareGhostIntensity = input.lensFlareGhostIntensity,
                lensFlareThreshold = input.lensFlareThreshold,
                lensFlareScattering = input.lensFlareScattering,
                lensFlareColorRamp = input.lensFlareColorRamp,
                lensFlareChromaticAberration = input.lensFlareChromaticAberration,
                lensFlareGhostCount = input.lensFlareGhostCount,
                lensFlareGhostDispersal = input.lensFlareGhostDispersal,
                lensFlareHaloFade = input.lensFlareHaloFade,
                lensFlareHaloIntensity = input.lensFlareHaloIntensity,
                lensFlareHaloSize = input.lensFlareHaloSize
            };

            //Main

            //Bloom

            //LensSurface

            //LensFlare

            settings.SetLensFlarePreset(input.lensFlareStyle);

            //Glare
            settings.allowGlare = input.allowGlare;
            settings.glareBlend = input.glareBlend;
            settings.glareIntensity = input.glareIntensity;
            settings.glareThreshold = input.glareThreshold;
            settings.glareStreaks = input.glareStreaks;
            settings.glareScattering = input.glareScattering;
            settings.glareStyle = input.glareStyle;
            settings.glareAngle = input.glareAngle;

            //Sample0
            settings.glareSample0Scattering = input.glareSample0Scattering;
            settings.glareSample0Angle = input.glareSample0Angle;
            settings.glareSample0Intensity = input.glareSample0Intensity;
            settings.glareSample0Offset = input.glareSample0Offset;
            //Sample1
            settings.glareSample1Scattering = input.glareSample1Scattering;
            settings.glareSample1Angle = input.glareSample1Angle;
            settings.glareSample1Intensity = input.glareSample1Intensity;
            settings.glareSample1Offset = input.glareSample1Offset;
            //Sample2
            settings.glareSample2Scattering = input.glareSample2Scattering;
            settings.glareSample2Angle = input.glareSample2Angle;
            settings.glareSample2Intensity = input.glareSample2Intensity;
            settings.glareSample2Offset = input.glareSample2Offset;
            //Sample3
            settings.glareSample3Scattering = input.glareSample3Scattering;
            settings.glareSample3Angle = input.glareSample3Angle;
            settings.glareSample3Intensity = input.glareSample3Intensity;
            settings.glareSample3Offset = input.glareSample3Offset;

            settings.SetGlarePreset(input.glareStyle);

            return settings;
        }
    }
}
