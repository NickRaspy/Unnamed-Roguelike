using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class ProBuilderObjectCreationTest : MonoBehaviour
{
    [SerializeField] private ProBuilderMesh mesh;
    void Start()
    {
        Debug.Log(mesh.vertexCount);
        int[] quant = new int[2] { -1, 1 };
        Vertex[] vertices = mesh.GetVertices();
        List<int> excludedSharedVertex = new();
        for (int i = 0; i < vertices.Length; i++)
        {
            if (Mathf.Abs(vertices[i].position.x) == mesh.GetComponent<MeshFilter>().mesh.bounds.max.x || Mathf.Abs(vertices[i].position.z) == mesh.GetComponent<MeshFilter>().mesh.bounds.max.z)
            {
                for(int j = 0; j < mesh.sharedVertices.Count; j++)
                {
                    if (mesh.sharedVertices[j].Contains(i) && !excludedSharedVertex.Contains(j))
                    {
                        if (Mathf.Abs(vertices[i].position.x) > Mathf.Abs(vertices[i].position.z))
                            vertices[i].position += quant[Random.Range(0, 2)] * new Vector3(Random.Range(0.1f, 0.15f), 0f, 0f);
                        else if (Mathf.Abs(vertices[i].position.x) < Mathf.Abs(vertices[i].position.z))
                            vertices[i].position += quant[Random.Range(0, 2)] * new Vector3(0f, 0f, Random.Range(0.1f, 0.15f));
                        else
                            vertices[i].position -= new Vector3(Random.Range(0.15f, 0.25f) * vertices[i].position.x/Mathf.Abs(vertices[i].position.x), 0f, Random.Range(0.15f, 0.25f)* vertices[i].position.z / Mathf.Abs(vertices[i].position.z));
                        mesh.SetSharedVertexPosition(j, vertices[i].position);
                        excludedSharedVertex.Add(j);
                        break;
                    }
                }
            }
        }
        List<Face> currentFaces = new List<Face>();
        for(int i = 0; i < mesh.faces.Count; i++) currentFaces.Add(mesh.faces[i]);
        mesh.Extrude(currentFaces, ExtrudeMethod.FaceNormal, 2);
        vertices = mesh.GetVertices();
        excludedSharedVertex.Clear();
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].position.y > 0f && (Mathf.Abs(vertices[i].position.x) == mesh.GetComponent<MeshFilter>().mesh.bounds.max.x || Mathf.Abs(vertices[i].position.z) == mesh.GetComponent<MeshFilter>().mesh.bounds.max.z))
            {
                Debug.Log("a");
                for (int j = 0; j < mesh.sharedVertices.Count; j++)
                {
                    if (mesh.sharedVertices[j].Contains(i) && !excludedSharedVertex.Contains(j))
                    {
                        if (Mathf.Abs(vertices[i].position.x) > Mathf.Abs(vertices[i].position.z))
                            vertices[i].position += new Vector3(0.5f * vertices[i].position.x / Mathf.Abs(vertices[i].position.x), 0f, 0f);
                        else if (Mathf.Abs(vertices[i].position.x) < Mathf.Abs(vertices[i].position.z))
                            vertices[i].position += new Vector3(0f, 0f, 0.5f * vertices[i].position.z / Mathf.Abs(vertices[i].position.z));
                        else
                            vertices[i].position += new Vector3(0.5f * vertices[i].position.x / Mathf.Abs(vertices[i].position.x), 0f, 0.5f * vertices[i].position.z / Mathf.Abs(vertices[i].position.z));
                        mesh.SetSharedVertexPosition(j, vertices[i].position);
                        excludedSharedVertex.Add(j);
                        break;
                    }
                }
            }
        }
        Vector3[] verticalsPositions = new Vector3[mesh.GetVertices().Length];
        for (int i = 0; i < verticalsPositions.Length; i++)
        {
            verticalsPositions[i] = mesh.GetVertices()[i].position;
        }
        mesh.RebuildWithPositionsAndFaces(verticalsPositions, mesh.faces);
        mesh.Refresh();
        Debug.Log(mesh.vertexCount);
    }
}
