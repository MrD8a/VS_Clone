using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 _lastFacing = Vector2.right;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        EnsureInputActions();
        inputActions?.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions?.Player.Disable();
    }

    private void EnsureInputActions()
    {
        if (inputActions == null)
            inputActions = new PlayerInputActions();
    }

    private void Update()
    {
        EnsureInputActions();
        movementInput = inputActions.Player.Move.ReadValue<Vector2>().normalized;
        if (movementInput.sqrMagnitude > 0.01f)
            _lastFacing = movementInput.normalized;
    }

    /// <summary>
    /// Last non-zero movement direction (default right). Use for weapons that fire in facing direction (e.g. shotgun).
    /// </summary>
    public Vector2 FacingDirection => _lastFacing;

    private void FixedUpdate()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = movementInput * moveSpeed;
    }

    public void ModifyMoveSpeed(float amount)
    {
        moveSpeed += amount;
    }

}
