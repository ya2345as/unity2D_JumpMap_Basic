using Unity.VisualScripting;
using UnityEngine;

public class move : MonoBehaviour
{

    public float maxSpeed;
    public float JumpPower;
    public game_manager gameManager;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D colliderr;

    //사운드
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    public AudioClip audioUi;

    AudioSource audioSource;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        colliderr = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        //점프
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
        }
        //정지
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.5f, rigid.linearVelocity.y);
        }
        //방향전환
        float h = Input.GetAxisRaw("Horizontal");
        if (h != 0)
        {
            spriteRenderer.flipX = h < 0;
        }
        //걷기 애니메이션
        if (Mathf.Abs(rigid.linearVelocity.x) < 0.4)
            anim.SetBool("isWalking", false);
        else
        {
            anim.SetBool("isWalking", true);
        }
    }
    void FixedUpdate()
    {
        //걷기
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //최대 속도
        if (rigid.linearVelocity.x > maxSpeed)
            rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocity.y);
        else if (rigid.linearVelocity.x < maxSpeed * (-1))
            rigid.linearVelocity = new Vector2(maxSpeed * (-1), rigid.linearVelocity.y);

        //점프 애니메이션 중단(Ray사용)
        if(rigid.linearVelocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayhit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayhit.collider != null)
                if (rayhit.distance < 1.0f)
                    anim.SetBool("isJumping", false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //적 닿음 -> 데미지
        if (collision.gameObject.tag == "enemy")
        {
            if (rigid.linearVelocity.y < 0 && transform.position.y > collision.transform.position.y)
                OnAttack(collision.transform);
            else
                OnDamaged(collision.transform.position);
        }
        //가시 -> 데미지
        if (collision.gameObject.tag == "sprike")
        {
                OnDamaged(collision.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //동전
        if (collision.gameObject.tag == "Item")
        {
            PlaySound("ITEM");
            bool isGold = collision.gameObject.name.Contains("gold");
            bool isSilver = collision.gameObject.name.Contains("silver");
            bool isBronze = collision.gameObject.name.Contains("bronze");

            if (isBronze)
                gameManager.stage_point += 50;
            else if(isSilver)
                gameManager.stage_point += 100;
            else if(isGold)
                gameManager.stage_point += 300;

            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            PlaySound("FINISH");
            //다음스테이지
            gameManager.NextStage();
        }
    }

    void OnAttack(Transform enemy)
    {
        PlaySound("ATTACK");
        gameManager.stage_point += 100;
        enemy_move enemyMove = enemy.GetComponent<enemy_move>();
        enemyMove.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos)
    {
        PlaySound("DAMAGED");
        //데미지
        gameManager.HealthDown();

        //투명화 + 무적
        gameObject.layer = 9;
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //튕겨나가기
        int dirc = transform.position.x - targetPos.x >0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        anim.SetTrigger("isDamaged");
        Invoke("offDamage", 1);
    }

    void offDamage()
    {
        gameObject.layer = 8;
        spriteRenderer.color = new Color(1, 1, 1,1);
    }
    public void Ondie()
    {
        PlaySound("DIE");
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;
        colliderr.enabled = false;
        anim.speed = 0f;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }
    public void VelocityZero()
    {
        rigid.linearVelocity = Vector2.zero;
    }
    public void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
            case "UI":
                audioSource.clip = audioUi;
                break;
        }
        audioSource.Play();
    }
}