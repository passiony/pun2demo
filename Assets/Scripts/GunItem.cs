using UnityEngine;

namespace MFPS
{
    public class GunItem : MonoBehaviour
    {
        public EGunType m_GunType;

        void Start()
        {
            Destroy(gameObject, FPSGame.REBORN_TIME);
        }
    }
}