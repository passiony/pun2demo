    using UnityEngine;
    using UnityEngine.Serialization;

    public enum EGunType
    {
        Rifle,
        AK47,
    }
    public class IGun :MonoBehaviour
    {
        public int BulletCount = 30;
        [SerializeField]
        protected EGunType m_GunType;
            
        [SerializeField]
        private Transform leftHandle;
        [SerializeField]
        private Transform rightHandle;

        protected bool m_Fire;
        protected PlayerController target;

        public EGunType GunType => m_GunType;
        
        public virtual void AddBullet(int bullet)
        {
            BulletCount += bullet;
        }
        
        public virtual void SetFire(bool fire)
        {
            m_Fire = fire;
        }

        public void SetTarget(PlayerController _target)
        {
            target = _target;
        }

        public virtual void Load()
        {
           var ik= target.GetComponent<IKControl>();
           ik.leftHandObj = leftHandle;
           ik.rightHandObj = rightHandle;
        }
    }
