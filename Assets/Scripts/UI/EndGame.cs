using MFPS;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public void OnQuitClick()
    {
        MyGameManager.Instance.LeaveRoom();
    }
}
