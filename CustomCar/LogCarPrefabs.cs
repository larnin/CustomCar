using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

//const string carBaseName = "Assets/Prefabs/NitronicCycle.prefab";

//static System.Random rnd = new System.Random();
//static Configs configs;
//static Spectrum.API.Logging.Logger logger;
//static Assets assets;
//static GameObject car;

//static Mesh carMesh;
//static Texture2D carDiffuse;
//static Texture2D carEmissive;
//static Texture2D carNormal;
//static Material carMat;

//public void Initialize(IManager manager, string ipcIdentifier)
//{
//    //Console.Out.WriteLine(Application.unityVersion);

//    configs = new Configs();
//    logger = new Spectrum.API.Logging.Logger("CustomCar");

//    var harmony = HarmonyInstance.Create("com.Larnin.CustomCar");
//    harmony.PatchAll(Assembly.GetExecutingAssembly());


//    assets = new Assets("cars");
//    loadCar();
//}

//[HarmonyPatch(typeof(CarLogic), "Awake")]
//internal class CarLogicAwake
//{

//    static void Postfix(CarLogic __instance)
//    {
//        Entry.logger.WriteLine("\n\n---------------------------------\n" + __instance.gameObject.name + "\n---------------------------------\n\n");
//        Entry.logCurrentObjectAndChilds(__instance.gameObject, true);

//        //changeCar(__instance.gameObject);

//        try
//        {
//            var asset = GameObject.Instantiate(car);
//            asset.transform.parent = __instance.transform;
//            asset.transform.localPosition = Vector3.zero;
//            asset.transform.localRotation = Quaternion.Euler(0, 180, 0);
//            asset.SetLayerRecursively(__instance.gameObject.GetLayer());
//            logger.WriteLine(asset.transform.parent.name);
//            var renderer = asset.GetComponentInChildren<MeshRenderer>();
//            renderer.material = carMat;

//            /*var mat = new Material(Shader.Find("Diffuse"));

//            foreach (var renderer in GameObject.FindObjectsOfType<Renderer>())
//            {
//                renderer.material = mat;
//            }*/

//        }
//        catch (Exception e)
//        {
//            Console.Out.WriteLine(e.Message);
//            Console.Out.WriteLine(e.ToString());
//        }
//    }
//}

//static void logCurrentObjectAndChilds(GameObject obj, bool hideStuff, int level = 0)
//{
//    const int offsetPerLevel = 2;
//    string offset = new string(' ', level * offsetPerLevel);

//    logger.WriteLine(offset + obj.GetType().Name + " - " + obj.name + " - " + obj.activeSelf + " - " + obj.transform.localPosition + " - " + obj.transform.localRotation.eulerAngles + " - " + obj.transform.localScale + " - " + obj.layer);
//    foreach (var comp in obj.GetComponents(typeof(Component)))
//    {
//        string text = offset + "   | " + comp.GetType().Name;
//        var behaviour = comp as Behaviour;
//        if (behaviour != null)
//        {
//            text += " - " + behaviour.enabled;
//        }
//        var renderer = comp as MeshRenderer;
//        if(renderer != null)
//        {
//            foreach(var m in renderer.materials)
//            {
//                text += logMaterial(m, level);
//            }
//        }
//        var skinnedRenderer = comp as SkinnedMeshRenderer;
//        if (skinnedRenderer != null)
//        {
//            foreach (var m in skinnedRenderer.materials)
//            {
//                text += logMaterial(m, level);
//            }
//        }
//        var filter = comp as MeshFilter;
//        if(filter != null)
//        {
//            text += " - " + filter.mesh.name + " - " + filter.mesh.GetIndexCount(0) + " - " + filter.mesh.GetTriangles(0).Length;
//        }
//        logger.WriteLine(text);
//        if(hideStuff)
//            disableSpecificObject(comp);
//    }
//    for (int i = 0; i < obj.transform.childCount; i++)
//        logCurrentObjectAndChilds(obj.transform.GetChild(i).gameObject, hideStuff, level + 1);
//}

