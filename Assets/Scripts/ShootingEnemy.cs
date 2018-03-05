using UnityEngine;
using System.Collections;

/// Launch projectile

public class ShootingEnemy : MonoBehaviour
{

    // 1 - Designer variables

    /// Projectile prefab for shooting

    public GameObject shotPrefab;

    private GameObject playerTarget;
    private Transform m_currentTarget;
    private GameController gc;
    [SerializeField] private float maxShootDistance = 100f;

    /// <summary>
    /// Cooldown in seconds between two shots
    /// </summary>
    public float shootingRate = 0.25f;


    // 2 - Cooldown

    private float shootCooldown;

    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gc.playSound("cannonFire");
        shootCooldown = 0f;
        if (GameObject.FindWithTag("Player"))
        {
            playerTarget = GameObject.FindWithTag("Player").gameObject;
        }
    }

    void Update()
    {
        

        if (shootCooldown > 0)
        {
            shootCooldown -= Time.deltaTime;
        }

        if (playerTarget != null)
        {
            m_currentTarget = playerTarget.transform;
            if (Vector3.Distance(m_currentTarget.position, transform.position) < maxShootDistance)
            {
                shootAtPlayer(m_currentTarget);
            }
        }
    }

    public void shootAtPlayer(Transform m_currentTarget)
    {
        if(true)
        {
            shootCooldown = shootingRate;
            Vector3 shotDirection = (transform.position - m_currentTarget.position);
            Instantiate(shotPrefab, shotDirection, transform.rotation);
        }
    }

    public void LookAt2D(Transform m_currentTarget)
    {
        Quaternion rotation = Quaternion.LookRotation
            (m_currentTarget.position - transform.position, transform.TransformDirection(Vector3.up));
        transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
        transform.LookAt(playerTarget.transform);
    }

    public bool CanAttack
    {
        get
        {
            return shootCooldown <= 0f;
        }
    }
}