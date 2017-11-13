using UnityEngine;
using UnityEngine.AI;

public class WaitingEnemyHelper : BrainEnemyHelper //умная монстра, ожидающая из-за угла, пока игрок не подойдёт поближе
{

    public override void Move()
    {
        if (!target)
        {
            _target = FindObjectOfType<TargetHelper>(); //ищем цель (по умолчанию-на игроке)
           if(_target) target = _target.transform;                  //назначаем цель
            return;
        }

        if (target && Vector3.Distance(transform.position, target.position) < 5.0F)//монстра замечает перса на расстоянии 5метров
        {
            defaultState = (UnityEngine.Random.Range(0, 2) == 0) ? EnemyState.Walk : EnemyState.Run; //рандомно бежит либо идет к нему
            if (speed == 0)//если скорость не задавалась 
                speed *= (defaultState == EnemyState.Walk) ? 1.4F : 2.2F;                       //с соотв. скоростью  

            if (Vector3.Distance(transform.position, target.position) > _radiusReagEnemy) //пока не подошли к персонажу вплотную- продолжаем движение
            {
                if (Time.time >= _nextStep)   //если настоло время след. шага     
                {
                    _audio.PlayOneShot(ClipRun);//звуковой эффект шага монстра 
                    _nextStep = Time.time + 0.25F;//следующий шаг через 0.25сек
                }
                GetComponent<NavMeshAgent>().speed = speed;
                GetComponent<NavMeshAgent>().SetDestination(target.position); //движение монстры осуществляем через NavMeshAgent
                State = defaultState;
            }
            else //по достижению дистанции 0.6 атакуем игрока
            {
                State = EnemyState.Walk;
                if (!inAttack) StartCoroutine(Attack());
            }
        }


    }
}