//static void disableSpecificObject(Component comp)
//{
//    var renderer = comp as MeshRenderer;
//    var skinnedRenderer = comp as SkinnedMeshRenderer;
//    if(renderer != null)
//    {
//        //if (renderer.name.Contains("Wheel") || renderer.name.Contains("HubVisual") || renderer.name.Contains("JetFlame") || renderer.name.Contains("CarCrossSection") || renderer.name.Contains("Refractor"))
//            renderer.enabled = false;
//    }
//    if(skinnedRenderer != null)
//    {
//        if (skinnedRenderer.gameObject.name == "Refractor")
//        {
//            carMat = skinnedRenderer.materials[2];
//            carMat.SetTexture(5, carDiffuse);
//            carMat.SetTexture(242, carEmissive);
//            carMat.SetVector(159, new Vector4(1, -1, 0, 0));
//            carMat.SetVector(160, new Vector4(1, -1, 0, 0));
//            carMat.SetVector(393, new Vector4(1, -1, 0, 0));
//            carMat.SetVector(401, new Vector4(1, -1, 0, 0));
//            carMat.SetVector(403, new Vector4(1, -1, 0, 0));
//        }
//        skinnedRenderer.enabled = false;
//    }
//}

//static string logMaterial(Material m, int level)
//{
//    const int offsetPerLevel = 2;
//    string text = "\n" + new string(' ', level * offsetPerLevel) + "     * " + m.name + " " + m.shader.name + "\n";

//    for (int i = 0; i < 10000; i++)
//    {
//        if (m.HasProperty(i))
//        {
//            text += i ;
//            text += " c " + m.GetColor(i);
//            text += " ca " + m.GetColorArray(i);
//            text += " f " + m.GetFloat(i);
//            text += " fa " + m.GetFloatArray(i);
//            text += " m " + m.GetMatrix(i).ToString().Replace('\n', '/');
//            text += " ma " + m.GetMatrixArray(i);
//            text += " t " + m.GetTexture(i);
//            text += " v " + m.GetVector(i);
//            text += " va " + m.GetVectorArray(i);
//            text += "\n";
//        }
//    }

//    return text;
//}

//static void loadCar()
//{
//    List<String> validNames = new List<string>();
//    foreach (var name in assets.Bundle.GetAllAssetNames())
//    {
//        logger.WriteLine(name);
//        if (name.EndsWith(".prefab"))
//            validNames.Add(name);
//    }

//    string fullName = "";

//    if (configs.carName.ToLower() == "random")
//        fullName = validNames[rnd.Next(validNames.Count)];
//    else
//    {
//        foreach(var name in validNames)
//        {
//            if(name.ToLower().Contains(configs.carName.ToLower()))
//            {
//                fullName = name;
//                break;
//            }
//        }
//    }

//    if(fullName == "")
//        fullName = validNames[rnd.Next(validNames.Count)];

//    car = assets.Bundle.LoadAsset<GameObject>(fullName);

//    carMesh = assets.Bundle.LoadAsset<Mesh>("assets/models/delorean.obj");
//    carDiffuse = assets.Bundle.LoadAsset<Texture2D>("assets/textures/deloreandiffuse1024.tga");
//    carEmissive = assets.Bundle.LoadAsset<Texture2D>("assets/textures/deloreanemit1024.tga");
//    carNormal = assets.Bundle.LoadAsset<Texture2D>("assets/textures/deloreannormal1024.tga");
//}

//static void changeCar(GameObject obj)
//{
//    for (int i = 0; i < obj.transform.childCount; i++)
//        changeCar(obj.transform.GetChild(i).gameObject);

//    if(obj.name == "Refractor")
//    {
//        var skinned = obj.GetComponent<SkinnedMeshRenderer>();
//        skinned.enabled = false;

//        /*var mat = skinned.material;
//        obj.RemoveComponent<SkinnedMeshRenderer>();

//        var renderer = obj.AddComponent<MeshRenderer>();
//        var filter = obj.AddComponent<MeshFilter>();
//        mat.SetTexture(5, carDiffuse);
//        renderer.material = mat;
//        filter.mesh = carMesh;*/

//        /*skinned.BakeMesh(carMesh);
//        var mesh = skinned.sharedMesh;

//        mesh.SetIndices(carMesh.GetIndices(0), carMesh.GetTopology(0), 0);
//        mesh.SetTriangles(carMesh.GetTriangles(0), 0);

