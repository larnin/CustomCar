using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomCar
{
    public class MaterialInfos
    {
        public Material material;
        public int diffuseIndex = -1;
        public int normalIndex = -1;
        public int emitIndex = -1;

        public void replaceMaterialInRenderer(Renderer r, int materialIndex)
        {
            if (material == null || r == null || materialIndex >= r.materials.Length)
                return;

            var m = r.materials[materialIndex];

            Material mat = UnityEngine.Object.Instantiate(material);
            if(diffuseIndex >= 0)
                mat.SetTexture(diffuseIndex, m.GetTexture("_MainTex")); //diffuse
            if(emitIndex >= 0)
                mat.SetTexture(emitIndex, m.GetTexture("_EmissionMap")); //emissive
            if (normalIndex >= 0)
                mat.SetTexture(normalIndex, m.GetTexture("_BumpMap")); //normal
            r.materials[materialIndex] = mat;
        }
    }

    public class MaterialPropertyInfo
    {
        public string shaderName;
        public string name;
        public int diffuseIndex = -1;
        public int normalIndex = -1;
        public int emitIndex = -1;

        public MaterialPropertyInfo(string _shaderName, string _name, int _diffuseIndex, int _normalIndex, int _emitIndex)
        {
            shaderName = _shaderName;
            name = _name;
            diffuseIndex = _diffuseIndex;
            normalIndex = _normalIndex;
            emitIndex = _emitIndex;
        }
    }

    public class CarInfos
    {
        public Dictionary<string, MaterialInfos> materials = new Dictionary<string, MaterialInfos>();
        public GameObject boostJet = null;
        public GameObject wingJet = null;
        public GameObject rotationJet = null;
        public GameObject wingTrail = null;

        public GameObject baseCar = null;
        public CarColors defaultColors;

        public void collectInfos()
        {
            getBaseCar();
            getJetsAndTrail();
            getMaterials();
        }

        void getBaseCar()
        {
            var prefab = G.Sys.ProfileManager_.carInfos_[0].prefabs_.carPrefab_;
            if(prefab == null)
            {
                ErrorList.add("Can't find the refractor base car prefab");
                return;
            }
            baseCar = prefab;
            defaultColors = G.Sys.ProfileManager_.carInfos_[0].colors_;
        }

        void getJetsAndTrail()
        {
            if (baseCar == null)
                return;

            foreach (var j in baseCar.GetComponentsInChildren<JetFlame>())
            {
                var name = j.gameObject.name;
                if (name == "BoostJetFlameCenter")
                    boostJet = j.gameObject;
                else if (name == "JetFlameBackLeft")
                    rotationJet = j.gameObject;
                else if (name == "WingJetFlameLeft1")
                    wingJet = j.gameObject;
            }
            wingTrail = baseCar.GetComponentInChildren<WingTrail>().gameObject;

            if (boostJet == null)
                ErrorList.add("No valid BoostJet found on Refractor");
            if (rotationJet == null)
                ErrorList.add("No valid RotationJet found on Refractor");
            if (wingJet == null)
                ErrorList.add("No valid WingJet found on Refractor");
            if (wingTrail == null)
                ErrorList.add("No valid WingTrail found on Refractor");
        }

        void getMaterials()
        {
            List<MaterialPropertyInfo> materialsNames = new List<MaterialPropertyInfo>
            {
                new MaterialPropertyInfo("Custom/LaserCut/CarPaint", "carpaint", 5, -1, -1),
                new MaterialPropertyInfo("Custom/LaserCut/CarWindow", "carwindow", -1, 218, 219),
                new MaterialPropertyInfo("Custom/Reflective/Bump Glow LaserCut", "wheel", 5, 218, 255),
                new MaterialPropertyInfo("Custom/LaserCut/CarPaintBump", "carpaintbump", 5, 218, -1),
                new MaterialPropertyInfo("Custom/Reflective/Bump Glow Interceptor Special", "interceptor", 5, 218, 255),
                new MaterialPropertyInfo("Custom/LaserCut/CarWindowTrans2Sided", "transparentglow", -1, 218, 219)
            };

            foreach(var c in G.Sys.ProfileManager_.carInfos_)
            {
                var prefab = c.prefabs_.carPrefab_;
                foreach (var renderer in prefab.GetComponentsInChildren<Renderer>())
                    foreach (var mat in renderer.materials)
                        foreach (var key in materialsNames)
                        {
                            if (materials.ContainsKey(key.name))
                                continue;
                            if (mat.shader.name == key.shaderName)
                            {
                                var m = new MaterialInfos();
                                m.material = mat;
                                m.diffuseIndex = key.diffuseIndex;
                                m.normalIndex = key.normalIndex;
                                m.emitIndex = key.emitIndex;
                                materials.Add(key.name, m);
                            }
                        }
            }
            
            foreach (var mat in materialsNames)
            {
                if (!materials.ContainsKey(mat.name))
                    ErrorList.add("Can't find the material: " + mat.name + " - shader: " + mat.shaderName);
            }

            materials.Add("donotreplace", new MaterialInfos());
        }
    }
}
