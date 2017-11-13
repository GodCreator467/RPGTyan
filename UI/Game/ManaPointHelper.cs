using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ManaPointHelper : MonoBehaviour {

    private Slider slider;
    private PlayerHelper player;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        slider.value = (player.ManaPoint) * 100 / (player.MaxManaPoint); //текущее кол-во маны деленное на максимальное - получаем имеющуюся ману в процентах (от 0 до 1)
    }
    void Awake()
    {
        slider = GetComponent<Slider>();
        player = FindObjectOfType<PlayerHelper>();
    }
}
