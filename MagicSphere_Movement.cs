using UnityEngine;

public class MagicSphereMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    // Normal walking speed applied as torque
    public float walkSpeed = 50f;
    // Sprinting speed applied as torque when Left Shift is held
    public float sprintSpeed = 100f;
    // Maximum angular velocity (rotation speed) for the sphere to avoid extreme spins
    public float maxAngularVelocity = 100f;

    [Header("Jump Settings")]
    // Jump force when the player presses the jump button (Spacebar) while grounded
    public float jumpForce = 50f;
    // Jump force when the player is sprinting (holding Left Shift) and presses Spacebar
    public float sprintJumpForce = 100f;
    // LayerMask for detecting ground objects (used for checking if player is grounded)
    public LayerMask groundMask;
    // Boolean to check if the player is grounded or not
    private bool isGrounded = false;

    private Rigidbody rb; // The Rigidbody component used for applying physics forces
    private Camera mainCamera; // The main camera used to get relative movement direction

    private void Start()
    {
        // Initialize Rigidbody and camera
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        // Set the maximum angular velocity for the sphere's rotation
        rb.maxAngularVelocity = maxAngularVelocity;

        // Check if camera is assigned, otherwise throw an error
        if (mainCamera == null)
            Debug.LogError("No main camera found. Please tag your camera as 'MainCamera'");
    }

    private void Update()
    {
        // Handle jump input — jump only occurs when the player is grounded
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Check if the player is sprinting (holding LeftShift) and apply respective jump force
            float force = Input.GetKey(KeyCode.LeftShift) ? sprintJumpForce : jumpForce;

            // Apply upward force for the jump with Impulse mode (instant application)
            rb.AddForce(Vector3.up * force, ForceMode.Impulse);

            // Once the player jumps, they are no longer grounded until they hit the ground again
            isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        // Handle player movement in FixedUpdate to sync with physics engine
        ApplyMovementTorque();
    }

    void ApplyMovementTorque()
    {
        // Get horizontal (A/D, Left/Right) and vertical (W/S, Up/Down) axis input from keyboard
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // If there is no movement input, we skip applying torque
        if (horizontal == 0 && vertical == 0) return;

        // Normalize the input to ensure consistent movement speed regardless of direction
        Vector3 inputDir = new Vector3(horizontal, 0, vertical).normalized;

        // Calculate the camera's forward and right direction (ignoring vertical axis)
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;

        camForward.y = 0; // Remove Y component to prevent vertical movement
        camRight.y = 0; // Remove Y component to prevent vertical movement

        // Normalize the vectors so that movement is consistent regardless of camera rotation
        camForward.Normalize();
        camRight.Normalize();

        // Calculate the final movement direction based on camera orientation and player input
        Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;

        // Convert movement direction into torque axis (we swap X and Z axes for proper rolling effect)
        Vector3 torqueAxis = new Vector3(moveDir.z, 0, -moveDir.x);

        // Use walking speed or sprinting speed based on whether the Left Shift key is held
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        // Apply torque to the Rigidbody based on movement direction and speed
        rb.AddTorque(torqueAxis * speed);
    }

    // Called when the sphere stays in contact with any colliders
    private void OnCollisionStay(Collision collision)
    {
        // Check if the sphere is touching a ground object (based on the LayerMask)
        if (((1 << collision.gameObject.layer) & groundMask) != 0)
        {
            // For valid ground collisions, check if the normal of the collision point is facing upwards
            foreach (ContactPoint contact in collision.contacts)
            {
                // If the contact's normal is mostly upward (indicating ground), mark the sphere as grounded
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                    break; // Exit the loop after confirming grounded
                }
            }
        }
    }

    // Called when the sphere exits contact with a collider (typically when it leaves the ground)
    private void OnCollisionExit(Collision collision)
    {
        // If the sphere is no longer touching the ground layer, set isGrounded to false
        if (((1 << collision.gameObject.layer) & groundMask) != 0)
        {
            isGrounded = false;
        }
    }
}
