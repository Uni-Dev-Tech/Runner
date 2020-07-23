using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControllerv2 : MonoBehaviour
{
    private CharacterController cc;
    private Animator animator;
    public AnimationClip[] clips;
    public Collider[] bodyParts;
    public ParticleSystem[] particleSystems;

    private float currentRoad; // текущая дорожка
    private float duration; // длина анимации поворота
    private float distance; // дистанция для поворота
    private bool move; // флаг спасающий от двойного нажатия/зажатия
    private bool jump; // флаг предотвращающий двойной вызов метода Jump()
    private bool falling; // флаг фиксит бак с двойным триггером "StartFallingTurn"

    public float jumpForce; // сила прыжка
    public float gravityForce; // сила гравитации
    private Vector3 vertical; // гравитация

    static public int coinScore; // счет монеток
    static public bool death; // смерть
    private bool start; // задержка перед началом
    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        //Инициализация полей
        currentRoad = 2f; distance = 3f; move = false;
        coinScore = 0; death = false; start = false;
        jump = false;
        duration = clips[2].length;
    }
    void Update()
    {
        float direction = Input.GetAxisRaw("Horizontal");
        float state = Input.GetAxisRaw("Vertical");
        if(start)
        {
            if (direction != 0 && !move && !death) TestTurn(direction, ref currentRoad, duration);
            if (state > 0 && !death && !move) Jump();
        }
        if(start == false)
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            {
                start = true;
                particleSystems[0].Play();
            }
    }
    private void FixedUpdate()
    {
        // Гравитация
        cc.Move(vertical * Time.fixedDeltaTime);
        if (cc.isGrounded) vertical = new Vector3(0, -1, 0);
        if (!cc.isGrounded) vertical += -Vector3.up * gravityForce;
        if(animator.speed < 2) animator.speed = WorldController.instance.speedModifier;
    }
    /// <summary>
    /// Метод прыжка
    /// </summary>
    public void Jump()
    {
        if (jump) return;
        if (!cc.isGrounded) return;
        jump = true;
        particleSystems[0].Stop();
        animator.SetTrigger("Jump");
        vertical += Vector3.up * jumpForce;
        StartCoroutine(InAir());
    }
    /// <summary>
    /// Метод проверки возможности поворота
    /// </summary>
    /// <param name="direction">Направление(влево/вправо)</param>
    /// <param name="currentRoad">Текущая дорога</param>
    /// <param name="duration">Длительность(скорость)</param>
    public void TestTurn(float direction, ref float currentRoad, float duration)
    {
        if (direction == 1)
            if (currentRoad > 2 || RightSideCheck.obstructionRight)
                return;
        if (direction == -1)
            if (currentRoad < 2 || LeftSideCheck.obstructionLeft)
                return;
        currentRoad += direction;
        StartCoroutine(Move(direction, duration));
    }
    /// <summary>
    /// Метод поворота игрока
    /// </summary>
    /// <param name="direction">Направление(влево/вправо)</param>
    /// <param name="duration">Длительность(скорость)</param>
    /// <returns></returns>
    IEnumerator Move(float direction, float duration)
    {
        move = true; falling = false;
        float elapsed = 0;
        if (cc.isGrounded) AnimationMove(direction);
        while (elapsed < duration)
        {
            if (!cc.isGrounded && animator.GetCurrentAnimatorStateInfo(0).IsName("MoveRight") && !falling ||
                !cc.isGrounded && animator.GetCurrentAnimatorStateInfo(0).IsName("MoveLeft") && !falling)
            {
                falling = true;
                animator.SetTrigger("StartFallingTurn");
                StartCoroutine(InAir());
            }
            float speed = (distance / duration) * Time.fixedDeltaTime;
            cc.Move(Vector3.right * direction * speed);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        move = false;
    }
    /// <summary>
    /// Метод вызова анимации поворота
    /// </summary>
    /// <param name="dir"></param>
    private void AnimationMove(float dir)
    {
        if (dir == 1) animator.SetTrigger("Right");
        else animator.SetTrigger("Left");
    }
    /// <summary>
    /// Метод отслеживающий момент приземления игрока(анимация)
    /// </summary>
    /// <returns></returns>
    IEnumerator InAir()
    {
        while(true)
        {
            yield return new WaitForFixedUpdate();
            if (cc.isGrounded)
            {
                animator.SetTrigger("Landing");
                break;
            }
        }
        particleSystems[0].Play();
        falling = false;
        Invoke("JumpPossibility", clips[6].length / 3.1f);
        yield break;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Danger"))
        {
            death = true;
            animator.SetTrigger("Death");
            animator.enabled = false;
            for (int i = 0; i < bodyParts.Length; i++)
            {
                bodyParts[i].isTrigger = false;
            }
            particleSystems[0].Stop();
            particleSystems[2].Play();
            Invoke("Death", 3f);
        }
        if (other.CompareTag("Coin"))
        {
            coinScore++;
            particleSystems[1].Play();
            Destroy(other.gameObject);
        }
    }
    /// <summary>
    /// Метод перезагрузки игры
    /// </summary>
    private void Death()
    {
        Time.timeScale = 0f;
        UIManagerGameplay.instance.pauseMenu.SetActive(true);
        UIManagerGameplay.instance.Switch(UIManagerGameplay.instance.pause, UIManagerGameplay.instance.deathMenu);
    }
    /// <summary>
    /// Метод задержки прыжка до тех пор пока не закончится анимация приземления(корутина InAir())
    /// </summary>
    private void JumpPossibility()
    {
        jump = false;
    }
}
