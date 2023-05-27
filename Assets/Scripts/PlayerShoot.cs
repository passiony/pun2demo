using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject hitEffect;

    public float FireInterval = 0.1f;
    private float invokeTime;

    void Update()
    {
        invokeTime += Time.deltaTime;
        if (Input.GetMouseButton(0))
        {
            if (invokeTime - FireInterval > 0)
            {
                Fire();
                invokeTime = 0;
            }
        }
    }

    void Fire()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var effect = GameObject.Instantiate(hitEffect, hit.point, Quaternion.Euler(hit.normal));
            Destroy(effect, 1);
        }
    }
}