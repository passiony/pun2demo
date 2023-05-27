    using UnityEngine;
    public class IGun :MonoBehaviour
    {
        public int BulletCount = 30;
        protected bool m_Fire;
        protected PlayerController target;
        
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
    }