//        List<Vector4> tangents = new List<Vector4>();
//        carMesh.GetTangents(tangents);
//        mesh.SetTangents(tangents);

//        List<Vector3> normals = new List<Vector3>();
//        carMesh.GetNormals(normals);
//        mesh.SetNormals(normals);

//        List<Vector2> uvs = new List<Vector2>();
//        carMesh.GetUVs(0, uvs);
//        mesh.SetUVs(0, uvs);

//        foreach(var m in skinned.materials)
//            m.SetTexture(5, carDiffuse);

//        skinned.BakeMesh(mesh);*/
//    }
//}

namespace CustomCar
{
    static class LogCarPrefabs
    {
        public static void logCars()
        {
            Spectrum.API.Logging.Logger logger = new Spectrum.API.Logging.Logger("CustomCar");

            Console.Out.WriteLine("Trying to extract cars infos");

            try
            {
                CarList carList = new CarList();

                foreach (var c in G.Sys.ProfileManager_.CarInfos_)
                    carList.cars.Add(getCarInfos(c));


                string json = carList.toJson();

                logger.WriteLine(json);

                Console.Out.WriteLine("Car infos extracted !");
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
                Console.Out.WriteLine(e.Source);
                Console.Out.WriteLine(e.StackTrace);
                Console.Out.WriteLine(e.ToString());
            }
            Console.Out.WriteLine("textures nb : " + allTextures.Count);
            Console.Out.WriteLine("mesh nb : " + allMeshs.Count);
        }

        public static void LogObjectAndChilds(GameObject obj)
        {

            Spectrum.API.Logging.Logger logger = new Spectrum.API.Logging.Logger("CustomCar_" + obj.name);

            Console.Out.WriteLine("Trying to extract infos from " + obj.name);

            try
            {
                var objInfo = getGameObjectInfos(obj);
                
                string json = objInfo.toJson();

                logger.WriteLine(json);

                Console.Out.WriteLine("Object extracted !");
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
                Console.Out.WriteLine(e.Source);
                Console.Out.WriteLine(e.StackTrace);
                Console.Out.WriteLine(e.ToString());
            }
            Console.Out.WriteLine("textures nb : " + allTextures.Count);
            Console.Out.WriteLine("mesh nb : " + allMeshs.Count);

        }

        public static List<Texture> allTextures = new List<Texture>();
        public static List<Mesh> allMeshs = new List<Mesh>();

        class CarList
        {
            public List<CarData> cars = new List<CarData>();

            public string toJson()
            {
                string s = "{\"cars\":[";
                for (int i = 0; i < cars.Count; i++)
                {
                    s += cars[i].toJson();
                    if (i < cars.Count - 1)
                        s += ",";
                }
                return s + "]}";
            }
        }

        class CarData
        {
            public GameObjectInfo carObject;
            public GameObjectInfo screenObject;
            public GameObjectInfo cutsceneSceenObject;
            public string carName;
            public CarColors carColor;

            public string toJson()
            {
                return "{\"carObject\":" + (carObject != null ? carObject.toJson() : "\"null\"") + ",\"screenObject\":" + (screenObject != null ? screenObject.toJson() : "\"null\"")
                    + ",\"cutsceneSceenObject\":" + (cutsceneSceenObject != null ? cutsceneSceenObject.toJson() : "\"null\"")
                    + ",\"carName\":\"" + carName + "\",\"carColor\":" + JsonUtility.ToJson(carColor) + "}";
            }
        }

        class GameObjectInfo
        {
            public List<GameObjectInfo> childs = new List<GameObjectInfo>();
            public string name;
            public bool active;
            public List<ComponentInfo> components = new List<ComponentInfo>();

            public string toJson()
            {
                string s = "{\"name\":\"" + name + "\",\"active\":\"" + active + "\",\"childs\":[";
                for (int i = 0; i < childs.Count; i++)
                {
                    if (childs[i] == null)
                        s += "{}";
                    else s += childs[i].toJson();
                    if (i < childs.Count - 1)
                        s += ",";
                }
                s += "],\"components\":[";
                for (int i = 0; i < components.Count; i++)
                {
                    if (components[i] == null)
                        s += "{}";
                    else s += components[i].toJson();
                    if (i < components.Count - 1)
                        s += ",";
                }
                s += "]}";
                return s;
            }
        }

