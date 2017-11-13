using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePreloader : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        SceneManager.LoadSceneAsync("myMainMenu");
    }


}
