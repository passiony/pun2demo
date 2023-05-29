using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class AutoFadeOut : MonoBehaviour
{
    public float VisiblityDuration = 1;
    public float FadeDuration = 1;
    private CanvasGroup m_Canvas;

    private float m_DisplayTime;

    private void Awake()
    {
        m_Canvas = this.GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        m_Canvas.alpha = 1;
        m_DisplayTime = Time.time;
    }

    private void Update()
    {
        // Update the flash alpha.
        var alpha = Mathf.Min((FadeDuration - (Time.time - (m_DisplayTime + FadeDuration)) / FadeDuration), 1) *
                    m_Canvas.alpha;
        m_Canvas.alpha = alpha;

        if (alpha <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}