        class ComponentInfo
        {
            public string type;
            public string tag;
            public CustomInfoBase customInfos;

            public string toJson()
            {
                return "{\"type\":\"" + type + "\",\"tag\":\"" + tag + "\"" + (customInfos != null ? ",\"customInfos\":" + customInfos.toJson() : "") + "}";
            }
        }

        abstract class CustomInfoBase
        {
            public abstract string toJson();
        }

        class CustomInfoMeshRenderer : CustomInfoBase
        {
            public List<MaterialInfo> materials = new List<MaterialInfo>();

            public override string toJson()
            {
                string s = "{\"materials\":[";
                for (int i = 0; i < materials.Count; i++)
                {
                    s += materials[i].toJson();
                    if (i < materials.Count - 1)
                        s += ",";
                }
                return s + "]}";
            }
        }

        class CustomInfoSkinnedMeshRenderer : CustomInfoBase
        {
            public List<MaterialInfo> materials = new List<MaterialInfo>();
            public Mesh sharedMesh;
            public string rootBoneName;
            public List<string> boneNames = new List<string>();

            public override string toJson()
            {
                if (!allMeshs.Contains(sharedMesh))
                    allMeshs.Add(sharedMesh);

                string s = "{\"rootBone\":\"" + rootBoneName + "\",\"sharedMesh\":" + JsonEx.toJson(sharedMesh) + ",\"materials\":[";
                for (int i = 0; i < materials.Count; i++)
                {
                    s += materials[i].toJson();
                    if (i < materials.Count - 1)
                        s += ",";
                }
                s += "],\"bones\":[";
                for(int i = 0; i < boneNames.Count; i++)
                {
                    s += "\"" + boneNames[i] + "\"";
                    if (i < boneNames.Count - 1)
                        s += ",";
                }
                return s + "]}";
            }
        }

        class CustomInfoMeshFilter : CustomInfoBase
        {
            public Mesh mesh;
            public Mesh sharedMesh;

            public override string toJson()
            {
                if (!allMeshs.Contains(sharedMesh))
                    allMeshs.Add(sharedMesh);
                if (!allMeshs.Contains(mesh))
                    allMeshs.Add(mesh);

                return "{\"mesh\":" + JsonEx.toJson(mesh) + ",\"sharedMesh\":" + JsonEx.toJson(sharedMesh) + "}";
            }
        }

        class CustomInfoTransform : CustomInfoBase
        {
            public Vector3 position;
            public Vector3 scale;
            public Quaternion rotation;

            public override string toJson()
            {
                Vector3 rot = rotation.eulerAngles;

                return "{\"position\":[" + position.x + "," + position.y + "," + position.z + "],\"rotation\":[" + rot.x + "," + rot.y + "," + rot.z
                    + "],\"scale\":[" + scale.x + "," + scale.y + "," + scale.z + "]}";
            }
        }

        class CustomInfoAnimation : CustomInfoBase
        {
            public Animation animation;

