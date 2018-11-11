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
            foreach (var w in wheelsToRemove)
                GameObject.Destroy(w);

            var refractor = obj.transform.Find("Refractor");
            if(refractor == null)
            {
                Console.Out.WriteLine("Can't remove refractor object");
                return;
            }

            GameObject.Destroy(refractor.gameObject);

            var newcar = GameObject.Instantiate(m_data.carToAdd, obj.transform);
            foreach (var r in newcar.GetComponentsInChildren<Renderer>())
                replaceMaterials(r);
            
            var carVisual = obj.GetComponent<CarVisuals>();
            carVisual.rotationJetFlames_ = new JetFlame[0];
            carVisual.boostJetFlames_ = new JetFlame[0];
            carVisual.wingJetFlames_ = new JetFlame[0];
            carVisual.lights_ = new Light[0];
            carVisual.driverPosition_ = null;
            carVisual.carBodyRenderer_ = newcar.GetComponentInChildren<SkinnedMeshRenderer>();
            List<Transform> wheelsToMove = new List<Transform>();
            for(int i = 0; i < newcar.transform.childCount; i++)
            {
                var c = newcar.transform.GetChild(i).gameObject;
                var name = c.name.ToLower();
                if (name.Contains("wheel"))
                {
                    wheelsToMove.Add(c.transform);
                    var wheelRenderer = c.GetComponentInChildren<MeshRenderer>();
                    CarWheelVisuals comp /*= null;
                    if(wheelRenderer != null)
                    {
                        comp = wheelRenderer.gameObject.AddComponent<CarWheelVisuals>();
                        comp.tire_ = wheelRenderer;
                    }
                    else comp */= c.AddComponent<CarWheelVisuals>();
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
            }
            foreach(var c in wheelsToMove)
                c.transform.parent = obj.transform;
        }

        void replaceMaterials(Renderer r)
        {
            Shader s = Shader.Find("Custom/Reflective/Bump Glow LaserCut");
            if (s == null)
            {
                Console.Out.WriteLine("Can't find shader !");
                return;
            }

            List<Material> newMaterials = new List<Material>();
            foreach(var m in r.materials)
            {
                Material mat = new Material(s);
                mat.SetTexture(5, m.GetTexture("_MainTex")); //diffuse
                mat.SetTexture(255, m.GetTexture("_EmissionMap")); //emissive
                mat.SetTexture(218, m.GetTexture("_BumpMap")); //normal
                newMaterials.Add(mat);
            }

            for(int i = 0; i < r.materials.Length; i++)
                r.materials[i] = newMaterials[i];
        }
    }
}
