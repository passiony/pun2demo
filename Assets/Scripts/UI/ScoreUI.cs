using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoSingleton<ScoreUI>
{
    [SerializeField] private TextMeshProUGUI[] m_ScoreTxt;

    public void UpdateScore(int team, int score)
    {
        m_ScoreTxt[team - 1].text = score.ToString();
    }
}