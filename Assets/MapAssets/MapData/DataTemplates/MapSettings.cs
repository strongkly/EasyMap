using System.Collections.Generic;
using Newtonsoft.Json;

public enum eMapDepthDimession : int
{
    DepthY = 0,
    DepthZ = 1,
}

public class MapSettings
{
    public const string mapSettingsProfilesPath = "MapAssets/Profiles/MapSetting.json";

    public eMapDepthDimession mapDepthDimession;

    public List<int> MapTextureSizes = new List<int> { 256, 512, 1024};

    public string textureSavePathRelativeToProject
    {
        get;
        private set;
    }

    public string mapDataSavePathRelativeToProject
    {
        get;
        private set;
    }

    public MapSettings(eMapDepthDimession depthDimession, string textureSavePathRelativeToProject,
        string mapDataSavePathRelativeToProject)
    {
        this.mapDepthDimession = depthDimession;
        this.textureSavePathRelativeToProject = textureSavePathRelativeToProject;
        this.mapDataSavePathRelativeToProject = mapDataSavePathRelativeToProject;
    }
}
