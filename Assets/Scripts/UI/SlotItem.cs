using UnityEngine;
using UnityEngine.UI;

public class SlotItem : MonoBehaviour
{
    public Text countText;

    public void SetCount(int count, int total)
    {
        countText.text = $"{count}/{total}";
    }
}