using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Anim
{
    IDLE = 1,
    RUNING = 2,
    BACK = 3,
    FORWARD = 4,
}

public class PlayerController : MonoBehaviour
{
    public float speed = 3;

    public int animation;

    public int Dirt;//左右朝向

    Animator animator;
    public Vector3 movement;
    public Vector3 Joymovement;
    private Rigidbody2D r2d;

    private Camera camera;
    void Start()
    {
        animator = GetComponent<Animator>();
        r2d = GetComponent<Rigidbody2D>();
        camera = Camera.main;

        EventCenter.Instance.AddEventListener<Vector2>(InputEvent.JoyStickMove, Move);
    }
    private void Move(Vector2 vector2)
    {
        Joymovement = vector2;
    }

    void FixedUpdate()
    {
        movement = new Vector3(Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed, Input.GetAxisRaw("Vertical") * Time.deltaTime * speed, 0);

        r2d.transform.Translate(movement);//移动,计算刚体

        r2d.transform.Translate(Joymovement * speed * Time.deltaTime, Space.World);

        if (Joymovement != Vector3.zero)
        {
            movement = Joymovement;
        }

        if (movement != Vector3.zero)//动画
        {
            if (Math.Abs(movement.x) < Math.Abs(movement.y))
            {
                if (movement.y > 0)
                {
                    animator.SetBool(AnimatorParamDefine.running, false);
                    animator.SetBool(AnimatorParamDefine.back, true);
                    animator.SetBool(AnimatorParamDefine.forward, false);

                    animation = (int)Anim.BACK;
                }
                else if (movement.y < 0)
                {
                    animator.SetBool(AnimatorParamDefine.back, false);
                    animator.SetBool(AnimatorParamDefine.running, false);
                    animator.SetBool(AnimatorParamDefine.forward, true);

                    animation = (int)Anim.FORWARD;
                }
            }
            else
            {
                animator.SetBool(AnimatorParamDefine.back, false);
                animator.SetBool(AnimatorParamDefine.forward, false);
                animator.SetBool(AnimatorParamDefine.running, true);

                animation = (int)Anim.RUNING;
            }
        }
        else
        {
            animator.SetBool(AnimatorParamDefine.back, false);
            animator.SetBool(AnimatorParamDefine.forward, false);
            animator.SetBool(AnimatorParamDefine.running, false);

            animation = (int)Anim.IDLE;
        }


        if (movement.x > 0)
        {
            transform.localScale = new Vector3(2, 2, 2);
            Dirt = 1;
        }
        if (movement.x < 0)
        {
            transform.localScale = new Vector3(-2, 2, 2);
            Dirt = -1;
        }
    }

    void LateUpdate()
    {
        //camera.transform.position = new Vector3(r2d.transform.position.x, r2d.transform.position.y, -10);
    }
}