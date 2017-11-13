using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class EnemyHelper : MonoBehaviour /*родительский класс для трёх типов монстры с разным преследованием цели (игрока),
    а именно для Brain, Stupid и Waiting-EnemyHelper  */
{
    protected float _nextStep = 0; //время последнего шага монстры (раз в 0.25сек)
    protected AudioSource _audio;// Для аудио-эффектов поедания, бега и смерти
    public AudioClip ClipRun; //бег
    public AudioClip ClipEat; //поедание игрока
    public AudioClip ClipDeath; //смерть
    public float _attackBomba;// время, через которое взрывная волна мана-бомбы (скилл игрока 2ур) достигнет монстра
    protected TargetHelper _target;//ссылка на цель
    protected int damage = 1; //атака монстры (переназначается при загрузке сцены игры в зависимости от сложности карты и уровня сцены)
    protected float speed = 1;//базовая скорость
    protected int MaxhealthPoint = 100;
    protected int healthPoint = 100;//стартовое значение хп монстра
    protected Transform target; //ссылка на цель преследования
    protected Animator animator; //анимация монстра
    public GameObject _slider;
    protected float _radiusAttackEnemy = 0.6F;//радиус атаки монстры
    protected float _radiusReagEnemy = 0.6F;//радиус реагирования монстры

    public int HealthPoint
    {
        get { return healthPoint; }
        set
        {
            if (value <= 0)
            {
                healthPoint = 0;
                Destroy(_slider);
                Death();
            }
            else healthPoint = value;
            _slider.GetComponent<Slider>().value = (HealthPoint) * 100 / (MaxhealthPoint); //значение слайдера привязано к значению хп моба
        }
    }

    public EnemyState State//состояние монстра (бег, хотьба)
    {
        get { return (EnemyState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }
    protected EnemyState defaultState;

    protected bool inAttack; //триггер атаки
    public bool InAttack
    {
        get { return inAttack; }
        set
        {
            inAttack = value;
            if (value != false) animator.SetTrigger("Attack");
            // проще  if (value), но для большей ясности. во втором часу ночи выражения типа if (value) воспринимаются как-то странно
        }
    }
    //________________________________________________________________________________________________________

    protected void Start()
    {
        switch (StatInfo.CurrentLevel) //урон монстры в зависимости от уровня сцены
        {
            case 3://1 уровень
                damage = 10;
                _radiusAttackEnemy = 0.6f;
                _radiusReagEnemy = 0.6f;
                break;
            case 4://2 уровень
                damage = 20;//все статы на порядок выше
                speed *= 1.8f;
                MaxhealthPoint *= 2;
                healthPoint = MaxhealthPoint;
                _radiusAttackEnemy = 30;//монстра, коль замахнулась, так или иначе нанесет урон по игроку (чтобы не игрок не лез на толпу и больше думал)
                _radiusReagEnemy = 1.0f;//и реагирует ближе
                break;
        }
        switch (StatInfo.Mode) //урон монстры в зависимости от уровня сложности карты
        {
            case "HARD":
                damage *= 3;
                break;
            case "NORMAL":
                damage *= 2;
                break;
            case "EASY":
                damage *= 1;//без изменений (пока что, мб стоит сделать еще легче в случае изменения наполнения уровней)
                break;
        }
        animator = GetComponentInChildren<Animator>(); //доступ к аниматору монстра
        _audio = GetComponent<AudioSource>();//ссылка на компонент музыки
    }

    protected void Update()
    {
        _audio.volume = Settings.fxLevel; //берем значение громкости из настроек громкости эффектов
        if (!(PlayerHelper.player) || !(PlayerHelper.player.GetComponent<PlayerHelper>().GetComponentInChildren<TargetHelper>()))//если на игроке нет маркера target
            target = null;
        if (target)
            SmoothLookAt(target.position, (inAttack) ? 0.95F : 2.0F + speed);//разворот в сторону цели с заданной скоростью(если цель рядом и монстр атакует- разворот медленнее)
        if (InAttack)
            return;
        if (_attackBomba != 0) //если есть время, через которое врага настигнет взрывная волна мана-бомбы, то учитываем это
            StartCoroutine(AttackBomba());
        if (HealthPoint != 0)//если монстр еще жив - продолжаем движение к цели
            Move();
    }

    protected IEnumerator AttackBomba()
    {
        yield return new WaitForSeconds(_attackBomba);// задержка по времени, пока взрывная волна мана-бомбы не достигнет монстра
        HealthPoint -= PlayerHelper.damageBomba; //наносим урон монстру
        _attackBomba = 0; //обнуляем задержку по времени (от взрывной волны)
    }

    protected IEnumerator Attack() //атака цели
    {
        InAttack = true;
        yield return new WaitForSeconds(0.8F); //задержка, пока анимация удара монстра из состояния замаха не перейдет в удар

        Vector3 spherePosition = transform.position + transform.forward * _radiusAttackEnemy / 2; //коллайдер врага получаем через создаваемую сферу перед монстром
        spherePosition.y += 0.75F;
        Collider[] colliders = Physics.OverlapSphere(spherePosition, _radiusAttackEnemy);

        foreach (Collider item in colliders)
        {
            if (item.GetComponent<PlayerHelper>())//если коллайдер в зоне удара- игрок
            {
                _audio.PlayOneShot(ClipEat); //звуковой эффект атаки монстра 
                PlayerHelper enemy = item.GetComponent<PlayerHelper>();
                enemy.HealthPoint -= damage;//наносим урон
            }
        }
        yield return new WaitForSeconds(1.55F);//задержка по времени до следующего удара
        InAttack = false;
    }

    protected void Death()//смерть монстра
    {
        StopAllCoroutines();
        _audio.PlayOneShot(ClipDeath); //звуковой эффект смерти монстра
        int random = Mathf.RoundToInt(UnityEngine.Random.Range(0.51F, 3.49F));//монстр отыгрывает смерть 1 из 2 анимаций падения
        animator.SetTrigger("Death0" + random);
        animator.applyRootMotion = true;
        speed = 0.0F;
        target = null;
        inAttack = false;
        Destroy(gameObject, 8.0F); //удаляем тело монстра через 8сек (если удалить без задержки- не будет полного отыгрыша _audio.PlayOneShot(ClipDeath)
        DestroyImmediate(GetComponent<Rigidbody>());
        DestroyImmediate(GetComponent<Collider>());
    }

    protected void SmoothLookAt(Vector3 target, float smooth) //разворот в сторону цели
    {
        Vector3 dir = target - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);
        transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * smooth);
    }
    public virtual void Move() { }   //определяем в наследуемых классах каждого вида монстры свой тип следования к цели
}

public enum EnemyState //состояние монстры
{
    Walk,
    Run
}

