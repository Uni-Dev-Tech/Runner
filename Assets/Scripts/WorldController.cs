using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public float speed = 2.5f; // скорость движение карты
    public float speedModifier = 1; // увеличивает скорость игры
    public float addSpeed = 0.01f; // увеличение скорости на ... за одну секунду
    public float speedLimit = 4f; // скоростной лимит
    public WorldBuilder worldBuilder; // путь к WorldBuilder

    public float minZ = -10; // крайняя граница для удаления платформ

    public delegate void TryToDelAndAddPLatform(); // ссылка на метод TryToDelAndPLatform из PlatformController
    public event TryToDelAndAddPLatform OnPLatformMovement; // ивент вызывающий метод создания платформы

    public static WorldController instance; // экземпляр объекта

    public bool start = false;

    // Проверка существования instance в единственном экземпляре
    private void Awake()
    {
        if (WorldController.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        WorldController.instance = this;
    }
    private void OnDestroy()
    {
        WorldController.instance = null;
    }
    void Start()
    {
        StartCoroutine(OnPlatformMovementCoroutine()); // вызов бесконечного цикла метода создания платформ
        Invoke("StartMotion", 4f);
    }

    void Update()
    {
        if (PlayerControllerv2.death == true) start = false;
        if (start == true) transform.position -= Vector3.forward * speed * speedModifier * Time.deltaTime; // движение карты
    }
    /// <summary>
    /// Вызов ивента каждую секунду
    /// </summary>
    /// <returns></returns>
    IEnumerator OnPlatformMovementCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            if (OnPLatformMovement != null) OnPLatformMovement();
            if (speedModifier < speedLimit) speedModifier += addSpeed;
        }
    }
    public void StartMotion()
    {
        start = true;
    }
}
