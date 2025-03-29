using UnityEditor;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 2.0f;
    public Camera cam; // used as internal 1st or 3rd person camera based on the vector 
    public GameObject fallingObject;

    private float oneSecond = 0.1f;
    private CharacterController characterController;
    private bool inTriggerZone = false;

    public float jumpThrust = 4.0f;
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
        Debug.Log(other.name + "enter");
    }

    private void OnTriggerExit(Collider other)
    {
        inTriggerZone = false;
        Debug.Log(other.name + "exist");
    }

    private void FixedUpdate()
    {
        if (inTriggerZone)
        {
            if (oneSecond <= 0.0f)
            {
                oneSecond = 0.1f;
                GameObject tmp = GameObject.Instantiate(fallingObject, gameObject.transform.position + new Vector3(0, 3, 0),
                Quaternion.identity);
                Debug.Log(gameObject.transform.position);
                tmp.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);
                Debug.Log(" OnStay");
            }
            oneSecond -= Time.deltaTime;
        }
    }



    // Update is called once per frame
    void Update()
    {
        Debug.Log("Grounded: " + characterController.isGrounded);//mesh byban ctrl shift c 

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveVector = new Vector3(x, 0, z);
        moveVector.Normalize();

        if (characterController.isGrounded)
        {
            yWithGravity = 0.0f;

            if (Input.GetButton("Jump"))
            {
                yWithGravity = jumpThrust;
            }
        }

        yWithGravity += Physics.gravity.y * Time.deltaTime;

        Vector3 moveAndJump = moveVector * speed;
        moveAndJump.y = yWithGravity;

        characterController.Move(moveAndJump * Time.deltaTime);// this allows jump and move at the same time

        //characterController.SimpleMove(moveVector * speed); //has restrictions on the y axis thus cant apply jump in simple jump 

        if (moveVector != Vector3.zero)
        {
            Quaternion lookAt = Quaternion.LookRotation(moveVector, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, 360 * Time.deltaTime);
        }

        cam.transform.position = gameObject.transform.position + new Vector3(0, 1, 5); 
        // 5 units behind the player, 1st person view 
        // -5 units behind the player, 3rd person view
        cam.transform.LookAt(gameObject.transform);



    }
}
