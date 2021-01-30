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
    public float ConfusionTime;
    public GameManager Manager;
    Rigidbody2D body;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        Manager = FindObjectOfType<GameManager>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Move()
    {
        float velocity = Input.GetAxis("Horizontal") * Speed;
        //flips the dircetion the player moves if they are confused. 
        if (ConfusionTime >= 0)
        {
            velocity *= -1;
            ConfusionTime -= Time.deltaTime;
        }

        

        if (velocity < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            animator.SetBool("Moving", true);
        }
        else if (velocity > 0)
        {
            transform.localScale = Vector3.one;
            animator.SetBool("Moving", true);
        }
        else
        {
            animator.SetBool("Moving", false);

        }

        bool isjumping = Input.GetButton("Jump");
        if (!isGrounded)
        {
            velocity *= airDrag;
        }
        body.velocity = Vector2.SmoothDamp(body.velocity, new Vector2(velocity, body.velocity.y), ref InputVelocity, 0.02f);


        if (isGrounded && isjumping)
        {

            animator.SetBool("Jumping", true);
            isGrounded = false;
            body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumpSoundAudioSource.PlayOneShot(jumpSoundAudioClip, 0.5F);

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
                animator.SetBool("Jumping", false);
            }
        }
    }

    private void FixedUpdate()
    {
        // Quit the game if user presses Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Manager._heldQuestItem != null && Manager._heldQuestItem.Item == QuestItem.ItemType.FeatherBoa)
        {
            body.gravityScale = .5f;
        }
        else
        {
            body.gravityScale = 1.5f;
        }
        if (Manager._heldQuestItem != null && Manager._heldQuestItem.Item == QuestItem.ItemType.CasioWatch)
        {
            Time.timeScale = 0.5f;
        }
        else
        {
            Time.timeScale = 1f;
        }
        Move();
        HandleCollisions();



    }



}
