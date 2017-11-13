using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UISoundLevelHelper : MonoBehaviour
{
    private Slider slider; //ссылка на слайдер Sound
    void Awake() //момент подгрузки скриптов на сцене
    {
        slider = GetComponent<Slider>(); //подключаемся к слайдеру, на котором висит скрипт
    }

    void Start()
    {
         slider.value = Settings.soundLevel; //при повторном открытие заставки игры считываем значения настроек из статич. класса Settings
    }
    public void Litor()
    {
        Settings.soundLevel = slider.value; //сохраняем значения полей в статический класс
    }
}
