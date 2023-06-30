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

public class MyPlayerUI : MonoBehaviour
{
    [SerializeField] private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    [SerializeField] private TextMeshProUGUI playerNameText;

    [SerializeField] private Slider playerHealthSlider;

    VRPlayerController target;

    float characterControllerHeight;

    Transform targetTransform;

    Vector3 targetPosition;


    void Update()
    {
        // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }

        // Reflect the Player Health
        if (playerHealthSlider != null)
        {
            playerHealthSlider.value = target.HP / (float)target.MaxHP;
        }
    }

    void LateUpdate()
    {
        // #Critical
        // Follow the Target GameObject on screen.
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;

            this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }
    }

    public void SetTarget(VRPlayerController _target)
    {
        if (_target == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }

        // Cache references for efficiency because we are going to reuse them.
        this.target = _target;
        targetTransform = this.target.GetComponent<Transform>();

        CharacterController _characterController = this.target.GetComponent<CharacterController>();

        // Get data from the Player that won't change during the lifetime of this Component
        if (_characterController != null)
        {
            characterControllerHeight = _characterController.height;
        }

        if (playerNameText != null)
        {
            playerNameText.text = this.target.photonView.Owner.NickName;
        }
    }
}