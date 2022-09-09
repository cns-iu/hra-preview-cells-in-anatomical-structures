using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils 
{
   public static Matrix4x4 ReflectX()
    {
        var result = new Matrix4x4(
            new Vector4(-1, 0, 0, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(0, 0, -1, 0),
            new Vector4(0, 0, 0, 1)
        );
        return result;
    }
}
