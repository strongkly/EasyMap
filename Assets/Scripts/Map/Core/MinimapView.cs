using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapView : MonoBehaviour
{
    public GameObject otherMark;
    public GameObject selfGo;
    public RectTransform mapTexTrans;
    public Transform staticMarkRoot, movingMarkRoot;
    public string mapName;

    List<GameObject> staticMark;
    Dictionary<GameObject, GameObject> movingObjRelateMarkDic;

    IMapData mapData;
    MapSettings ms;
    float widthDelta, heightDelta;

    bool valid;
    void Start()
    {
        Validate();
        Initialize();
    }

    void Validate()
    {
        if (otherMark == null)
            valid = false;
        else if (selfGo == null)
            valid = false;
        else if (mapTexTrans == null)
            valid = false;
        else if (movingMarkRoot == null || staticMarkRoot == null)
            valid = false;
        else
            valid = true;
    }

    void Initialize()
    {
        if (!valid)
            return;
        ms = FileHelp.LoadJsonFormatObjectByDataPathRelativePath<MapSettings>
            (MapSettings.mapSettingsProfilesPath);
        mapData = FileHelp.LoadJsonFormatObjectByDataPathRelativePath<MapData>
            (string.Format("{0}/{1}.json", ms.mapDataSavePathRelativeToProject, mapName));

        widthDelta = mapTexTrans.rect.width / mapData.GetMapWidth();
        heightDelta = mapTexTrans.rect.height / mapData.GetMapWidth();
        staticMark = new List<GameObject>();
        movingObjRelateMarkDic = new Dictionary<GameObject, GameObject>();
    }

    void Update()
    {
        if (!valid)
            return;

        UpdateSelf();
        UpdateMovingMark();
    }

    #region Update logic
    void UpdateSelf()
    {
        Vector3 selfPos = selfGo.transform.position -
            new Vector3(mapData.GetMapOffsetX(),
            ms.mapDepthDimession == eMapDepthDimession.DepthY ? mapData.GetMapOffsetY() : 0,
            ms.mapDepthDimession == eMapDepthDimession.DepthY ? 0 : mapData.GetMapOffsetY());
        selfPos.Set(-selfPos.x * widthDelta, -selfPos.GetDepth(ms.mapDepthDimession) * heightDelta, 0);
        mapTexTrans.localPosition = selfPos;
    }

    void UpdateMovingMark()
    {
        FilterAllMovingMark();
        UpdateAllMovingMarkPos();
    }

    void FilterAllMovingMark()
    {
        var iter = movingObjRelateMarkDic.Keys.GetEnumerator();
        while (iter.MoveNext())
        {
            if (iter.Current == null)
            {
                DisposeMovingMark(movingObjRelateMarkDic[iter.Current]);
                movingObjRelateMarkDic.Remove(iter.Current);
                iter = movingObjRelateMarkDic.Keys.GetEnumerator();
            }
        }
    }

    void UpdateAllMovingMarkPos()
    {
        var iter = movingObjRelateMarkDic.Keys.GetEnumerator();
        while (iter.MoveNext())
        {
            UpdateMovingMarkPos(iter.Current);
        }
    }
    #endregion

    #region static mark,these marks will never move
    public void SetStaticPoint(Vector3 worldPos, eStaticMarkType markType)
    {
        if (!valid)
            return;
        GameObject go = CreateStaticMark();
        OperateNewStaticMark(go);
        SetStaticMarkLayer(go, GetStaticMarkLayerByType(markType));
        go.transform.localPosition = TransformStaticPoint(worldPos);
    }

    GameObject CreateStaticMark()
    {
        return GameObject.Instantiate(otherMark);
    }

    void OperateNewStaticMark(GameObject go)
    {
        //Add new static mark initial logic here...
        go.SetActive(true);
    }

    Vector2 TransformStaticPoint(Vector3 worldPos)
    {
        Vector3 pos = worldPos -
            new Vector3(mapData.GetMapOffsetX(),
            ms.mapDepthDimession == eMapDepthDimession.DepthY ? mapData.GetMapOffsetY() : 0,
            ms.mapDepthDimession == eMapDepthDimession.DepthY ? 0 : mapData.GetMapOffsetY());
        pos.Set(pos.x * widthDelta, pos.GetDepth(ms.mapDepthDimession) * heightDelta, 0);
        return pos;
    }

    int GetStaticMarkLayerByType(eStaticMarkType markType)
    {
        switch (markType)
        {
            case eStaticMarkType.NPC:
                return 1;
            default:
                return 0;
        }
    }

    void SetStaticMarkLayer(GameObject mark, int layer)
    {
        Transform parent;
        if (IsStaticLayerValid(layer))
            parent = staticMarkRoot.GetChild(0);
        else
            parent = staticMarkRoot.GetChild(layer);
        mark.transform.SetParent(parent, false);
    }

    bool IsStaticLayerValid(int layer)
    {
        return layer >= 0 && layer < staticMarkRoot.childCount;
    }

    void DisposeStaticMark(GameObject mark)
    {
        Destroy(mark);
    }
    #endregion

    #region moving mark,these marks will changing position while game is running
    public void SetMovingMark(GameObject movingObj, eMovingMarkType markType)
    {
        if (!valid)
            return;
        GameObject go = CreateMovingMark();
        OperateNewMovingMark(go);
        SetMovingMarkLayer(go, GetMovingMarkLayerByType(markType));
        movingObjRelateMarkDic.Add(movingObj, go);
        UpdateMovingMarkPos(movingObj);
    }

    void UpdateMovingMarkPos(GameObject movingObj)
    {
        movingObjRelateMarkDic[movingObj].transform.localPosition =
            TransformMovingPoint(movingObj.transform.position);
    }

    GameObject CreateMovingMark()
    {
        return GameObject.Instantiate(otherMark);
    }

    void OperateNewMovingMark(GameObject go)
    {
        //Add new static mark initial logic here...
        go.SetActive(true);
    }

    Vector2 TransformMovingPoint(Vector3 worldPos)
    {
        Vector3 result = worldPos - selfGo.transform.position;
        result.Set(result.x * widthDelta, result.GetDepth(ms.mapDepthDimession) * heightDelta, 0);
        return result;
    }

    int GetMovingMarkLayerByType(eMovingMarkType markType)
    {
        switch (markType)
        {
            case eMovingMarkType.MovingNPC:
                return 1;
            default:
                return 0;
        }
    }

    void SetMovingMarkLayer(GameObject mark, int layer)
    {
        Transform parent;
        if (IsMovingLayerValid(layer))
            parent = movingMarkRoot.GetChild(0);
        else
            parent = movingMarkRoot.GetChild(layer);
        mark.transform.SetParent(parent, false);
    }

    bool IsMovingLayerValid(int layer)
    {
        return layer >= 0 && layer < movingMarkRoot.childCount;
    }

    void DisposeMovingMark(GameObject mark)
    {
        Destroy(mark);
    }
    #endregion
}

public enum eStaticMarkType : int
{
    NPC = 1,
}

public enum eMovingMarkType : int
{
    MovingNPC = 1,
}
