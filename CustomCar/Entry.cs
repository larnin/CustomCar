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
using Events.Car;
using Events.LocalClient;
using Events;
using Events.Stunt;
using Events.Local;
using Events.ClientToAllClients;
using Spectrum.API.Experimental;

namespace CustomCar
{
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

    public class Entry : IPlugin
    {
        const string carBaseName = "Assets/Prefabs/NitronicCycle.prefab";

        static System.Random rnd = new System.Random();
        static Configs configs;
        static Spectrum.API.Logging.Logger logger;
        static Assets assets;
        static GameObject car;

        static Mesh carMesh;
        static Texture2D carDiffuse;
        static Texture2D carEmissive;
        static Texture2D carNormal;
        static Material carMat;

        public void Initialize(IManager manager, string ipcIdentifier)
        {
            //Console.Out.WriteLine(Application.unityVersion);

            configs = new Configs();
            logger = new Spectrum.API.Logging.Logger("CustomCar");

            var harmony = HarmonyInstance.Create("com.Larnin.CustomCar");
            harmony.PatchAll(Assembly.GetExecutingAssembly());


            assets = new Assets("cars");
            loadCar();
        }

        [HarmonyPatch(typeof(CarLogic), "Awake")]
        internal class CarLogicAwake
        {

            static void Postfix(CarLogic __instance)
            {
                Entry.logger.WriteLine("\n\n---------------------------------\n" + __instance.gameObject.name + "\n---------------------------------\n\n");
                Entry.logCurrentObjectAndChilds(__instance.gameObject, true);

                //changeCar(__instance.gameObject);

                try
                {
                    var asset = GameObject.Instantiate(car);
                    asset.transform.parent = __instance.transform;
                    asset.transform.localPosition = Vector3.zero;
                    asset.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    asset.SetLayerRecursively(__instance.gameObject.GetLayer());
                    logger.WriteLine(asset.transform.parent.name);
                    var renderer = asset.GetComponentInChildren<MeshRenderer>();
                    renderer.material = carMat;

                    /*var mat = new Material(Shader.Find("Diffuse"));

                    foreach (var renderer in GameObject.FindObjectsOfType<Renderer>())
                    {
                        renderer.material = mat;
                    }*/

                }
                catch (Exception e)
                {
                    Console.Out.WriteLine(e.Message);
                    Console.Out.WriteLine(e.ToString());
                }
            }
        }
        
        static void logCurrentObjectAndChilds(GameObject obj, bool hideStuff, int level = 0)
        {
            const int offsetPerLevel = 2;
            string offset = new string(' ', level * offsetPerLevel);
            
            logger.WriteLine(offset + obj.GetType().Name + " - " + obj.name + " - " + obj.activeSelf + " - " + obj.transform.localPosition + " - " + obj.transform.localRotation.eulerAngles + " - " + obj.transform.localScale + " - " + obj.layer);
            foreach (var comp in obj.GetComponents(typeof(Component)))
            {
                string text = offset + "   | " + comp.GetType().Name;
                var behaviour = comp as Behaviour;
                if (behaviour != null)
                {
                    text += " - " + behaviour.enabled;
                }
                var renderer = comp as MeshRenderer;
                if(renderer != null)
                {
                    foreach(var m in renderer.materials)
                    {
                        text += logMaterial(m, level);
                    }
                }
                var skinnedRenderer = comp as SkinnedMeshRenderer;
                if (skinnedRenderer != null)
                {
                    foreach (var m in skinnedRenderer.materials)
                    {
                        text += logMaterial(m, level);
                    }
                }
                var filter = comp as MeshFilter;
                if(filter != null)
                {
                    text += " - " + filter.mesh.name + " - " + filter.mesh.GetIndexCount(0) + " - " + filter.mesh.GetTriangles(0).Length;
                }
                logger.WriteLine(text);
                if(hideStuff)
                    disableSpecificObject(comp);
            }
            for (int i = 0; i < obj.transform.childCount; i++)
                logCurrentObjectAndChilds(obj.transform.GetChild(i).gameObject, hideStuff, level + 1);
        }

