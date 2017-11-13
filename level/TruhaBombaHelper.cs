using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruhaBombaHelper : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StopAllCoroutines();
        Destroy(gameObject, 3.0F);//уничтожаем облако взрыва через 3сек (длительность анимации)
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
