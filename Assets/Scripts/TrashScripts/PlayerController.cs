using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    static public Animator animator;
    public AnimationClip[] clips;
    public Collider[] bodyParts;
    private CharacterController cc;
    private bool isInMovement = false;
    public float distance;
    private float currentDistance = 0f;
    private float currentDir = 0f;

    private float road = 2; // переменная хранящая номер дорожки
    private bool turn; // контролирует перемещение игрока

    public float jumpForce; // сила прыжка
    public float gravityForce; // сила гравитации 
    private bool isInJump = false; // ограничивает проигрывание анимации прыжка и двойной прыжок
    static public bool run; // определяет бежит ли ГГ
    static public bool death = false;
    private Vector3 vertical; // гравитация/прыжок
    private bool landing = true; // контролирует переходы анимации в прыжке

    private float lengthTime; // переменная хранящая длину проигрываемой анимации
    static public float lengthTimeStart;
    private int coinScore = 0; // счет монеток
    public ParticleSystem[] particleSystems; // 1 - RoadDust, 2 - TakeCoin, 3 - StarsAboveHead
    private void Start()
    {
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        // Определяет длину проигрываемой анимации
        lengthTime = clips[2].length * 0.5f;
        lengthTimeStart = clips[0].length;
        coinScore = 0;
    }
    private void Update()
    {
        float dir = Input.GetAxisRaw("Horizontal");
        float pos = Input.GetAxisRaw("Vertical");
        cc.Move(vertical * Time.deltaTime);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run") == true)
        {
            run = true;
            particleSystems[0].Play();
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run") == false)
        {
            run = false;
            particleSystems[0].Stop();
        }

        if (cc.isGrounded)
        {
            isInJump = false;
            vertical = new Vector3(0, -1, 0);
        }
        if (!cc.isGrounded) pos = 0;

        if (isInJump == false && pos != 0 && run == true)
        {
            if (pos > 0)
            {
                isInJump = true;
                landing = false;
                animator.SetTrigger("Jump");
            }
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Falling") == true && cc.isGrounded == true)
        {
            if(landing == false) animator.SetTrigger("Landing");
            landing = true;
        }
        if (run == true && cc.isGrounded == false && landing == true)
        {
            landing = false;
            animator.SetTrigger("StartFalling");
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("MoveRight") && cc.isGrounded == false && landing == true
            || animator.GetCurrentAnimatorStateInfo(0).IsName("MoveLeft") && cc.isGrounded == false && landing == true)
        {
            landing = false;
            animator.SetTrigger("StartFallingTurn");
        }

        if (isInMovement == false && dir != 0 && WorldController.instance.start == true)
        {
            turn = false;
            currentDir = dir;
            currentDistance = distance;
            if (dir > 0 && road < 3 && RightSideCheck.obstructionRight == false)
            {
                isInMovement = true;
                road += 1; turn = true;
                animator.SetTrigger("Right");
            }
            if (dir < 0 && road > 1 && LeftSideCheck.obstructionLeft == false)
            {
                isInMovement = true;
                road -= 1; turn = true;
                animator.SetTrigger("Left");
            }
        }
        if(isInMovement == true && turn == true)
        {
            turn = false;
            StartCoroutine(MoveTest());
        }
        if(isInJump == true && pos > 0)
        {
            Jump();
        }
    }
    private void FixedUpdate()
    {
        if(!cc.isGrounded) vertical += -Vector3.up * gravityForce;
    }
    /// <summary>
    /// Поворот налево/направо
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveTest()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            float speed = distance / lengthTime;
            float tmpDis = Time.fixedDeltaTime * speed;
            cc.Move(Vector3.right * currentDir * tmpDis);
            currentDistance -= tmpDis;
            if (currentDistance <= 0)
            {
                isInMovement = false;
                break;
            }
        }
    }
    /// <summary>
    /// Поворот налево/направо
    /// </summary>
    //private void Move()
    //{
    //    if (currentDistance <= 0)
    //    {
    //        isInMovement = false;
    //        return;
    //    }
    //    float speed = distance / lengthTime;
    //    float tmpDis = Time.deltaTime * speed;
    //    cc.Move(Vector3.right * currentDir * tmpDis);
    //    currentDistance -= tmpDis;
    //}
    /// <summary>
    /// Прыжок
    /// </summary>
    private void Jump()
    {
        vertical += Vector3.up * jumpForce;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Danger"))
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
    private void Death()
    {
        SceneManager.LoadScene(0);
    }
}
