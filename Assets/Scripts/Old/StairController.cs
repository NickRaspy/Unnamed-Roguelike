using UnityEngine;

public class StairController : MonoBehaviour
{
    public Position position;
    public Way way;
    public Transform[] outPoints;
    public GameObject parentPlatform;
    public GameObject childPlatform;
    public enum Position
    {
        None = 0, Left = 4, Right = 1, Front = 2, Back = 3
    }
    public enum Way
    {
        Up = 0, Down = 1
    }
}