            public override string toJson()
            {
                string s = "{\"clips\":[";

                List<AnimationState> anims = new List<AnimationState>();
                foreach (AnimationState state in animation)
                    anims.Add(state);
                for(int i = 0; i < anims.Count; i++)
                {
                    var state = anims[i];
                    var clip = state.clip;
                    s += "{\"name\":\"" + state.name + "\",\"clipName\":\"" + clip.name + "\",\"lenght\":" + clip.length + ",\"framerate\":" + clip.frameRate + ",\"speed\":" + state.speed
                        + ",\"weight\":" + state.weight + ",\"warpMode\":\"" + state.wrapMode.ToString() + "\",\"legacy\":\"" + (clip.legacy ? "true" : "false")
                        + "\",\"blendMode\":\"" + state.blendMode.ToString() + "\",\"bounds\":" + JsonEx.toJson(clip.localBounds) + ",\"Events\":{";

                    for(int j = 0; j < clip.events.Length; j++)
                    {
                        var e = clip.events[j];
                        s += "{\"time\":" + e.time + "}";

                        if (j < clip.events.Length - 1)
                            s += ",";
                    }
                    s += "}}";



                    if (i < anims.Count - 1)
                        s += ",";
                }
                s += "]";

                if (animation.clip != null)
                {
                    var clip = animation.clip;
                    s += ",\"clip\":{\"name\":\"" + clip.name + "\",\"lenght\":" + clip.length + ",\"framerate\":" + clip.frameRate
                        + ",\"weight\":" + ",\"legacy\":\"" + (clip.legacy ? "true" : "false")
                        + "\",\"bounds\":" + JsonEx.toJson(clip.localBounds) + "\"Events\":{";

                    for (int j = 0; j < clip.events.Length; j++)
                    {
                        var e = clip.events[j];
                        s += "{\"time\":" + e.time + "}";

                        if (j < clip.events.Length - 1)
                            s += ",";
                    }
                    s += "}}";
                }
                s += ",\"animateOnlyIfVisible\":\"" + (animation.animateOnlyIfVisible ? "true" : "false") + "\"";
                s += ",\"animatePhysic\":\"" + (animation.animatePhysics ? "true" : "false") + "\"";
                s += ",\"cullingType\":\"" + animation.cullingType.ToString() + "\"";
                s += ",\"playing\":\"" + (animation.isPlaying ? "true" : "false") + "\"";
                s += ",\"clipCount\":" + animation.GetClipCount();
                s += ",\"bounds\":" + JsonEx.toJson(animation.localBounds);
                s += ",\"playAuto\":\"" + (animation.playAutomatically ? "true" : "false") + "\"";
                s += ",\"warpmode\":\"" + animation.wrapMode.ToString() + "\"";

                return s + "}";
            }
        }

        class CustomInfoColorChanger : CustomInfoBase
        {
            public ColorChanger.RendererChanger[] rendererChangers;

            public override string toJson()
            {
                string s = "{\"rendererChangers\":[";
                for(int i = 0; i < rendererChangers.Length; i++)
                {
                    var rChanger = rendererChangers[i];
                    s += "{\"renderer\":\"" + rChanger.renderer_ + "\",\"uniformChanger\":[";
                    for(int j = 0; j < rChanger.uniformChangers_.Length; j++)
                    {
                        var u = rChanger.uniformChangers_[j];
                        s += "{\"materialIndex\":" + u.materialIndex_ + ",\"colorType\":\"" + u.colorType_.ToString() + "\",\"name\":\"" + u.name_.ToString() + "\",\"mul\":" + u.mul_
                            + ",\"alpha\":\"" + (u.alpha_ ? "true" : "false") + "\"}";

                        if (j < rChanger.uniformChangers_.Length - 1)
                            s += ",";
                    }
                    s += "]}";
                    if (i < rendererChangers.Length - 1)
                        s += ",";
                }
                return s + "]}";
            }
        }

        class CustomInfoJetFlame : CustomInfoBase
        {
            public Vector3 rotationAxis;

            public override string toJson()
            {
                return "{\"rotationAxis\":[" + rotationAxis.x + "," + rotationAxis.y + "," + rotationAxis.z + "]}";
            }
        }

        class MaterialInfo
        {
            public string materialName;
            public string materialShaderName;
            public List<MaterialAttributeInfo> attributes = new List<MaterialAttributeInfo>();

            public string toJson()
            {
                string s = "{\"name\":\"" + materialName + "\",\"shader\":\"" + materialShaderName + "\",\"attributes\":[";
                for (int i = 0; i < attributes.Count; i++)
                {
                    s += attributes[i].toJson();
                    if (i < attributes.Count - 1)
                        s += ",";
                }

                return s + "]}";
            }
        }

        class MaterialAttributeInfo
        {
            public int index;

            public Vector4 vector;
            public Vector4[] vectorArray;
            public float floatValue;
            public float[] floatArray;
            public Matrix4x4 matrix;
            public Matrix4x4[] matrixarray;
            public Texture texture;

