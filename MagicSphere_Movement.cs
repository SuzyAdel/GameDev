using UnityEngine;
using UnityEngine.InputSystem; // Added for Input System

public class MagicSphereMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 50f;
    public float sprintSpeed = 100f;
    public float maxSpin = 100f;

    [Header("Jump")]
    public float jumpPower = 50f;
    public float sprintJumpPower = 100f;
    public float groundCheckDistance = 0.6f;
    public LayerMask groundMask; // Corrected variable name

    private Rigidbody rb;
    private Camera mainCamera;
    private Vector2 moveInput; // Added for Input System

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        // Limit maximum rotation speed 
        rb.maxAngularVelocity = maxSpin; // Corrected variable name
    }

    void Update()
    {
        // Handle jumping in Update for better responsiveness no flapping
        if (Input.GetButton("Jump") && IsGrounded()) // Changed Input.GetKeyDown to Input System
        {
            // Apply stronger jump when sprinting by holding Left Shift using sprintJumpPower
            float force = Keyboard.current.leftShiftKey.isPressed ? sprintJumpPower : jumpPower; // Corrected variable names, and changed Input.GetKey
            rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = GetCameraRelativeMovement();

        if (movement != Vector3.zero)
        {
            // Apply torque for natural rolling movement which is different
            // from Unity's default rigidbody movement because it uses torque instead of force which makes it more realistic
            ApplyMovementTorque(movement);
        }
    }

    private bool IsGrounded()
    {
        // Only allow jumping when grounded by raycasting downwards can be done by using a sphere collider 
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask); // Corrected variable name
    }

    private Vector3 GetCameraRelativeMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector2 input = new Vector2(x, z);

        // Movement relative to camera orientation 
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        // Flatten camera directions which means ignoring y-axis
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Combine and normalize movement direction by camera orientation and input
        return (cameraForward * input.y + cameraRight * input.x).normalized;
    }

    private void ApplyMovementTorque(Vector3 direction)
    {
        // Use moveSpeed (50) or sprintSpeed (100) when holding Left Shift
        float torque = Keyboard.current.leftShiftKey.isPressed ? sprintSpeed : moveSpeed; // Corrected variable names, and changed Input.GetKey

        // Convert movement direction to torque axis (z and x swapped for proper rolling)
        Vector3 torqueAxis = new Vector3(direction.z, 0, -direction.x);

        rb.AddTorque(torqueAxis * torque);
    }

    // Visualize ground check in editor by raycasting downwards
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }

    // Input System callback for movement
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}