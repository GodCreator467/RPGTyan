using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBarHelper : MonoBehaviour {

    Inventory _inventory; //ссылка на инвентарь с числом ячеек 
    RectTransform _content;
    public  int CellInWindow = 49;
    public int CellInString= 7;
    // Use this for initialization
    void Start ()
    {

        _content = GetComponent<RectTransform>();
        _inventory = GetComponentInParent<Inventory>(); //для получения числа _inventory.Capacity

        if (_inventory.capacity >49)
            for (int i = 0; i < Mathf.Ceil(((float)_inventory.capacity - CellInWindow) / CellInString) ; i++) //на каждую лишнюю строку ячеек 
            {
                _content.sizeDelta = new Vector2(_content.sizeDelta.x,_content.sizeDelta.y +52); //увеличиваем высоту окна инвентаря на Cell.height +4 = 52
            }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
