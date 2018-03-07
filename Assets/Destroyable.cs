using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour {

    private GameController gc;
    [SerializeField] public GameObject explosion;

    void Start()
    {
        //GameObject gameControllerObject = GameObject.FindWithTag("gc");
        //if (gameControllerObject != null)
        //{
        //    gc = gameControllerObject.GetComponent<GameController>();
        //}
    }

    public virtual void DestroySelf()
    {
            
            GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
            Destroy(expl, 3);
            // play sound on destruction of asteroids
            //FindObjectOfType<AudioScript>().DestroyAsteroidSource.Play();
    }
}
