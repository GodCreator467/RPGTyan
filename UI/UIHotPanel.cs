using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHotPanel : MonoBehaviour
{
    public Text hpText;//текстовый счетчик банок хп
    public Text manaText;//текстовый счетчик банок мана-бомб
    public Text manaBottleText;//текстовый счетчик банок маны
    public Text ShieldText;//текстовый счетчик доступных очков применения неуязвимости
    public Text InvisibilityText;//текстовый счетчик доступных очков применения невидимости

    public Button _ButtonМanaBottle;//ссылка на кнопку мана банки
    public Button _ButtonМanaBomba;//ссылка на кнопку мана бомбы
    public Button _ButtonHP;//ссылка на кнопку хп банок 
    public Button _ButtonShield;//ссылка на кнопку скилла неуязвимости
    public Button _ButtonInvisibility;//ссылка на кнопку скилла невидимости

    private float NextUseHP = 0;//время, через которое кнопка снова станет активна
    private float NextUseMana = 0;
    private float NextUseManaBottle = 0;
    public static float NextUseShield = 0;
    public static float NextUseInvisibility = 0;

    public float _delayUsingBottle = 2.0f; //время задержки использования хп или мана- банки
    public float _delayUsingBomba = 10.0f;//--//-- мана-бомбы
    public float _delayUsingSkill3Lvl = 60.0f; //--//-- скиллов 3ур (неуязвимость и невидимость)


    // Use this for initialization
    void Start()
    {
        _activeSkill(); //назначаем доступность скиллов в соответствии с уровнем игрока
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerHelper.player)//если перс еще жив       
            definition(); //иногда проверку кнопок приостанавливаем для недоступности кнопки после использования эффекта(через NextUseHP и NextUseMana)

    }

    public void definition()
    {   /*Счетчик мана-банок, хп-банок,мана-бомб*/
        CheckStaticData(hpText, StatInfo._hp, _ButtonHP, UIUsingResources.prUseResHP); //проверяем стат. счетчик данных ресурсов первой кнопки горячей панели
        CheckStaticData(manaBottleText, StatInfo._mana, _ButtonМanaBottle, UIUsingResources.prUseResManaBottle);// ---//--- второй кнопки
        CheckStaticData(manaText, StatInfo.CalculationCountManaBomba(), _ButtonМanaBomba, UIUsingResources.prManaBomba);// ---//--- третьей кнопки
        /*Тайм мана-банки, хп-банок,мана-бомбы*/
        _TimerUseButton(StatInfo._hp, ref NextUseHP, ref UIUsingResources.prUseResHP, _ButtonHP, _delayUsingBottle);  //задержка использования кнопки и её разблокировка по истечению тайма
        _TimerUseButton(StatInfo._mana, ref NextUseManaBottle, ref UIUsingResources.prUseResManaBottle, _ButtonМanaBottle, _delayUsingBottle);
        _TimerUseButton(StatInfo._manaBomba, ref NextUseMana, ref UIUsingResources.prUseResMana, _ButtonМanaBomba, _delayUsingBomba);
        /*Скиллы 3ур*/
        CheckManaPoint(ShieldText, _ButtonShield, UIUsingResources.prUseResShield);// подсчет кол-ва маны для использования скиллов
        CheckManaPoint(InvisibilityText, _ButtonInvisibility, UIUsingResources.prUseResInvisibility);
        _TimerUseButton(int.Parse(ShieldText.text), ref NextUseShield, ref UIUsingResources.prUseResShield, _ButtonShield, _delayUsingSkill3Lvl); //тайм использования Shield
        _TimerUseButton(int.Parse(InvisibilityText.text), ref NextUseInvisibility, ref UIUsingResources.prUseResInvisibility, _ButtonInvisibility, _delayUsingSkill3Lvl);//тайм использования  Invisibility


    }
    private void CheckManaPoint(Text _textCount, Button _button, bool _useRes)//проверка кол-ва маны для скилла
    {   //возможное кол-во применений скилла с текущим кол-вом маны

        int _countSkill = (int)((PlayerHelper.player.GetComponent<PlayerHelper>().ManaPoint) / (_button.GetComponent<UIUsingResources>()._priceSkill));

        if (_textCount.text != _countSkill.ToString())
        {
            _textCount.text = _countSkill.ToString(); //назначаем новое кол-во возможных применений скилла
            if (_textCount.GetComponent<Animation>()) //если есть анимация отображения изменения кол-ва ресурсов на горячей панели
                _textCount.GetComponent<Animation>().Play(); //отыгрываем её
            if (_countSkill == 0 && !_useRes)
                _button.interactable = false;
        }
    }
    private void CheckStaticData(Text _textCount, int _staticData, Button _button, bool _useRes)//проверка значения стат. счетчика данных ресурсов кнопки на горячей панели
    {
        if (_textCount.text != (_staticData).ToString()) //если значение в StatInfo изменилось
        {
            _textCount.text = (_staticData).ToString(); //обновляем значение
            if (_textCount.GetComponent<Animation>()) //если есть анимация отображения изменения кол-ва ресурсов на горячей панели
                _textCount.GetComponent<Animation>().Play(); //отыгрываем её
            if (_staticData == 0) //если стат. счетчик=0, кнопку блокируем,
                _button.interactable = false;
            else
            {
                if (!_useRes && _button.GetComponent<UIUsingResources>()._access)//если ресурс не отсчитывает тайм после использования и он вообще доступен на уровне
                    _button.interactable = true;//активируем кнопку с навыком
            }
        }
    }


    private void _TimerUseButton(int _staticData, ref float _nextTimeUse, ref bool _useRes, Button _button, float _delayTime)    //задержка использования кнопки и её разблокировка по истечению тайма
    {
        if (_button.interactable && _useRes) //если юзаем ресурс- делаем задержку на её использование 
        {
            _button.interactable = false;
            _nextTimeUse = Time.time + _delayTime;
        }
        if (_delayTime == 60 && Time.time > (_nextTimeUse - 50))//если скилл невидимости или неуязвимости и уже прошло 10сек их эффекта
        {
            if (_button.name == _ButtonInvisibility.name && !(PlayerHelper.player.GetComponent<PlayerHelper>().GetComponentInChildren<TargetHelper>()))//если применялся навык невидимости ..
                PlayerHelper.player.GetComponent<PlayerHelper>()._target.SetActive(true);//вновь делаем игрока видимым
            if (UIUsingResources.prUseResShield && _button.name == _ButtonShield.name)//если активна неуязвимость 
                _useRes = false;
        }

        if (Time.time > _nextTimeUse) //если 1) прошло время блока кнопки после использования
        { //2)счетчик возможных юзов скилла(банки, бомбы) не ноль
            _useRes = false;
            _nextTimeUse = 0;//обнуляем
            if (!_button.IsInteractable() && !(_staticData == 0) && _button.GetComponent<UIUsingResources>()._access) //3)если есть требуемые ресурсы и и навык доступен на текущем уровне
                _button.interactable = true; // разблокируем кнопку
        }
    }

    public void _activeSkill()//активируем навык на горячей панели при апе на новый уровень
    {
        switch (StatInfo._Level) //назначаем соответствующий уровню объём опыта, требуемый для апа на след. уровень
        {
            case 1://1 уровень имеем по умолчанию
                _ButtonHP.GetComponent<UIUsingResources>()._access = true;//можем юзать хп-банку с 1ур
                break;
            case 2:
                _ButtonHP.GetComponent<UIUsingResources>()._access = true;//можем юзать хп-банку с 1ур
                _ButtonМanaBomba.GetComponent<UIUsingResources>()._access = true;//можем юзать мана-бомбу со 2ур
                break;
            case 3:
                _ButtonHP.GetComponent<UIUsingResources>()._access = true;//можем юзать хп-банку с 1ур
                _ButtonМanaBomba.GetComponent<UIUsingResources>()._access = true;//можем юзать мана-бомбу со 2ур
                _ButtonМanaBottle.GetComponent<UIUsingResources>()._access = true;//мана банку для скиллов- с 3ур
                _ButtonShield.GetComponent<UIUsingResources>()._access = true; ;//скилл неуязвимости
                _ButtonInvisibility.GetComponent<UIUsingResources>()._access = true; ;//скилл невидимости
                break;
        }
    }
}
