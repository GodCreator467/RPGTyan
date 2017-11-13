using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIFXLevelHelper : MonoBehaviour {
    private Slider slider; //ссылка на слайдер FX
    void Awake() //момент подгрузки скриптов на сцене
    {
        slider = GetComponent<Slider>(); //подключаемся к слайдеру, на котором висит скрипт
    }

    void Start()
    {
        slider.value = Settings.fxLevel; //при повторном открытие заставки игры считываем значения настроек из статич. класса Settings
    }
    public void Litor ()
    {
        Settings.fxLevel = slider.value; //сохраняем значения полей в статический класс
    }
}
