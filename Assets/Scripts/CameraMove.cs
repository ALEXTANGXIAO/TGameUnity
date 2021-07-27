using System;
using System.Collections;
using UnityEngine;


public class CameraMove : MonoBehaviour
{
    public float Speed = 5;
    public float xSpeed = 200;

    public float ySpeed = 200;
    public float yMinLimit = -50;
    public float yMaxLimit = 50;

    public float x = 0.0f;
    public float y = 0.0f;
    float damping = 5.0f;

    private Vector3 dir;
    private Vector3 camdir;
    private bool canControl = true;

    void Start()
    {
        EventCenter.Instance.AddEventListener<Vector2>(InputEvent.JoyStickMove, JoyMove);
       // EventCenter.Instance.AddEventListener<Vector2>(InputEvent.JoyScreenMove, JoyScreenMove);
    }

    void Update()
    {
        if (canControl)
        {
            this.transform.Translate(dir * Speed * Time.deltaTime, Space.World);
            //if (Input.GetMouseButton(0))//鼠标左键
            //{
            //    Quaternion rotationJoy = Quaternion.Euler(camdir.y, camdir.x, 0.0f);
            //    transform.rotation = Quaternion.Lerp(transform.rotation, rotationJoy, Time.deltaTime * damping * Speed);
            //}


            float H = Input.GetAxis("Horizontal");
            float V = Input.GetAxis("Vertical");
            Vector3 cf = this.transform.forward;
            Vector3 cr = this.transform.right;
            this.transform.position += cf * Time.deltaTime * V * Speed;
            this.transform.position += cr * Time.deltaTime * H * Speed;

            if (Input.GetMouseButton(1))//鼠标右键
            {
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }
            Quaternion rotation = Quaternion.Euler(y, x, 0.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * damping * Speed);
        }
    }


    private void JoyScreenMove(Vector2 vector2)
    {
        camdir.x += vector2.x * xSpeed * 0.02f;
        camdir.y -= vector2.y * ySpeed * 0.02f;
        camdir.y = ClampAngle(camdir.y, yMinLimit, yMaxLimit);
    }

    void JoyMove(Vector2 dir)
    {
        this.dir.x = dir.x;
        this.dir.z = dir.y;
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
