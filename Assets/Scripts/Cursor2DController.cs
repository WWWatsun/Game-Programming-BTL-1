using UnityEngine;

public class Cursor2DController : MonoBehaviour
{
    [SerializeField] private Texture2D aimCursor;
    [SerializeField] private Vector2 hotspot = new Vector2(16, 16);
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    private GameManager2D.GameState lastState = (GameManager2D.GameState)(-1);

    private void Update()
    {
        if (GameManager2D.Instance == null) return;

        var state = GameManager2D.Instance.GetGameState();
        if (state == lastState) return;   // ✅ không đổi state thì không đụng cursor

        lastState = state;
        ApplyForState(state);
    }

    private void ApplyForState(GameManager2D.GameState state)
    {
        bool playing = state == GameManager2D.GameState.PLAYING;

        if (playing)
        {
            Cursor.SetCursor(aimCursor, hotspot, cursorMode);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}