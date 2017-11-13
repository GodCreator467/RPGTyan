using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatInfo
{

    public static int BLtree;//begin level (restart or again button)
    public static int BLhp;
    public static int BLmana;
    public static int BLmanaBomba;
    public static int BLLevel; //стат. счетчик уровня игрока
    public static int BLmoney;
    public static int BLExpa; //кол-во опыта игрока на текущем уровне
    public static string Mode; //3 уровня сложности

    public static int CurrentLevel = 1;//текущая сцена (для Restart и Again кнопок повтора старта CurrentLevel сцены. 1ур- сцена 3, 2ур- сцена 4)
    public static int _tree = 0;
    public static int _hp = 0;
    public static int _mana = 0;
    public static int _manaBomba = 0;
    public static int _Level = 1; //стат. счетчик уровня игрока
    public static int _money = 0;
    public static int Expa = 0; //кол-во опыта игрока на текущем уровне
    public static int _acceptLevelGame = 1;//доступные уровни(изначально-1, при выборе уровня на сцене уровней- принимает значение выбранного, при загрузке сцены WinLevel- инкрементируется)
    public static int CalculationCountManaBomba()
    {
        return (_manaBomba = Mathf.Min(_tree, _hp, _mana));
    }
    public static void _nullAllResources()
    {
        //обнуляем все данные при старте 1ур
        _tree = 0;
        _hp = 0;
        _mana = 0;
        _manaBomba = 0;
        _Level = 1;
        _money = 0;
        CurrentLevel = 3;//текущая сцена -1ур
        _acceptLevelGame = 1;
        Expa = 0;
    }
    public static void BeginLevel()//запись всех стат. данных на начало уровня
    {
        BLtree = _tree;
        BLhp = _hp;
        BLmana = _mana;
        BLmanaBomba = _manaBomba;
        BLLevel = _Level;
        BLmoney = _money;
        BLExpa = Expa;
    }
    public static void RestartLevel()//запись всех стат. данных на начало уровня
    {
        _tree = BLtree;
        _hp = BLhp;
        _mana = BLmana;
        _manaBomba = BLmanaBomba;
        _Level = BLLevel;
        _money = BLmoney;
        Expa = BLExpa;
    }

}
