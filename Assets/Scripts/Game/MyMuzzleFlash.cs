/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;

namespace MFPS
{
    public class MyMuzzleFlash : MonoBehaviour
    {
        [Tooltip("The name of the shader tint color property.")] [SerializeField]
        protected string m_TintColorPropertyName = "_TintColor";

        [Tooltip("The alpha value to initialize the muzzle flash material to.")] [Range(0, 1)] [SerializeField]
        protected float m_StartAlpha = 0.5f;

        [Tooltip("The minimum fade speed - the larger the value the quicker the muzzle flash will fade.")]
        [SerializeField]
        protected float m_MinFadeSpeed = 3;

        [Tooltip("The maximum fade speed - the larger the value the quicker the muzzle flash will fade.")]
        [SerializeField]
        protected float m_MaxFadeSpeed = 4;

        private Material m_Material;
        private Light m_Light;
        private ParticleSystem m_Particles;

        private int m_TintColorPropertyID;
        private Color m_Color;
        private float m_StartLightIntensity;
        private float m_FadeSpeed;
        private float m_TimeScale = 1;

        private int m_StartLayer;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        private void Awake()
        {
            m_TintColorPropertyID = Shader.PropertyToID(m_TintColorPropertyName);

            var muzzleRenderer = GetComponent<Renderer>();
            if (muzzleRenderer != null)
            {
                m_Material = muzzleRenderer.sharedMaterial;
            }

            m_Light = GetComponent<Light>();
            m_Particles = GetComponent<ParticleSystem>();
            // If a light exists set the start light intensity. Every time the muzzle flash is enabed the light intensity will be reset to its starting value.
            if (m_Light != null)
            {
                m_StartLightIntensity = m_Light.intensity;
            }

            m_StartLayer = gameObject.layer;
        }

        /// <summary>
        /// The muzzle flash has been enabled.
        /// </summary>
        private void OnEnable()
        {
            m_Color.a = 0;
            if (m_Material != null)
            {
                m_Material.SetColor(m_TintColorPropertyID, m_Color);
            }

            if (m_Light != null)
            {
                m_Light.intensity = 0;
            }

            if (m_Particles != null)
            {
                m_Particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        /// <summary>
        /// A weapon has been fired and the muzzle flash needs to show. Set the starting alpha value and light intensity if the light exists.
        /// </summary>
        /// <param name="item">The item that the muzzle flash is attached to.</param>
        /// <param name="itemActionID">The ID which corresponds to the ItemAction that spawned the muzzle flash.</param>
        /// <param name="pooled">Is the muzzle flash pooled?</param>
        /// <param name="characterLocomotion">The character that the muzzle flash is attached to.</param>
        public void Show()
        {
            // The muzzle flash may be inactive if the object isn't pooled.
            gameObject.SetActive(true);

            m_Color = Color.white;
            m_Color.a = m_StartAlpha;
            if (m_Material != null)
            {
                m_Material.SetColor(m_TintColorPropertyID, m_Color);
            }

            m_FadeSpeed = Random.Range(m_MinFadeSpeed, m_MaxFadeSpeed);
            if (m_Light != null)
            {
                m_Light.intensity = m_StartLightIntensity;
            }

            if (m_Particles != null)
            {
                m_Particles.Play(true);
            }
        }

        /// <summary>
        /// Decrease the alpha value of the muzzle flash to give it a fading effect. As soon as the alpha value reaches zero place the muzzle flash back in
        /// the object pool. If a light exists decrease the intensity of the light as well.
        /// </summary>
        private void Update()
        {
            if (m_Color.a > 0)
            {
                m_Color.a = Mathf.Max(m_Color.a - (m_FadeSpeed * Time.deltaTime * m_TimeScale), 0);
                if (m_Material != null)
                {
                    m_Material.SetColor(m_TintColorPropertyID, m_Color);
                }

                // Keep the light intensity synchronized with the alpha channel's value.
                if (m_Light != null)
                {
                    m_Light.intensity = m_StartLightIntensity * (m_Color.a / m_StartAlpha);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}