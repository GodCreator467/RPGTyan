using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class ExpaPointHelper : MonoBehaviour {

    private Slider slider;
    private PlayerHelper player;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        slider.value = (player.ExpaPoint) * 100 / (player.MaxExpaPoint); //текущее кол-во опыта деленное на максимальное - получаем имеющийся опыта в процентах (от 0 до 1)
    }
    void Awake()
    {
        slider = GetComponent<Slider>();
        player = FindObjectOfType<PlayerHelper>();
    }
}
