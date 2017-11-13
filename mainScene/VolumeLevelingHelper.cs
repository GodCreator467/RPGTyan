using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeLevelingHelper : MonoBehaviour
{

    private AudioSource source;
    // Use this for initialization

    void Start()
    {
        //  Time.timeScale = 1.0F; //нормальный ход времени (на случай, если игра была на паузе)
        source = GetComponent<AudioSource>();//ссылка на компонент музыки        
    }
    public void Litor()
    {
        if (source)
            source.volume = Settings.soundLevel; //берем значение громкости из настроек 
    }

}
