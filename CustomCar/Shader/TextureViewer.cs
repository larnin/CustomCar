using UnityEngine;
using System.Collections;
using System;
using System.Windows.Forms;

public class TextureViewer : MonoBehaviour
{
    static TextureViewer m_instance = null;
    public static TextureViewer instance { get { return m_instance; } }

    Texture m_texture = null;

    public static void Instanciate()
    {
        if (m_instance != null)
        {
            Console.Out.WriteLine("TextureViewer is already instanciated");
            return;
        }

        var obj = new GameObject("ShaderTool");
        var tool = obj.AddComponent<TextureViewer>();
        m_instance = tool;

        DontDestroyOnLoad(obj);
    }

    public static void Destroy()
    {
        if (m_instance == null)
        {
            Console.Out.WriteLine("The TextureViewer is not instanciated, create it before destroying");
            return;
        }

        Destroy(m_instance.gameObject);
        m_instance = null;
    }

    public void SetTexture(Texture texture)
    {
        m_texture = texture;
    }

    void OnGUI()
    {
        GUILayout.Window(1, new Rect(410, 10, 400, 400), UpdateWindow, "Texuture viewer", GUILayout.MaxWidth(400), GUILayout.MaxHeight(400));
    }

    void UpdateWindow(int ID)
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Close"))
        {
            Close();
            return;
        }
        if (GUILayout.Button("Save"))
            Save();
        GUILayout.EndHorizontal();

        if (m_texture == null)
            GUILayout.Label("No texture");
        else GUILayout.Box(m_texture, GUILayout.MaxWidth(400), GUILayout.MaxHeight(400));

    }

    void Close()
    {
        m_texture = null;
        Destroy(gameObject);
    }

    void Save()
    {
        if (m_texture == null)
            return;

        using (SaveFileDialog dialog = new SaveFileDialog())
        {
            dialog.InitialDirectory = "c://";
            dialog.Filter = "Image files (*.png) | *.png";
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ObjExporter.saveTextureToFile(m_texture, dialog.FileName);
            }
        }
    }
}
