using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHaptic : MonoBehaviour
{
    [SerializeField] [Range(0, 1)]
    private float instensity;

    private XRBaseController controller;

    private void Awake()
    {
        controller = this.GetComponent<XRBaseController>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            TriggerHaptic(0.5f);
        }
    }

    public void TriggerHaptic(float duration)
    {
        if (instensity > 0)
        {
            controller.SendHapticImpulse(instensity, duration);
        }
    }
}