using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player movement controller. Reads input from the new Input System
/// (<see cref="PlayerInputActions"/>) and moves the player via <see cref="Rigidbody2D"/>.
///
/// Also exposes <see cref="FacingDirection"/> (last non-zero movement direction) for
/// weapons like <see cref="ShotgunWeapon"/> that fire in the player's facing direction.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // ── Inspector fields ──────────────────────────────────────────────

    [Tooltip("Movement speed in world units per second.")]
    [SerializeField] private float moveSpeed = 5f;

    // ── Runtime state ─────────────────────────────────────────────────

    /// <summary>Physics body used for movement.</summary>
    private Rigidbody2D _rb;

    /// <summary>Current frame's normalized movement input.</summary>
    private Vector2 _movementInput;

    /// <summary>Last non-zero movement direction (defaults to right).</summary>
    private Vector2 _lastFacing = Vector2.right;

    /// <summary>Generated input action asset instance.</summary>
    private PlayerInputActions _inputActions;

    // ── Public accessors ──────────────────────────────────────────────

    /// <summary>
    /// Last non-zero movement direction (default right).
    /// Used by weapons that fire in the player's facing direction (e.g. shotgun).
    /// </summary>
    public Vector2 FacingDirection => _lastFacing;

    // ── Unity lifecycle ───────────────────────────────────────────────

    /// <summary>Cache the Rigidbody2D and create the input actions.</summary>
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _inputActions = new PlayerInputActions();
    }

    /// <summary>Enable input actions when the component becomes active.</summary>
    private void OnEnable()
    {
        EnsureInputActions();
        _inputActions?.Player.Enable();
    }

    /// <summary>Disable input actions when the component becomes inactive.</summary>
    private void OnDisable()
    {
        _inputActions?.Player.Disable();
    }

    /// <summary>
    /// Read movement input each frame and update the facing direction.
    /// </summary>
    private void Update()
    {
        EnsureInputActions();
        _movementInput = _inputActions.Player.Move.ReadValue<Vector2>().normalized;

        // Track the last non-zero direction for facing.
        if (_movementInput.sqrMagnitude > 0.01f)
            _lastFacing = _movementInput.normalized;
    }

    /// <summary>
    /// Apply movement velocity to the Rigidbody2D in FixedUpdate for smooth physics.
    /// </summary>
    private void FixedUpdate()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        _rb.linearVelocity = _movementInput * moveSpeed;
    }

    // ── Public stat modifiers ─────────────────────────────────────────

    /// <summary>Adjust move speed by a flat amount (for upgrades).</summary>
    public void ModifyMoveSpeed(float amount)
    {
        moveSpeed += amount;
    }

    // ── Private helpers ───────────────────────────────────────────────

    /// <summary>Lazy-create input actions if they were null (safety net).</summary>
    private void EnsureInputActions()
    {
        if (_inputActions == null)
            _inputActions = new PlayerInputActions();
    }
}
