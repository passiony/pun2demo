using System;
using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateCharacterController.UI;
using Photon.Pun.Demo.PunBasics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;
    private VRPlayerController target;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private SlotItem slotItem;
    [SerializeField] private GameObject scorePrefab;

    [SerializeField] private Transform scorePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject deadPanel;
    [SerializeField] private GameObject healthFlash;

    private Dictionary<string, TextMeshProUGUI> scores;

    void Awake()
    {
        Instance = this;
        scores = new Dictionary<string, TextMeshProUGUI>();
    }

    public void SetTarget(VRPlayerController _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target != null)
        {
            if (hpSlider != null)
            {
                hpSlider.value = target.HP / (float)target.MaxHP;
                hpText.text = $"{target.HP}";
            }

            if (slotItem != null)
            {
                var gun = target.WeaponComponent.GetCurrentWeapon();
                slotItem.SetCount(gun.ClipRemaining, gun.ClipSize);
            }

            //更新计分板
            // RefreshScoreBorad();
        }
    }

    // void RefreshScoreBorad()
    // {
    //     if (MyGameManager.Instance.ScoreBoard != null)
    //     {
    //         bool endGame = false;
    //         foreach (var player in MyGameManager.Instance.ScoreBoard)
    //         {
    //             if (!scores.ContainsKey(player.Key))
    //             {
    //                 var go = Instantiate(scorePrefab, scorePanel);
    //                 go.SetActive(true);
    //                 var tmp = go.GetComponent<TextMeshProUGUI>();
    //                 scores.Add(player.Key, tmp);
    //             }
    //
    //             scores[player.Key].text = player.Key + " : " + player.Value;
    //         }
    //     }
    // }

    public void OnQuitClick()
    {
        MyGameManager.Instance.LeaveRoom();
    }

    public void ShowDamage()
    {
        healthFlash.SetActive(true);
    }

    public void ShowDeadPanel()
    {
        deadPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowLosePanel()
    {
        losePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator CoDelayCallFunc(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}