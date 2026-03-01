using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerScript : MonoBehaviour
{
    public static PlayerScript Instance;
    bool isPlaying = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        //Only register this when player is playing
        if (GameManager2D.Instance.GetGameState() == GameManager2D.GameState.PLAYING)
        {
            if (context.performed)
            {
                Debug.Log("Shoot");
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                Debug.Log(worldPosition.x + " " + worldPosition.y);
                Collider2D hit = Physics2D.OverlapPoint(worldPosition);
                if (hit != null)
                {
                    TargetScript_2D target = hit.GetComponent<TargetScript_2D>();
                    target.OnHit();
                }
                else
                {
                    Debug.Log("Gà");
                    GameManager2D.Instance.RegisterWhip();
                }
            }
        }
    }

    public void SetPlaying(bool isPlaying)
    {
        this.isPlaying = isPlaying;
    }
}
