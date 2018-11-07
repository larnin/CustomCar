using Spectrum.API;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Spectrum.API.Configuration;
using System.IO;
using Harmony;
using System.Reflection;

namespace CustomCar
{
    public static class Constants
    {
        public const int carNb = 10;
    }

    public class Configs
    {
        public string carName;
        public Configs()
        {
            var settings = new Settings("CustomCar");

            var entries = new Dictionary<string, string>
            {
                {"CarName", "Random" },
            };

            foreach (var s in entries)
                if (!settings.ContainsKey(s.Key))
                    settings.Add(s.Key, s.Value);

            settings.Save();

            carName = (string)settings["CarName"];
        }
    }

    public class Entry : IPlugin, IUpdatable
    {
        public void Initialize(IManager manager, string ipcIdentifier)
        {
            LogCarPrefabs.logCars();

            var harmony = HarmonyInstance.Create("com.Larnin.CustomCar");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            var profileManager = G.Sys.ProfileManager_;
            var oldCars = profileManager.carInfos_.ToArray();
            profileManager.carInfos_ = new CarInfo[Constants.carNb];

            var unlocked = (Dictionary<string, int>)profileManager.GetType().GetField("unlockedCars_", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(profileManager);

            for (int i = 0; i < profileManager.carInfos_.Length; i++)
            {
                if (i < oldCars.Length)
                {
                    profileManager.carInfos_[i] = oldCars[i];
                    continue;
                }
                var car = new CarInfo();
                car.name_ = "Wtf is that " + i + " ?";
                car.prefabs_ = oldCars[0].prefabs_;
                car.prefabs_.screenPrefab_ = null;
                car.colors_ = oldCars[0].colors_;
                profileManager.carInfos_[i] = car;
                unlocked.Add(car.name_, i);
            }

            var carColors = new CarColors[Constants.carNb];
            for (int i = 0; i < carColors.Length; i++)
                carColors[i] = G.Sys.ProfileManager_.carInfos_[i].colors_;

            for (int i = 0; i < profileManager.ProfileCount_; i++)
            {
                Profile p = profileManager.GetProfile(i);

                var field = p.GetType().GetField("carColorsList_", BindingFlags.Instance | BindingFlags.NonPublic);
                field.SetValue(p, carColors);
            }
        }
        
        int currentIndex = 0;

        public void Update()
        {
            //if(currentIndex < LogCarPrefabs.allTextures.Count)
            //{
            //    Texture t = LogCarPrefabs.allTextures[currentIndex];

            //    LogCarPrefabs.saveTextureToFile(t, t.name + ".png");
            //    Console.Out.WriteLine("saved texture " + t.name);
            //    currentIndex++;
            //}

            //if (currentIndex < LogCarPrefabs.allMeshs.Count)
            //{
            //    Mesh m = LogCarPrefabs.allMeshs[currentIndex];

            //    ObjExporter.MeshToFile(m, Application.dataPath + "/exportedMeshs/" + m.name + ".obj");
            //    Console.Out.WriteLine("saved obj " + m.name);
            //    currentIndex++;
            //}

            if (Input.GetKeyDown("j"))
                startNextAnimation();
        }

        static int animationIndex = 0;
        static void startNextAnimation()
        {
            List<string> animations = new List<string> { "BaseAnimation", "BoostOpen", "JetOpen", "WingsOpen", "Assemble", "Overheat"};

            var cars = UnityEngine.Object.FindObjectsOfType<CarVisuals>();

            foreach(var c in cars)
            {
                var a = c.GetComponentInChildren<Animation>();
                var clip = a[animations[animationIndex]];
                if (clip == null)
                    a.Stop();
                else
                {
                    clip.clip.wrapMode = WrapMode.Loop;
                    a.clip = clip.clip;
                    a.Stop();
                    a.Play();
                }
            }

            Console.Out.WriteLine("Playing " + animations[animationIndex]);

            animationIndex++;
            if (animationIndex >= animations.Count)
                animationIndex = 0;
        }
    }
}
