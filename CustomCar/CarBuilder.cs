using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomCar
{
    public class CarData
    {
        public string name;
        public GameObject carToAdd;
        public CarColors defaultColors;
    }

    public class CarBuilder
    {
        CarData m_data;

        public GameObject CreateCar(CarData data, GameObject model)
        {
            m_data = data;

            var obj = CreateBaseCar(model);
            ReplaceModel(obj);

            return obj;
        }

        GameObject CreateBaseCar(GameObject model)
        {
            var obj = GameObject.Instantiate(model);
            GameObject.DontDestroyOnLoad(obj);
            obj.SetActive(false);
            obj.name = m_data.name;
            return obj;
        }

        void ReplaceModel(GameObject obj)
        {
            List<GameObject> wheelsToRemove = new List<GameObject>();
            for(int i = 0; i < obj.transform.childCount; i++)
            {
                var c = obj.transform.GetChild(i).gameObject;
                if (c.name.ToLower().Contains("wheel"))
                    wheelsToRemove.Add(c);
            }

            var refractor = obj.transform.Find("Refractor");
            if(refractor == null)
            {
                Console.Out.WriteLine("Can't remove refractor object");
                return;
            }

            GameObject boostJet = null;
            GameObject wingJet = null;
            GameObject rotationJet = null;
            GameObject wingTrail = null;
            foreach (var j in refractor.GetComponentsInChildren<JetFlame>())
            {
                var name = j.gameObject.name;
                if (name == "BoostJetFlameCenter")
                    boostJet = j.gameObject;
                else if (name == "JetFlameBackLeft")
                    rotationJet = j.gameObject;
                else if (name == "WingJetFlameLeft1")
                    wingJet = j.gameObject;
            }
            wingTrail = refractor.GetComponentInChildren<WingTrail>().gameObject;

            Material baseMat = null;
            foreach(var o in obj.GetComponentsInChildren<MeshRenderer>())
            {
                var mat = o.material;
                if(mat.shader.name == "Custom/Reflective/Bump Glow LaserCut")
                {
                    baseMat = mat;
                    break;
                }
            }

            var colorChanger = obj.GetComponent<ColorChanger>();
            if (colorChanger != null)
                colorChanger.rendererChangers_ = new ColorChanger.RendererChanger[0];

            var newcar = GameObject.Instantiate(m_data.carToAdd, obj.transform);
            foreach (var r in newcar.GetComponentsInChildren<Renderer>())
            {
                ReplaceMaterials(r, baseMat);
                if(colorChanger != null)
                    AddMaterialColorChanger(colorChanger, r.transform);
            }

            var boostJets = new List<JetFlame>();
            var wingJets = new List<JetFlame>();
            var rotationJets = new List<JetFlame>();
            
            PlaceJets(newcar, boostJet, wingJet, rotationJet, boostJets, wingJets, rotationJets, wingTrail);

            var carVisual = obj.GetComponent<CarVisuals>();
            carVisual.rotationJetFlames_ = rotationJets.ToArray();
            carVisual.boostJetFlames_ = boostJets.ToArray();
            carVisual.wingJetFlames_ = wingJets.ToArray();
            carVisual.lights_ = new Light[0];
            carVisual.driverPosition_ = null;
            carVisual.carBodyRenderer_ = newcar.GetComponentInChildren<SkinnedMeshRenderer>();

            MakeMeshSkinned(carVisual.carBodyRenderer_);

            List<Transform> wheelsToMove = new List<Transform>();
            for(int i = 0; i < newcar.transform.childCount; i++)
            {
                var c = newcar.transform.GetChild(i).gameObject;
                var name = c.name.ToLower();
                if (name.Contains("wheel"))
                {
                    wheelsToMove.Add(c.transform);
                    var wheelRenderer = c.GetComponentInChildren<MeshRenderer>();
                    CarWheelVisuals comp = c.AddComponent<CarWheelVisuals>();
                    comp.tire_ = c.GetComponentInChildren<MeshRenderer>();
                    if (carVisual != null)
                    {
                        if (name.Contains("front"))
                        {
                            if (name.Contains("left"))
                                carVisual.wheelFL_ = comp;
                            else if (name.Contains("right"))
                                carVisual.wheelFR_ = comp;
                        }
                        else if (name.Contains("back"))
                        {
                            if (name.Contains("left"))
                                carVisual.wheelBL_ = comp;
                            else if (name.Contains("right"))
                                carVisual.wheelBR_ = comp;
                        }
                    }
                }

                if (name.Contains("driverposition"))
                    carVisual.driverPosition_ = c.transform;

                if (name.Contains("defaultcolor"))
                    loadDefaultColors(c.gameObject);
            }

            foreach (var w in wheelsToRemove)
                GameObject.Destroy(w);
            
            GameObject.Destroy(refractor.gameObject);
        }

        void ReplaceMaterials(Renderer r, Material baseMat)
        {
            for(int i = 0; i < r.transform.childCount; i++)
            {
                var n = r.transform.GetChild(i).gameObject.name.ToLower();
                if (n.StartsWith("#") && n.Contains("donotreplace"))
                    return;
            }

            if (baseMat == null)
            {
                Console.Out.WriteLine("base material is null");
                return;
            }

            List<Material> newMaterials = new List<Material>();
            foreach(var m in r.materials)
            {
                Material mat = UnityEngine.Object.Instantiate(baseMat);

                mat.SetTexture(5, m.GetTexture("_MainTex")); //diffuse
                mat.SetTexture(255, m.GetTexture("_EmissionMap")); //emissive
                mat.SetTexture(218, m.GetTexture("_BumpMap")); //normal
                newMaterials.Add(mat);
            }
            
            r.materials = newMaterials.ToArray();
        }

        void PlaceJets(GameObject obj, GameObject boostJet, GameObject wingJet, GameObject rotationJet
            , List<JetFlame> boostJets, List<JetFlame> wingJets, List<JetFlame> rotationJets, GameObject wingtrail)
        {
            int childNb = obj.transform.childCount;
            for (int i = 0; i < childNb; i++)
            {
                var child = obj.GetChild(i).gameObject;
                var name = child.name.ToLower();
                if (boostJet != null && name.Contains("boostjet"))
                {
                    var jet = GameObject.Instantiate(boostJet, child.transform);
                    jet.transform.localPosition = Vector3.zero;
                    jet.transform.localRotation = Quaternion.identity;
                    boostJets.Add(jet.GetComponentInChildren<JetFlame>());
                }
                else if (wingJet != null && name.Contains("wingjet"))
                {
                    var jet = GameObject.Instantiate(wingJet, child.transform);
                    jet.transform.localPosition = Vector3.zero;
                    jet.transform.localRotation = Quaternion.identity;
                    wingJets.Add(jet.GetComponentInChildren<JetFlame>());
                    wingJets.Last().rotationAxis_ = JetDirection(child.transform);
                }
                else if (rotationJet != null && name.Contains("rotationjet"))
                {
                    var jet = GameObject.Instantiate(rotationJet, child.transform);
                    jet.transform.localPosition = Vector3.zero;
                    jet.transform.localRotation = Quaternion.identity;
                    rotationJets.Add(jet.GetComponentInChildren<JetFlame>());
                    rotationJets.Last().rotationAxis_ = JetDirection(child.transform);
                }
                else if(wingtrail != null && name.Contains("wingtrail"))
                {
                    var trail = GameObject.Instantiate(wingtrail, child.transform);
                    trail.transform.localPosition = Vector3.zero;
                    trail.transform.localRotation = Quaternion.identity;
                }
                else PlaceJets(child, boostJet, wingJet, rotationJet, boostJets, wingJets, rotationJets, wingtrail);
            }
        }

        void AddMaterialColorChanger(ColorChanger colorChanger, Transform obj)
        {
            Renderer r = obj.GetComponent<Renderer>();
            if (r == null)
                return;

            var uniformChangers = new List<ColorChanger.UniformChanger>();

            for(int i = 0; i < obj.childCount; i++)
            {
                var o = obj.GetChild(i).gameObject;
                var name = o.name;
                if (!name.StartsWith("#"))
                    continue;

                name = name.Remove(0, 1); //remove #

                var s = name.Split(';');
                if (s.Length != 5)
                    continue;

                var uniformChanger = new ColorChanger.UniformChanger();
                uniformChanger.colorType_ = colorType(s[0]);
                int materialIndex = 0;
                int.TryParse(s[1], out materialIndex);
                uniformChanger.materialIndex_ = materialIndex;
                uniformChanger.name_ = uniformName(s[2]);
                float multiplier = 0;
                float.TryParse(s[3], out multiplier);
                uniformChanger.mul_ = multiplier;
                uniformChanger.alpha_ = s[4].ToLower() == "true";

                uniformChangers.Add(uniformChanger);
            }

            if (uniformChangers.Count == 0)
                return;

            var renderChanger = new ColorChanger.RendererChanger();
            renderChanger.renderer_ = r;
            renderChanger.uniformChangers_ = uniformChangers.ToArray();

            var renders = colorChanger.rendererChangers_.ToList();
            renders.Add(renderChanger);
            colorChanger.rendererChangers_ = renders.ToArray();
        }

        void MakeMeshSkinned(SkinnedMeshRenderer renderer)
        {
            var mesh = renderer.sharedMesh;
            if (mesh == null)
            {
                Console.Out.WriteLine("The mesh on " + renderer.name + " (SkinnedMeshRenderer) is null");
                return;
            }
            if (!mesh.isReadable)
            {
                Console.Out.WriteLine("Can't read the car mesh " + mesh.name + "\nYou must allow read on it's unity inspector !");
                return;
            }
            if (mesh.vertices.Length == mesh.boneWeights.Length)
                return;

            var bones = new BoneWeight[mesh.vertices.Length];
            for (int i = 0; i < bones.Length; i++)
                bones[i].weight0 = 1;
            mesh.boneWeights = bones;
            var t = renderer.transform;
            var bindPoses = new Matrix4x4[1] { t.worldToLocalMatrix * renderer.transform.localToWorldMatrix };
            mesh.bindposes = bindPoses;
            renderer.bones = new Transform[1] { t };
        }

        void loadDefaultColors(GameObject obj)
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var o = obj.transform.GetChild(i).gameObject;
                var name = o.name.ToLower();

                if (!name.StartsWith("#"))
                    continue;

                name = name.Remove(0, 1); //remove #

                var s = name.Split(';');
                if (s.Length != 2)
                    continue;

                Color c = ColorEx.HexToColor(s[1]);
                c.a = 1;
                if (s[0] == "primary")
                    m_data.defaultColors.primary_ = c;
                else if (s[0] == "secondary")
                    m_data.defaultColors.secondary_ = c;
                else if (s[0] == "glow")
                    m_data.defaultColors.glow_ = c;
                else if (s[0] == "sparkle")
                    m_data.defaultColors.sparkle_ = c;
            }
        }

        Vector3 JetDirection(Transform t)
        {
            int nb = t.childCount;
            for(int i = 0; i < nb; i++)
            {
                var n = t.GetChild(i).gameObject.name.ToLower();
                if (!n.StartsWith("#"))
                    continue;
                n = n.Remove(0, 1);
                if (n.Contains("front"))
                    return new Vector3(-1, 0, 0);
                if (n.Contains("back"))
                    return new Vector3(1, 0, 0);
                if (n.Contains("left"))
                    return new Vector3(0, 1, -1);
                if (n.Contains("right"))
                    return new Vector3(0, -1, 1);
                var s = n.Split(';');
                if (s.Length != 4)
                    continue;
                if (!s[0].Contains("dir"))
                    continue;
                var v = Vector3.zero;
                float.TryParse(s[1], out v.x);
                float.TryParse(s[2], out v.y);
                float.TryParse(s[3], out v.z);
                return v;
            }
            return Vector3.zero;
        }

        ColorChanger.ColorType colorType(string name)
        {
            name = name.ToLower();
            if (name == "primary")
                return ColorChanger.ColorType.Primary;
            else if (name == "secondary")
                return ColorChanger.ColorType.Secondary;
            else if (name == "glow")
                return ColorChanger.ColorType.Glow;
            else if (name == "sparkle")
                return ColorChanger.ColorType.Sparkle;
            return ColorChanger.ColorType.Primary;
        }

        MaterialEx.SupportedUniform uniformName(string name)
        {
            name = name.ToLower();
            if (name == "color")
                return MaterialEx.SupportedUniform._Color;
            else if (name == "color2")
                return MaterialEx.SupportedUniform._Color2;
            else if (name == "emitcolor")
                return MaterialEx.SupportedUniform._EmitColor;
            else if (name == "reflectcolor")
                return MaterialEx.SupportedUniform._ReflectColor;
            else if (name == "speccolor")
                return MaterialEx.SupportedUniform._SpecColor;
            return MaterialEx.SupportedUniform._Color;
        }
    }
}
