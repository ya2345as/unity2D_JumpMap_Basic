using UnityEngine;

public class enemy_move : MonoBehaviour
{

    Rigidbody2D rigid;
    public int nextMove;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D colliderr;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        Invoke("Think", 5);
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colliderr = GetComponent<CapsuleCollider2D>();
    }


    void FixedUpdate()
    {
        rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);
        //낭떠러지 감지
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayhit = Physics2D.Raycast(frontVec, Vector3.down, 3.5f, LayerMask.GetMask("Platform"));
        if (rayhit.collider == null)
            Trun();

    }

    void Think()
    {
        //랜덤 움직이는 속도
        nextMove = Random.Range(-1, 2) * 2;

        //애니메이션
        anim.SetInteger("isWalking", nextMove);

        //방향전환
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove > 0;

        //랜덤 움직이는 시간
        float nextTinkTime = Random.Range(1f, 2f);
        Invoke("Think", nextTinkTime);        

    }
    void Trun()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove > 0;
        CancelInvoke();
        Invoke("Think", 2);
    }
    public void OnDamaged()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipX = true;
        spriteRenderer.flipY = true;
        colliderr.enabled = false;
        anim.speed = 0f;
        rigid.AddForce(Vector2.up *5, ForceMode2D.Impulse);
        Invoke("UnActive", 5);

    }
    void UnActive()
    {
        gameObject.SetActive(false);
    }
}
