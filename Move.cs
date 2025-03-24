using UnityEditor;
using UnityEngine;
// lect 4 
public class Move : MonoBehaviour
{
    public float speed = 2.0f;
    public Camera cam;
    public GameObject fallingObject;

    private float oneSecond = 0.1f;
    private CharacterController characterController;
    private bool inTriggerZone = false;//flag in zone or not

    public float jumpThrust = 4.0f; // how much it goes up 
    private float yWithGravity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }


    private void OnTriggerEnter(Collider other)
    {
        inTriggerZone = true;
        oneSecond = 0.1f;
    }

    private void OnTriggerExit(Collider other)
    {
        inTriggerZone = false;
    }

    private void FixedUpdate()//called every fixed frame, used when dealing with physics, falling objects in right area
    {
        if (inTriggerZone)
        {
            if (oneSecond <= 0.0f)
            {
                oneSecond = 0.1f;
                GameObject tmp = GameObject.Instantiate(fallingObject, gameObject.transform.position + new Vector3(0, 3, 0),
                Quaternion.identity);
                Debug.Log(gameObject.transform.position);
                tmp.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value,
                    Random.value);
                // Debug.Log(other.name + " OnStay");
            }
            oneSecond -= Time.deltaTime;
        }
    }
    



    // Update is called once per frame
    void Update()
    {

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveVector = new Vector3(x, 0, z);
        moveVector.Normalize();
        // JUMP 
        if (characterController.isGrounded)// if not the oject will fly like flappy bird 
        {
            yWithGravity = 0.0f;

            if (Input.GetButton("Jump"))// SPACE BAR automatically 
            {
                yWithGravity = jumpThrust; // not normilized to allow jump thrust
            }
        }

        yWithGravity += Physics.gravity.y * Time.deltaTime;// gravit max at once , so time(multiply with time to spread the effect )
                                                           // instanteous? no so * Time.deltatime 

        Vector3 moveAndJump = moveVector * speed; // move * speed
        moveAndJump.y = yWithGravity;

        characterController.Move(moveAndJump * Time.deltaTime);

        //characterController.SimpleMove(moveVector * speed);
        
        if(moveVector != Vector3.zero)
        {
            Quaternion lookAt = Quaternion.LookRotation(moveVector, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, 360 * Time.deltaTime);
        }

        cam.transform.position = gameObject.transform.position + new Vector3(0, 1, -5);
        cam.transform.LookAt(gameObject.transform);



    }
}
