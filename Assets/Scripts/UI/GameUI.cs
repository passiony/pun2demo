using System.Collections.Generic;
using Photon.Pun.Demo.PunBasics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;
    private PlayerController target;
    private Slider hpSlider;
    private TextMeshProUGUI hpText;
    private SlotItem slotItem;
    private GameObject scorePrefab;

    private Transform scorePanel;
    [SerializeField]
    private GameObject winPanel;
    [SerializeField]
    private GameObject losePanel;
    private Dictionary<string, TextMeshProUGUI> scores;
    private bool gameOver;
    private bool win;
    
    void Awake()
    {
        Instance = this;
        hpSlider = transform.Find("HpSlider").GetComponent<Slider>();
        hpText = transform.Find("HpSlider/Text").GetComponent<TextMeshProUGUI>();
        slotItem = transform.Find("SlotItem").GetComponent<SlotItem>();
        scorePanel = transform.Find("Scores/SlotPanel");
        scorePrefab = scorePanel.Find("ScoreTemp").gameObject;
        scores = new Dictionary<string, TextMeshProUGUI>();
    }

    public void SetTarget(PlayerController _target)
    {
        target = _target;
    }

    void Update()
    {
        if (gameOver)
        {
            return;
        }
        if (target != null)
        {
            if (hpSlider != null)
            {
                hpSlider.value = target.HP / (float)target.MaxHP;
                hpText.text = $"{target.HP}";
            }

            if (slotItem != null)
            {
                slotItem.SetCount(target.CurrentBaseGun.BulletCount);
            }

            //更新计分板
            if (MyGameManager.Instance.ScoreBoard != null)
            {
                bool endGame = false;
                foreach (var player in MyGameManager.Instance.ScoreBoard)
                {
                    if (!scores.ContainsKey(player.Key))
                    {
                        var go = Instantiate(scorePrefab, scorePanel);
                        go.SetActive(true);
                        var tmp = go.GetComponent<TextMeshProUGUI>();
                        scores.Add(player.Key, tmp);
                    }
                    scores[player.Key].text = player.Key + " : " + player.Value;
                    if (player.Value >= 3)
                    {
                        gameOver = true;
                        if (PlayerController.LocalPlayer.photonView.Owner.NickName == player.Key)
                        {
                            win = true;
                            winPanel.SetActive(true);
                        }
                        else
                        {
                            win = false;
                            losePanel.SetActive(true);
                        }
                    }
                }
            }
        }
    }
}