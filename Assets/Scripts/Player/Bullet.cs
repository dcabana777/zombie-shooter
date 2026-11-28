
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 25f;
    [SerializeField] private float lifetime = 3f;

    // piercing settings
    [SerializeField] private bool isPiercing = false;   // determines if bullet can pierce enemy
    [SerializeField] private int maxEnemiesHit = 3; // max number of enemies pierced

    private int enemiesHit = 0; // tracks enemies bullet pierced

    private HashSet<EnemyPathfinding> hitEnemies = new HashSet<EnemyPathfinding>(); // hashset tracks unique enemies hit

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        // moves bullet forwards
        rb.linearVelocity = transform.forward * speed;

        // destroy bullet when done
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        EnemyPathfinding enemy = other.GetComponentInParent<EnemyPathfinding>();


        if (isPiercing && enemy != null)
        {
            // counts enemy only once
            if (hitEnemies.Add(enemy))
            {
                enemiesHit++;
                Debug.Log("Enemies hit: " + enemiesHit);
            }

            // destroys after reaching max enemy count
            if (enemiesHit >= maxEnemiesHit)
            {
                Destroy(gameObject);

            }

            return;
        }

        // destroy when hitting walls or other solid objects
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }

}
