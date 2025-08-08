using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float xinput;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpforce = 5f;
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

        AnimatorContollers();
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void AnimatorContollers()
    {
        bool isRunning = (rb.velocity.x != 0);
        anim.SetBool("isRunning", isRunning);
    }

    private void Movement()
    {
        xinput = Input.GetAxisRaw("Horizontal");
        // Debug.Log("X Input: " + xinput);
        rb.velocity = new Vector2(xinput * speed, rb.velocity.y);
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
    }
}
