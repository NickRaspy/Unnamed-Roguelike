using System;
using UnityEngine;
using Random = UnityEngine.Random;
//borrowed from https://www.geeksforgeeks.org/check-whether-a-given-point-lies-inside-a-triangle-or-not/
class TrianglePoint
{
    static public Vector3 RandomPoint(Vector3 A, Vector3 B, Vector3 C) //based on this https://stackoverflow.com/a/61804756
    {
        float r1 = Random.Range(0f, 1f);
        float r2 = Random.Range(0f, 1f);
        float sqrtR1 = Mathf.Sqrt(r1);
        float x = (1 - sqrtR1) * A.x + (sqrtR1 * (1 - r2)) * B.x + (sqrtR1 * r2) * C.x;
        float y = (1 - sqrtR1) * A.y + (sqrtR1 * (1 - r2)) * B.y + (sqrtR1 * r2) * C.y;
        float z = (1 - sqrtR1) * A.z + (sqrtR1 * (1 - r2)) * B.z + (sqrtR1 * r2) * C.z;
        return new Vector3(x, y, z);
    }
    static public Vector3 CenterOfTriangle(Vector3 A, Vector3 B, Vector3 C)
    {
        float x = (A.x + B.x + C.x) / 3f;
        float y = (A.y + B.y + C.y) / 3f;
        float z = (A.z + B.z + C.z) / 3f;
        return new Vector3(x, y, z);
    }
    static public Vector3 MidPoint(Vector3 A, Vector3 B)
    {
        float x = (A.x + B.x) / 2f;
        float y = (A.y + B.y) / 2f;
        float z = (A.z + B.z) / 2f;
        return new Vector3(x, y, z);
    }
}