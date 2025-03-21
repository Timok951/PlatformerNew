using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb2d;

    public float MoveSpeed;

    public Vector2 vect;

    private bool IsGrounded;

    public float JumpForce;

    public SkeletonAnimation skeletonanimation;

    public AnimationReferenceAsset idle, walking, death,winanimation;

    public String currentstate;

    public String currentanimation;


    public void SetAnimation(AnimationReferenceAsset animation, bool loop, float timeScale)
    {
        if (animation.name.Equals(currentanimation))
        {
            return; 
        }

        skeletonanimation.state.SetAnimation(0, animation, loop).TimeScale = timeScale;
        currentanimation = animation.name;
    }

    public void SetCharacterstate(string state)
    {
        if (state.Equals("Idle"))
        {
            SetAnimation(idle, true, 1f);
        }

        if (state.Equals("Walking"))
        {
            SetAnimation(walking, true, 1f);
        }

        if (state.Equals("Death"))
        {
            SetAnimation(death, true, 1f);
        }
        if (state.Equals("Win"))
        {
            SetAnimation(winanimation, true, 1f);
        }
    }

    private void SetAnimationState()
    {
        if (!IsGrounded) 
        {
            Debug.Log("JUMPING");
            return;
        }

        if (Mathf.Abs(rb2d.velocity.x) < 0.1f) 
        {
            Debug.Log("IDLE START");
            SetCharacterstate("Idle");
        }
        else
        {
            Debug.Log("WALKING START");
            SetCharacterstate("Walking");
        }
    }

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        currentstate = "Idle";
        SetCharacterstate(currentstate);
    }

    private void JumpPlayer()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        rb2d.AddForce(transform.up * JumpForce, ForceMode2D.Impulse);
        IsGrounded = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 3 && collision.contacts[0].normal.y > 0.5f)
        {
            IsGrounded=true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 3)
        {
            IsGrounded=false;
        }
    }

    private void Death()
    {
        rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
        this.enabled = false;
        SetCharacterstate("Death");
        float animationTime = death.Animation.Duration;
        Invoke("Restart", animationTime);
    }

    private void Restart()
    {
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private  void Update()
    {
        HorizontalMovePlayer();

    }

    private void Win()
    {
        rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
        this.enabled = false;
        SetCharacterstate("Win");

        float animationTime = winanimation.Animation.Duration;
        Invoke("Restart", animationTime);



    }

    private void FixedUpdate()
    {

        if ((Input.GetKey(KeyCode.W)|| Input.GetKey(KeyCode.Space)) && IsGrounded)
        {
            JumpPlayer();
        }
        SetAnimationState();
        if (transform.position.y < -8)
        {
            Death();
           
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 6)
        {
            Death();
        }
        if (collision.gameObject.layer == 7)
        {
            Win();
        }
    }

    private void HorizontalMovePlayer()
    {
        float MoveInput = Input.GetAxis("Horizontal");
        vect = new Vector2(MoveInput, 0);
        rb2d.velocity = new Vector2(vect.x * MoveSpeed, rb2d.velocity.y);
        Rotate();
    }

    private void Rotate()
    {
        if (rb2d.velocity.x < 0)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);

        }
        if (rb2d.velocity.x > 0)
        {
            gameObject.transform.rotation = Quaternion.Euler(0,0,0);
        }
    }





}
