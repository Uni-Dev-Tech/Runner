using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public GameObject[] freePlatforms; // массив свобобных платформ
    public GameObject[] obstraclePlatforms; // массив платформ с препятсвиями
    public GameObject[] decoratePlatforms; // массив декоративных платформ
    public GameObject[] longHighLevelPlatforms;
    public GameObject coins, coin, barrier, crane;
    public bool afterStart = false;

    public Transform platformContainer; // транформ всех платформ
    private Transform lastPlatform = null; // трансформ последней платформы

    private bool isObstacle; // хранит инф о последней платформе
    static public int longhighlevelplatforms = 0;

    void Start()
    {
        Init();
    }
    /// <summary>
    /// Создает начальный набор платформ
    /// </summary>
    public void Init()
    {
        CreateFreePlatform();
        CreateFreePlatform();
        afterStart = true;
        for (int i = 0; i < 8; i++)
        {
            CreatePlatform();
        }
    }
    /// <summary>
    /// Создает плафторму в зависимости от предыдущей
    /// </summary>
    public void CreatePlatform()
    {
        if (isObstacle) CreateFreePlatform();
        else CreateObstrackPlatform();
    }
    /// <summary>
    /// Метод создания и размещения свободной платформы
    /// </summary>
    private void CreateFreePlatform()
    {
        Vector3 pos = (lastPlatform == null) ? // Если платформ еще нет (начало игры), 
            platformContainer.position :  // то присваивается начальное положение,
            lastPlatform.GetComponent<PlatformController>().endPoint.position; // либо крайняя позиция последней платформы 
        int index = Random.Range(0, freePlatforms.Length); // рандомно выбирается платформа из массива платформ по индексу
        GameObject res = Instantiate(freePlatforms[index], pos, Quaternion.identity, platformContainer); // создает платформу по индексу из массива,
        // ставит на крайнюю позицию последеней платформы, с нулевым поворотом, помещает в иерархию родителя 
        lastPlatform = res.transform; // присваивает позицию объекта
        CreateRightDecorativePlatform();
        CreateLeftDecorativePlatform();
        int chance = Random.Range(0, 101);
        if (chance > 50 && afterStart == true)
        {
            CreateJointObstrack(0, 6);
            CreateCoinFreePlatform();
        }
        if (chance > 30) isObstacle = false; // false - последний объект свободный
    }
    /// <summary>
    /// Метод создания и размещения платформ с препятствиями
    /// </summary>
    private void CreateObstrackPlatform()
    {
        Vector3 pos = (lastPlatform == null) ? // Если платфром еще нет (начало игры),
            platformContainer.position : // то присваивается начальное положение, 
            lastPlatform.GetComponent<PlatformController>().endPoint.position; // либо крайняя позиция последней платформы
        int index = Random.Range(0, obstraclePlatforms.Length); // рандомно выбирается платформа из массва платфром по индексу
        GameObject res = Instantiate(obstraclePlatforms[index], pos, Quaternion.identity, platformContainer); // создает платформу по индексму из массива,
        // ставит на крайнюю позицию последней платформы, с нулевым поворотом, помещает в иерархию родителя
        lastPlatform = res.transform; // присваивает позицию объекта
        isObstacle = true; // true - последний объект с препятствием
        CreateRightDecorativePlatform();
        CreateLeftDecorativePlatform();
        int chance = Random.Range(0, 101);
        if (chance > 50) CreateCoinObstrackPlatform();
        if (index == 3 || index == 5 || index == 6) CreateLongHighLevel();
        if (chance > 50) isObstacle = true; // true - последний объект с препятствием
    }
    private void CreateLongHighLevel()
    {
        int count = Random.Range(0, 5);
        longhighlevelplatforms += count;
        for(int i = 0; i < count; i++)
        {
            Vector3 pos = (lastPlatform == null) ?
                platformContainer.position :
                lastPlatform.GetComponent<PlatformController>().endPoint.position;
            int index = Random.Range(0, longHighLevelPlatforms.Length);
            GameObject res = Instantiate(longHighLevelPlatforms[index], pos, Quaternion.identity, platformContainer);
            lastPlatform = res.transform;
            CreateRightDecorativePlatform();
            CreateLeftDecorativePlatform();
            int chance = Random.Range(0, 101);
            if (chance > 50)
            {
                CreateCoinObstrackPlatform();
                CreateJointObstrack(3, 6);
            }
        }
    }
    /// <summary>
    /// Метод создания и размещения декоративных платформ слева
    /// </summary>
    private void CreateLeftDecorativePlatform()
    {
        Vector3 pos = lastPlatform.GetComponent<PlatformController>().leftPoint.position; // крайняя позиция последней платформы слева
        int index = Random.Range(0, decoratePlatforms.Length); // рандомная платформа массива платформ по индексу
        Instantiate(decoratePlatforms[index], pos, Quaternion.Euler(0, 180, 0), lastPlatform); // создает платформу
    }
    /// <summary>
    /// Метод создания и размещение декоративных платформ справа
    /// </summary>
    private void CreateRightDecorativePlatform()
    {
        Vector3 pos = lastPlatform.GetComponent<PlatformController>().rightPoint.position; // крайняя позиция последней платформы справа
        int index = Random.Range(0, decoratePlatforms.Length); // рандомная платформа массива платформ по индексу
        Instantiate(decoratePlatforms[index], pos, Quaternion.identity, lastPlatform); // создает платформу
    }
    /// <summary>
    /// Метод создания и размещения монет на платформе
    /// </summary>
    private void CreateCoinFreePlatform()
    {
        int point = Random.Range(0, 3);
        Vector3 pos;
        if (point == 0) pos = lastPlatform.GetComponent<PlatformController>().leftCoin.position;
        else if (point == 1) pos = lastPlatform.GetComponent<PlatformController>().centerCoin.position;
        else pos = lastPlatform.GetComponent<PlatformController>().rightCoin.position;
        Instantiate(coins, pos, Quaternion.identity, lastPlatform);
    }
    /// <summary>
    /// Метод создания и размещения монет на платформе с препятствиями
    /// </summary>
    private void CreateCoinObstrackPlatform()
    {
        int road = Random.Range(0, 3);
        if (road == 0)
        {
            int length = lastPlatform.GetComponent<PlatformController>().lfCoin.Length;
            Vector3[] pos = new Vector3[length];
            for (int i = 0; i < length; i++)
            {
                pos[i] = lastPlatform.GetComponent<PlatformController>().lfCoin[i].position;
                Instantiate(coin, pos[i], Quaternion.Euler(-90, 0, 0), lastPlatform);
            }
        }
        if (road == 1)
        {
            int length = lastPlatform.GetComponent<PlatformController>().centCoin.Length;
            Vector3[] pos = new Vector3[length];
            for (int i = 0; i < length; i++)
            {
                pos[i] = lastPlatform.GetComponent<PlatformController>().centCoin[i].position;
                Instantiate(coin, pos[i], Quaternion.Euler(-90, 0, 0), lastPlatform);
            }
        }
        if (road == 2)
        {
            int length = lastPlatform.GetComponent<PlatformController>().rhtCoin.Length;
            Vector3[] pos = new Vector3[length];
            for (int i = 0; i < length; i++)
            {
                pos[i] = lastPlatform.GetComponent<PlatformController>().rhtCoin[i].position;
                Instantiate(coin, pos[i], Quaternion.Euler(-90, 0, 0), lastPlatform);
            }
        }
    }
    /// <summary>
    /// Метод создающий динамическое препятствие
    /// </summary>
    /// <param name="startRange">Первая цифра диапазона точек (зависит от типа платформы)</param>
    /// <param name="endRange">Последняя цифра диапазона точек (зависит от типа платформы, не входит в диапазон)</param>
    private void CreateJointObstrack(int startRange, int endRange)
    {
        int point = Random.Range(startRange, endRange);
        Vector3 pos;
        switch(point)
        {
            case 0:
                break;
            case 1:
                pos = lastPlatform.GetComponent<PlatformController>().leftBarrierPoint.position;
                Instantiate(barrier, pos, Quaternion.identity, lastPlatform);
                break;
            case 2:
                pos = lastPlatform.GetComponent<PlatformController>().rightBarrierPoint.position;
                Instantiate(barrier, pos, Quaternion.Euler(0, 180, 0), lastPlatform);
                break;
            case 3:
                pos = lastPlatform.GetComponent<PlatformController>().craneCenterPoint.position;
                Instantiate(crane, pos, Quaternion.identity, lastPlatform);
                break;
            case 4:
                pos = lastPlatform.GetComponent<PlatformController>().craneLeftPoint.position;
                Instantiate(crane, pos, Quaternion.identity, lastPlatform);
                break;
            case 5:
                pos = lastPlatform.GetComponent<PlatformController>().craneRightPoint.position;
                Instantiate(crane, pos, Quaternion.identity, lastPlatform);
                break;
        }
    }
}
