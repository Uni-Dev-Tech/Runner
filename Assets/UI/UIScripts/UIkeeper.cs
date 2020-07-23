using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIkeeper : MonoBehaviour
{
    static public float musicVolume;
    static public float soundEffectsVolume;
    static public bool accept = false;
    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Destroy(this.gameObject);
        }
    }
    public void Update()
    {
        if (accept == true)
        {
            Destroy(this.gameObject);
        }
    }
}
