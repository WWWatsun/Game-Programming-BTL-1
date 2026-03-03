using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairFollowMouse : MonoBehaviour
{
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private Canvas canvas;

    void Awake()
    {
        if (crosshair == null) crosshair = GetComponent<RectTransform>();
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (canvas == null || crosshair == null) return;
        if (Mouse.current == null) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPos
        );

        crosshair.anchoredPosition = localPos;
    }
}