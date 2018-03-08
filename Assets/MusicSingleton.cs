using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSingleton : MonoBehaviour {

    public GameObject musicPlayer;
    private void Awake()
    {
        musicPlayer = GameObject.Find("MUSIC");
        if (musicPlayer==null)
        {
            musicPlayer = this.gameObject;
            musicPlayer.name = "MUSIC";
            DontDestroyOnLoad(musicPlayer);
        }
        else
        {
            if (this.gameObject.name!="MUSIC")
            {
                Destroy(this.gameObject);
            }
            
        }
    }


    /*
    private static MusicSingleton instance = null;
    private static MusicSingleton Instance{
        get { return Instance; }
    }

    private void Awake()
    {
        if (instance!=null && instance!=this)
        {
            Destroy(gameObject);
            return;
        }
        else {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	*/
}
