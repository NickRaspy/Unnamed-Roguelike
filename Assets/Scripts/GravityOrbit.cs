using UnityEngine;

public class GravityOrbit : MonoBehaviour
{
    public float gravity;
    public bool fixedDirection;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GravityCtrl>())
        {
            other.GetComponent<GravityCtrl>().gravity = this;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<GravityCtrl>())
        {
            other.GetComponent<GravityCtrl>().gravity = null;
        }
    }
    public void Attract(Transform body, float rotationSpeed = 50f)
    {
        Vector3 gravityUp = fixedDirection ? transform.up : (body.transform.position - transform.position).normalized;
        body.GetComponent<Rigidbody>().AddForce(gravity * -gravityUp);
        Quaternion targetRotation = Quaternion.FromToRotation(body.up, gravityUp) * body.rotation;
        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
