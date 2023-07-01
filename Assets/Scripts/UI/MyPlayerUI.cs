// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerUI.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in PUN Basics Tutorial to deal with the networked player instance UI display tha follows a given player to show its health and name
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MFPS
{
    public class MyPlayerUI : MonoBehaviour
    {
        [SerializeField] private Vector3 screenOffset = new Vector3(0f, 1, 0f);

        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI playerHpText;

        [SerializeField] private Slider playerHealthSlider;

        VRPlayerController target;

        float characterControllerHeight;

        Transform targetTransform;

        Vector3 targetPosition;

        private bool followTarget = false;

        void Update()
        {
            // Reflect the Player Health
            if (playerHealthSlider != null)
            {
                playerHealthSlider.value = target.HP / (float)target.MaxHP;
                playerHpText.text = $"{target.HP}/{target.MaxHP}";
            }
        }

        void LateUpdate()
        {
            // #Critical
            // Follow the Target GameObject on screen.
            if (targetTransform && followTarget)
            {
                var position = targetTransform.position;
                targetPosition = position;

                // this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
                this.transform.position = targetPosition + screenOffset;

                var forward = position - XRPlayer.Instance.Head.position;
                forward.y = 0;
                if (forward.z != 0)
                {
                    transform.forward = forward;
                }
            }
        }

        public void SetTarget(VRPlayerController _target, bool follow)
        {
            if (_target == null)
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> PlayMakerManager target for PlayerUI.SetTarget.",
                    this);
                return;
            }

            this.target = _target;
            followTarget = follow;
            targetTransform = this.target.GetComponent<Transform>();

            if (playerNameText != null)
            {
                playerNameText.text = this.target.photonView.Owner.NickName;
            }
        }
    }
}