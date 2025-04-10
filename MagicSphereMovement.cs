using UnityEngine;

public class MagicSphereMovement : MonoBehaviour
{
    // Movement Settings: Adjusts walking and sprinting speeds.
    [Header("Movement Settings")]
    public float walkSpeed = 50f;                // Speed applied when walking (applies torque for rolling)
    public float sprintSpeed = 100f;             // Speed applied when sprinting (applies torque for rolling)
    public float maxAngularVelocity = 100f;     // Max angular velocity to limit extreme spinning

    // Jump Settings: Configures forces for jumping while walking or sprinting.
    [Header("Jump Settings")]
    public float jumpForce = 50f;               // Jump force when walking
    public float sprintJumpForce = 100f;        // Jump force when sprinting
    public LayerMask groundMask;                // Layer mask to detect ground objects
    bool isGrounded = false;                    // Boolean to check if the sphere is grounded

    Rigidbody rb;                               // Reference to the Rigidbody for applying forces

    // Initialization: Getting the Rigidbody component and configuring max angular velocity.
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Reference to Rigidbody for physics-based movement
        rb.mass = 10; // Set the mass of the sphere to 10
        rb.maxAngularVelocity = maxAngularVelocity; // Prevent unrealistic spinning by limiting angular velocity
    }

    // Update: Handles input for jumping and checking grounded status.
    void Update()
    {
        // Check if the player pressed the Jump button and is grounded (no double jumps)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Determine the appropriate jump force (based on sprinting)
            float force = Input.GetKey(KeyCode.LeftShift) ? sprintJumpForce : jumpForce;

            // Apply an upward force to the Rigidbody to simulate the jump
            rb.AddForce(Vector3.up * force, ForceMode.Impulse);

            // Mark the sphere as not grounded until it lands
            isGrounded = false;
        }
    }

    // FixedUpdate: Handles physics-based movement.
    void FixedUpdate()
    {
        ApplyMovementTorque(); // Apply movement torque based on player input
    }

    // ApplyMovementTorque: Applies torque for rolling movement based on player input.
    void ApplyMovementTorque()
    {
        // Get input values for horizontal and vertical movement (WASD or Arrow keys)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // If no input is received, don't apply torque (prevents unwanted movement)
        if (horizontal == 0 && vertical == 0) return;

        // Calculate direction vector based on user input (normalized to avoid diagonal speed boost)
        Vector3 inputDir = new Vector3(horizontal, 0, vertical).normalized;

        // Calculate the axis of torque (we swap X and Z axes to make it roll correctly)
        Vector3 torqueAxis = new Vector3(inputDir.z, 0, -inputDir.x);

        // Choose the movement speed (normal or sprinting based on LeftShift key)
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        // Apply torque to the Rigidbody to simulate rolling movement
        rb.AddTorque(torqueAxis * speed);
    }

    // OnCollisionStay: Detects if the sphere is grounded by checking for contact with the ground layer.
    private void OnCollisionStay(Collision collision)
    {
        // Check if the collided object is on the ground layer (specified by groundMask)
        if (((1 << collision.gameObject.layer) & groundMask) != 0)
        {
            // Iterate through collision contacts to find any upward-facing normals (indicating the ground)
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f) // If the collision normal points mostly upwards, consider the sphere grounded
                {
                    isGrounded = true;
                    break; // Exit the loop once we confirm grounding
                }
            }
        }
    }

    // OnCollisionExit: Sets isGrounded to false when the sphere leaves the ground.
    private void OnCollisionExit(Collision collision)
    {
        // If the sphere stops touching the ground layer, mark it as not grounded
        if (((1 << collision.gameObject.layer) & groundMask) != 0)
        {
            isGrounded = false;
        }
    }
}
