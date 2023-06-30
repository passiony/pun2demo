using System;
using System.Collections;
using Photon.Pun.Demo.PunBasics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;
    private VRPlayerController target;
    [SerializeField] private TextMeshProUGUI[] scoreTexts;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject deadPanel;
    [SerializeField] private GameObject healthFlash;
    [SerializeField] private TextMeshProUGUI m_TimerTxt;

    void Awake()
    {
        Instance = this;
    }

    public void SetTarget(VRPlayerController _target)
    {
        target = _target;
    }

    public void RefreshScoreBorad(int[] scores)
    {
        bool endGame = false;
        for (int i = 1; i < scores.Length; i++)
        {
            var tmp = scoreTexts[i - 1];
            tmp.text = scores[i].ToString();
        }
    }

    public void OnQuitClick()
    {
        MyGameManager.Instance.LeaveRoom();
    }

    public void ShowDamage()
    {
        healthFlash.SetActive(true);
    }

    public void ShowDeadPanel(Transform avatar, Action callback)
    {
        deadPanel.SetActive(true);
        deadPanel.transform.position = avatar.position + avatar.forward * 2;
        
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
        StartCoroutine(CoTimerInterval(5, 1, callback));
    }

    IEnumerator CoTimerInterval(int count, float delay, Action callback)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(delay);
            m_TimerTxt.text = (count - i).ToString();
        }

        deadPanel.GetComponent<AutoFadeOut>().enabled = true;
        callback?.Invoke();
    }

    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
    }

    public void ShowLosePanel()
    {
        losePanel.SetActive(true);
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
    }
}