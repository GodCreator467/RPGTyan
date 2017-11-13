using UnityEngine;

public class StupidEnemyHelper : EnemyHelper//монстра, не обходящая препятствия, чующая игрока через всю карту,
                                            //но проходящая (на 1 сцене игры) между прутьями, покидая кладбище (чего не делают умные зомбаки)
{
    public override void Move()
    {
        if (!target)
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

        if (Vector3.Distance(transform.position, target.position) > _radiusReagEnemy)
        {
            if (Time.time >= _nextStep)   //если настоло время след. шага     
            {
                _audio.PlayOneShot(ClipRun);//звуковой эффект шага монстра 
                _nextStep = Time.time + 0.25F;//следующий шаг через 0.25сек
            }
            Vector3 goal = target.position;
            transform.position = Vector3.MoveTowards(transform.position, goal, speed * Time.deltaTime);//движение осуществляем по прямой через MoveTowards
            State = defaultState;
        }
        else
        {
            if (!inAttack) StartCoroutine(Attack());
        }
    }
}