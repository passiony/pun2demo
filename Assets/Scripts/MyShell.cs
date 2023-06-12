/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using System;
using Opsive.Shared.Game;
using Opsive.Shared.Utility;
using Opsive.UltimateCharacterController.Objects;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Represents a shell casing which uses the trajectory object for kinematic shell movement.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class MyShell : MonoBehaviour
{
    [Tooltip("Time to live in seconds before the shell is removed.")] [SerializeField]
    protected float m_Lifespan = 5;

    [Tooltip("Chance of shell not being removed after settling on the ground.")] [Range(0, 1)] [SerializeField]
    protected float m_Persistence = 1;

    private float m_RemoveTime;
    private Vector3 m_StartScale;

    private Rigidbody m_Rigidbody;
    private Collider m_Collider;
    private AudioSource m_AudioSource;
    
    /// <summary>
    /// Initialize the default values.
    /// </summary>
    protected void Awake()
    {
        m_StartScale = transform.localScale;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<Collider>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void AddForce(Vector3 force, Vector3 torque)
    {
        m_Rigidbody.AddForce(force);
        m_Rigidbody.AddTorque(torque);
    }

    /// <summary>
    /// The shell has been spawned - reset the timing and component values.
    /// </summary>
    protected void OnEnable()
    {
        m_RemoveTime = Time.time + m_Lifespan;
        transform.localScale = m_StartScale;

        if (m_Collider != null)
        {
            m_Collider.enabled = true;
        }
    }

    /// <summary>
    /// Move and rotate the object according to a parabolic trajectory.
    /// </summary>
    protected void FixedUpdate()
    {
        if (Time.time > m_RemoveTime)
        {
            // The shell should be removed.
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, TimeUtility.FramerateDeltaTime * 0.2f);
            if (Time.time > m_RemoveTime + 0.5f)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// The object has collided with another object.
    /// </summary>
    /// <param name="hit">The RaycastHit of the object. Can be null.</param>
    private void OnCollisionEnter(Collision collision)
    {
        SurfaceMamager.Instance.ShowShell(collision.transform.position);
    }

}