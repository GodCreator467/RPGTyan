using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemMoney : MonoBehaviour
{
    public GameObject _gold; //ссылка на дочерний объект монетки
    private AudioSource _audio;// Для эффекта сбора монетки
    private bool sbor; //факт сбора монетки
    void Awake()
    {
        _audio = GetComponent<AudioSource>();//ссылка на компонент музыки
        _audio.volume = Settings.fxLevel; //берем значение громкости из настроек громкости эффектов
    }

    private void Start()
    {
      //  Vector3 vect = new Vector3(transform.position.x, transform.position.y, transform.position.z);
     //   vect.y = 0.4f;
      //  transform.position = vect; //На случай ручного распределения ресурсов на сцене. монетка на сцене должна быть на высоте 0.4
    }
    void Update()
    {
        if (PlayerHelper.player && Vector3.Distance(transform.position, PlayerHelper.player.position) < 0.7F)//если игрок еще жив и он подошёл к монетке
            StartCoroutine(CheckPickUp());
    }

    IEnumerator CheckPickUp()
    {

        if (!sbor) //если еще не собрали монетку 
        {
            sbor = true; //факт сбора true
            StatInfo._money++;//кидаем монетку в копилочку
            _audio.PlayOneShot(_audio.clip); //запускаем аудио сбора монетки        
            Destroy(_gold);//удаляем физический элемент монетки на карте        
        }
        yield return new WaitForSeconds(0.91F); //ожидаем, пока проиграется аудио сбора монетки               
        Destroy(gameObject); //в конце удаляем пустой gameObject с audio volume
               
            

    }
}
