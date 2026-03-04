using UnityEngine;
using UnityEngine.InputSystem;

public class Aim2DInput : MonoBehaviour
{
    [SerializeField] private Camera cam;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        var state = GameManager.Instance.GetGameState();
        bool playing = state == GameManager.GameState.PLAYING;

        // Để Cursor2DController xử lý cursor, nên Aim2DInput ko cần đụng vào cursor nữa
        // // Cursor behavior: hide only when playing, show otherwise (SummaryUI sẽ có chuột)
        // Cursor.visible = !playing;
        // Cursor.lockState = playing ? CursorLockMode.Confined : CursorLockMode.None;

        // ESC pause vẫn cho phép bấm ở mọi state (trừ khi bạn muốn khác)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            GameManager.Instance.PauseGame();
            return;
        }

        // Only allow shooting when playing
        if (!playing) return;

        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            AudioManager.Instance?.PlayShoot();

            Target t = TargetBool2D.Instance != null ? TargetBool2D.Instance.ActiveTarget : null;

            if (t == null || !t.gameObject.activeInHierarchy)
            {
                GameManager.Instance.RegisterMiss();
                TargetBool2D.Instance?.RequestRespawnWithDelay();
                return;
            }

            Vector2 screenPos = Mouse.current.position.ReadValue();
            Vector3 world = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
            Vector2 clickPos = new Vector2(world.x, world.y);

            Vector2 targetPos = new Vector2(t.transform.position.x, t.transform.position.y);

            float radius = 0.5f;
            var sr = t.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) radius = Mathf.Max(sr.bounds.extents.x, sr.bounds.extents.y);

            bool hit = Vector2.Distance(clickPos, targetPos) <= radius;

            if (hit) t.TargetHit();
            else GameManager.Instance.RegisterMiss();

            TargetBool2D.Instance?.RequestRespawnWithDelay();
        }
    }
}