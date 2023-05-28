using UnityEngine;

public class RotateAround : MonoBehaviour
{
    public Vector3 RotateAxis;

    public float RotateSpeed;

    void Update()
    {
        transform.Rotate(RotateAxis, RotateSpeed * Time.deltaTime);
    }
}