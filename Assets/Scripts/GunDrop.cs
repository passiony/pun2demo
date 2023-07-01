using System.Collections.Generic;
using UnityEngine;

namespace MFPS
{
    public class GunDrop : MonoSingleton<GunDrop>
    {
        public GunItem[] m_GunModels;

        private Dictionary<EGunType, GunItem> m_GunDic = new Dictionary<EGunType, GunItem>();

        void Start()
        {
            foreach (var gun in m_GunModels)
            {
                m_GunDic.Add(gun.m_GunType, gun);
            }
        }

        public void CreateDrop(GunWeapon gun)
        {
            var prefab = m_GunDic[gun.Type];
            var go = GameObject.Instantiate(prefab);
            go.transform.position = gun.transform.position;
            go.transform.rotation = gun.transform.rotation;
        }
    }
}