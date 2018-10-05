using Newtonsoft.Json;

public class MapData : IMapData
{
    [JsonProperty]
    public float MapWidth
    {
        private set;
        get;
    }
    [JsonProperty]
    public float MapHeight
    {
        private set;
        get;
    }
    [JsonProperty]
    public float MapOffsetX
    {
        get;
        private set;
    }
    [JsonProperty]
    public float MapOffsetY
    {
        get;
        private set;
    }

    public MapData()
    {
    }

    public MapData(float mapWidth, float mapHeight, float offsetX, float offsetY)
    {
        this.MapWidth = mapWidth;
        this.MapHeight = mapHeight;
        this.MapOffsetX = offsetX;
        this.MapOffsetY = offsetY;
    }

    public float GetMapHeight()
    {
        return MapHeight;
    }

    public float GetMapWidth()
    {
        return MapWidth;
    }

    public float GetMapOffsetX()
    {
        return MapOffsetX;
    }

    public float GetMapOffsetY()
    {
        return MapOffsetY;
    }
}
