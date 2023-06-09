using Photon.Pun;
using UnityEngine;

public class Ak47Gun : BaseGun
{
    private float invokeTime;
    private Transform root;

    protected override void Awake()
    {
        base.Awake();
        root = transform.parent;
    }

    void Update()
    {
        invokeTime += Time.deltaTime;
        if (m_Fire)
        {
            if (invokeTime - m_FireInterval > 0)
            {
                if (BulletCount > 0)
                {
                    Fire();
                }
                else
                {
                    DryFire();
                }

                invokeTime = 0;
            }
        }
    }

    protected override void Fire()
    {
        base.Fire();
        RaycastHit hit;
        // Debug.DrawLine(root.position, root.position + root.forward * 100, Color.red);
        if (Physics.Raycast(root.position, root.forward, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                var player = hit.collider.GetComponent<PlayerController>();
                if (player != target)
                {
                    target.AttachPlayer(player);
                }
            }
            else
            {
                var effect = GameObject.Instantiate(m_HitEffect);
                effect.transform.position = hit.point;
                effect.transform.forward = hit.normal;
                Destroy(effect, 1);
            }
        }
    }

}