            public string toJson()
            {
                string s = "{\"index\":" + index + ",";

                if (vector.x != 0 && vector.y != 0 && vector.z != 0 && vector.w != 0)
                    s += "\"vector\":{\"x\":" + vector.x + ",\"y\":" + vector.y + ",\"z\":" + vector.z + ",\"w\":" + vector.w + "},";
                if (vectorArray != null && vectorArray.Length > 0)
                {
                    s += "\"vectorArray\":[";
                    for (int i = 0; i < vectorArray.Length; i++)
                    {
                        s += "{\"x\":" + vectorArray[i].x + ",\"y\":" + vectorArray[i].y + ",\"z\":" + vectorArray[i].z + ",\"w\":" + vectorArray[i].w + "}";
                        if (i < vectorArray.Length - 1)
                            s += ",";
                    }
                    s += "],";
                }
                if (floatValue != 0)
                    s += "\"float\":" + floatValue + ",";
                if (floatArray != null && floatArray.Length > 0)
                {
                    s += "\"floatArray\":[";
                    for (int i = 0; i < floatArray.Length; i++)
                    {
                        s += floatArray[i];
                        if (i < floatArray.Length - 1)
                            s += ",";
                    }
                }
                if (matrix != Matrix4x4.identity)
                {
                    s += "\"matrix\":[" + matrix.m00 + "," + matrix.m01 + "," + matrix.m02 + "," + matrix.m03 + ","
                                        + matrix.m10 + "," + matrix.m11 + "," + matrix.m12 + "," + matrix.m13 + ","
                                        + matrix.m20 + "," + matrix.m21 + "," + matrix.m22 + "," + matrix.m23 + ","
                                        + matrix.m30 + "," + matrix.m31 + "," + matrix.m32 + "," + matrix.m33 + "],";
                }
                if (matrixarray != null && matrixarray.Length > 0)
                {
                    s += "\"matrixArray\":[";
                    for (int i = 0; i < matrixarray.Length; i++)
                    {
                        s += "[" + matrixarray[i].m00 + "," + matrixarray[i].m01 + "," + matrixarray[i].m02 + "," + matrixarray[i].m03 + ","
                                 + matrixarray[i].m10 + "," + matrixarray[i].m11 + "," + matrixarray[i].m12 + "," + matrixarray[i].m13 + ","
                                 + matrixarray[i].m20 + "," + matrixarray[i].m21 + "," + matrixarray[i].m22 + "," + matrixarray[i].m23 + ","
                                 + matrixarray[i].m30 + "," + matrixarray[i].m31 + "," + matrixarray[i].m32 + "," + matrixarray[i].m33 + "]";
                        if (i < matrixarray.Length - 1)
                            s += ",";
                    }
                }
                if (texture != null)
                {
                    s += "\"texture\":\"" + texture.name + "\",";
                    if (!allTextures.Contains(texture))
                        allTextures.Add(texture);
                }

                s = s.Remove(s.LastIndexOf(','), 1);
                return s + "}";
            }
        }

        static class JsonEx
        {
            public static string toJson(Mesh m)
            {
                if (m == null)
                    return "\"null\"";
                return "{\"name\":\"" + m.name + "\",\"vertexNb\":" + m.vertices.Length + ",\"triangleNb\":" + m.triangles.Length + ",\"submeshNb\":" + m.subMeshCount
                    + ",\"bounds\":" + toJson(m.bounds) + "}";
            }

            public static string toJson(Bounds b)
            {
                return "{\"center\":[" + b.center.x + "," + b.center.y + "," + b.center.z + "],\"size\":[" + b.size.x + "," + b.size.y + "," + b.size.z + "]}";
            }
        }

        static CarData getCarInfos(CarInfo car)
        {
            CarData data = new CarData();
            data.carName = car.name_;
            data.carColor = car.colors_;

            data.carObject = getGameObjectInfos(car.prefabs_.carPrefab_);
            data.screenObject = getGameObjectInfos(car.prefabs_.screenPrefab_);
            data.cutsceneSceenObject = getGameObjectInfos(car.prefabs_.cutsceneScreenPrefab_);

            return data;
        }

        static GameObjectInfo getGameObjectInfos(GameObject obj)
        {
            if (obj == null)
                return null;

            GameObjectInfo data = new GameObjectInfo();

            data.active = obj.activeSelf;
            data.name = obj.name;

            for (int i = 0; i < obj.transform.childCount; i++)
                data.childs.Add(getGameObjectInfos(obj.transform.GetChild(i).gameObject));

            foreach (var comp in obj.GetComponents(typeof(Component)))
                data.components.Add(getComponentInfos(comp));

            return data;
        }

