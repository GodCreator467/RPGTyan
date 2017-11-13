using UnityEngine;
using System.Collections;

public class UIPauseButtonHelper : MonoBehaviour {
    public AudioClip _1lvl;
    public AudioClip _2lvl;
    public void PauseGame()
    {
        Time.timeScale = 0.0F;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0F;
        if (!GetComponent<AudioSource>().isPlaying)//если еще не запещено проигрывание
        {
            switch (StatInfo.CurrentLevel)
            {
                case 3://1 уровень
                    GetComponent<AudioSource>().PlayOneShot(_1lvl);
                    break;
                case 4://2 уровень
                    GetComponent<AudioSource>().PlayOneShot(_2lvl);
                    break;
            }
        }


    }
    void Start()//включаем фон. музыку уровня
    {
        switch (StatInfo.CurrentLevel)
        {
            case 3://1 уровень
                GetComponent<AudioSource>().PlayOneShot(_1lvl);
                break;
            case 4://2 уровень
                GetComponent<AudioSource>().PlayOneShot(_2lvl);
                break;
        }


    }
}
