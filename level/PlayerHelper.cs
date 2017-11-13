using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerHelper : MonoBehaviour
{

    public static Transform player; //для обнаружения позиции игрока лежачими предметами
    [SerializeField] //для отображения приватного поля в Unity для отладки
    private int healthPoint = 100;// кол-во хп перса
    [SerializeField]
    private int manaPoint = 0;//  кол-во маны перса
    [SerializeField]
    private int expaPoint = 0;// кол-во опыта перса
    [SerializeField]
    private int MaxhealthPoint = 100;// макс. кол-во хп перса
    [SerializeField]
    private int maxExpaPoint = 500; //кол-во опыта для апа перса на след. уровень (зависит от уровня)
    [SerializeField]
    private int maxManaPoint = 100;// макс. кол-во маны перса (зависит от уровня)
    public Text _level;//ссылка на счетчик уровня перса на сцене
    public UIHotPanel _uiHotPanel; //ссылка на горячую панель навыков для активации доступности при апе


    float _nextStep = 0; //время последнего шага героя (раз в 0.25сек)
    private AudioSource _audio;// Для спец-эффектов бега, атаки и прыжка
    public AudioClip ClipRun; //бег
    public AudioClip ClipAttack; //атака
    public AudioClip ClipJump; //прыжок
    public AudioClip ClipAttackBomba; //взрыв бомбы 
    public AudioClip ClipDrink; //игрок пьет из банки
    private bool _jump; //факт наличия прыжка
    private GameObject Truha;//эффект трухи при ударе по монстру
    private float _lastAttack = 0;//время окончания последней атаки
    public float RadiusBomba = 20; //радиус взрыва мана-бомбы
    public bool prUseResShield;//признак использования неуязвимости
    public bool prUseResInvisibility;//признак использования невидимости
    public GameObject _target;// ссылка на свой маркер цели игрока
    private Quaternion _beginPos;//позиция игрока до разворота для удара (возвращаемся к ней после атаки)
    private RaycastHit hit;//объект по которому кликнули
    private bool AutoTarget;//признак выбора цели для автоследования и автоатаки
    public GameObject _helpMenu; //при апе перса появляется help-menu с описанием новых скиллов
    public GameObject _helpMenu2lvl;//2 и 3ур страницы help-menu
    public GameObject _helpMenu3lvl;
    public GameObject _sliderHP;//ссылки на слайдеры хп,маны и опыта игрока
    public GameObject _sliderMP;
    public GameObject _sliderExpa;
    public int MaxHealthPoint
    {
        get { return MaxhealthPoint; }
        set
        {
            if (value <= 0) //если назначили макс. кол-во хп от 0 и менее (к примеру, получили травму)
            {
                MaxhealthPoint = 0;
                Death();
            }
            else
                MaxhealthPoint = value;
        }
    }
    public int MaxManaPoint
    {
        get { return maxManaPoint; }
        set
        {
            if (value <= 0) //если назначили макс. кол-во маны от 0 и менее (пока не знаю для каких случаев)
            {
                maxManaPoint = 0;
            }
            else
                maxManaPoint = value;
        }
    }

    public int ManaPoint
    {
        get { return manaPoint; }
        set
        {
            if (value <= 0)
            {
                manaPoint = 0;
            }
            else
            {

                if (value > manaPoint) //если мана прибавляется, значит мы пьем мана банку
                {
                    manaPoint = value;
                    _audio.PlayOneShot(ClipDrink); //звук глотка из банки
                    if (value > maxManaPoint) //а если восстановление мана превышает макс. значение маны перса
                    {
                        manaPoint = maxManaPoint; //маны не может быть больше максимума. Режем максимальный эффект банки
                    }
                }
                else
                    manaPoint = value;//убавляем ману
            }
            _sliderMP.GetComponent<Slider>().value = (ManaPoint) * 100 / (MaxManaPoint); //текущее кол-во маны 
        }
    }
    public int HealthPoint
    {
        get { return healthPoint; }
        set
        {
            if (UIUsingResources.prUseResShield && value <= healthPoint)//если активен навык неязвимости и игрока бьют
                return;//урон не проходит            
            if (value <= 0)
            {
                healthPoint = 0;
                Death();
            }
            else
            {
                if (value > healthPoint) //если хп прибавляется, значит мы пьем хп банку
                {
                    _audio.PlayOneShot(ClipDrink); //звук глотка хп банки
                    healthPoint = value;
                    if (value >= MaxHealthPoint) //а если восстановление хп превышает макс. значение хп перса
                    {
                        healthPoint = MaxHealthPoint; //хп не может быть больше максимума. Режем максимальный эффект банки
                    }
                }else
                    healthPoint = value;//урон от монстров
            }
            _sliderHP.GetComponent<Slider>().value = (HealthPoint) * 100 / (MaxHealthPoint); //текущее кол-во хп 
        }
    }

    public int ExpaPoint
    {
        get { return expaPoint; }
        set
        {
            StatInfo.Expa = value;//записываем текущее значение опыта на уровне в стат. класс
            if (value > MaxExpaPoint)//если мы набрали достаточно опыта для апа на новый уровень
            {
                _level.text = (++StatInfo._Level).ToString();//апаем уровень, обновляя  стат. данные и отражая в счетчике на сцене                                                      
                expaPoint = value - MaxExpaPoint;//начисляем лишний опыт на новом уровне       
                StatInfo.Expa = expaPoint;//записываем текущее значение опыта на следующем уровне в стат. класс
                _uiHotPanel._activeSkill(); //проверяем доступность навыков в соответствии с уровнем

                switch (StatInfo._Level) //назначаем соответствующий уровню объём опыта, требуемый для апа на след. уровень
                {
                    // case 1: //1 уровень имеем по умолчанию
                    //    MaxExpaPoint = 500;
                    //    break;
                    case 2:
                        MaxExpaPoint = 3000;
                        MaxHealthPoint = 200;
                        MaxManaPoint = 200;
                        damage = 50;
                        Time.timeScale = 0.0F;//вызов хелп-меню с инфой о новых скиллах на 2ур
                        _helpMenu.SetActive(true);
                        _helpMenu2lvl.SetActive(true);
                        break;
                    case 3:
                        MaxExpaPoint = 10000;
                        MaxHealthPoint = 500;
                        MaxManaPoint = 500;
                        damage = 100;
                        Time.timeScale = 0.0F;//вызов хелп-меню с инфой о новых скиллах на 3ур
                        _helpMenu.SetActive(true);
                        _helpMenu3lvl.SetActive(true);
                        break;
                }
                HealthPoint = MaxHealthPoint; //хп и мана по максимуму при апе
                ManaPoint = MaxManaPoint;
            }
            else
                expaPoint = value;
            _sliderExpa.GetComponent<Slider>().value = (ExpaPoint) * 100 / (MaxExpaPoint); //текущее кол-во опыта 
        }
    }
    public int MaxExpaPoint
    {
        get { return maxExpaPoint; }
        set { maxExpaPoint = value; }
    }

    public float speed = 2.0F;
    private float gravity = 6.0F;

    public float rotationValue = 90.0F;

    public int damage = 50;
    public static int damageBomba = 100;
    private Vector3 moveDirection = Vector3.zero;

    public CharState State
    {
        get { return (CharState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }

    private bool inAttack;
    public bool InAttack  //пока true- выполняется только код атаки
    {
        get { return inAttack; }
        set
        {
            inAttack = value;
            if (value != false) animator.SetTrigger("Attack");

        }
    }
    private bool inAttackBomba;

    public bool InAttackBomba
    {
        get { return inAttackBomba; }
        set
        {
            inAttackBomba = value;
            if (value != false)
                animator.SetTrigger("InAttackBomba");
        }
    }

    private bool inJump;

    public bool InJump  //пока true- выполняется только код прыжка
    {
        get { return inJump; }
        set
        {
            inJump = value;
            if (value != false)
                animator.SetTrigger("Jump");
        }
    }

    public GameObject GameOverWindow;
    CharacterController controller;
    Animator animator;
    //________________________________________________________________________________________________________

    private void Start()
    {
        switch (StatInfo._Level) //сверяем уровень игрока
        {
            case 1:
                MaxExpaPoint = 500;
                MaxHealthPoint = 100;
                MaxManaPoint = 100;
                damage = 35;
                break;
            case 2:
                MaxExpaPoint = 3000;
                MaxHealthPoint = 200;
                MaxManaPoint = 200;
                damage = 50;
                break;
            case 3:
                MaxExpaPoint = 10000;
                MaxHealthPoint = 500;
                MaxManaPoint = 500;
                damage = 100;
                break;
        }
        HealthPoint = MaxHealthPoint;//хп полные
        ManaPoint = 0;//мана нулевая
        _level.text = (StatInfo._Level).ToString(); //счетчик уровня игрока на сцене
        ExpaPoint = StatInfo.Expa; //кол-во опыта игрока на текущем уровне из стат. класса
        _uiHotPanel._activeSkill();//активируем доступность навыков на гор. панели соответственно текущему уровню

        GameObject _perst = Instantiate(Resources.Load<GameObject>("Perst"), transform.position, Quaternion.identity) as GameObject; //даем герою указающий на финиш перст
        _perst.GetComponent<UIPerstHelper>().parentHero = this; // перст ссылается на нашего героя (для размещения над ним единожды, а не в каждом кадре-если бы он был не на движущемся герое, а просто на сцене)
    }
    void Awake()
    {
        player = transform; //положение нашего игрока
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();//ссылка на компонент музыки
    }

    private void RotateTowards(Vector3 target, float speedRotate = 1) //разворот в сторону клика по монстру
    {
        Vector3 dir = target - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);
        transform.forward = Vector3.Slerp(transform.forward, dir, speedRotate);

    }
    private void AutoMove()
    {//пока не прервем авто-движение своим движением или не добежим до врага или враг не умрёт от чего-либо
        if (hit.transform == null || CrossPlatformInputManager.GetAxis("Horizontal") != 0 ||
            CrossPlatformInputManager.GetAxis("Vertical") != 0)
        {
            AutoTarget = false;
            return;
        }

        //задаем плавный разворот к цели
        RotateTowards(hit.point, 2 * Time.deltaTime);//разворот перса в сторону цели

        if (Vector3.Distance(hit.transform.position, player.position) > 0.6f)//пока цель далеко для атаки
        {
            //бежим к цели     
            moveDirection = new Vector3(0.0F, 0.0F, 1); //движение вперед
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            moveDirection.y -= gravity * Time.deltaTime; //учет гравитации
            controller.Move(moveDirection * Time.deltaTime);
            //  if (State != CharState.running) //задаем бег персу 1 раз
            State = CharState.running;
            if (controller.isGrounded && Time.time >= _nextStep)   //если настоло время след. шага     и перс стоит на чем-то, а не в воздухе 
            {
                _audio.PlayOneShot(ClipRun);//звуковой эффект шага перса 
                _nextStep = Time.time + 0.25F;//следующий шаг через 0.25сек
            }
        }
        else
            State = CharState.idle;

    }
    void Update()
    {
        _audio.volume = Settings.fxLevel; //берем значение громкости из настроек громкости эффектов
        if (!AutoTarget && Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject()) //если был клик левой кнопкой мышки не по GUI элементам
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //пускаем луч от места клика на экране до физ объекта в игре ScreenPointToRay(Input.mousePosition)           

            if (Physics.Raycast(ray, out hit) && hit.transform.GetComponent<EnemyHelper>()) // считываем объект по которому кликнули и если объект- разновидность врага
                AutoTarget = true;

        }
        if (AutoTarget)
        {
            AutoMove(); //следование к цели с последующей автоатакой
            if (hit.transform != null && Vector3.Distance(hit.transform.position, player.position) <= 0.6f)//если добежали до врага и цель жива 
            {
                State = CharState.idle;
                if (Time.time >= _lastAttack) //и если прошлая атака завершилась
                {
                    _lastAttack = Time.time + 1.0f;//время завершения атаки
                    StartCoroutine(Attack());//атакуем врага
                }

            }
        }

        if (AutoTarget) return; //раз в данный момент проходит ряд кадров атаки- дальнейший код не обрабатываем

        if (UIUsingResources.prManaBomba && Time.time >= _lastAttack) //если есть признак задействования манабомбы и прошлая атака завершилась
        {
            _lastAttack = Time.time + 5.0f;//время завершения атаки
            StartCoroutine(AttackBomba());
        }
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded) // прыжок на ПК
            Jump();

        if (CrossPlatformInputManager.GetButtonDown("Jump") && controller.isGrounded) //прыжок на мобильной платформе
            Jump();

        if (InJump)        //раз в данный момент проходит ряд кадров прыжка- дальнейший код не обрабатываем
            return;

        if (_jump && controller.isGrounded)
        {
            _audio.PlayOneShot(ClipJump); //звуковой эффект прыжка перса в конце прыжка
            _jump = false;
        }
        //движение на мобильной платформе
        MoveMobile(CrossPlatformInputManager.GetAxis("Vertical"), CrossPlatformInputManager.GetAxis("Horizontal"));
        //Move();//движение на ПК
    }



    void Jump()
    {
        InJump = true;
        _jump = true; //для звука прыжка в конце
        moveDirection.y += gravity * Time.deltaTime + 3;
        controller.Move(moveDirection * Time.deltaTime);
        InJump = false;
    }

    void Move()
    {

        if (controller.isGrounded)
        {
            moveDirection = new Vector3(0.0F, 0.0F, Input.GetAxis("Vertical")); //движение вперед
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
        }

        moveDirection.y -= gravity * Time.deltaTime; //учет гравитации
        controller.Move(moveDirection * Time.deltaTime);

        if (moveDirection.z != 0) //если движение вперед есть- бежим, иначе стоим (работа анимации)
        {
            State = CharState.running;

            if (controller.isGrounded && Time.time >= _nextStep)   //если настоло время след. шага     и перс стоит на чем-то, а не в воздухе 
            {
                _audio.PlayOneShot(ClipRun);//звуковой эффект шага перса 
                _nextStep = Time.time + 0.25F;//следующий шаг через 0.25сек
            }
        }
        else
            State = CharState.idle;
        transform.Rotate(transform.up, rotationValue * Input.GetAxis("Horizontal") * Time.deltaTime); //поворот
    }


    public void MoveMobile(float vertical, float horizontal)
    {
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(0.0F, 0.0F, vertical); //движение вперед
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
        }

        moveDirection.y -= gravity * Time.deltaTime; //учет гравитации
        controller.Move(moveDirection * Time.deltaTime);

        if (moveDirection.z != 0)// || AutoTarget) //если движение вперед есть- бежим, иначе стоим (работа анимации)
        {
            if (State != CharState.running) //задаем бег персу 1 раз
                State = CharState.running;
            if (controller.isGrounded && Time.time >= _nextStep)   //если настоло время след. шага     и перс стоит на чем-то, а не в воздухе 
            {
                _audio.PlayOneShot(ClipRun);//звуковой эффект шага перса 
                _nextStep = Time.time + 0.25F;//следующий шаг через 0.25сек
            }
        }
        else
            State = CharState.idle;
        transform.Rotate(transform.up, rotationValue * horizontal * Time.deltaTime * 0.5f); //поворот на телефоне в 2 раза медленнее ввиду сложности управления(в сравнении с ПК)
    }
    IEnumerator AttackBomba()
    {
        InAttackBomba = true;//триггер анимации игрока - Поза мага

        yield return new WaitForSeconds(1.0F);// задержка по времени между позой мага и взрывом


        Truha = Resources.Load<GameObject>("TruhaBomba"); //создаем элемент облака      
                                                          //  Truha.transform.localScale = new Vector3(RadiusBomba, 1, RadiusBomba); //размер радиусом 20
        Instantiate(Truha, transform.position + transform.up * 0.5f, Quaternion.identity);//центр облака чуть выше земли (но так, чтобы не пересекать её)

        Vector3 spherePosition = transform.position + transform.up * 2; //центр взрыва -  над игроком

        Collider[] colliders = Physics.OverlapSphere(spherePosition, RadiusBomba); //радиус взрыва- 20 метров

        _audio.PlayOneShot(ClipAttackBomba); //звуковой эффект взрыва бомбы
        foreach (Collider item in colliders)
        {
            if (item.GetComponent<EnemyHelper>())
            {
                EnemyHelper enemy = item.GetComponent<EnemyHelper>();
                enemy._attackBomba = (Vector3.Distance(enemy.transform.position, player.position)) * 3 / RadiusBomba; //время, через которое взрывная волна достигнет монстра.
                enemy._attackBomba += 0.5f;// 0.5f-поправка на время,т.к. фактически видимая часть волны достигает монстру позже (есть еще маловидимая часть волны)
                ExpaPoint += (damageBomba / 2);//начисляем опыт персу в половину урона по цели
            }
        }
        InAttackBomba = false; //триггер анимации перса отключаем
        UIUsingResources.prManaBomba = false; //признак применения манабомбы убираем
    }
    IEnumerator Attack()
    {
        InAttack = true;
        yield return new WaitForSeconds(0.3F);
        Truha = Resources.Load<GameObject>("Truha"); //создаем элемент трухи
        Truha.transform.localScale = new Vector3(1, 0.5f, 1);
        Instantiate(Truha, hit.transform.position + hit.transform.up * 0.25f, Quaternion.identity);//труха чуть выше земли(но чтобы не пересекать её)
        _audio.PlayOneShot(ClipAttack); //звуковой эффект атаки перса ногой 
        EnemyHelper enemy = hit.transform.GetComponent<EnemyHelper>();
        enemy.HealthPoint -= damage;//наносим цели урон                
        ExpaPoint += damage;//начисляем опыт персу                  
        InAttack = false;
    }

    void Death()
    {
        GameOverWindow.SetActive(true);
        Destroy(gameObject);
    }
}

public enum CharState
{
    idle,
    running
}