        static void disableSpecificObject(Component comp)
        {
            var renderer = comp as MeshRenderer;
            var skinnedRenderer = comp as SkinnedMeshRenderer;
            if(renderer != null)
            {
                //if (renderer.name.Contains("Wheel") || renderer.name.Contains("HubVisual") || renderer.name.Contains("JetFlame") || renderer.name.Contains("CarCrossSection") || renderer.name.Contains("Refractor"))
                    renderer.enabled = false;
            }
            if(skinnedRenderer != null)
            {
                if (skinnedRenderer.gameObject.name == "Refractor")
                {
                    carMat = skinnedRenderer.materials[2];
                    carMat.SetTexture(5, carDiffuse);
                    carMat.SetTexture(242, carEmissive);
                    carMat.SetVector(159, new Vector4(1, -1, 0, 0));
                    carMat.SetVector(160, new Vector4(1, -1, 0, 0));
                    carMat.SetVector(393, new Vector4(1, -1, 0, 0));
                    carMat.SetVector(401, new Vector4(1, -1, 0, 0));
                    carMat.SetVector(403, new Vector4(1, -1, 0, 0));
                }
                skinnedRenderer.enabled = false;
            }
        }

        static string logMaterial(Material m, int level)
        {
            const int offsetPerLevel = 2;
            string text = "\n" + new string(' ', level * offsetPerLevel) + "     * " + m.name + " " + m.shader.name + "\n";

            for (int i = 0; i < 10000; i++)
            {
                if (m.HasProperty(i))
                {
                    text += i ;
                    text += " c " + m.GetColor(i);
                    text += " ca " + m.GetColorArray(i);
                    text += " f " + m.GetFloat(i);
                    text += " fa " + m.GetFloatArray(i);
                    text += " m " + m.GetMatrix(i).ToString().Replace('\n', '/');
                    text += " ma " + m.GetMatrixArray(i);
                    text += " t " + m.GetTexture(i);
                    text += " v " + m.GetVector(i);
                    text += " va " + m.GetVectorArray(i);
                    text += "\n";
                }
            }

            return text;
        }

        static void loadCar()
        {
            List<String> validNames = new List<string>();
            foreach (var name in assets.Bundle.GetAllAssetNames())
            {
                logger.WriteLine(name);
                if (name.EndsWith(".prefab"))
                    validNames.Add(name);
            }

            string fullName = "";

            if (configs.carName.ToLower() == "random")
                fullName = validNames[rnd.Next(validNames.Count)];
            else
            {
                foreach(var name in validNames)
                {
                    if(name.ToLower().Contains(configs.carName.ToLower()))
                    {
                        fullName = name;
                        break;
                    }
                }
            }

            if(fullName == "")
                fullName = validNames[rnd.Next(validNames.Count)];

            car = assets.Bundle.LoadAsset<GameObject>(fullName);

            carMesh = assets.Bundle.LoadAsset<Mesh>("assets/models/delorean.obj");
            carDiffuse = assets.Bundle.LoadAsset<Texture2D>("assets/textures/deloreandiffuse1024.tga");
            carEmissive = assets.Bundle.LoadAsset<Texture2D>("assets/textures/deloreanemit1024.tga");
            carNormal = assets.Bundle.LoadAsset<Texture2D>("assets/textures/deloreannormal1024.tga");
        }

        static void changeCar(GameObject obj)
        {
            for (int i = 0; i < obj.transform.childCount; i++)
                changeCar(obj.transform.GetChild(i).gameObject);

            if(obj.name == "Refractor")
            {
                var skinned = obj.GetComponent<SkinnedMeshRenderer>();
                skinned.enabled = false;

                /*var mat = skinned.material;
                obj.RemoveComponent<SkinnedMeshRenderer>();

                var renderer = obj.AddComponent<MeshRenderer>();
                var filter = obj.AddComponent<MeshFilter>();
                mat.SetTexture(5, carDiffuse);
                renderer.material = mat;
                filter.mesh = carMesh;*/

                /*skinned.BakeMesh(carMesh);
                var mesh = skinned.sharedMesh;

                mesh.SetIndices(carMesh.GetIndices(0), carMesh.GetTopology(0), 0);
                mesh.SetTriangles(carMesh.GetTriangles(0), 0);

                List<Vector4> tangents = new List<Vector4>();
                carMesh.GetTangents(tangents);
                mesh.SetTangents(tangents);

                List<Vector3> normals = new List<Vector3>();
                carMesh.GetNormals(normals);
                mesh.SetNormals(normals);

                List<Vector2> uvs = new List<Vector2>();
                carMesh.GetUVs(0, uvs);
                mesh.SetUVs(0, uvs);

                foreach(var m in skinned.materials)
                    m.SetTexture(5, carDiffuse);

                skinned.BakeMesh(mesh);*/
            }
        }
    }
}
