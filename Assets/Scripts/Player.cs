using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private float speed = 8f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float jumpforce = 5f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float dashTime = 1f;
    [SerializeField] private float dashCooldown;

    private float xinput;

    private bool isDashing = false;
    private bool canDash = true;
    private float dashDirection;

    private float comboTime = 0.3f; // Combo window time
    private bool isattacking = false;
    private int comboCounter=0;
    private float comboTimer;

    private Rigidbody2D rb;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        CheckInput();

        CheckTime();

        AnimatorContollers();
    }

    private void CheckInput()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.K))&&IsGrounded() )   Jump();
        if (Input.GetKeyDown(KeyCode.L) && canDash && !isDashing)   StartCoroutine(Dash());
        if (Input.GetKeyDown(KeyCode.J) && IsGrounded())
        {
            isattacking = true;
            comboTimer = comboTime; // 重置连击计时器
        }
    }

    private void CheckTime()
    {
        comboTimer -= Time.deltaTime;
        if (comboTimer < 0) comboCounter = 0;
    }

    private void AnimatorContollers()
    {
        bool isRunning = (rb.velocity.x != 0);
        bool isGrounded = IsGrounded();
        anim.SetBool("isRunning", isRunning);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isDashing", isDashing);
        anim.SetBool("isAttacking", isattacking);
        anim.SetInteger("comboCounter", comboCounter);
    }

    private void Movement()
    {
        if (isDashing) return;
        if (isattacking)
        {
            // 如果正在攻击，禁止移动
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }    
        xinput = Input.GetAxisRaw("Horizontal");
        // Debug.Log("X Input: " + xinput);
        rb.velocity = new Vector2(xinput * speed, rb.velocity.y);
        if (xinput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (xinput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private IEnumerator Dash()
    {
        Debug.Log("Dash started");
        isDashing = true;
        canDash = false;
        // 决定方向（假设角色朝右 localScale.x > 0）
        dashDirection = transform.localScale.x > 0 ? 1 : -1;

        // 保存原本的重力
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0; // 冲刺期间不受重力影响

        // 冲刺持续 dashTime 秒
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);
        yield return new WaitForSeconds(dashTime);

        // 恢复
        rb.gravityScale = originalGravity;
        rb.velocity = Vector2.zero;
        isDashing = false;

        // 冷却
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        Debug.Log("Dash ended");
    }

    public void AttackOver()
    {
        isattacking = false;
        comboCounter++;
        if (comboCounter > 2) comboCounter = 0;
    }
}
