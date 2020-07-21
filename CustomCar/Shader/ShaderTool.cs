using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using CustomCar;
using System.Windows.Forms;

public class ShaderTool : MonoBehaviour
{
    class MaterialInfo
    {
        public Material material;
        public string objectName;
        public int index;
    }

    static ShaderTool m_instance = null;
    static public ShaderTool instance { get { return m_instance; } }

    List<ShaderInfo> m_shaderInfos = null;

    List<GameObject> m_selectedObjects = new List<GameObject>();
    List<MaterialInfo> m_compatibleMaterials = new List<MaterialInfo>();

    CustomCar.ComboBox m_comboMaterial;
    Vector2 m_scrollPos = new Vector2();

    ControleList<string> m_stringList = new ControleList<string>("0");
    ControleList<bool> m_focusedList = new ControleList<bool>(false);


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
        if(m_selectedObjects.Count == 0 || m_selectedObjects[0] == null)
        { 
            FindCar();
            if (m_selectedObjects.Count > 0 && m_selectedObjects[0] != null)
            {
                m_compatibleMaterials.Clear();
                foreach(var o in m_selectedObjects)
                    UpdateMaterialList(o);
            }
        }
    }

    void OnGUI()
    {
        GUILayout.Window(0, new Rect(10, 10, 400, 600), UpdateWindow, "Shader tool", GUILayout.MaxWidth(400), GUILayout.MaxHeight(600));
    }

    void UpdateWindow(int ID)
    {
        m_stringList.ResetIndex();
        m_focusedList.ResetIndex();

        if (m_selectedObjects.Count == 0 || m_selectedObjects[0] == null)
        {
            if (GUILayout.Button("Close"))
                OnClose();
            GUILayout.Label("No local car found");
        }
        else if (m_compatibleMaterials.Count == 0)
        {
            if (GUILayout.Button("Close"))
                OnClose();
            GUILayout.Label("No compatible material found");
        }
        else
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close"))
                OnClose();
            if (GUILayout.Button("Export"))
                OnExportClick();
            if (GUILayout.Button("Import"))
                OnImportClick();
            if (GUILayout.Button(new GUIContent("Apply prefab", "Apply this configuration to the prefab car.\nIt allow to keep theses parameters if the car is destroyed")))
                OnApplyPrefab();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Materials");
            UpdateComboMaterials();
            int oldComboIndex = m_comboMaterial.SelectedItemIndex;
            int materialIndex = m_comboMaterial.Show();

            GUILayout.EndHorizontal();

            if (materialIndex < 0 || materialIndex >= m_compatibleMaterials.Count)
                GUILayout.Label("Invalid material index");
            else
            {
                m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);
                PrintMaterial(m_compatibleMaterials[materialIndex]);
                GUILayout.EndScrollView();
            }

            m_comboMaterial.DisplayList();
            if(m_comboMaterial.SelectedItemIndex != oldComboIndex)
            {
                m_stringList.Reset("0");
                m_focusedList.Reset(false);
            }
        }
    }

    void FindCar()
    {
        m_selectedObjects.Clear();
        m_selectedObjects.Add(G.Sys.PlayerManager_?.Current_?.playerData_?.Car_);
        m_selectedObjects.Add(G.Sys.PlayerManager_?.Current_?.playerData_?.CarScreen_);

        if (m_selectedObjects[0] != null)
            Console.Out.WriteLine("Car found");
    }

    void UpdateMaterialList(GameObject obj)
    {
        if (obj == null)
            return;

        var renderers = obj.GetComponentsInChildren<Renderer>();

        foreach(var r in renderers)
        {
            for(int i = 0; i < r.sharedMaterials.Length; i++)
            {
                var m = r.sharedMaterials[i];

                if (m == null || m.shader == null)
                    continue;

                var name = m.shader.name;

                var shader = m_shaderInfos.Find(x => { return x.name == name; });
                if (shader == null)
                    continue;

                var mat = new MaterialInfo();
                mat.material = m;
                mat.index = i;
                mat.objectName = r.gameObject.FullNameWithoutRoot();

                if (m_compatibleMaterials.Exists(x => { return x.material == m; }))
                {
                    Console.Out.WriteLine("Mat already found");
                    continue;
                }

                m_compatibleMaterials.Add(mat);
            }
        }

        Console.Out.WriteLine("Found " + m_compatibleMaterials.Count + " compatible materials");
    }

    void UpdateComboMaterials()
    {
        m_compatibleMaterials.RemoveAll(x => { return x == null; });

        GUIContent[] comboList = new GUIContent[m_compatibleMaterials.Count];
        for (int i = 0; i < m_compatibleMaterials.Count; i++)
            comboList[i] = new GUIContent(m_compatibleMaterials[i].material.name);

        m_comboMaterial.SetContent(comboList);
    }

    void PrintMaterial(MaterialInfo m)
    {
        if(m.material.shader == null)
        {
            GUILayout.Label("Null material shader");
            return;
        }

        var s = m_shaderInfos.Find(x => { return x.name == m.material.shader.name; });

        if(s == null)
        {
            GUILayout.Label("No valid template found for this material shader " + m.material.shader.name);
            return;
        }

        PrintMaterial(m, s);
    }

    void PrintMaterial(MaterialInfo m, ShaderInfo s)
    {
        GUILayout.Label("Shader: " + s.name);
        GUILayout.Label("Object: " + m.objectName + " - " + m.index);

        foreach(var u in s.uniforms)
        {
            switch(u.type)
            {
                case UniformType.Color:
                    PrintColor(m.material, u.name);
                    break;
                case UniformType.Float:
                    PrintFloat(m.material, u.name);
                    break;
                case UniformType.Int:
                    PrintInt(m.material, u.name);
                    break;
                case UniformType.Vector:
                    PrintVector(m.material, u.name);
                    break;
                case UniformType.Texture:
                    PrintTexture(m.material, u.name);
                    break;
                default:
                    PrintInvalid(m.material, u.name);
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

        float value;
        if (ShowFloatField(color.r, out value))
            color.r = value;
        if (ShowFloatField(color.g, out value))
            color.g = value;
        if (ShowFloatField(color.b, out value))
            color.b = value;
        if (ShowFloatField(color.a, out value))
            color.a = value;

        m.SetColor(name, color);

        GUILayout.EndHorizontal();
    }

    void PrintFloat(Material m, string name)
    {
        var value = m.GetFloat(name);

        GUILayout.BeginHorizontal();
        GUILayout.Label(name + ": ");

        float getValue;
        if (ShowFloatField(value, out getValue))
            m.SetFloat(name, getValue);

        GUILayout.EndHorizontal();
    }

    void PrintInt(Material m, string name)
    {
        var value = m.GetInt(name);

        GUILayout.BeginHorizontal();
        GUILayout.Label(name + ": ");

        int getValue;
        if (ShowIntField(value, out getValue))
            m.SetInt(name, getValue);
        
        GUILayout.EndHorizontal();
    }

    void PrintVector(Material m, string name)
    {
        var vector = m.GetVector(name);

        GUILayout.BeginHorizontal();
        GUILayout.Label(name + ": ");

        float value;
        if (ShowFloatField(vector.x, out value))
            vector.x = value;
        if (ShowFloatField(vector.y, out value))
            vector.y = value;
        if (ShowFloatField(vector.z, out value))
            vector.z = value;
        if (ShowFloatField(vector.w, out value))
            vector.w = value;
        
        m.SetVector(name, vector);

        GUILayout.EndHorizontal();
    }

    void PrintTexture(Material m, string name)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(name + ": ");

        if (GUILayout.Button("View"))
            OnTextureView(m, name);

        if (GUILayout.Button("Select"))
            OnTextureSelect(m, name);

        GUILayout.EndHorizontal();
    }

    void OnTextureView(Material m, string name)
    {
        var texture = m.GetTexture(name);

        var viewer = TextureViewer.instance;
        if (viewer == null)
            TextureViewer.Instanciate();
        viewer = TextureViewer.instance;
        if (viewer == null)
            return;
        viewer.SetTexture(texture);
    }

    void OnTextureSelect(Material m, string name)
    {
        using (OpenFileDialog dialog = new OpenFileDialog())
        {
            dialog.InitialDirectory = "c:\\";
            dialog.Filter = "Image files (*.png) | *.png";
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var texture = Resource.LoadTextureFromFile(dialog.FileName, 1, 1);
                if(texture != null)
                {
                    m.SetTexture(name, texture);
                }
            }
        }
    }

    bool ShowFloatField(float inValue, out float outValue)
    {
        int focusIndex = m_focusedList.GetNextIndex();
        int stringIndex = m_stringList.GetNextIndex();

        var focusName = "Float" + stringIndex.ToString();

        GUI.SetNextControlName(focusName);

        bool hadFocus = m_focusedList.GetItem(focusIndex);

        var value = hadFocus ? m_stringList.GetItem(stringIndex) : inValue.ToString();

        var text = GUILayout.TextField(value);

        bool haveFocus = GUI.GetNameOfFocusedControl() == focusName;
        m_focusedList.SetItem(focusIndex, haveFocus);
        m_stringList.SetItem(stringIndex, text);

        bool upDateValue = false;

        if (hadFocus && !haveFocus) //looseFocus
            upDateValue = true;

        // it doesn't work well
        //if (haveFocus && Event.current.Equals(Event.KeyboardEvent("return")))
        //    upDateValue = true;

        if (upDateValue)
        {
            float parseValue;
            if (float.TryParse(text, out parseValue))
            {
                outValue = parseValue;
                m_stringList.SetItem(stringIndex, outValue.ToString());
                return true;
            }
            m_stringList.SetItem(stringIndex, inValue.ToString());
        }

        outValue = inValue;
        return false;
    }

    bool ShowIntField(int inValue, out int outValue)
    {
        int focusIndex = m_focusedList.GetNextIndex();
        int stringIndex = m_stringList.GetNextIndex();

        var focusName = "Int" + stringIndex.ToString();

        GUI.SetNextControlName(focusName);

        bool hadFocus = m_focusedList.GetItem(focusIndex);

        var value = hadFocus ? m_stringList.GetItem(stringIndex) : inValue.ToString();

        var text = GUILayout.TextField(value);

        bool haveFocus = GUI.GetNameOfFocusedControl() == focusName;
        m_focusedList.SetItem(focusIndex, haveFocus);
        m_stringList.SetItem(stringIndex, text);

        bool upDateValue = false;

        if (hadFocus && !haveFocus) //looseFocus
            upDateValue = true;

        // it doesn't work well
        //if (haveFocus && Event.current.Equals(Event.KeyboardEvent("return")))
        //    upDateValue = true;

        if (upDateValue)
        {
            int parseValue;
            if (int.TryParse(text, out parseValue))
            {
                outValue = parseValue;
                m_stringList.SetItem(stringIndex, outValue.ToString());
                return true;
            }
            m_stringList.SetItem(stringIndex, inValue.ToString());
        }

        outValue = inValue;
        return false;
    }

    void OnImportClick()
    {

    }

    void OnExportClick()
    {

    }

    void OnApplyPrefab()
    {
        var cars = G.Sys.PlayerManager_?.Current_?.playerData_?.carPrefabs_;
        if (cars == null)
            return;

        ApplyPrefab(cars.Value.carPrefab_);
        ApplyPrefab(cars.Value.cutsceneScreenPrefab_);
        ApplyPrefab(cars.Value.screenPrefab_);
    }

    void ApplyPrefab(GameObject prefab)
    {
        if (prefab == null)
            return;

        var renderers = prefab.GetComponentsInChildren<Renderer>();
        foreach(var r in renderers)
        {
            var path = r.gameObject.FullNameWithoutRoot();
            foreach (var m in m_compatibleMaterials)
            {
                if(m.objectName == path)
                {
                    var materials = r.materials;
                    if(materials.Length > m.index)
                    {
                        materials[m.index] = new Material(m.material);
                    }
                    r.materials = materials;
                }
            }
        }
    }

    void OnClose()
    {
        Destroy();
    }
}
