using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using CustomCar;

public class ShaderTool : MonoBehaviour
{
    static ShaderTool m_instance = null;

    List<ShaderInfo> m_shaderInfos = null;

    GameObject m_car = null;
    GameObject m_screen = null;
    List<Material> m_compatibleMaterials = new List<Material>();

    CustomCar.ComboBox m_comboMaterial;

    public static void Instanciate()
    {
        if(m_instance != null)
        {
            Console.Out.WriteLine("ShaderTool is already instanciated");
            return;
        }

        var obj = new GameObject("ShaderTool");
        var tool = obj.AddComponent<ShaderTool>();
        m_instance = tool;

        DontDestroyOnLoad(obj);
    }

    public static void Destroy()
    {
        if(m_instance == null)
        {
            Console.Out.WriteLine("The ShaderTool is not instanciated, create it before destroying");
            return;
        }

        Destroy(m_instance.gameObject);
        m_instance = null;
    }

    void Awake()
    {
        m_shaderInfos = SerializeShaderListInfos.Read("ShaderInfos");

        GUIContent[] comboList = new GUIContent[1];
        comboList[0] = new GUIContent("None");
        m_comboMaterial = new CustomCar.ComboBox(comboList);
    }
    
    void Update()
    {
        if(m_car == null)
        {
            FindCar();
            if (m_car != null)
            {
                m_compatibleMaterials.Clear();
                UpdateMaterialList(m_car);
                UpdateMaterialList(m_screen);
            }
        }
    }

    void OnGUI()
    {
        GUILayout.Window(0, new Rect(10, 10, 400, 600), UpdateWindow, "Shader tool", GUILayout.ExpandWidth(true));
        
    }

    static Vector2 m_scrollPos = new Vector2();

    void UpdateWindow(int ID)
    {
        if (m_car == null)
        {
            GUILayout.Label("No local car found");
        }
        else if (m_compatibleMaterials.Count == 0)
        {
            GUILayout.Label("No compatible material found");
        }
        else
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Materials");
            UpdateComboMaterials();
            int materialIndex = m_comboMaterial.Show();

            GUILayout.EndHorizontal();

            if (materialIndex < 0 || materialIndex >= m_compatibleMaterials.Count)
                GUILayout.Label("Invalid material index");
            else
            {
                GUILayout.BeginScrollView(m_scrollPos);
                PrintMaterial(m_compatibleMaterials[materialIndex]);
                GUILayout.EndScrollView();
            }
        }
    }

    void FindCar()
    {
        m_car = G.Sys.PlayerManager_?.Current_?.playerData_?.Car_;
        m_screen = G.Sys.PlayerManager_?.Current_?.playerData_?.CarScreen_;

        if (m_car != null)
            Console.Out.WriteLine("Car found");
    }

    void UpdateMaterialList(GameObject obj)
    {
        if (obj == null)
            return;

        var renderers = obj.GetComponentsInChildren<Renderer>();

        foreach(var r in renderers)
        {
            foreach(var m in r.sharedMaterials)
            {
                if (m == null || m.shader == null)
                    continue;

                var name = m.shader.name;

                var shader = m_shaderInfos.Find(x => { return x.name == name; });
                if (shader == null)
                    continue;

                if (m_compatibleMaterials.Contains(m))
                    continue;

                m_compatibleMaterials.Add(m);
            }
        }

        Console.Out.WriteLine("Found " + m_compatibleMaterials.Count + " compatible materials");
    }

    void UpdateComboMaterials()
    {
        m_compatibleMaterials.RemoveAll(x => { return x == null; });

        GUIContent[] comboList = new GUIContent[m_compatibleMaterials.Count];
        for (int i = 0; i < m_compatibleMaterials.Count; i++)
            comboList[i] = new GUIContent(m_compatibleMaterials[i].name);

        m_comboMaterial.SetContent(comboList);
    }

    void PrintMaterial(Material m)
    {
        if(m.shader == null)
        {
            GUILayout.Label("Null material shader");
            return;
        }

        var s = m_shaderInfos.Find(x => { return x.name == m.shader.name; });

        if(s == null)
        {
            GUILayout.Label("No valid template found for this material shader " + m.shader.name);
            return;
        }

        PrintMaterial(m, s);
    }

    void PrintMaterial(Material m, ShaderInfo s)
    {
        GUILayout.Label("Shader " + s.name);

        foreach(var u in s.uniforms)
        {
            switch(u.type)
            {
                case UniformType.Color:
                    PrintColor(m, u.name);
                    break;
                case UniformType.Float:
                    PrintFloat(m, u.name);
                    break;
                case UniformType.Int:
                    PrintInt(m, u.name);
                    break;
                case UniformType.Vector:
                    PrintVector(m, u.name);
                    break;
                default:
                    PrintInvalid(m, u.name);
                    break;
            }
        }
    }

    void PrintInvalid(Material m, string name)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(name + ": ");
        GUIStyle red = new GUIStyle(GUIStyle.none);
        red.normal.textColor = Color.red;
        GUILayout.Label("Type not supported yet", red);

        GUILayout.EndHorizontal();
    }

    void PrintColor(Material m, string name)
    {
        var color = m.GetColor(name);

        GUILayout.BeginHorizontal();
        GUILayout.Label(name + ": ");

        var rText = GUILayout.TextField(color.r.ToString());
        var gText = GUILayout.TextField(color.g.ToString());
        var bText = GUILayout.TextField(color.b.ToString());
        var aText = GUILayout.TextField(color.a.ToString());

        float value = 0;
        if (float.TryParse(rText, out value))
            color.r = value;
        if (float.TryParse(gText, out value))
            color.g = value;
        if (float.TryParse(bText, out value))
            color.b = value;
        if (float.TryParse(aText, out value))
            color.a = value;

        m.SetColor(name, color);

        GUILayout.EndHorizontal();
    }

    void PrintFloat(Material m, string name)
    {
        var value = m.GetFloat(name);

        GUILayout.BeginHorizontal();
        GUILayout.Label(name + ": ");

        var vText = GUILayout.TextField(value.ToString());

        if (float.TryParse(vText, out value))
            m.SetFloat(name, value);

        GUILayout.EndHorizontal();
    }

    void PrintInt(Material m, string name)
    {
        var value = m.GetInt(name);

        GUILayout.BeginHorizontal();
        GUILayout.Label(name + ": ");

        var vText = GUILayout.TextField(value.ToString());

        if (int.TryParse(vText, out value))
            m.SetInt(name, value);

        GUILayout.EndHorizontal();
    }

    void PrintVector(Material m, string name)
    {
        var vector = m.GetVector(name);

        GUILayout.BeginHorizontal();
        GUILayout.Label(name + ": ");

        var xText = GUILayout.TextField(vector.x.ToString());
        var yText = GUILayout.TextField(vector.y.ToString());
        var zText = GUILayout.TextField(vector.z.ToString());
        var wText = GUILayout.TextField(vector.w.ToString());

        float value = 0;
        if (float.TryParse(xText, out value))
            vector.x = value;
        if (float.TryParse(yText, out value))
            vector.y = value;
        if (float.TryParse(zText, out value))
            vector.z = value;
        if (float.TryParse(wText, out value))
            vector.w = value;

        m.SetVector(name, vector);

        GUILayout.EndHorizontal();
    }
}
