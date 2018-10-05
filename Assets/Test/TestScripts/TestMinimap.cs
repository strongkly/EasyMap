using UnityEngine;
using System;
using System.Collections.Generic;

public class TestMinimap : MonoBehaviour
{
    [SerializeField]
    GameObject npcObj;
    [SerializeField]
    GameObject viewRoot;

    string posx, posy, posz;

    List<GameObject> allInsGos = new List<GameObject>();

    void OnGUI()
    {
        posx = GUILayout.TextField(posx);
        posy = GUILayout.TextField(posy);
        posz = GUILayout.TextField(posz);
        if (GUILayout.Button("add static obj"))
        {
            GameObject go = GameObject.Instantiate(npcObj);
            go.transform.position = new Vector3((float)Convert.ToDouble(posx),
                (float)Convert.ToDouble(posy), (float)Convert.ToDouble(posz));
            go.SetActive(true);
            viewRoot.GetComponent<MinimapView>().SetStaticPoint(go.transform.position, eStaticMarkType.NPC);
            allInsGos.Add(go);
        }
        if (GUILayout.Button("add moving obj"))
        {
            GameObject go = GameObject.Instantiate(npcObj);
            go.transform.position = new Vector3((float)Convert.ToDouble(posx),
                (float)Convert.ToDouble(posy), (float)Convert.ToDouble(posz));
            go.SetActive(true);
            go.AddComponent<RandomMove>();
            viewRoot.GetComponent<MinimapView>().SetMovingMark(go, eMovingMarkType.MovingNPC);
            allInsGos.Add(go);
        }
    }

    void OnDestroy()
    {
        var iter = allInsGos.GetEnumerator();
        while (iter.MoveNext())
        {
            Destroy(iter.Current);
        }   
    }
}