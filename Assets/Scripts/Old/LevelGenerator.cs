using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


public class LevelGenerator : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private int maxPlatformAmount;
    private int currentPlatformAmount;
    [Header("Objects")]
    [SerializeField] private GameObject startPlatform;
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private GameObject partitionPrefab;
    [SerializeField] private GameObject stair;
    public List<Platform> allPlatforms = new();
    public List<Platform> currentPlatforms = new();
    private Platform changingPlatform = new();
    void Start()
    {
        allPlatforms.Add(new Platform() {platform = startPlatform, conPos = new Vector3(0,0,0), excludedPositions = new() {StairController.Position.None } });
        GenerateStairs(startPlatform.transform, allPlatforms[0].excludedPositions);
        int interval = 0;
        while(currentPlatformAmount < maxPlatformAmount)
        {
            List<Platform> usingCurrentPlatforms = new(currentPlatforms);
            currentPlatforms.Clear();
            foreach (Platform platform in usingCurrentPlatforms)
            {
                GenerateStairs(platform.platform.transform, platform.excludedPositions);
            }
            if (interval > maxPlatformAmount) break; else interval++;
        }
        Debug.Log("Result: " + currentPlatformAmount);
    }
    private void GenerateStairs(Transform platform, List<StairController.Position> excludedPositions)
    {
        Mesh mesh = platform.GetComponent<MeshFilter>().mesh;
        Vector3 newPos = new();
        List<int> posIndex = new() { 1, 2, 3, 4 };
        int amount = Random.Range(1, 5);
        for (int i = 0; i < amount; i++)
        {
            int r = Random.Range(0, posIndex.Count);
            if (excludedPositions.Any(x => (int)x == posIndex[r])) continue;
            GameObject newStair = Instantiate(stair, platform);
            newStair.GetComponent<StairController>().parentPlatform = platform.gameObject;
            newStair.GetComponent<StairController>().way = (StairController.Way)Random.Range(0, 2);
            if(newStair.GetComponent<StairController>().way == StairController.Way.Up) newStair.transform.Find("Down").gameObject.SetActive(false);
            else newStair.transform.Find("Up").gameObject.SetActive(false);
            switch (posIndex[r])
            {
                case 1:
                    newPos = new Vector3(mesh.bounds.max.x, 0f, 0f);
                    newStair.GetComponent<StairController>().position = StairController.Position.Right;
                    break;
                case 2:
                    newPos = new Vector3(0f, 0f, mesh.bounds.max.z);
                    newStair.GetComponent<StairController>().position = StairController.Position.Front;
                    break;
                case 3:
                    newPos = new Vector3(0f, 0f, mesh.bounds.min.z);
                    newStair.GetComponent<StairController>().position = StairController.Position.Back;
                    break;
                case 4:
                    newPos = new Vector3(mesh.bounds.min.x, 0f, 0f);
                    newStair.GetComponent<StairController>().position = StairController.Position.Left;
                    break;
            }
            posIndex.RemoveAt(r);
            newStair.transform.localPosition = newPos;
            newStair.transform.LookAt(platform);
            newStair.transform.Rotate(0, 180f, 0, Space.Self);
        }
        foreach(Transform stair in platform)
        {
            /*            GeneratePlatform(stair.GetComponent<StairController>(), new Platform());*/
            if (stair.GetComponent<StairController>()) GeneratePlatforms(stair.GetComponent<StairController>(), Random.Range(0, 2) == 1);
        }
    }
    public void GeneratePlatforms(StairController stair, bool isConglomerate)
    {
        if (isConglomerate)
        {
            GeneratePlatform(stair, changingPlatform);
            int r = Random.Range(1, 4);
            for (int i = 0; i < r; i++)
            {
                Vector3 partitionPos = new();
                Vector3 partitionRot = new();
                Vector3 outPoint = new();
                List<StairController.Position> positions = new() { StairController.Position.Back, StairController.Position.Right, StairController.Position.Front, StairController.Position.Left };
                if(changingPlatform.excludedPositions != null)foreach (StairController.Position pos in changingPlatform.excludedPositions) { positions.Remove(pos); }
                StairController.Position exPos = new();
                int r2 = Random.Range(0, positions.Count);
                changingPlatform.excludedPositions.Add(positions[r2]);
                Mesh platformMesh = changingPlatform.platform.GetComponent<MeshFilter>().mesh;
                GameObject partition = Instantiate(partitionPrefab, changingPlatform.platform.transform);
                Mesh partitionMesh = partition.GetComponent<MeshFilter>().mesh;
                switch (positions[r2])
                {
                    case StairController.Position.Right:
                        partitionPos = new(platformMesh.bounds.max.x, 0f, 0f);
                        outPoint = partitionPos + new Vector3(partitionMesh.bounds.max.z + platformMesh.bounds.max.x, 0f, 0f);
                        partitionRot = new(0, 90f, 0f);
                        exPos = StairController.Position.Left;
                        break;
                    case StairController.Position.Front:
                        partitionPos = new(0f, 0f, platformMesh.bounds.max.z);
                        outPoint = partitionPos + new Vector3(0f, 0f, partitionMesh.bounds.max.z + platformMesh.bounds.max.z);
                        partitionRot = new(0, 0f, 0f);
                        exPos = StairController.Position.Back;
                        break;
                    case StairController.Position.Back:
                        partitionPos = new(0f, 0f, platformMesh.bounds.min.z);
                        outPoint = partitionPos - new Vector3(0f, 0f, partitionMesh.bounds.max.z + platformMesh.bounds.max.z);
                        partitionRot = new(0, 180f, 0f);
                        exPos = StairController.Position.Front;
                        break;
                    case StairController.Position.Left:
                        partitionPos = new(platformMesh.bounds.min.x, 0f, 0f);
                        outPoint = partitionPos - new Vector3(partitionMesh.bounds.max.z + platformMesh.bounds.max.x, 0f, 0f);
                        partitionRot = new(0, 270f, 0f);
                        exPos = StairController.Position.Right;
                        break;
                }
                Debug.Log(outPoint);
                partition.transform.localPosition = partitionPos;
                partition.transform.localRotation = Quaternion.Euler(partitionRot);
                switch (positions[r2])
                {
                    case StairController.Position.Right:
                        outPoint = partition.transform.position + new Vector3(partitionMesh.bounds.max.z + platformMesh.bounds.max.x, 0f, 0f);
                        break;
                    case StairController.Position.Front:
                        outPoint = partition.transform.position + new Vector3(0f, 0f, partitionMesh.bounds.max.z + platformMesh.bounds.max.z);
                        break;
                    case StairController.Position.Back:
                        outPoint = partition.transform.position - new Vector3(0f, 0f, partitionMesh.bounds.max.z + platformMesh.bounds.max.z);
                        break;
                    case StairController.Position.Left:
                        outPoint = partition.transform.position - new Vector3(partitionMesh.bounds.max.z + platformMesh.bounds.max.x, 0f, 0f);
                        break;
                }
                GeneratePlatform(positions[r2], outPoint, changingPlatform.conPos.z / Mathf.Abs(changingPlatform.conPos.z), changingPlatform);
            }
        }
        else GeneratePlatform(stair, new Platform());
    }
    private void GeneratePlatform(StairController stair, Platform platform)
    {
        Vector3 newConPos = new();
        if (allPlatforms.Count != 0) newConPos = allPlatforms.Any(x => x.platform == stair.parentPlatform) ? allPlatforms.Find(x => x.platform == stair.parentPlatform).conPos : newConPos;
        switch (stair.position)
        {
            case StairController.Position.Right:
                newConPos += new Vector3(1, 0, stair.way == 0 ? 1 : -1);
                break;
            case StairController.Position.Front:
                newConPos += new Vector3(0, 1, stair.way == 0 ? 1 : -1);
                break;
            case StairController.Position.Back:
                newConPos += new Vector3(0, -1, stair.way == 0 ? 1 : -1);
                break;
            case StairController.Position.Left:
                newConPos += new Vector3(-1, 0, stair.way == 0 ? 1 : -1);
                break;
        }
        if (allPlatforms.Any(x => x.conPos == newConPos))
        {
            stair.childPlatform = allPlatforms.Find(x => x.conPos == newConPos).platform;
            allPlatforms.Find(x => x.conPos == newConPos).excludedPositions.Add(stair.position);
            return;
        }
        GameObject newPlatform = Instantiate(platformPrefab, stair.outPoints[(int)stair.way].position, Quaternion.identity);
        currentPlatformAmount++;
        Mesh mesh = newPlatform.GetComponent<MeshFilter>().mesh;
        Vector3 newPos = new();
        StairController.Position position = new();
        switch (stair.position)
        {
            case StairController.Position.Right:
                newPos = new Vector3(mesh.bounds.max.z, 0f, 0f);
                position = StairController.Position.Left;
                break;
            case StairController.Position.Front:
                newPos = new Vector3(0f, 0f, mesh.bounds.max.x);
                position = StairController.Position.Back;
                break;
            case StairController.Position.Back:
                newPos = new Vector3(0f, 0f, mesh.bounds.min.x);
                position = StairController.Position.Front;
                break;
            case StairController.Position.Left:
                newPos = new Vector3(mesh.bounds.min.z, 0f, 0f);
                position = StairController.Position.Right;
                break;
        }
        stair.childPlatform = newPlatform;
        newPlatform.transform.localPosition += newPos;
        newPlatform.name = Random.Range(0f, 10000f).ToString();
        Debug.Log((newConPos == null) + " " + (newPlatform == null));
        platform.platform = newPlatform; 
        platform.excludedPositions = new() { position }; 
        platform.conPos = newConPos;
        currentPlatforms.Add(platform);
        allPlatforms.Add(platform);
    }
    private void GeneratePlatform(StairController.Position excludedPosition, Vector3 outPoint, float way, Platform platform)
    {
        Vector3 newConPos = new();
        switch (excludedPosition)
        {
            case StairController.Position.Right:
                newConPos += new Vector3(1, 0, way == 0 ? 1 : -1);
                break;
            case StairController.Position.Front:
                newConPos += new Vector3(0, 1, way == 0 ? 1 : -1);
                break;
            case StairController.Position.Back:
                newConPos += new Vector3(0, -1, way == 0 ? 1 : -1);
                break;
            case StairController.Position.Left:
                newConPos += new Vector3(-1, 0, way == 0 ? 1 : -1);
                break;
        }
        GameObject newPlatform = Instantiate(platformPrefab, outPoint, Quaternion.identity);
        StairController.Position position = new();
        switch (excludedPosition)
        {
            case StairController.Position.Right:
                position = StairController.Position.Left;
                break;
            case StairController.Position.Front:
                position = StairController.Position.Back;
                break;
            case StairController.Position.Back:
                position = StairController.Position.Front;
                break;
            case StairController.Position.Left:
                position = StairController.Position.Right;
                break;
        }
        newPlatform.name = Random.Range(0f, 10000f).ToString();
        platform.platform = newPlatform; platform.excludedPositions = new() { position }; platform.conPos = newConPos;
        Debug.Log(platform.platform.name);
        currentPlatforms.Add(platform);
        allPlatforms.Add(platform);
    }
    [Serializable]
    public class Platform
    {
        public List<StairController.Position> excludedPositions;
        public GameObject platform;
        public Vector3 conPos;
    }
}
