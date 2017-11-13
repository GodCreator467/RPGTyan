using UnityEngine;
using UnityEngine.UI;
public class IUButton : MonoBehaviour {
    public static string _clickButton;
	// Use this for initialization
public void nameButton()//при клике по кнопке передаем её имя в UIMenuButtonHelper
    {
        _clickButton = transform.GetComponent<Button>().name;
    }
}
