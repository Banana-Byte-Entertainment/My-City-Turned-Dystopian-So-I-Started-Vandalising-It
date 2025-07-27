using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject vehicleToSpawn;
    public float spawnDelay;
    private float spawnTimer = 0;
    public float startDelay = 0f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (startDelay > 0)
        {
            startDelay -= Time.deltaTime;
            return;
        }

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            Instantiate(vehicleToSpawn, transform.position, transform.rotation);
            spawnTimer = spawnDelay;
        }
    }
}
