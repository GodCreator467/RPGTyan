using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenuButtonHelper : MonoBehaviour
{
    
    public GameObject MenuLevels;//ссылка на панель меню уровней (для скрытия)
    public GameObject HelpMenuLevel;//хелп подсказка соотв. уровня (на 1ур- StartGame)
    public GameObject Help2scene;//хелп меню 2ур игры
    private void Start()
    {               
        //разблокируем доступные и блокируем недоступные уровни
        Button[] _massivButtonLevels = GetComponentsInChildren<Button>(); //получаем массив доступных уровней

        for (int i = 0; i < _massivButtonLevels.Length-3; i++) //массив сверяем весь, кроме последних трёх кнопок (кнопка глав меню, меню сохранений игр и загрузок сохраненных игр)
        {
            if(i< StatInfo._acceptLevelGame)//сверяем текущую кнопку уровня с максимально доступным уровнем
                _massivButtonLevels[i].interactable = true; //разблокируем её
            else
                _massivButtonLevels[i].interactable = false; //все остальные кнопки блочим
        }


    }
    public void GoToMenu()
    {        
        switch (IUButton._clickButton) //проверяем имя кнопки, по которой кликаем
        {
            case "myMainMenu":
                StatInfo.CurrentLevel = 1;//глав. меню
                break;
            case "Level-1":
                StatInfo._nullAllResources();//обнуление всех прошлых достижений  
                StatInfo.BeginLevel();
                MenuLevels.SetActive(false);//скрываем меню уровней
                HelpMenuLevel.SetActive(true);//высвечиваем Help-подсказку старта игры (на сцене она продублируется с доступной кнопкой Continue)
                break;
            case "Level-2":
                StatInfo.CurrentLevel = 4;//текущая сцена вторая
                StatInfo._acceptLevelGame = 2;
                StatInfo.BeginLevel();
                MenuLevels.SetActive(false);//скрываем меню уровней
                Help2scene.SetActive(true);//высвечиваем Help-подсказку 2ур игры
                break;
        }
        
        Time.timeScale = 1.0F; //ход времени возобновляем
        SceneManager.LoadSceneAsync(StatInfo.CurrentLevel); 
    }
}
