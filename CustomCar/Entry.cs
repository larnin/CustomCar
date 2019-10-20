using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using System.Collections.Generic;
using System;
using Harmony;
using System.Reflection;
using Spectrum.API.Storage;
using UnityEngine;

namespace CustomCar
{
    public class Entry : IPlugin, IUpdatable
    {
        List<Assets> assets = new List<Assets>();

        public void Initialize(IManager manager, string ipcIdentifier)
        {
            try
            {
                var harmony = HarmonyInstance.Create("com.Larnin.CustomCar");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch(Exception e)
            {
                Console.Out.WriteLine(e.ToString());
            }

            
            try
            {
                CarInfos carInfos = new CarInfos();
                carInfos.collectInfos();
                CarBuilder builder = new CarBuilder();
                builder.createCars(carInfos);
            }
            catch(Exception e)
            {
                ErrorList.add("An error occured while trying to load cars assets");
                Console.Out.WriteLine(e.ToString());
            }

            ReloadProfiles();

            //disable loging cars
            //LogCarPrefabs.logCars();

            Events.MainMenu.Initialized.Subscribe(data =>
            {
                if (ErrorList.haveErrors())
                    ErrorList.show();
            });
        }

        int currentIndex = 0;

        public void Update()
        {
            //don't export anything
            //if(currentIndex < LogCarPrefabs.allTextures.Count)
            //{
            //    Texture t = LogCarPrefabs.allTextures[currentIndex];

            //    ObjExporter.saveTextureToFile(t, t.name + ".png");
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
        }

        void ReloadProfiles()
        {
            var profileManager = G.Sys.ProfileManager_;

            if (profileManager == null)
                return;

            var profiles = profileManager.GetType().GetField("profiles_", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(profileManager) as List<Profile>;

            LoadProfiles(profiles);

            //profileManager.GetType().GetField("profiles_", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(profileManager, new List<Profile>());

            //profileManager.GetType().GetMethod("LoadProfiles", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(profileManager, null);
        }

        void LoadProfiles(List<Profile> profiles)
        {
            List<string> filePathsInDirWithPattern = Resource.GetFilePathsInDirWithPattern(Resource.PersonalProfilesDirPath_, "*.xml", null);
            int index = 0;

            foreach (string current in filePathsInDirWithPattern)
            {
                if (profiles.Count <= index)
                    break;
                
                Profile profile = Profile.Load(current);
                if (profile == null)
                    continue;
                
                Profile baseProfile = profiles[index];
                var carColorList_ = profile.GetType().GetField("carColorsList_", BindingFlags.NonPublic | BindingFlags.Instance);
                carColorList_.SetValue(baseProfile, carColorList_.GetValue(profile));

                GameObject.Destroy(profile.gameObject);

                index++;
            }
        }
    }
}
