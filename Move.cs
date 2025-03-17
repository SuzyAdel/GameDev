using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 10.0f; // This is the speed of the object
    public Camera cam;  // Drag and drop main camera, and below, we'll make it follow the player
    //public GameObject ObjectName; // This is the object that will be seen in Unity straight away 
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        Vector3 moveVector = new Vector3(x, 0, z);
        moveVector.Normalize(); // Normalize to prevent diagonal speed-up

        transform.Translate(moveVector * speed * Time.deltaTime, Space.World);

        Quaternion lookAt = Quaternion.LookRotation(moveVector, Vector3.up); // Fixed Quarternion typo

        if (moveVector != Vector3.zero)
        {
            // transform.forward = moveVector; // The slime axis flips, so make it relative to space
            // transform.rotation = lookAt; // Causes snapping
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, 720 * Time.deltaTime); // 360 or 720 is the angular speed

            // Camera follows the player correctly
            cam.transform.position = gameObject.transform.position + new Vector3(0, 1, -5);
            cam.transform.LookAt(gameObject.transform.position);
        }
    }
}
