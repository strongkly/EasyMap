using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extend
{
    public static float GetDepth(this Vector3 v, eMapDepthDimession dimess)
    {
        if (dimess == eMapDepthDimession.DepthY)
            return v.y;
        else
            return v.z;
    }

    public static Vector3 SetDepth(this Vector3 v, eMapDepthDimession dimess, float val)
    {
        if (dimess == eMapDepthDimession.DepthY)
            v.Set(v.x, val, v.z);
        else
            v.Set(v.x, v.y, val);
        return v;
    }

    public static float GetHeight(this Vector3 v, eMapDepthDimession dimess)
    {
        if (dimess == eMapDepthDimession.DepthY)
            return v.z;
        else
            return v.y;
    }

    public static Vector3 SetHeight(this Vector3 v, eMapDepthDimession dimess, float val)
    {
        if (dimess == eMapDepthDimession.DepthY)
            v.Set(v.x, v.y, val);
        else
            v.Set(v.x, val, v.z);
        return v;
    }
}
