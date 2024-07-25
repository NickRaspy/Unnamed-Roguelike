using System.Collections.Generic;
using UnityEngine;

public class GravityCtrl : MonoBehaviour
{
    public GravityOrbit gravity;
    public float rotSpeed = 20f;
    void Update()
    {
        gravity?.Attract(transform, rotSpeed);
    }
}
