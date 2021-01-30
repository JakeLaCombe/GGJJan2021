using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float Speed;
    public float airDrag;
    public float jumpForce;
    public Vector2 InputVelocity;
    public bool isGrounded = false;
    public Transform bottomTransform;
    public AudioSource jumpSoundAudioSource;
    public AudioClip jumpSoundAudioClip;
    private float previousPositionY;
    Rigidbody2D body;
    //Animator animator;
    // Start is called before the first frame update
    void Start()
    {

        body = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Move()
    {
        float velocity = Input.GetAxis("Horizontal") * Speed;
        bool isjumping = Input.GetButton("Jump");
        if (!isGrounded)
        {
            velocity *= airDrag;
        }
        body.velocity = Vector2.SmoothDamp(body.velocity, new Vector2(velocity, body.velocity.y), ref InputVelocity, 0.02f);


        if (isGrounded && isjumping)
        {
            Debug.Log("Jumping");
            //animator.SetBool("IsJumping", true);
            isGrounded = false;
            body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumpSoundAudioSource.PlayOneShot(jumpSoundAudioClip, 0.5F);
            Debug.Log(jumpForce);
        }
        if (!isGrounded && !isjumping && body.velocity.y > 0.01f)
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y * 0.95f);
        }


    }

    private void HandleCollisions()
    {
        bool wasOnGround = isGrounded;
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(bottomTransform.position, 0.06f);
        foreach (var collider in colliders)
        {
            if (collider.gameObject != gameObject && collider.gameObject.tag == "Ground")
            {

                isGrounded = true;

            }
        }
    }

    private void FixedUpdate()
    {

        Move();
        HandleCollisions();
        previousPositionY = transform.position.y;


    }



}
