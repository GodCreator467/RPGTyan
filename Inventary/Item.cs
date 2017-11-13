using UnityEngine;
using UnityEngine.EventSystems;//для перетаскивания предметов в инвентаре
using System.Collections;
using System;
using UnityEngine.UI;
public class Item : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [HideInInspector]
    public Cell cell;  //ссылка родителя- клетки
    public GameObject itemObject; //ссылка на представление предмета на сцене
    private Transform canvas;

    void Start()
    {
        cell = GetComponentInParent<Cell>();  //подключаемся к ячейке, в которой наш предмет (скрипт)
        canvas = GameObject.Find("Canvas").transform;
    }

    public void OnDrag(PointerEventData eventData) //захват предмета при клике
    {
        transform.SetParent(canvas); //переназначаем родителя предмета на канву для норм. визуального перетаскивания
        transform.position = eventData.position; // координаты предмета в режиме реального времени
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float distance = float.MaxValue; //расстояние перетаскивания- максимально
        Cell newCell = cell; //ссылка на новую ячейку (изначально- эта же ячейка)

        for (int i = 0; i < Inventory.content.Length; i++)
        {
            float temp = Vector2.Distance(transform.position, Inventory.content[i].transform.position); //расстояние между конечной позицией предмета и всеми ячейками
            
                if (temp < distance && Inventory.content[i].transform.childCount == 0) //ищем ближайшую не занятую ячейку
            {
                distance = temp;
                newCell = Inventory.content[i];
            }
        }
        if (distance > 70) //если мы выбрасываем предмет за пределы инвентаря
        {
            

            //добавляем на сцену  itemObject и удаляем его в инвентаре item
            GameObject newItemObject = Instantiate(itemObject); //создаем заного предмет на сцене
            switch (itemObject.name.ToString()) //убавляем число ресурсов в стат. классе
            {
                case "HP": //смотрим по имени префаба на сцене, т.к. имя префаба в инвентаре может быть с индексом ввиду копий.
                    StatInfo._hp--;
                    
                    break;
                case "Mana":
                    StatInfo._mana--;
                    break;
                case "Tree":
                    StatInfo._tree--;
                    break;
            }

            //  newItemObject.transform.localScale = Vector3.one *0.4f; //назначаем нужный масштаб
            newItemObject.transform.rotation = PlayerHelper.player.transform.rotation;// разворачиваем предмет под игрока чтобы его дальнейшая позиция назначалась перед нами(а не где то позади игрока)           
            newItemObject.transform.position = PlayerHelper.player.transform.position + newItemObject.transform.forward * 2 +
            newItemObject.transform.right * (UnityEngine.Random.Range(1, 15) / 5.0f) + newItemObject.transform.up * 0.4f; // позиция перед игроком немного вправо
            Destroy(gameObject);            
        } else
        {
            //предмет распологаем в новой ячейке
            transform.SetParent(newCell.transform);
            transform.position = newCell.transform.position;
            transform.localScale = Vector2.one;
        }

    }
}
