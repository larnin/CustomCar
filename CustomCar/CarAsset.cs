using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;
using NAudio.Wave;
using System.IO;
using Spectrum.API.Configuration;
using Newtonsoft.Json.Linq;

namespace CustomCar
{
    public enum SoundType
    {
        Horn,
        Jump,
        Teleport,
        WingsOpen,
        WingsClose,
        Impact,
        GravityToggled,
        BoostBurst,
        BoostRelease,
        BoostLoop,
        BoostCooldown,
        BoostServo,
    }

    public class CarSettings
    {
        public class SoundInfo
        {
            public bool disableDefault;
            public string filename;
        }

        public string carBundlebName;
        public string carName;
        public SoundInfo[] sounds;

        public CarSettings()
        {
            sounds = new SoundInfo[Enum.GetValues(typeof(SoundType)).Length];
            for(int i = 0; i < sounds.Length; i++)
            {
                sounds[i] = new SoundInfo();
                sounds[i].disableDefault = false;
                sounds[i].filename = ((SoundType)(i)).ToString();
            }
            carBundlebName = "";
            carName = "";
        }

        public static CarSettings LoadFromZipEntry(ZipArchiveEntry entry)
        {
            var stream = new StreamReader(entry.Open(), Encoding.Default);
            return LoadFromJson(stream.ReadToEnd());
        }

        public static CarSettings LoadFromJson(string str)
        {
            var settings = new CarSettings();

            var json = JObject.Parse(str);

            var jCar = json.GetValue("Car");
            if(jCar != null && jCar.Type == JTokenType.Object)
            {
                var jObjCar = (JObject)jCar;

                var jFile = jObjCar.GetValue("File");
                var jName = jObjCar.GetValue("Name");

                if (jFile != null && jFile.Type == JTokenType.String)
                    settings.carName = jFile.ToString();

                if (jName != null && jName.Type == JTokenType.String)
                    settings.carName = jFile.ToString();
            }

            var jSounds = json.GetValue("Sounds");
            if(jSounds != null && jSounds.Type == JTokenType.Object)
            {
                var jObjSounds = (JObject)jSounds;

                foreach (var s in Enum.GetValues(typeof(SoundType)))
                {
                    var jSound = jObjSounds.GetValue(s.ToString());

                    var sound = settings.sounds[(int)s];

                    if(jSound != null && jSound.Type == JTokenType.Object)
                    {
                        var jObjSound = (JObject)jSound;

                        var jFile = jObjSound.GetValue("File");
                        var jDisableDefault = jObjSound.GetValue("DisableDefault");

                        if (jFile != null && jFile.Type == JTokenType.String)
                            sound.filename = jFile.ToString();

                        if (jDisableDefault != null && jDisableDefault.Type == JTokenType.Boolean)
                            sound.disableDefault = (bool)jDisableDefault;
                    }
                }
            }

            return settings;
        }
    }

    public class CarAsset
    {
        class SoundInfo
        {
            public bool disableDefault = false;

        }
        
        AssetBundle m_bundle;
        SoundInfo[] m_sounds;
        
        CarAsset()
        {
            m_sounds = new SoundInfo[Enum.GetValues(typeof(SoundType)).Length];
            for (int i = 0; i < m_sounds.Length; i++)
                m_sounds[i] = new SoundInfo();
        }

        public bool DisableDefault(SoundType type)
        {
            return m_sounds[(int)type].disableDefault || SoundExist(type);
        }

        public bool SoundExist(SoundType type)
        {
            return false;
        }

        public void PlaySound(SoundType type, float volume, bool playLoop)
        {

        }

        public void StopSound(SoundType type)
        {

        }

        public void UpdateBoostState(bool boosting)
        {

        }

        public static CarAsset CreateFromPrefab(string filename)
        {
            CarAsset asset = new CarAsset();
            asset.m_bundle = AssetBundle.LoadFromFile(filename);

            //nothing more to do on soundInfo

            return asset;
        }

        public static CarAsset CreateFromZip(string filename)
        {
            CarAsset asset = new CarAsset();

            using (FileStream zipFile = new FileStream(filename, FileMode.Open))
            {
                using (ZipArchive zip = new ZipArchive(zipFile, ZipArchiveMode.Read))
                {
                    var settingsEntry = zip.GetEntry("settings.json");
                    CarSettings settings = null;
                    if (settingsEntry == null)
                        settings = new CarSettings();
                    else settings = CarSettings.LoadFromZipEntry(settingsEntry);

                    asset.LoadCarAsset(zip, settings);

                    asset.LoadSoundAssets(zip, settings);
                }
            }

            return asset;
        }

        public static CarAsset CreateFromAudioZip(string filename)
        {
            CarAsset asset = new CarAsset();

            using (FileStream zipFile = new FileStream(filename, FileMode.Open))
            {
                using (ZipArchive zip = new ZipArchive(zipFile, ZipArchiveMode.Read))
                {
                    var settingsEntry = zip.GetEntry("settings.json");
                    CarSettings settings = null;
                    if (settingsEntry == null)
                        settings = new CarSettings();
                    else settings = CarSettings.LoadFromZipEntry(settingsEntry);
                    
                    asset.LoadSoundAssets(zip, settings);
                }
            }

            return asset;
        }

        void LoadCarAsset(ZipArchive zip, CarSettings settings)
        {
            ZipArchiveEntry carEntry = null;
            foreach(var e in zip.Entries)
            {
                var name = e.Name;
                if(settings.carBundlebName != "" && name == settings.carBundlebName)
                {
                    carEntry = e;
                    break;
                }
                if(settings.carBundlebName == "")
                {
                    int index = name.IndexOf('.');
                    if(index < 0)
                    {
                        carEntry = e;
                        break;
                    }
                }
            }

            if(carEntry == null)
            {
                //log error
                return;
            }

            var stream = carEntry.Open();
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                bytes = ms.ToArray();
            }

            m_bundle = AssetBundle.LoadFromMemory(bytes);
        }

        void LoadSoundAssets(ZipArchive zip, CarSettings settings)
        {
            for(int i = 0; i < settings.sounds.Length; i++)
            {
                m_sounds[i].disableDefault = settings.sounds[i].disableDefault;
                //todo more
            }
        }
    }
}
