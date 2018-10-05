using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class MapSetter : EditorWindow
{
    static List<int> mapSizeValue = new List<int> { 256, 512, 1024 };
    static MapSettings ms;
    static GUIStyle errorStyle = new GUIStyle();

    eMapDepthDimession eDepthDimess = eMapDepthDimession.DepthZ;
    string textureSavePathRelativeToProject = "MapAssets/MapTextures";
    string mapDataSavePathRelativeToProject = "MapAssets/MapData";
    string error = "";

    [MenuItem("MapTool/MapSettings")]
    static void OpenWindow()
    {
        MapSetter window = EditorWindow.GetWindow<MapSetter>();
        Initialize();
    }

    static void Initialize()
    {
        string path = string.Format(@"{0}/{1}", Application.dataPath, MapSettings.mapSettingsProfilesPath);
        if (FileHelp.FileExist(path))
        {
            ms = FileHelp.LoadJsonFormatObject<MapSettings>(path);
            EditorWindow.GetWindow<MapSetter>().InitWithExistProfiler();
        }
        errorStyle.normal.textColor = Color.red;
    }

    public void InitWithExistProfiler()
    {
        if (ms != null)
        {
            textureSavePathRelativeToProject = ms.textureSavePathRelativeToProject;
            mapDataSavePathRelativeToProject = ms.mapDataSavePathRelativeToProject;
            eDepthDimess = ms.mapDepthDimession;
        }
    }

    void OnGUI()
    {
        eDepthDimess = (eMapDepthDimession)EditorGUILayout.EnumPopup("Depth dimession", eDepthDimess);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("map texture save path:");
        textureSavePathRelativeToProject = EditorGUILayout.TextField(textureSavePathRelativeToProject);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("map data save path:");
        mapDataSavePathRelativeToProject = EditorGUILayout.TextField(mapDataSavePathRelativeToProject);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Apply"))
        {
            try
            {
                if (Validate())
                    Generate();
                this.Close();
            }
            catch (System.Exception e)
            {
                error = e.Message;
            }
        }
        if (GUILayout.Button("Cancel"))
        {
            this.Close();
        }
        EditorGUILayout.EndHorizontal();

       
        GUILayout.Label(error, errorStyle);
    }

    void Generate()
    {
        MapSettings ms = new MapSettings(eDepthDimess, textureSavePathRelativeToProject, 
            mapDataSavePathRelativeToProject);
        FileHelp.SaveStringFile(JsonConvert.SerializeObject(ms),
            FileHelp.GetFullPathByDataPathRelativePath(MapSettings.mapSettingsProfilesPath));
        AssetDatabase.Refresh();
    }

    bool Validate()
    {
        return ValidateSavePath();
    }

    bool ValidateSavePath()
    {
        bool result = false;
        if (!FileHelp.FolderExistInDataPathRelativePath(textureSavePathRelativeToProject))
            throw new System.Exception("the save path for map textures does not exists!\n" +
                FileHelp.GetFullPathByDataPathRelativePath(textureSavePathRelativeToProject));
        else if (!FileHelp.FolderExistInDataPathRelativePath(mapDataSavePathRelativeToProject))
            throw new System.Exception("the save path for map data does not exists!\n" +
                FileHelp.GetFullPathByDataPathRelativePath(mapDataSavePathRelativeToProject));
        else
            result = true;
        return result;
    }
}
