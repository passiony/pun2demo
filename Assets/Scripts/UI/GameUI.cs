using System;
using System.Collections;
using MFPS;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;
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

    void Update()
    {
        if (XRPlayer.Instance.Player)
        {
            transform.position = XRPlayer.Instance.Player.Avatar.Head.position + Vector3.up;

            var forward = transform.position - XRPlayer.Instance.Head.position;
            forward.y = 0;
            if (forward.z != 0)
            {
                transform.forward = forward;
            }
        }
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
        // healthFlash.SetActive(true);
    }

    public void ShowDeadPanel(Action callback)
    {
        deadPanel.SetActive(true);

        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
        StartCoroutine(CoTimerInterval(FPSGame.REBORN_TIME, 1, callback));
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