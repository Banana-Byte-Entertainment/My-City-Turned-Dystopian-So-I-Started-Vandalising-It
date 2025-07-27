using UnityEngine;

public class NPCVehicle : MonoBehaviour
{
    public float speed = 10f;
    public float timeTillDeletion = 60f;
    public Rigidbody rigidBody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (timeTillDeletion <= 0)
            Destroy(gameObject);
        else
            timeTillDeletion -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        rigidBody.MovePosition(rigidBody.position + transform.forward * speed * Time.deltaTime);
    }
}
