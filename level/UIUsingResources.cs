using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIUsingResources : MonoBehaviour
{
    
    public Text res; //счетсчик ресурсов на кнопке
    public GameObject itemHP; //ресурс 1 . Ресурсы, необходимые для эффекта на горячей панели (для хп- 1 банка хп, для бомбы- 3 разных ресурса)
    public GameObject itemMana; //ресурс 2
    public GameObject itemTree;  //ресурс 3
    public static bool prManaBomba; //признак применения мана-бомбы для кода атаки в PlayerHelper
    public static bool prUseResHP;//признак использования хп (для блокировки кнопки и остановки обработки их доступности в UIHotPanel
    public static bool prUseResMana;//признак использования мана бомбы (для блокировки кнопок и остановки обработки их доступности в UIHotPanel
    public static bool prUseResManaBottle;//признак использования мана банки 
    public static bool prUseResShield;//признак использования неуязвимости
    public static bool prUseResInvisibility;//признак использования невидимости
    public int _priceSkill;//стоимость применения скилла
        
    [HideInInspector]
    public bool _access;//доступность навыка на текущем уровне
    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }
    public void use() //при клике по иконке на горячей панели
    {
        switch (res.name.ToString()) //определим какой эффект применяем
        {
            case "TextHP":

                prUseResHP = true;//признак использования ресурса                                             
                StatInfo._hp--; //сокращаем счетчик в стат. классе                                                
                Inventory.DestroyItem(itemHP);// ищем ресурс в инвентаре и дестроим
                PlayerHelper.player.GetComponent<PlayerHelper>().HealthPoint += 40; //восстанавливаем 40хп                              
                break;
            case "TextMana":
                prUseResManaBottle = true;
                StatInfo._mana--;                         
                Inventory.DestroyItem(itemMana);// ищем ресурс  в инвентаре и дестроим     
                PlayerHelper.player.GetComponent<PlayerHelper>().ManaPoint += 40; //восстанавливаем 40 маны            
                break;
            case "TextBomba":
                prUseResMana = true;
                StatInfo._mana--; StatInfo._tree--; StatInfo._hp--; //сокращаем счетчик в стат. классе                                          
                Inventory.DestroyItem(itemHP, itemMana, itemTree);// ищем ресурс  в инвентаре и дестроим
                UIUsingResources.prManaBomba = true; //активируем признак атаки мана бомбой                
                break;
            case "TextShield":
                prUseResShield = true;
                PlayerHelper.player.GetComponent<PlayerHelper>().ManaPoint -= _priceSkill; //сокращаем кол-во маны           
                break;
            case "TextInvisibility":
                prUseResInvisibility = true;
                PlayerHelper.player.GetComponent<PlayerHelper>()._target.SetActive(false);
                PlayerHelper.player.GetComponent<PlayerHelper>().ManaPoint -= _priceSkill; //сокращаем кол-во маны                                                     
                break;
        }
        if (transform.GetComponent<Animation>())//если есть анимация тайма после использования на кнопке- активируем её
            transform.GetComponent<Animation>().Play();
    }
}
