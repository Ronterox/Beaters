//////////////////////////////////////////////////////
// MK Glow Camera Data Legacy	    	    	    //
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de                            //
// Copyright © 2020 All rights reserved.            //
//////////////////////////////////////////////////////

namespace MK.Glow.Legacy
{
    internal class CameraDataLegacy : CameraData
    {
        public static implicit operator CameraDataLegacy(UnityEngine.Camera input)
        {
            var data = new CameraDataLegacy
            {
                width = input.pixelWidth,
                height = input.pixelHeight,
                stereoEnabled = input.stereoEnabled,
                aspect = input.aspect,
                worldToCameraMatrix = input.worldToCameraMatrix
            };


            return data;
        }
    }
}
