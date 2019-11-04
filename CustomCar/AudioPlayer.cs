﻿using NAudio.Wave;
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
        static BufferedWaveProvider m_provider;

        public static void Init()
        {
            var asset = new Assets("Sound/noot");

            foreach (var n in asset.Bundle.GetAllAssetNames())
            {
                if (!n.EndsWith(".prefab"))
                    continue;
                var obj = asset.Bundle.LoadAsset<GameObject>(n);
                if (obj == null)
                    continue;
                var source = obj.GetComponentInChildren<AudioSource>();
                if (source == null)
                    continue;
                
                var clip = source.clip;
                if (clip == null)
                    continue;

                float[] data = new float[clip.samples * clip.channels];
                clip.GetData(data, data.Length);

                m_provider = new BufferedWaveProvider(new WaveFormat(48000, 16, 1));
                var samples = GetSamplesWaveData(data, data.Length);
                m_provider.AddSamples(samples, 0, samples.Length);

                m_outputDevice = new WaveOutEvent()
                {
                    NumberOfBuffers = 10,
                    DesiredLatency = 85
                };
                m_outputDevice.Init(m_provider);
            }
        }

        public static void Play()
        {
            m_outputDevice.Play();
        }

        public static byte[] GetSamplesWaveData(float[] samples, int samplesCount)
        {
            var pcm = new byte[samplesCount * 2];
            int sampleIndex = 0,
                pcmIndex = 0;

            while (sampleIndex < samplesCount)
            {
                var outsample = (short)(samples[sampleIndex] * short.MaxValue);
                pcm[pcmIndex] = (byte)(outsample & 0xff);
                pcm[pcmIndex + 1] = (byte)((outsample >> 8) & 0xff);

                sampleIndex++;
                pcmIndex += 2;
            }

            return pcm;
        }
    }
}
