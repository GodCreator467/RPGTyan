using UnityEngine;
using System.Collections;

public class SpawnerPointHelper : MonoBehaviour
{

    private GameObject enemy;
    private float time = 0;//тайм, пока кладбище спит и монстра не спаунится
    private int _k=1;//коэфф. скорости спауна монстров (на 2ур медленнее)

    void Start()
    {
        enemy = Resources.Load<GameObject>("BrainEnemy");//монстра обходит препятствия
        //enemy = Resources.Load<GameObject>("StupidEnemy");//монстра не обходит препятствия, может падать в пропасть
        switch (StatInfo.CurrentLevel) //урон монстры в зависимости от уровня сцены StatInfo.CurrentLevel
        {
            case 3://1 уровень (заходить в игру надо через глав. меню, иначе не считывается 
                time = 100.0F;//тайм карты на 1ур
                break;
            case 4://2 уровень
                time = 0;//тайм на карте 2ур
                _k = 3;
                break;
            default://на случай debug 2ур(без захода через глав. меню)
               // enemy = Resources.Load<GameObject>("StupidEnemy");//монстра не обходит препятствия, может падать в пропасть
                break;
        }
        StartCoroutine(Spawn());

    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(time); //тайм, пока монстра не начнет появляться массово на карте
        while (true)
        {
            yield return new WaitForSeconds(_k*2.0F);
            Instantiate(enemy, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(_k*8.0F, _k*12.0F));
        }
    }
}
