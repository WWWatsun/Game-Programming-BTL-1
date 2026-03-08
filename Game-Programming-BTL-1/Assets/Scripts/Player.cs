using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float sprintMultiplier = 1.75f;

    [Header("Look Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float maxLookAngle = 85.0f;

    [Header("Gun Reference")]
    [SerializeField] private Gun gun;
    [SerializeField] private float fireRate = 10f; // shots per second when holding fire
    [SerializeField] private float minRecoil = 0.5f;
    [SerializeField] private float maxRecoil = 5f;

    // Singleton instance
    public static Player Instance { get; private set; }

    // References
    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float verticalRotation = 0f;
    private bool isPlaying;

    private Coroutine fireCoroutine;
    private float recoil = 0f;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        isPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleLook();
    }

    // -------------
    // Input methods
    // -------------
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isPlaying) return; // Ignore input if the game hasn't started
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (!isPlaying) return; // Ignore input if the game hasn't started
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!isPlaying) return; // Ignore input if the game hasn't started
        if (context.performed && characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!isPlaying) return; // Ignore input if the game hasn't started
        if (context.performed)
        {
            // Implement interaction logic here
            Debug.Log("Interact!");
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (!isPlaying) return; // Ignore input if the game hasn't started

        if (context.performed)
        {
            StartFiring();
        }
        else if (context.canceled)
        {
            StopFiring();
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (!isPlaying) return; // Ignore input if the game hasn't started
        if (context.performed)
        {
            Debug.Log("Aim!");
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (!isPlaying) return; // Ignore input if the game hasn't started
        if (context.performed)
        {
            moveSpeed *= sprintMultiplier; // Increase speed by 50% when sprinting
        }
        else if (context.canceled)
        {
            moveSpeed /= sprintMultiplier; // Reset speed when not sprinting
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (!isPlaying) return; // Ignore input if the game hasn't started
        if (context.performed)
        {
            // Implement crouch logic here
            Debug.Log("Crouch!");
        }
    }

    // -------------
    // Private methods
    // -------------
    private void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small negative value to keep grounded
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        verticalRotation -= mouseY + recoil;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void StartFiring()
    {
        if (gun == null) return;
        if (fireCoroutine != null) return;

        fireCoroutine = StartCoroutine(FireRoutine());
    }

    private void StopFiring()
    {
        if (fireCoroutine == null) return;
        StopCoroutine(fireCoroutine);
        fireCoroutine = null;
        recoil = 0f;
    }

    private IEnumerator FireRoutine()
    {
        // clamp fire rate
        float rate = Mathf.Max(0.0001f, fireRate);
        float interval = 1f / rate;

        // Immediately fire once on press
        gun.Fire();

        while (true)
        {
            yield return new WaitForSeconds(interval);
            gun.Fire();
            recoil = Mathf.Min(maxRecoil, recoil + minRecoil);
        }
    }

    // -------------
    // Public methods
    // -------------

    public void StartGame()
    {
        isPlaying = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void EndGame()
    {
        isPlaying = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        moveInput = Vector2.zero;
        lookInput = Vector2.zero;
    }

    public void ResetPlayer()
    {
        EndGame();
        transform.position = Vector3.zero;
        velocity = Vector3.zero;
        lookInput = Vector2.zero;
        verticalRotation = 0f;
    }

    public void SetMouseSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }
}
