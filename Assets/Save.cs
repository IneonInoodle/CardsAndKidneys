using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;
using System.Text;

public class Save : MonoBehaviour {

	// Use this for initialization
	
    string message;
    string loadMessage = "Yeah working";
    string data;
    FileInfo f;

    void Start()
    {
        f = new FileInfo(Application.persistentDataPath  + "\\" + "myFile.txt");
        Screen.SetResolution(800, 600, true);
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, 500, 500));
        GUILayout.Label(message + " " + data);
        if (GUILayout.Button("Save"))
        {

            if (!f.Exists)
            {
                message = "Creating File";
                Savee();
            }
            else
            {
                message = "Saving";
                Savee();
            }
        }
        if (GUILayout.Button("Load"))
        {
            if (f.Exists)
            {
                Load();
            }
            else
            {
                message = "No File found";
            }
        }
        GUILayout.EndArea();
    }

    void Savee()
    {
        StreamWriter w;
        if (!f.Exists)
        {
            w = f.CreateText();
        }
        else
        {
            f.Delete();
            w = f.CreateText();
        }
        w.WriteLine(loadMessage);
        w.Close();
    }

    void Load()
    {
        StreamReader r = File.OpenText(Application.persistentDataPath  + "\\" + "myFile.txt");
        string info = r.ReadToEnd();
        r.Close();
        data = info;
    }
}
