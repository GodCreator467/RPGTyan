using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; //для подгрузки сцен

public class UIStartButton : MonoBehaviour {

	public void StartGame() //вызываемый метод при нажатии кнопки START
    {

        // SceneManager.LoadScene("MenuLevels", LoadSceneMode.Additive); //загрузка сцены доступных уровней
        SceneManager.LoadSceneAsync("MenuLevels", LoadSceneMode.Single);
    }
}
