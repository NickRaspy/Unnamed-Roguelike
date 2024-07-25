using UnityEngine;

public class MultiplePlatformsInOneTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject partitionPrefab;
    [SerializeField] private GameObject platformPrefab;
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        for(int i = 0; i < 4; i++)
        {
            Vector3 pos = transform.position;
            Vector3 plusPos = new Vector3();
            Vector3 rot = new Vector3();
            switch(i)
            {
                case 0:
                    pos += new Vector3(0f, 0f, mesh.bounds.max.z);
                    break;
                case 1:
                    pos += new Vector3(0f, 0f, mesh.bounds.min.z);
                    rot = new Vector3(0f, 180f, 0f);
                    break; 
                case 2:
                    pos += new Vector3(mesh.bounds.max.x, 0f, 0f);
                    rot = new Vector3(0f, 90f, 0f);
                    break;
                case 3:
                    pos += new Vector3(mesh.bounds.min.x, 0f, 0f);
                    rot = new Vector3(0f, 270f, 0f);
                    break;
            }
            GameObject newPart = Instantiate(partitionPrefab, transform);
            newPart.transform.position = pos; newPart.name = i.ToString();
            newPart.transform.localRotation = Quaternion.Euler(rot);
            Mesh partitionMesh = newPart.GetComponent<MeshFilter>().mesh;
            GameObject newPlat = Instantiate(platformPrefab, newPart.transform);
            switch (i)
            {
                case 0:
                    plusPos = newPart.transform.position + new Vector3(0f, 0f, partitionMesh.bounds.max.z + mesh.bounds.max.z);
                    break;
                case 1:
                    plusPos = newPart.transform.position - new Vector3(0f, 0f, partitionMesh.bounds.max.z + mesh.bounds.max.z);
                    break;
                case 2:
                    plusPos = newPart.transform.position + new Vector3(partitionMesh.bounds.max.z + mesh.bounds.max.x, 0f, 0f);
                    break;
                case 3:
                    plusPos = newPart.transform.position - new Vector3(partitionMesh.bounds.max.z + mesh.bounds.max.x, 0f, 0f);
                    break;
            }
            newPlat.transform.position = plusPos;
            newPlat.transform.localRotation = Quaternion.Euler(-rot);
            Debug.Log(partitionMesh.bounds);
        }
    }
}
