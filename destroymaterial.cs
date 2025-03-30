using UnityEngine;

public class destroymaterial : MonoBehaviour
{
    private float destroy_time = 30.0f;// if it fell on the ground distroy after 30 sec 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        destroy_time -= Time.deltaTime;
        if (destroy_time <= 0)
        {
            Destroy(gameObject);// mynfa3sah a destroy myself fa game obj delets material
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Spike")// if fell on the spike // shell  distroy , later will change to burst with animation 
        {
            Destroy(gameObject);
        }
    }
}
