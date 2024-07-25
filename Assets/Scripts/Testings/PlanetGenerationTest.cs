using Assets.Generators;
using UnityEngine;

public class PlanetGenerationTest : MonoBehaviour
{
    [SerializeField] private int resolution, size;
    [SerializeField] private bool hasWater;
    [SerializeField] private Material planetMaterial;
    [SerializeField] private Material waterMaterial;
    [SerializeField] private GameObject gravity;
    void Start()
    {
        Planet planet = new GameObject().AddComponent<Planet>();
        planet.transform.position = new Vector3(0, 0, 0);
        planet.Initialize(Random.Range(int.MinValue, int.MaxValue), resolution, size, hasWater, new GasGiantGenerator(), planetMaterial, waterMaterial);
        planet.GenerateMesh();
        foreach(Transform t in planet.transform) t.gameObject.AddComponent<MeshCollider>();
        Instantiate(gravity, planet.transform);
    }
}
