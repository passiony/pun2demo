using System;
using MFPS;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace MFPS
{
    /// <summary>
    /// 武器类
    /// </summary>
    public class GunWeapon : MonoBehaviour
    {
        [FormerlySerializedAs("m_ID")] [SerializeField]
        protected EGunType mType;

        protected VRPlayerController m_Owner;
        protected Transform m_CharacterTransform;
        protected Animator m_Animator;

        [SerializeField] protected Transform FirePointLocation;

        [Tooltip("The amount of time that must elapse before the item can be used again.")] [SerializeField]
        protected float m_FireRate = 0.1f;

        [Tooltip("The number of rounds to fire in a single shot.")] [SerializeField]
        protected int m_FireCount = 1;

        [Tooltip("The random spread of the bullets once they are fired.")] [Range(0, 360)] [SerializeField]
        protected float m_Spread = 0.01f;

        [Tooltip("Fire All Audios")] [SerializeField]
        protected FireAudio m_FireAudio;

        [Tooltip("The amount of damage to apply to the hit object.")] [SerializeField]
        protected float m_DamageAmount = 10;

        [Tooltip("The amount of force to apply to the hit object.")] [SerializeField]
        protected float m_ImpactForce = 2;

        [Tooltip("The number of frames to add the impact force to.")] [SerializeField]
        protected int m_ImpactForceFrames = 15;

        [Tooltip("The number of rounds in the clip.")] [SerializeField]
        protected int m_ClipSize = 50;

        [Tooltip("The maximum distance in which the hitscan fire can reach.")] [SerializeField]
        protected float m_HitscanFireRange = float.MaxValue;

        [Tooltip("The maximum number of objects the hitscan cast can collide with.")] [SerializeField]
        protected int m_MaxHitscanCollisionCount = 15;

        [Tooltip("A LayerMask of the layers that can be hit when fired at.")] [SerializeField]
        protected LayerMask m_ImpactLayers = 1;

        [Tooltip("Specifies if the hitscan can detect triggers.")] [SerializeField]
        protected QueryTriggerInteraction m_HitscanTriggerInteraction = QueryTriggerInteraction.Ignore;

        [Tooltip("A reference to the muzzle flash prefab.")] [SerializeField]
        protected GameObject m_MuzzleFlash;

        [Tooltip("A reference to the shell prefab.")] [SerializeField]
        protected GameObject m_Shell;

        [SerializeField] protected Transform ShellLocation;

        [Tooltip("The velocity that the shell should eject at.")] [SerializeField]
        protected MinMaxVector3 m_ShellVelocity = new MinMaxVector3(new Vector3(3, 0, 0), new Vector3(4, 2, 0));

        [Tooltip("The torque that the projectile should initialize with.")] [SerializeField]
        protected MinMaxVector3 m_ShellTorque = new MinMaxVector3(new Vector3(-50, -50, -50), new Vector3(50, 50, 50));

        [Tooltip("Eject the shell after the specified delay.")] [SerializeField]
        protected float m_ShellEjectDelay;

        [Tooltip("A reference to the smoke prefab.")] [SerializeField]
        protected GameObject m_Smoke;

        [SerializeField] protected Transform SmokeLocation;

        [Tooltip("Spawn the smoke after the specified delay.")] [SerializeField]
        protected float m_SmokeSpawnDelay;

        [SerializeField] protected GameObject m_Tracer;

        [SerializeField] protected Transform TracerLocation;

        [SerializeField] protected TextMeshProUGUI m_ClipTxt;

        private float m_LastUseTime;
        private bool m_Firing;
        private int m_ClipRemaining;
        private RaycastHit m_RaycastHit;
        private RaycastHit[] m_HitscanRaycastHits;
        private RaycastHitComparer m_RaycastHitComparer = new RaycastHitComparer();

        public int ClipRemaining => m_ClipRemaining;
        public int ClipSize => m_ClipSize;

        public EGunType Type => mType;

        private void Awake()
        {
            m_HitscanRaycastHits = new RaycastHit[m_MaxHitscanCollisionCount];
            m_FireAudio.Init(GetComponent<AudioSource>());
            m_Animator = GetComponent<Animator>();
        }

        public void Equip(VRPlayerController owner)
        {
            gameObject.SetActive(true);
            m_Owner = owner;
            m_CharacterTransform = owner.transform;
            m_ClipRemaining = m_ClipSize;
            UpdateClipText();
            m_FireAudio.PlayEquipFire();
            m_Animator.Play("Equip");
        }

        public void CheckReload(bool autoReload)
        {
            if (ClipRemaining <= 0 && autoReload)
            {
                Reload();
            }
        }

        public void Reload()
        {
            m_ClipRemaining = m_ClipSize;
            UpdateClipText();
            m_FireAudio.PlayReloadFire();
            m_Animator.Play("Reload");
        }

        public virtual bool UseItem()
        {
            if (Time.time - m_LastUseTime > m_FireRate)
            {
                m_LastUseTime = Time.time;
                return Fire();
            }

            return false;
        }

        void UpdateClipText()
        {
            m_ClipTxt.text = $"{ClipRemaining}/{ClipSize}";
        }

        bool Fire()
        {
            if (m_ClipRemaining <= 0)
            {
                m_Animator.Play("DryFire");
                m_FireAudio.PlayDryFire();
                return false;
            }

            m_Animator.Play("Fire");

            m_Firing = true;
            m_ClipRemaining -= m_FireCount;
            UpdateClipText();

            for (int i = 0; i < m_FireCount; ++i)
            {
                HitscanFire();
            }

            ApplyFireEffects();
            return true;
        }


        private void HitscanFire()
        {
            var firePoint = FirePointLocation.position;
            //useLookPosition ? m_LookSource.LookPosition(true) : m_ShootableWeaponPerspectiveProperties.FirePointLocation.position;
            var fireDirection = FireDirection(firePoint);
            var fireRay = new Ray(firePoint, fireDirection);
            Debug.DrawLine(firePoint, firePoint + fireDirection * m_HitscanFireRange, Color.red);

            var hitCount = Physics.RaycastNonAlloc(fireRay, m_HitscanRaycastHits, m_HitscanFireRange,
                m_ImpactLayers.value, m_HitscanTriggerInteraction);
            var hasHit = false;

            for (int i = 0; i < hitCount; ++i)
            {
                var closestRaycastHit = QuickSelect.SmallestK(m_HitscanRaycastHits, hitCount, i, m_RaycastHitComparer);
                var hitGameObject = closestRaycastHit.transform.gameObject;
                // The character can't shoot themself.
                if (hitGameObject.transform.IsChildOf(m_CharacterTransform))
                {
                    continue;
                }

                if (hitGameObject.CompareTag("Player"))
                {
                    var player = hitGameObject.GetComponentInParent<VRPlayerController>();
                    if (player.IsSameTeam(m_Owner))
                    {
                        continue;
                    }
                }

                if (m_Owner.photonView.IsMine)
                {
                    // The shield can absorb some (or none) of the damage from the hitscan.
                    var damageAmount = m_DamageAmount;
                    // If the shield didn't absorb all of the damage then it should be applied to the character.
                    if (damageAmount > 0)
                    {
                        // Debug.Log("HitTarget:" + hitGameObject.name);
                        var damageTarget = DamageUtility.GetDamageTarget(hitGameObject);
                        if (damageTarget != null)
                        {
                            Debug.Log("damageTarget:" + damageTarget.Owner.name);
                            var pooledDamageData = new DamageData();
                            pooledDamageData.SetDamage(m_Owner, damageAmount, closestRaycastHit.point, fireDirection,
                                m_ImpactForce, m_ImpactForceFrames, 0, closestRaycastHit.collider);

                            damageTarget.Damage(pooledDamageData);
                        }
                        else
                        {
                            // If the damage target exists it will apply a force to the rigidbody in addition to procesing the damage. Otherwise just apply the force to the rigidbody. 
                            if (m_ImpactForce > 0 && closestRaycastHit.rigidbody != null &&
                                !closestRaycastHit.rigidbody.isKinematic)
                            {
                                float RigidbodyForceMultiplier = 50;
                                closestRaycastHit.rigidbody.AddForceAtPosition(
                                    (m_ImpactForce * RigidbodyForceMultiplier) * fireDirection,
                                    closestRaycastHit.point);
                            }
                        }
                    }
                }

                // 表面管理器将根据子弹击中的类型应用效果。
                SurfaceMamager.Instance.ShowDecal(closestRaycastHit);
                // SurfaceManager.SpawnEffect(closestRaycastHit, m_SurfaceImpact, m_CharacterLocomotion.GravityDirection,
                //     m_CharacterLocomotion.TimeScale, m_Item.GetVisibleObject());

                hasHit = true;
                break;
            }

            // 如果没有击中任何对象，则仍应生成跟踪器。
            if (!hasHit && m_Tracer != null)
            {
                AddHitscanTracer(MathUtility.TransformPoint(firePoint, Quaternion.LookRotation(fireDirection),
                    new Vector3(0, 0, 100)));
            }
        }

        /// <summary>
        /// Adds any effects (muzzle flash, shell, recoil, etc) to the fire position.
        /// </summary>
        protected virtual void ApplyFireEffects()
        {
            // Spawn a muzzle flash.
            if (m_MuzzleFlash != null)
            {
                SpawnMuzzleFlash();
            }

            // Spawn a shell.
            if (m_Shell != null)
            {
                EjectShell();
            }

            // Spawn the smoke.
            if (m_Smoke != null)
            {
                SpawnSmoke();
            }

            //fire Audio
            if (m_FireAudio != null)
            {
                m_FireAudio.PlayFire();
            }
        }

        /// <summary>
        /// Spawns the muzzle flash.
        /// </summary>
        private void SpawnMuzzleFlash()
        {
            m_MuzzleFlash.gameObject.SetActive(true);
            var muzzleFlashObj = m_MuzzleFlash.GetComponent<MyMuzzleFlash>();
            if (muzzleFlashObj != null)
            {
                muzzleFlashObj.Show();
            }
        }

        /// <summary>
        /// Ejects the shell.
        /// </summary>
        private void EjectShell()
        {
            var shellLocation = ShellLocation;
            var shell = Instantiate(m_Shell, shellLocation.position, shellLocation.rotation);
            var shellObj = shell.GetComponent<MyShell>();
            if (shellObj != null)
            {
                shellObj.AddForce(m_ShellVelocity.RandomValue, m_ShellTorque.RandomValue);
            }
        }

        /// <summary>
        /// Spawns the Smoke.
        /// </summary>
        private void SpawnSmoke()
        {
            // var smokeLocation = SmokeLocation;
            // var smoke = GameObject.Instantiate(m_Smoke, smokeLocation.position, smokeLocation.rotation);
            // var smokeObj = smoke.GetComponent<Smoke>();
            // if (smokeObj != null)
            // {
            //     smokeObj.Show(m_Item, m_ID, m_CharacterLocomotion);
            // }
        }

        /// <summary>
        /// Adds a tracer to the hitscan weapon.
        /// </summary>
        /// <param name="position">The position that the tracer should move towards.</param>
        protected virtual void AddHitscanTracer(Vector3 position)
        {
            // var tracerLocation = TracerLocation;
            // var tracerObject = ObjectPoolBase.Instantiate(m_Tracer, tracerLocation.position, tracerLocation.rotation);
            // var tracer = tracerObject.GetComponent<Tracer>();
            // if (tracer != null)
            // {
            //     tracer.Initialize(position);
            // }
        }

        /// <summary>
        /// 计算射击的方向
        /// Determines the direction to fire.
        /// </summary>
        /// <returns>The direction to fire.</returns>
        private Vector3 FireDirection(Vector3 firePoint)
        {
            // var direction = (m_FireInLookSourceDirection ? m_LookSource.LookDirection(firePoint, false, m_ImpactLayers, true, true) : 
            //                             m_ShootableWeaponPerspectiveProperties.FirePointLocation.forward);
            var direction = FirePointLocation.forward;
            // Add the spread in a random direction.
            if (m_Spread > 0)
            {
                direction += Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), direction) * transform.up *
                             UnityEngine.Random.Range(0, m_Spread / 360);
            }

            return direction;
        }
    }
}