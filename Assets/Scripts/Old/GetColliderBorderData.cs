using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class GetColliderBorderData : MonoBehaviour
{
    void Start()
    {
        Collider collider = GetComponent<Collider>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Debug.Log(mesh.bounds.max.x + " " + mesh.bounds.min.x + " " + mesh.bounds.max.z + " " + mesh.bounds.min.z);
/*        ProBuilderMesh mesh = GetComponent<ProBuilderMesh>();
        List<Vector3> vectors = new List<Vector3>();
        foreach(Vector3 vector in mesh.positions) if (!vectors.Contains(vector)) vectors.Add(vector);
        foreach (Vector3 vector in vectors) Debug.Log(vector);*/
    }
}
