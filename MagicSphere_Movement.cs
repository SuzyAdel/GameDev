using UnityEngine;

public class MagicSphereMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 50f;               // Torque multiplier when walking (default movement)
    public float sprintSpeed = 100f;            // Torque multiplier when holding Left Shift (sprint)
    public float maxAngularVelocity = 100f;     // Cap the angular velocity to prevent unrealistic spinning

    [Header("Jump Settings")]
    public float jumpForce = 50f;               // Jump force when standing still or walking
    public float sprintJumpForce = 100f;        // Jump force when sprinting (running jump)
    public float groundCheckRadius = 0.6f;      // Radius of sphere used to detect ground collision
    public LayerMask groundMask;                // Mask to filter what counts as ground (e.g., "Ground" layer)

    private Rigidbody rb;
    private Camera mainCamera;
    private bool isGrounded;                    // Caches grounded state to be used each frame

    void Start()
    {
        // Cache references to Rigidbody and Main Camera
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        // Safety checks to catch missing references early
        if (rb == null)
            Debug.LogError("Rigidbody not found on object!");

        if (mainCamera == null)
            Debug.LogError("Main Camera not found in scene!");

        // Set maximum rotation velocity to avoid infinite spinning or physics bugs
        rb.maxAngularVelocity = maxAngularVelocity;
    }

    void Update()
    {
        // Check ground status once per frame to avoid redundant physics calls
        isGrounded = CheckIfGrounded();

        // Jump only if grounded (prevents double-jumps or mid-air jumps)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Use higher jump force when sprinting (Left Shift is held)
            float appliedJumpForce = Input.GetKey(KeyCode.LeftShift) ? sprintJumpForce : jumpForce;

            // Apply upward impulse to simulate jump using physics
            rb.AddForce(Vector3.up * appliedJumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Handle player movement in physics step for more consistent motion
        MovePlayer();
    }

    void MovePlayer()
    {
        // Get raw input from Input Manager (Horizontal: A/D, Vertical: W/S)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Normalize movement vector to avoid faster diagonal movement
        Vector3 inputDir = new Vector3(x, 0, z).normalized;
        if (inputDir == Vector3.zero) return; // Exit early if no input

        // Get camera-relative direction (forward and right vectors)
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;

        // Ignore vertical camera tilt for ground movement
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Combine input and camera orientation for intuitive controls
        Vector3 moveDir = (camForward * z + camRight * x).normalized;

        // Convert world direction to torque axis (z and x swapped)
        Vector3 torqueAxis = new Vector3(moveDir.z, 0, -moveDir.x);

        // Choose walking or sprinting speed based on Left Shift input
        float torqueSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        // Apply torque to simulate rolling in the intended direction
        rb.AddTorque(torqueAxis * torqueSpeed);
    }

    bool CheckIfGrounded()
    {
        // If no groundMask set in inspector, fallback to default 'everything' mask
        if (groundMask == 0)
        {
            Debug.LogWarning("Ground mask not assigned! Defaulting to all layers.");
            return Physics.CheckSphere(transform.position, groundCheckRadius);
        }

        // Use Physics.CheckSphere to verify if player is touching ground layer
        return Physics.CheckSphere(transform.position, groundCheckRadius, groundMask);
    }

    void OnDrawGizmosSelected()
    {
        // Visualize the ground check radius in editor for easier tweaking
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }
}
