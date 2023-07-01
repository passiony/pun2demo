using System;
using UnityEngine;

namespace MFPS
{
    public class PlayerBody : MonoBehaviour
    {

        public EBodyPart m_BodyPart;

        public float GetRealDamage(float damage)
        {
            switch (m_BodyPart)
            {
                case EBodyPart.Head:
                    return 100;
                case EBodyPart.Chest:
                    return damage + damage;
                case EBodyPart.Arm:
                    return damage;
                case EBodyPart.Leg:
                    return (damage * 0.5f);
                default:
                    return damage;
            }
        }
    }
}