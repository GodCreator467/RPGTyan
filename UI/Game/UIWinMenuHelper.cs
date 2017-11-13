using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIWinMenuHelper : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {

        GameObject PauseButton = Canvas.FindObjectOfType<UIPauseButtonHelper>().gameObject; //блокируем кнопку паузы
        PauseButton.SetActive(false);
        Time.timeScale = 0.0F;
        if (StatInfo._acceptLevelGame != 2) //если текущий уровень не второй, а след. уровень сцены не третий, который пока не создан
            StatInfo._acceptLevelGame++;//след. уровень становится доступным
    }


}
