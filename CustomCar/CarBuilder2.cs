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

            return infos;
        }
    }
}
