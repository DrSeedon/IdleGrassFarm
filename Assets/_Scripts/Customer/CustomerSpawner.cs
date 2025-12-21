using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public float spawnRadius = 10f;
    public float spawnInterval = 5f;
    public int maxCustomers = 20;

    List<Customer> activeCustomers = new List<Customer>();
    float spawnTimer;

    void Start()
    {
        spawnTimer = spawnInterval;
        SpawnInitialCustomers();
    }

    void SpawnInitialCustomers()
    {
        int toSpawn = Mathf.Min(maxCustomers, 5);
        for (int i = 0; i < toSpawn; i++)
        {
            SpawnCustomer();
        }
    }

    void Update()
    {
        activeCustomers.RemoveAll(c => c == null);

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f && activeCustomers.Count < maxCustomers)
        {
            SpawnCustomer();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnCustomer()
    {
        if (customerPrefab == null) return;

        Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
        randomPos.y = transform.position.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, spawnRadius, NavMesh.AllAreas))
        {
            GameObject customerObj = Instantiate(customerPrefab, hit.position, Quaternion.identity);
            Customer customer = customerObj.GetComponent<Customer>();
            
            if (customer != null)
            {
                activeCustomers.Add(customer);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
