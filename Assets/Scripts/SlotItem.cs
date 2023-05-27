using UnityEngine;
using UnityEngine.UI;

public class SlotItem : MonoBehaviour
{
    public Text countText;

    public void SetCount(int count)
    {
        countText.text = count.ToString();
    }
}