using NAudio.Wave;
using Spectrum.API;
using Spectrum.API.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace CustomCar
{
    public static class AudioPlayer
    {
        static WaveOutEvent m_outputDevice;
        static AudioFileReader m_audioFile;

        public static void Init()
        {
            var asset = new Assets("Sound/noot");

            foreach (var n in asset.Bundle.GetAllAssetNames())
            {
                var clip = asset.Bundle.LoadAsset<AudioClip>(n);
                
            }
        }
        
    }
}
