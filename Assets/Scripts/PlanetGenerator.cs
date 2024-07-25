using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using Random = UnityEngine.Random;

public class PlanetGenerator : MonoBehaviour
{
    #region MESH
    private ProBuilderMesh mesh;
    private List<SharedVertex> sv = new();
    private List<Face> faces = new();
    private List<Face> excludedFaces = new();
    private Vertex[] vertices;
    #endregion
    [SerializeField]private GameObject testObject;
    private Vector3 playerPosition;
    public Vector3 PlayerPosition { get { return playerPosition; } set {  playerPosition = value; } }
    void Start()
    {
        Preparation();
        LandGeneration();
        WaterGeneration();
        Refresh();
        Debug.Log(GetBigChunks().Count);
        DecorationTest();
        SetPlayerPosition();
    }
    void Preparation()
    {
        mesh = GetComponent<ProBuilderMesh>();
        sv = mesh.sharedVertices.ToList();
        faces = mesh.faces.ToList();
        vertices = mesh.GetVertices();
    }
    void Refresh()
    {
        Vector3[] verticalsPositions = new Vector3[mesh.GetVertices().Length];
        for (int i = 0; i < verticalsPositions.Length; i++)
        {
            verticalsPositions[i] = mesh.GetVertices()[i].position;
        }
        mesh.RebuildWithPositionsAndFaces(verticalsPositions, mesh.faces);
        mesh.Refresh();
        faces = mesh.faces.ToList();
        List<Face> tempExFaces = new(); excludedFaces.ForEach(a => tempExFaces.Add(faces.Find(x => x.indexes[0] == a.indexes[0])));
        excludedFaces = tempExFaces;
        sv = mesh.sharedVertices.ToList();
        vertices = mesh.GetVertices();
    }
    void SetPlayerPosition()
    {
        List<Face> allFaces = new(); faces.ForEach(a => { allFaces.Add(a); }); excludedFaces.ForEach(a => allFaces.Remove(a));
        Face cFace = allFaces[Random.Range(0, allFaces.Count)];
        playerPosition = TrianglePoint.CenterOfTriangle(vertices[cFace.indexes[0]].position, vertices[cFace.indexes[1]].position, vertices[cFace.indexes[2]].position)*(transform.localScale.x + 10f);
    }
    void LandGeneration()
    {
/*        List<SharedVertex> copySV = new(); sv.ForEach(v => copySV.Add(v));
        int rmaxSV = Random.Range(sv.Count / 5, sv.Count / 3);
        List<SharedVertex> chosenSV = new();
        for (int i = 0; i < rmaxSV; i++)
        {
            int r = Random.Range(0, copySV.Count);
            chosenSV.Add(sv[r]); copySV.Remove(sv[r]);
        }*/
        for (int i = 0; i < sv.Count; i++)
        {
            vertices[sv[i][0]].position += vertices[sv[i][0]].position / Random.Range(20, 40);
            mesh.SetSharedVertexPosition(sv.IndexOf(sv[i]), vertices[sv[i][0]].position);
        }
    }
    void WaterGeneration()
    {
        List<SharedVertex> copySV = new(); sv.ForEach(v => copySV.Add(v));
        int r = Random.Range(0, copySV.Count);
        SharedVertex nextSV = copySV[r];
        List<int> excludedVerticles = new();
        int cr = Random.Range(0,2), ir = Random.Range(10, 30);
        for (int i = 0; i < ir; i++)
        {
            mesh.SetSharedVertexPosition(sv.IndexOf(nextSV), vertices[nextSV[0]].position - vertices[nextSV[0]].position / 5f);
            List<int> vs = new(); nextSV.ToList().ForEach(v => vs.Add(v));
            if (excludedVerticles.Count != 0)
            {
                foreach (int eV in excludedVerticles) if (vs.Contains(eV)) vs.Remove(eV);
            }
            if (vs.Count == 0) break;
            for (int j = 0; j < vs.Count; j++)
            {
                if (faces.Any(x => x.indexes.Contains(vs[j]))) faces.Find(x => x.indexes.Contains(vs[j])).indexes.ToList().ForEach(a => excludedVerticles.Add(a));
            }
            r = cr; if (r >= vs.Count) r = Random.Range(0, vs.Count);
            if (faces.Any(x => x.indexes.Contains(vs[r])))
            {
                Face face = faces.Find(x => x.indexes.Contains(vs[r]));
                List<int> newIndexes = new(); face.indexes.ToList().ForEach(a => newIndexes.Add(a)); newIndexes.Remove(vs[r]);
                r = Random.Range(0, newIndexes.Count);
                nextSV = sv.Find(x => x.Contains(newIndexes[r]));
                nextSV.ToList().ForEach(a => excludedFaces.Add(faces.Find(x => x.indexes.Contains(a))));
            }
        }
    }
    void DecorationTest()
    {
        int rMax = Random.Range(0, mesh.faceCount / 10);
        if (rMax == 0) return;
        GameObject decoration = new() { name = "Decoration" };
        decoration.transform.parent = transform; decoration.transform.localPosition = Vector3.zero;
        List<Face> allFaces = new(); faces.ForEach(a => { allFaces.Add(a); }); excludedFaces.ForEach(a => allFaces.Remove(a));
        List<Face> selFaces = new();
        for (int i = 0; i < rMax; i++)
        {
            int r = Random.Range(0, allFaces.Count);
            selFaces.Add(allFaces[r]); allFaces.RemoveAt(r);
        }
        foreach (Face face in selFaces)
        {
            List<int> verticesIndexes = new(); List<Vector3> verticesPos = new();
            face.indexes.ToList().ForEach(a => verticesIndexes.Add(a));
            verticesIndexes.ForEach(a => verticesPos.Add(vertices[a].position));
            Vector3 cot = TrianglePoint.CenterOfTriangle(verticesPos[0], verticesPos[1], verticesPos[2]);
            Vector3 point = TrianglePoint.RandomPoint(TrianglePoint.MidPoint(cot, verticesPos[0]), TrianglePoint.MidPoint(cot, verticesPos[1]), TrianglePoint.MidPoint(cot, verticesPos[2])) * transform.localScale.x;
            GameObject test = new() { name = "Point" }; test.transform.position = point + transform.position; test.transform.parent = decoration.transform;
            test.transform.LookAt(transform.position);
            Instantiate(testObject, test.transform).transform.Rotate(-90f, 0f, 0f);
            excludedFaces.Add(face);
        }
    }
    List<Face[]> GetMediumChunks()
    {
        List<Face[]> mediumChunks = new();
        List<Face> allFaces = new(); faces.ForEach(a => { allFaces.Add(a); }); excludedFaces.ForEach(a => allFaces.Remove(a));
        List<Face> checkedFaces = new();
        foreach(Face face in allFaces)
        {
            if (checkedFaces.Contains(face)) continue;
            Face[] mCF = new Face[4]; mCF[0] = face;
            List<SharedVertex> fSV = new(); face.indexes.ToList().ForEach(a => fSV.Add(sv.Find(x => x.Contains(a))));
            int c = 1;
            bool hasEx = false;
            foreach(SharedVertex cSV in fSV)
            {
                bool done = false;
                for (int i = 0; i < cSV.Count; i++)
                {
                    for (int j = 0; j < cSV.Count; j++)
                    {
                        if (faces.Any(x => x.indexes.Contains(cSV[i]) && x.indexes.Contains(cSV[j])))
                        {
                            if (!excludedFaces.Contains(faces.Find(x => x.indexes.Contains(cSV[i]) && x.indexes.Contains(cSV[j]))) && !checkedFaces.Contains(faces.Find(x => x.indexes.Contains(cSV[i]) && x.indexes.Contains(cSV[j]))))
                            {
                                mCF[c] = faces.Find(x => x.indexes.Contains(cSV[i]) && x.indexes.Contains(cSV[j]));
                            }
                            else hasEx = true;
                            done = true;
                            break;
                        }
                    }
                    if (done) break;
                }
                if (hasEx) break;
                c++;
            }
            if (!hasEx)
            {
                mediumChunks.Add(mCF);
                mCF.ToList().ForEach(a => checkedFaces.Add(a));
            }
        }
        return mediumChunks;
    }
    List<Face[]> GetBigChunks()
    {
        List<Face[]> bigChunks = new();
        List<Face> allFaces = new(); faces.ForEach(a => { allFaces.Add(a); }); excludedFaces.ForEach(a => allFaces.Remove(a));
        List<Face> checkedFaces = new();
        foreach (Face face in allFaces)
        {
            if (checkedFaces.Contains(face)) continue;
            Face[] bCF = new Face[13]; bCF[0] = face;
            List<SharedVertex> fSV = new(); face.indexes.ToList().ForEach(a => fSV.Add(sv.Find(x => x.Contains(a))));
            int c = 1;
            bool hasEx = false;
            foreach (SharedVertex cSV in fSV)
            {
                foreach(int v in cSV)
                {
                    if (!excludedFaces.Any(x => x.indexes.Contains(v)) && !checkedFaces.Any(x => x.indexes.Contains(v)))
                    {
                        if (!bCF.ToList().Contains(faces.Find(x => x.indexes.Contains(v))))
                        {
                            bCF[c] = faces.Find(x => x.indexes.Contains(v));
                            c++;
                        }
                    }
                    else hasEx = true;
                }
                if (hasEx) break;
            }
            if (!hasEx)
            {
                bigChunks.Add(bCF);
                bCF.ToList().ForEach(a => checkedFaces.Add(a));
            }
        }
        return bigChunks;
    }
}
