using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CameraHelper : MonoBehaviour
{

    public Text TextMoneyCounter; //окно вывода кол-ва голды у игрока. Коннектим к камере, т.к. надо подсоединять к объекту, который уже есть на сцене, а не к префабу.
    public Transform target; //ссыль на перса
    private float _cameraForward = -0.5f; //расстояние камеры до перса по горизонтали сзади (знак -)
    private float _cameraHeight; //расстояние камеры до перса по вертикали сверху (знак +)
    //разброс ресурсов на карте
    public float minXborder, minZborder; // край координат
    public float maxXborder, maxZborder; // край координат
    public int HP; //кол-во ресурсов на сцене
    public int Mana;
    public int Tree;
    public int Gold;

    private float _HeightY = 0.5f;//высота ресурса
    private float minDist = 0.4f; // дистанция проверки на коллайдеры вокруг
    private Collider[] _colliders; // список коллайдеров
    private Vector3 targetPosition; //позиция камеры за игроком
    private float distance;//дистанция от камеры до её базового положения за игроком (при движении дистанция не ноль)
    private float fallowSpeed; //скорость движения камеры
    public GameObject scrollCamera;//обзор камеры
    public GameObject _helpMenu;
    public GameObject _helpMenuStart;// стартовое хелп. меню уровня игры
    public void View()
    {
        float _view = scrollCamera.GetComponent<Scrollbar>().value; //положение камеры (0..1)
        _cameraForward = _view * (-3) - 0.5f;//положение камеры зависит от бегунка scrollCamera (от -0.5 при value=0..до -3.5 при value=1)
    }
    void Start()
    {
        Time.timeScale = 0.0F;//изначально пауза на случай хелп-меню при старте    
        _helpMenu.SetActive(true);
        _helpMenuStart.SetActive(true);//выводим стартовое хелп. меню       

        switch (StatInfo.CurrentLevel) //при выборе уже пройденного уровня доступный уровень откатывается на выбранный
        {
            case 3://1 уровень
                StatInfo._acceptLevelGame = 1;
                break;
            case 4://2 уровень
                StatInfo._acceptLevelGame = 2;
                Instantiate(Resources.Load<GameObject>("MinnoePole"), new Vector3(0, 0, 0), Quaternion.identity);//ожидающая монстра на карте 
                break;
        }
        RandomResources("HP", HP);
        RandomResources("Mana", Mana);
        RandomResources("Tree", Tree);
        RandomResources("Gold", Gold);
    }



    private void RandomResources(string nameResource, int _countResource)
    {
        bool check;//проверка на наличие предмета в создаваемом месте
        float x = 0;
        float z = 0;//рандомные координаты
        float y = 0;
        for (int i = 0; i < _countResource; i++)
        {
            do
            {
                check = false; // проверка пройдена
                x = UnityEngine.Random.Range(minXborder, maxXborder); // рандомные координаты на поверхности terrain
                z = UnityEngine.Random.Range(minZborder, maxZborder);
                Vector3 newPos = new Vector3(x, y, z);
                y = Terrain.activeTerrain.SampleHeight(newPos);
                y += _HeightY;//поднимаем предмет относительно terrain на 0.5
                _colliders = Physics.OverlapSphere(new Vector3(x, y, z), minDist); // берем список коллайдеров, которые есть вокруг точки

                if (_colliders.Length != 0) //если есть хоть один коллайдер другого объекта                
                    check = true;
            }
            while (check); // выйдем только при false - когда вокруг не будет ни одного предмета с коллайдером
            Instantiate(Resources.Load<GameObject>(nameResource), new Vector3(x, y, z), Quaternion.identity); // собственно, ставим наш предмет
        }
    }

    void Update()
    {
        TextMoneyCounter.text = StatInfo._money.ToString(); //обновляем данные числа собранных монеток 
        if (!target) return;//если перса нету- камеру никуда не двигаем
                            //
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && _cameraForward < -0.5f)//опускаемся ниже (колесико вверх)
        {
            _cameraForward += 0.5f;//приближаем камеру по горизонтали                      
        }
        else
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && _cameraForward > -3.5f) //поднимаем камеру и отдаляемся от перса назад (колесико вниз)
            _cameraForward -= 0.5f;//приближаем камеру по горизонтали

        _cameraHeight = 0.48f * _cameraForward * _cameraForward + 0.85f; //высота камеры зависит от её расстояния по горизонтали до перса по параболлической ф-ции (y=a*(x^2))

        if (PlayerHelper.player.GetComponent<PlayerHelper>().State == CharState.running)//перс  бежит
            fallowSpeed = 3;//быстрый поворот когда игрок в движении(бежит)
        else
            fallowSpeed = 1;//плавный поворот, когда игрок никуда не бежит, и выбрали цель для атаки совсем рядом (пока игрок бьет цель камера плавно разворачивается на своё место)
        SmoothFallow(target); //для следования камеры за игроком
        SmoothLookAt(target.position + target.up * 0.85F + target.forward * 0.85f);
    }

    void SmoothFallow(Transform target)
    {
        targetPosition = (new Vector3(target.position.x, target.position.y + _cameraHeight, target.position.z)) + (target.forward * (_cameraForward));//позиция камеры немного сверху и сзади
        distance = Vector3.Distance(transform.position, targetPosition);
        if (distance > 0f)
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * fallowSpeed);//1 на Time.deltaTime * fallowSpeed     
    }
    void SmoothLookAt(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * fallowSpeed);
    }
}
