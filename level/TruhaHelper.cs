using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruhaHelper : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        StopAllCoroutines();
        Destroy(gameObject, 0.5F);
      //  DestroyImmediate(GetComponent<Collider>());
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
