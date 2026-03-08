using UnityEngine;

public class StopBgmOnStart : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance?.StopBgm();
    }
}