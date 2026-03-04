using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    public float baseSpeed = 10f;
    public float boostMultiplier = 3f;
    public float verticalSpeed = 5f;
    public float rotationSpeed = 50f;
    public float bankAngleLimit = 15f; // max bank angle

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalInput;
    private bool isBoosting;

    private float targetYaw;
    private float targetPitch;
    private float targetRoll;

    private GameInput gameInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        gameInput = new GameInput();
        gameInput.Enable();

        // Register input callbacks
        gameInput.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        gameInput.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        gameInput.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        gameInput.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        gameInput.Player.Dash.performed += ctx => isBoosting = ctx.ReadValue<float>() > 0.5f;
        gameInput.Player.Dash.canceled += ctx => isBoosting = false;

        // Optional: lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDestroy()
    {
        // Unsubscribe
        gameInput.Player.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        gameInput.Player.Move.canceled -= ctx => moveInput = Vector2.zero;

        gameInput.Player.Look.performed -= ctx => lookInput = ctx.ReadValue<Vector2>();
        gameInput.Player.Look.canceled -= ctx => lookInput = Vector2.zero;

        gameInput.Player.Dash.performed -= ctx => isBoosting = ctx.ReadValue<float>() > 0.5f;
        gameInput.Player.Dash.canceled -= ctx => isBoosting = false;

        gameInput.Disable();
    }

    void Update()
    {
        // Handle vertical movement
        float up = gameInput.Player.Up.ReadValue<float>();
        float down = gameInput.Player.Down.ReadValue<float>();
        verticalInput = (up > 0.5f) ? 1f : (down > 0.5f) ? -1f : 0f;

        // Handle rotation input (yaw and pitch)
        float yawInput = lookInput.x;
        float pitchInput = -lookInput.y; // invert if needed

        targetYaw += yawInput * rotationSpeed * Time.deltaTime;
        targetPitch += pitchInput * rotationSpeed * Time.deltaTime;

        // Clamp pitch to avoid flipping
        targetPitch = Mathf.Clamp(targetPitch, -80f, 80f);

        // Calculate roll for banking based on lateral movement
        targetRoll = -moveInput.x * bankAngleLimit;
    }

    void FixedUpdate()
    {
        float currentSpeed = baseSpeed * (isBoosting ? boostMultiplier : 1f);

        // Forward/Backward and strafing movement
        Vector3 desiredDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        if (desiredDirection.magnitude > 1f)
            desiredDirection.Normalize();

        Vector3 desiredVelocity = desiredDirection * currentSpeed;
        Vector3 velocityError = desiredVelocity - rb.linearVelocity;
        rb.AddForce(velocityError / 0.2f, ForceMode.Acceleration); // inertia factor

        // Vertical movement (ascend/descend)
        rb.AddForce(Vector3.up * verticalInput * verticalSpeed, ForceMode.Acceleration);

        // Rotation (yaw, pitch, roll)
        Quaternion targetRotation = Quaternion.Euler(targetPitch, targetYaw, targetRoll);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 0.1f));

        // Optional: Add banking tilt to the drone's local rotation for visual effect
        transform.localEulerAngles = new Vector3(targetPitch, targetYaw, targetRoll);
    }
}