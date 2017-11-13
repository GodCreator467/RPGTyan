using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishHelper : MonoBehaviour
{

    public static Transform _finish; //для обнаружения финиша игроком

    // Use this for initialization
    void Start()
    {
        _finish = transform; //записываем своё положение в статическое поле для доступа в др. скриптах (для ориентации Перста на финишь)

    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerHelper.player)//если игрок еще жив
            CheckPickUp();
    }

    void CheckPickUp()
    {
        if (Vector3.Distance(transform.position, PlayerHelper.player.position) < 1.0F) //если подошёл игрок
        {
            //Подгружаем WinMenu и размещаем его по центру
            Instantiate(Resources.Load<GameObject>("WinMenu"), GameObject.Find("Canvas").transform.position, Quaternion.identity, GameObject.Find("Canvas").transform);
            Destroy(gameObject);
        }
    }
}
