using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPerstHelper : MonoBehaviour {

    public PlayerHelper parentHero;
    // Use this for initialization
    void Start () {
        /*если бы перст был не на компоненте героя- нижеследующую строку обрабатывали бы в каждом кадре
        transform.position = PlayerHelper.player.transform.position +
    PlayerHelper.player.transform.up * (0.2f + PlayerHelper.player.GetComponent<CharacterController>().height * 0.5f); //перст всегда над героем(0.5-поправка на scale героя)*/
        transform.SetParent(parentHero.transform);//помещаем перст на героя
        transform.position = parentHero.transform.position + transform.up * (0.2f + PlayerHelper.player.GetComponent<CharacterController>().height * 0.5f); //перст всегда над героем(0.5-поправка на scale героя)
    }
	
	// Update is called once per frame
	void Update () {

        if (FinishHelper._finish)
        //ротация на финишь
        RotateTowards(FinishHelper._finish.transform.position); //разворот на финишь
        else
            Destroy(gameObject);//если финиша достигли и его объект уничтожен- удаляем и перст


    }

    private void RotateTowards(Vector3 target) //моментальный разворот на указанный объект
    {
        Vector3 dir = target - transform.position;//вектор расстояния
        dir = new Vector3(dir.x, 0, dir.z);//высоту не учитываем
        transform.forward = Vector3.Slerp(transform.forward, dir, 1);       
    }
}
