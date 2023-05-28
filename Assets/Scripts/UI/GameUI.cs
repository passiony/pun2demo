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

    void Awake()
    {
        Instance = this;
        hpSlider = transform.Find("HpSlider").GetComponent<Slider>();
        hpText = transform.Find("HpSlider/Text").GetComponent<TextMeshProUGUI>();
        slotItem = transform.Find("SlotItem").GetComponent<SlotItem>();
    }

    public void SetTarget(PlayerController _target)
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
                slotItem.SetCount(target.CurrentBaseGun.BulletCount);
            }
        }
    }
}