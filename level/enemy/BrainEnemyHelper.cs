using UnityEngine;
using UnityEngine.AI;

public class BrainEnemyHelper : EnemyHelper//умная монстра, обходящая препятствия и чуящая игрока через всю карту
{
    protected NavMeshAgent _navMeshAgent;

    public override void Move()
    {
        if (!target)//если цель преследования не задана
        {
            _target = FindObjectOfType<TargetHelper>(); //ищем цель (по умолчанию-на игроке)
            if (_target)
            {
                target = _target.transform;                  //назначаем цель
                defaultState = (UnityEngine.Random.Range(0, 2) == 0) ? EnemyState.Walk : EnemyState.Run; //рандомно бежим либо идем к цели
                if (speed == 0)//если скорость не задавалась 
                    speed *= (defaultState == EnemyState.Walk) ? 1.4F : 2.2F;                       //с соотв. скоростью               
            }
            return;
        }

        if (Vector3.Distance(transform.position, target.position) > _radiusReagEnemy)//пока не подошли к персонажу вплотную- продолжаем движение
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
        else
        {//если подошли к цели - атакуем
            State = EnemyState.Walk;
            if (!inAttack) StartCoroutine(Attack());
        }
    }
}

