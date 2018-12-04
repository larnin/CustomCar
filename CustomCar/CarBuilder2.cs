using Spectrum.API;
using Spectrum.API.Experimental;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace CustomCar
{
    public class CarBuilder2
    {
        class CreateCarReturnInfos
        {
            public GameObject car;
            public CarColors colors;

        }

        CarInfos m_infos;

        public void createCars(CarInfos infos)
        {
            m_infos = infos;
            var cars = loadAssetsBundle();

            foreach (var car in cars)
            {
                var carInfos = createCar(car);
            }
                

        }

        List<GameObject> loadAssetsBundle()
        {
            var dirStr = Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), Defaults.PrivateAssetsDirectory);
            var files = Directory.GetFiles(dirStr);

            List<GameObject> cars = new List<GameObject>();

            foreach (var f in files)
            {
                var index = f.LastIndexOf('/');
                if (index < 0)
                    index = f.LastIndexOf('\\');
                var name = f.Substring(index + 1);
                var asset = new Assets(name);

                GameObject car = null;
                foreach (var n in asset.Bundle.GetAllAssetNames())
                {
                    if (car == null && n.EndsWith(".prefab"))
                    {
                        car = asset.Bundle.LoadAsset<GameObject>(n);
                        break;
                    }
                }

                if (car == null)
                    ErrorList.add("Can't find a prefab in the asset bundle " + f);
                else
                {
                    car.name = name;
                    cars.Add(car);
                }
            }

            return cars;
        }

        CreateCarReturnInfos createCar(GameObject car)
        {
            var infos = new CreateCarReturnInfos();

            var obj = GameObject.Instantiate(m_infos.baseCar);
            obj.name = car.name;
            GameObject.DontDestroyOnLoad(obj);
            obj.SetActive(false);

            removeOldCar(obj);
            var newCar = addNewCarOnPrefab(obj, car);
            setCarDatas(obj, newCar);

            return infos;
        }

        void removeOldCar(GameObject obj)
        {
            List<GameObject> wheelsToRemove = new List<GameObject>();
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var c = obj.transform.GetChild(i).gameObject;
                if (c.name.ToLower().Contains("wheel"))
                    wheelsToRemove.Add(c);
            }
            if (wheelsToRemove.Count != 4)
                ErrorList.add("Found " + wheelsToRemove.Count + " wheels on base prefabs, expected 4");

            var refractor = obj.transform.Find("Refractor");
            if (refractor == null)
            {
                ErrorList.add("Can't find the Refractor object on the base car prefab");
                return;
            }
            GameObject.Destroy(refractor.gameObject);
            foreach (var w in wheelsToRemove)
                GameObject.Destroy(w);
        }

        GameObject addNewCarOnPrefab(GameObject obj, GameObject car)
        {
            return GameObject.Instantiate(car, obj.transform);
        }

        void setCarDatas(GameObject obj, GameObject car)
        {
            setColorChanger(obj.GetComponent<ColorChanger>(), car);
            setCarVisuals(obj.GetComponent<CarVisuals>(), car);
        }

        void setColorChanger(ColorChanger colorChanger, GameObject car)
        {
            if(colorChanger == null)
            {
                ErrorList.add("Can't find the ColorChanger component on the base car");
                return;
            }

            colorChanger.rendererChangers_ = new ColorChanger.RendererChanger[0];
            foreach (var r in car.GetComponentsInChildren<Renderer>())
            {
                replaceMaterials(r);
                if (colorChanger != null)
                    addMaterialColorChanger(colorChanger, r.transform);
            }
        }

        class MaterialPropertyExport
        {
            public enum PropertyType
            {
                Color,
                ColorArray,
                Float,
                FloatArray,
                Int,
                Matrix,
                MatrixArray,
                Texture,
                TextureOffset,
                TextureScale,
                Vector,
                VectorArray,
            }

            public string fromName;
            public string toName;
            public int fromID = -1;
            public int toID = -1;
            public PropertyType type;
        }

        void replaceMaterials(Renderer r)
        {
            string[] matNames = new string[r.materials.Length];
            for (int i = 0; i < matNames.Length; i++)
                matNames[i] = "Wheel";
            List<MaterialPropertyExport>[] matProperties = new List<MaterialPropertyExport>[r.materials.Length];
            for (int i = 0; i < matProperties.Length; i++)
                matProperties[i] = new List<MaterialPropertyExport>();

            fillMaterialInfos(r, matNames, matProperties);

            //todo change materials type from infos
        }

        void fillMaterialInfos(Renderer r, string[] matNames, List<MaterialPropertyExport>[] matProperties)
        {
            int childCount = r.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                var name = r.transform.GetChild(i).name.ToLower();
                if (!name.StartsWith("#"))
                    continue;
                name = name.Remove(0, 1);
                var s = name.Split(';');
                if (s.Length == 0)
                    continue;
                if (s[0].Contains("mat"))
                {
                    if (s.Length != 3)
                    {
                        ErrorList.add(s[0] + " property on " + r.gameObject.FullName() + " must have 2 arguments");
                        continue;
                    }
                    int index = 0;
                    if (!int.TryParse(s[1], out index))
                    {
                        ErrorList.add("First argument of " + s[0] + " on " + r.gameObject.FullName() + " property must be a number");
                        continue;
                    }
                    if (index < matNames.Length)
                        matNames[index] = s[2];
                }
                else if (s[0].Contains("export"))
                {
                    if (s.Length != 5)
                    {
                        ErrorList.add(s[0] + " property on " + r.gameObject.FullName() + " must have 3 arguments");
                        continue;
                    }
                    int index = 0;
                    if (!int.TryParse(s[1], out index))
                    {
                        ErrorList.add("First argument of " + s[0] + " on " + r.gameObject.FullName() + " property must be a number");
                        continue;
                    }
                    if (index >= matNames.Length)
                        continue;
                    MaterialPropertyExport p = new MaterialPropertyExport();
                    bool found = false;

                    foreach (MaterialPropertyExport.PropertyType pType in Enum.GetValues(typeof(MaterialPropertyExport.PropertyType)))
                    {
                        if (s[2] == pType.ToString().ToLower())
                        {
                            found = true;
                            p.type = pType;
                            break;
                        }
                    }
                    if (!found)
                    {
                        ErrorList.add("The property " + s[2] + " on " + r.gameObject.FullName() + " is not valid");
                        continue;
                    }
                    if (!int.TryParse(s[3], out p.fromID))
                        p.fromName = s[3];
                    if (!int.TryParse(s[4], out p.toID))
                        p.toName = s[4];

                    matProperties[index].Add(p);
                }
            }
        }
        
        void addMaterialColorChanger(ColorChanger colorChanger, Transform t)
        {

        }

        void setCarVisuals(CarVisuals visuals, GameObject car)
        {
            if(visuals == null)
            {
                ErrorList.add("Can't find the CarVisuals component on the base car");
                return;
            }
        }
    }
}
