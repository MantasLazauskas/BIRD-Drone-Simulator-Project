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

    // Use the abstract input provider (assign DroneInput component in inspector or allow GetComponent fallback)
    [SerializeField] private DroneInputBase inputSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        Debug.Log($"DroneController Awake: rb.isKinematic={rb.isKinematic} mass={rb.mass} drag={rb.linearDamping} constraints={rb.constraints}");

        if (inputSource == null)
            inputSource = GetComponent<DroneInputBase>();

        // Optional: lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (inputSource == null)
        {
            Debug.LogWarning("DroneController: no inputSource assigned or found on GameObject.");
            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
            verticalInput = 0f;
            isBoosting = false;
        }
        else
        {
            // Read current values each frame
            moveInput = inputSource.Move();
            lookInput = inputSource.Look();

            float up = inputSource.Up();
            float down = inputSource.Down();
            verticalInput = (up > 0.5f) ? 1f : (down > 0.5f) ? -1f : 0f;

            isBoosting = inputSource.DashInput() > 0.5f;
        }

        // Rotation input
        float yawInput = lookInput.x;
        float pitchInput = -lookInput.y;

        targetYaw += yawInput * rotationSpeed * Time.deltaTime;
        targetPitch += pitchInput * rotationSpeed * Time.deltaTime;

        targetPitch = Mathf.Clamp(targetPitch, -80f, 80f);

        targetRoll = -moveInput.x * bankAngleLimit;

        // Lightweight logging only when input exists
        if (moveInput.sqrMagnitude > 0.0001f || isBoosting)
            Debug.Log($"DroneController Update: moveInput={moveInput} dash={isBoosting} vertical={verticalInput}");
    }

    void FixedUpdate()
    {
        float currentSpeed = baseSpeed * (isBoosting ? boostMultiplier : 1f);

        Vector3 desiredDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        if (desiredDirection.magnitude > 1f)
            desiredDirection.Normalize();

        Vector3 desiredVelocity = desiredDirection * currentSpeed;

        // Use rb.velocity and apply force to reach desiredVelocity
        Vector3 velocityError = desiredVelocity - rb.linearVelocity;
        rb.AddForce(velocityError / 0.2f, ForceMode.Acceleration);

        // Vertical
        rb.AddForce(Vector3.up * verticalInput * verticalSpeed, ForceMode.Acceleration);

        // Rotation via physics
        Quaternion targetRotation = Quaternion.Euler(targetPitch, targetYaw, targetRoll);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 10f * Time.fixedDeltaTime));

        if (desiredVelocity.sqrMagnitude > 0.0001f)
            Debug.Log($"DroneController FixedUpdate: desired={desiredVelocity} rb.velocity={rb.linearVelocity}");
    }
}