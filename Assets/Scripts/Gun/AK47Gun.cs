using UnityEngine;

public class AK47Gun : IGun
{
    public GameObject hitEffect;
    public float FireInterval = 0.2f;

    private float invokeTime;
    private Transform firePoint;
    private MuzzleFlash muzzleFlash;
    private Transform root;

    private void Awake()
    {
        root = transform.parent;
        firePoint = transform.Find("FirePoint");
        muzzleFlash = transform.Find("FirePoint/MuzzleFlash").GetComponent<MuzzleFlash>();
    }

    public override void Load()
    {
        
    }
    void Update()
    {
        invokeTime += Time.deltaTime;
        if (m_Fire)
        {
            if (invokeTime - FireInterval > 0)
            {
                if (BulletCount > 0)
                {
                    Fire();
                }

                invokeTime = 0;
            }
        }
    }

    void Fire()
    {
        BulletCount--;
        muzzleFlash.Show();

        RaycastHit hit;
        // Debug.DrawLine(root.position, root.position + root.forward * 100, Color.red);
        if (Physics.Raycast(root.position, root.forward, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                var player = hit.collider.GetComponent<PlayerController>();
                if (player != target)
                {
                    player.OnDamage();
                }
            }
            else
            {
                var effect = GameObject.Instantiate(hitEffect);
                effect.transform.position = hit.point;
                effect.transform.forward = hit.normal;
                Destroy(effect, 1);
            }
        }
    }
}