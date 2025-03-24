using UnityEngine;

public class destroymaterial : MonoBehaviour
{
    private float destroy_time = 5.0f;// 5 secS
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        destroy_time -= Time.deltaTime;
        if (destroy_time  <= 0)
        {
            Destroy(gameObject);// mynfa3sah a destroy myself fa game obj delets material
        }
    }
}
