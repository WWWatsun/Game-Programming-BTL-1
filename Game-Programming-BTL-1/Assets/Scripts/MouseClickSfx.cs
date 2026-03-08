using UnityEngine;
using UnityEngine.InputSystem;

public class MouseClickSfx : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current == null) return;

        // phát click cho mọi cú bấm chuột trái
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Nếu đang PLAYING thì để Aim2DInput phát "shoot" (tránh double sound)
            if (GameManager.Instance != null &&
                GameManager.Instance.GetGameState() == GameManager.GameState.PLAYING)
                return;

            AudioManager.Instance?.PlayClick();
        }
    }
}