        static ComponentInfo getComponentInfos(Component comp)
        {
            if (comp == null)
                return null;
            ComponentInfo data = new ComponentInfo();

            data.customInfos = getCustomInfos(comp);
            data.tag = comp.tag;
            data.type = comp.GetType().Name;

            return data;
        }

        static CustomInfoBase getCustomInfos(Component comp)
        {
            if (comp is MeshRenderer)
                return getMeshRendererInfos(comp as MeshRenderer);
            else if (comp is SkinnedMeshRenderer)
                return getMeshRendererInfos(comp as SkinnedMeshRenderer);
            else if (comp is MeshFilter)
                return getMeshFilterInfos(comp as MeshFilter);
            else if (comp is Transform)
                return getTransformInfos(comp as Transform);
            else if (comp is Animation)
                return getAnimationInfos(comp as Animation);
            else if (comp is ColorChanger)
                return getColorChangerInfos(comp as ColorChanger);
            else if (comp is JetFlame)
                return getJetFlameInfos(comp as JetFlame);

            return null;
        }

        static CustomInfoTransform getTransformInfos(Transform t)
        {
            CustomInfoTransform data = new CustomInfoTransform();
            data.position = t.localPosition;
            data.rotation = t.localRotation;
            data.scale = t.localScale;
            return data;
        }

        static CustomInfoAnimation getAnimationInfos(Animation a)
        {
            CustomInfoAnimation data = new CustomInfoAnimation();
            data.animation = a;
            return data;
        }

        static CustomInfoMeshRenderer getMeshRendererInfos(MeshRenderer m)
        {
            CustomInfoMeshRenderer data = new CustomInfoMeshRenderer();
            foreach (var mat in m.materials)
                data.materials.Add(getMaterialInfos(mat));
            return data;
        }

        static CustomInfoSkinnedMeshRenderer getMeshRendererInfos(SkinnedMeshRenderer m)
        {
            CustomInfoSkinnedMeshRenderer data = new CustomInfoSkinnedMeshRenderer();
            foreach (var mat in m.materials)
                data.materials.Add(getMaterialInfos(mat));
            foreach (var b in m.bones)
                data.boneNames.Add(b.name);
            data.sharedMesh = m.sharedMesh;
            data.rootBoneName = m.rootBone != null ? m.rootBone.name : "null";
            return data;
        }

        static CustomInfoMeshFilter getMeshFilterInfos(MeshFilter m)
        {
            CustomInfoMeshFilter data = new CustomInfoMeshFilter();
            data.mesh = m.mesh;
            data.sharedMesh = m.sharedMesh;
            return data;
        }

        static MaterialInfo getMaterialInfos(Material m)
        {
            MaterialInfo data = new MaterialInfo();
            if (m == null)
                return data;
            data.materialName = m.name;
            data.materialShaderName = m.shader != null ? m.shader.name : "null";

            for (int i = 0; i < 100000; i++)
            {
                if (m.HasProperty(i))
                {
                    MaterialAttributeInfo attribute = new MaterialAttributeInfo();
                    attribute.index = i;
                    attribute.vector = m.GetVector(i);
                    attribute.vectorArray = m.GetVectorArray(i);
                    attribute.floatValue = m.GetFloat(i);
                    attribute.floatArray = m.GetFloatArray(i);
                    attribute.matrix = m.GetMatrix(i);
                    attribute.matrixarray = m.GetMatrixArray(i);
                    attribute.texture = m.GetTexture(i);

                    data.attributes.Add(attribute);
                }
            }

            return data;
        }

        static CustomInfoColorChanger getColorChangerInfos(ColorChanger c)
        {
            CustomInfoColorChanger data = new CustomInfoColorChanger();
            data.rendererChangers = c.rendererChangers_;
            return data;
        }

        static CustomInfoJetFlame getJetFlameInfos(JetFlame f)
        {
            CustomInfoJetFlame data = new CustomInfoJetFlame();
            data.rotationAxis = f.rotationAxis_;
            return data;
        }
    }
}
