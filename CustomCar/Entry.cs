using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using System.Collections.Generic;
using System;
using HarmonyLib;
using System.Reflection;
using Spectrum.API.Storage;
using UnityEngine;

namespace CustomCar
{
    public class Entry : IPlugin, IUpdatable
    {
        public void Initialize(IManager manager, string ipcIdentifier)
        {
            try
            {
                var harmony = new Harmony("com.Larnin.CustomCar");
                
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch(Exception e)
            {
                Console.Out.WriteLine(e.ToString());
            }

            //AudioPlayer.Init();
            
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

            ModdedCarsColors.LoadAll();

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
            //AudioPlayer.Update();
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
    }

    
}
