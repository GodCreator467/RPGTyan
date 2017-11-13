using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemObejct : MonoBehaviour
{
    public GameObject VisualEffect; //ссылка на дочерний объект визуального эффекта сбора
    public GameObject item;
    private AudioSource _audio;// Для эффекта сбора ресурса
    private bool sbor; //факт сбора ресурсы


    void Awake()
    {

        _audio = GetComponent<AudioSource>();//ссылка на компонент музыки
        if (_audio)
            _audio.volume = Settings.fxLevel; //берем значение громкости из настроек громкости эффектов        
    }

    private void Start()
    {
      //  Vector3 vect = new Vector3(transform.position.x, transform.position.y, transform.position.z);
      //  vect.y = 0.4f;
       // transform.position = vect; //На случай, если вручную добавляю элементы на сцене. элементы на сцене должны быть на высоте 0.4
    }
    void Update()
    {
        if (PlayerHelper.player && Vector3.Distance(transform.position, PlayerHelper.player.position) < 0.7F)//если игрок еще жив и если он подошёл
            StartCoroutine(CheckPickUp());
    }

    IEnumerator CheckPickUp()
    {
        if (!sbor && Inventory.AddItem(item)) //если еще не собрали
        {
            sbor = true; //факт сбора true

            switch (item.name.ToString()) //наращиваем число ресурсов в стат. классе
            {
                case "hp-icon":
                    StatInfo._hp++;                  
                    break;
                case "mana-icon":
                    StatInfo._mana++;
                    break;
                case "tree-icon":
                    StatInfo._tree++;
                    break;
            }



            if (VisualEffect)
                VisualEffect.SetActive(true); //визуальный эффект сбора
            if (_audio) //если есть компонента аудио эффекта на ресурсе            
                _audio.PlayOneShot(_audio.clip); //запускаем аудио сбора                 
        }

        yield return new WaitForSeconds(1.0F); //ожидаем, пока проиграются эффекты сбора
        Destroy(gameObject); //  предмет на сцене удаляем
    }

}
