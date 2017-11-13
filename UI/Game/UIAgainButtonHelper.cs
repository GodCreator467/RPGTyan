using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIAgainButtonHelper : MonoBehaviour {

	public void RestartGame()
    {
        //стат. данные откатываются на начало уровня
        StatInfo.RestartLevel();
         SceneManager.LoadSceneAsync(StatInfo._acceptLevelGame+2);//начиная с 3 сцены- уровни игры
    }
}
