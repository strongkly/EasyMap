using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMove : MonoBehaviour
{
    MapSettings ms;

    void Awake()
    {
        ms = FileHelp.LoadJsonFormatObjectByDataPathRelativePath<MapSettings>
            (MapSettings.mapSettingsProfilesPath);
    }

    void Update ()
    {
        Translate();
        Vector3 pos = transform.position;
        if (pos.x < -10 || pos.x > 10 || pos.GetDepth(ms.mapDepthDimession) < -10 || pos.GetDepth(ms.mapDepthDimession) > 10)
        {
            Destroy(gameObject);
        }	
	}

    void Translate()
    {
        if (Random.Range(0, 100) == 99)
        {
            Vector3 newPos = Vector3.zero;
            newPos.Set(Random.Range(-1f, 1f), transform.position.y, transform.position.z);
            newPos = newPos.SetDepth(ms.mapDepthDimession, Random.Range(-1f, 1f));
            transform.position = newPos;
        }
    }
}
