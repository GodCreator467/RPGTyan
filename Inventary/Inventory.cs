using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Inventory : MonoBehaviour
{

    [SerializeField] //для отображения закрытого поля для ввода в окне Unity
    public int capacity; //количество ячеек в инвентаре
    [SerializeField]
    private Transform view; //панель инвентаря, куда помещаются предметы
    public static Cell[] content;



    void Start()
    {
        content = new Cell[capacity]; //массив ссылок на скрипты  Cell
        CreateCells();
        gameObject.SetActive(false); //скрываем инвентарь
        FillingInventory(); //заполняем инвентарь вещами с прошлых сцен
    }

    private void FillingInventory()
    {
        if (StatInfo._tree != 0)
            for (int i = 0; i < StatInfo._tree; i++)
                AddItem(Resources.Load<GameObject>("tree-icon"));

        if (StatInfo._mana != 0)
            for (int i = 0; i < StatInfo._mana; i++)
                AddItem(Resources.Load<GameObject>("mana-icon"));

        if (StatInfo._hp != 0)
            for (int i = 0; i < StatInfo._hp; i++)
                AddItem(Resources.Load<GameObject>("hp-icon"));

    }

    void CreateCells()
    {
        for (int i = 0; i < capacity; i++)
        {
            GameObject cell = Instantiate(Resources.Load<GameObject>("Cell"));

            cell.transform.SetParent(view); //помещаем иконку на инвентарь
            cell.transform.localScale = Vector2.one; //масштаб иконки 1 к 1
            cell.name = string.Format("Cell [{0}]", i); //проименуем объекты ячеек 
            content[i] = cell.GetComponent<Cell>(); //добавляем иконку к массиву ссылок на иконки (пока пустые)
        }
    }

    public static bool AddItem(GameObject item)
    {
        for (int i = 0; i < content.Length; i++)
        {
            if (content[i].transform.childCount == 0) //если в ячейке нет иконок (предметов)
            {
                GameObject newItem = Instantiate(item); //создаем иконку добавляемого предмета 

                newItem.transform.SetParent(content[i].transform);//добавляем предмет в инвентарь. В этот момент в cell нет ссылки на род. класс. 
                newItem.transform.localScale = Vector2.one; //назначаем нужный масштаб
                newItem.transform.position = newItem.transform.parent.position; // позицию в ячейке
                newItem.GetComponent<Item>().cell = content[i]; //у иконки в момент перетаскивания перед инстанцированием нет Cell(ссылки на ячейку). Добавляем её.
                                                                //хоть это и дублируется (т.к. аналогично cell присваивается в Item.cs для собирательства предметов
                return true;

            }
        }
        return false;
    }

    public static void DestroyItem(GameObject item1, GameObject item2 = null, GameObject item3 = null)
    {
        for (int i = 0; i < content.Length; i++)
        {
            if (content[i].transform.childCount != 0) //если в ячейке есть иконка (предмет)
            {
                if (item1 && content[i].transform.GetChild(0).transform.name == (item1.transform.name + "(Clone)")) //если хп банку надо фиксить и очередная ячейка её содержит
                {
                    item1 = null; //ссылке на объект даем пустое значение, чтобы более не проверять на наличие этого объекта
                    Destroy(content[i].transform.GetChild(0).gameObject); //удаляем объект из инвентаря

                }

                if (item2 && content[i].transform.GetChild(0).transform.name == (item2.transform.name + "(Clone)")) // Аналогично для двух др. переданных объектов на проверку(если передавали)
                {
                    item2 = null;
                    Destroy(content[i].transform.GetChild(0).gameObject);
                }

                if (item3 && content[i].transform.GetChild(0).transform.name == (item3.transform.name + "(Clone)")) //при динамической подгрузке ресурса к его имени добавляется "(Clone)"
                {
                    item3 = null;
                    Destroy(content[i].transform.GetChild(0).gameObject);
                }
            }
            if (!item1 && !item2 && !item3) //если более не надо ничего удалять в инвентаре- завершаем цикл
                break;
        }
    }
}
