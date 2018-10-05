using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class MapGenerator : EditorWindow
{
    const string mapSettingsProfilesPath = "MapAssets/Profiles/MapSetting.json";
    static string[] mapSizeOptions;
    static List<int> mapSizeValue = new List<int> { 256, 512, 1024 };
    static MapSettings ms;

    [MenuItem("MapTool/MapGenerator")]
    static void OpenWindow()
    {
        MapGenerator window = EditorWindow.GetWindow<MapGenerator>();
        Initialize();
        InitMapSettings(window);
    }

    float findBoundProgress;
    string mapName;
    
    int mapSizeIdx = 0;
    Camera mapCamera;
    //float mapWidth, mapHeight;

    static void Initialize()
    {
        InitMapSizeOption();
    }

    static void InitMapSizeOption()
    {
        mapSizeOptions = new string[mapSizeValue.Count];
        for (int i = 0; i < mapSizeValue.Count; i++)
            mapSizeOptions[i] = string.Format("{0}*{0}", mapSizeValue[i].ToString());
    }

    static void InitMapSettings(MapGenerator window)
    {
        if (FileHelp.FileExistInDataPathRelativePath(mapSettingsProfilesPath))
        {
            ms = FileHelp.LoadJsonFormatObjectByDataPathRelativePath<MapSettings>
                (mapSettingsProfilesPath);
        }
        else
        {
            EditorWindow.GetWindow<MapGenerator>().Close();
            throw new System.Exception(@"easy map cannot find map settings, did you forgot to generate one,please check 'MapTool/MapSettings'.");
        }
    }

    void OnGUI()
    {
        //EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField("Map Width:");
        //mapWidth = EditorGUILayout.FloatField(mapWidth);
        //EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField("Map Height:");
        //mapHeight = EditorGUILayout.FloatField(mapHeight);
        //EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Map Name:");
        mapName = EditorGUILayout.TextField(mapName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Map Texture Size:");
        mapSizeIdx = EditorGUILayout.Popup(mapSizeIdx, mapSizeOptions);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Generate"))
        {
            Generate();
        }
    }

    void Generate()
    {
        Collider collider = FindMaxColliderInScene();
        if (collider != null)
        {
            GenerateMapTexture(collider);
            GenerateMapData(collider);
            EndingWork();
        }
        else
        {
            throw new System.Exception("场景中没有找到任何包围盒，无法确定场景尺寸");
        }
    }

    #region Generate Map Data
    void GenerateMapData(Collider collider)
    {
        Vector3 pos = mapCamera.transform.position;
        MapData md = new MapData(mapCamera.orthographicSize * 2, mapCamera.orthographicSize * 2,
            pos.x - mapCamera.orthographicSize,
            pos.GetDepth(ms.mapDepthDimession) - mapCamera.orthographicSize);
        SaveMapDataAsJsonFormatTextFile(Newtonsoft.Json.JsonConvert.SerializeObject(md));
    }

    void SaveMapDataAsJsonFormatTextFile(string jsonFormat)
    {
        string path = string.Format(@"{0}/{1}/{2}.json", Application.dataPath, 
            ms.mapDataSavePathRelativeToProject, mapName);
        StreamWriter sw = File.CreateText(path);
        sw.Write(jsonFormat);
        sw.Close();

        AssetDatabase.Refresh();
    }
    #endregion

    #region Generate Map Texture
    void GenerateMapTexture(Collider collider)
    {
        mapCamera = CreateOrthonormalCamera(collider);
        byte[] bytes = GetTextureFromCamera(mapCamera);
        SaveTextureAsSpriteAssets(bytes);
    }

    Collider FindMaxColliderInScene()
    {
        EditorUtility.DisplayProgressBar("Finding Bounds", "Finding biggest colider box as map bounds", findBoundProgress);
        findBoundProgress = 0;
        GameObject[] gos = GameObject.FindObjectsOfType<GameObject>() as GameObject[];
        float maxVolumn = 0,curVolumn = 0;
        Collider collider, result = null;
        for (int i = 0; i < gos.Length; i++)
        {
            collider = gos[i].GetComponent<Collider>();
            if (collider != null)
            {
                curVolumn = GetAcreageByCollider(collider);
                if (curVolumn > maxVolumn)
                {
                    maxVolumn = curVolumn;
                    result = collider;
                }
            }
            findBoundProgress = i / (float)gos.Length;
        }
        EditorUtility.ClearProgressBar();
        return result;
    }

    float GetAcreageByCollider(Collider collider)
    {
        float boundWidth = collider.bounds.size.x;
        float boundHeight = collider.bounds.size.GetDepth(ms.mapDepthDimession);

        return boundWidth * boundHeight;
    }

    Camera CreateOrthonormalCamera(Collider collider)
    {
        GameObject mapCamera = new GameObject();
        OperateCameraTransform(mapCamera, collider);

        mapCamera.name = "MapCamera";
        Camera cam = mapCamera.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = Mathf.Max(collider.bounds.size.x, 
            collider.bounds.size.GetDepth(ms.mapDepthDimession)) / 2;
        cam.nearClipPlane = 0.3f;
        cam.farClipPlane = collider.bounds.size.GetHeight(ms.mapDepthDimession) + 11;

        return cam;
    }

    void OperateCameraTransform(GameObject camObj, Collider sceneBound)
    {
        Vector3 camPos = sceneBound.bounds.center;

        float sceneHeight = sceneBound.bounds.size.GetHeight(ms.mapDepthDimession);
        camObj.transform.position = camPos.SetHeight(ms.mapDepthDimession,
            sceneHeight + (ms.mapDepthDimession == eMapDepthDimession.DepthY ? -10 : 10));

        if (ms.mapDepthDimession == eMapDepthDimession.DepthZ)
            camObj.transform.Rotate(new Vector3(90, 0, 0));
    }

    byte[] GetTextureFromCamera(Camera camera)
    {
        int mapSize = mapSizeValue[mapSizeIdx];
        RenderTexture result = new RenderTexture(mapSize, mapSize, 24, RenderTextureFormat.ARGB32);
        camera.targetTexture = result;
        camera.Render();
        RenderTexture.active = result;

        Texture2D t2D = new Texture2D(result.width, result.height);

        t2D.ReadPixels(new Rect(0, 0, t2D.width, t2D.height), 0, 0);
        t2D.Apply();
        byte[] bytes = t2D.EncodeToPNG();

        return bytes;
    }

    void SaveTextureAsSpriteAssets(byte[] bytes)
    {
        string path = string.Format(@"{0}/{1}/{2}.png", Application.dataPath, 
            ms.textureSavePathRelativeToProject, mapName);
        FileStream fs = File.Create(path);
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
        AssetDatabase.Refresh();

        TextureImporter ti = TextureImporter.GetAtPath(string.Format(@"Assets/{0}/{1}.png",
            ms.textureSavePathRelativeToProject, mapName)) as TextureImporter;
        ti.textureType = TextureImporterType.Sprite;
        ti.SaveAndReimport();
    }
    #endregion

    void EndingWork()
    {
        DestroyImmediate(mapCamera.gameObject);
    }
}
