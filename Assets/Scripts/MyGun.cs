using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateCharacterController.Items.Actions;
using UnityEngine;

public class MyGun : MonoBehaviour
{
    private GunWeapon m_Weapon;
    
    void Start()
    {
        m_Weapon = this.GetComponent<GunWeapon>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            m_Weapon.UseItem();
        }
    }
}
