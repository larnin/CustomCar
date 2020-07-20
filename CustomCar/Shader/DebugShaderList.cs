using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCar
{
    static class DebugShaderList
    {
        class MaterialInfo
        {
            public MaterialInfo(string _objectName, int _materialIndex, string _materialName)
            {
                objectName = _objectName;
                materialIndex = _materialIndex;
                materialName = _materialName;
            }

            public string objectName;
            public int materialIndex;
            public string materialName;
        }

        class ShaderInfo
        {
            public ShaderInfo(string name)
            {
                shaderName = name;
                materials = new List<MaterialInfo>();
            }

            public string shaderName;
            public List<MaterialInfo> materials;
        }

        public static void Exec()
        {
            List<ShaderInfo> shaderList = new List<ShaderInfo>();
            var profileManager = G.Sys.ProfileManager_;
            foreach (var c in profileManager.carInfos_)
            {
                CollectShadersOnObject(c.prefabs_.carPrefab_, shaderList, "Car");
                CollectShadersOnObject(c.prefabs_.cutsceneScreenPrefab_, shaderList, "cutscene");
                CollectShadersOnObject(c.prefabs_.screenPrefab_, shaderList, "screen");
            }

            Print(shaderList, "shaders.txt");
        }

        static void CollectShadersOnObject(GameObject obj, List<ShaderInfo> list, string id)
        {
            if (obj == null)
                return;

            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach(var r in renderers)
            {
                var objName = id + "/" + GetObjectFullName(r.gameObject);
                var mats = r.sharedMaterials;
                for(int i = 0; i < mats.Length; i++)
                {
                    var m = mats[i];
                    if (m == null || m.shader == null)
                        continue;
                    var name = m.shader.name;
                    var item = list.Find(x => { return x.shaderName == name; });
                    if(item == null)
                    {
                        item = new ShaderInfo(name);
                        list.Add(item);
                    }
                    item.materials.Add(new MaterialInfo(objName, i, m.name));
                }
            }
        }

        static void Print(List<ShaderInfo> list, string file)
        {
            Spectrum.API.Logging.Logger logger = new Spectrum.API.Logging.Logger(file);

            foreach(var s in list)
            {
                logger.WriteLine("Shader " + s.shaderName);
                logger.WriteLine("\tMaterials " + s.materials.Count);
                foreach(var m in s.materials)
                    logger.WriteLine("\t\t" + m.objectName + " # " + m.materialName + " # " + m.materialIndex);
                logger.WriteLine("");
            }
            Console.Out.WriteLine("Shaders infos extracted !");
        }

        public static string GetObjectFullName(GameObject obj)
        {
            string name = obj.name;
            while(obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                name = obj.name + "/" + name;
            }
            return name;
        }
    }
}
