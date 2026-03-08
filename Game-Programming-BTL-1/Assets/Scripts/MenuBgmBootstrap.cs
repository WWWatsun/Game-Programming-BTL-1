using UnityEngine;

public class MenuBgmBootstrap : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance?.PlayMenuBgm();
    }
}