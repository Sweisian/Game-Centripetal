using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Launch projectile

public class ShootingScript : MonoBehaviour
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
        shootCooldown = 0f;
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
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
                LookAt2D(m_currentTarget);
                shootAtPlayer(m_currentTarget);
            }
        }
    }

    public void shootAtPlayer(Transform m_currentTarget)
    {
        if (CanAttack)
        {
            
   
            shootCooldown = shootingRate;
            //Vector3 shotDirection = (transform.position - m_currentTarget.position).normalized;
            gc.playSound("cannonFire");
            Instantiate(shotPrefab, transform.position, transform.rotation);
        }
    }

    public void LookAt2D(Transform m_currentTarget)
    {
        Vector3 dir = m_currentTarget.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public bool CanAttack
    {
        get
        {
            return shootCooldown <= 0f;
        }
    }
}
