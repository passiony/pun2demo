using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    
    //这里是人物行走时的速度
    public float walkSpeed = 5f;
    
    //人物跑动时的速度
    public float runSpeed = 6f;
    
    //重力
    public float gravity = -9.81f;
    
    //三维向量组x，y，z
    Vector3 velocity;

    public Transform groundCheck;
    
    //检测人物是否与地面碰撞的半径
    public float groundDistance = 0.4f;

    public LayerMask groundMask;
    
    //人物是否在地面上
    bool isGrounded;

    public float jumpHeight = 2f;
    
    // Start is called before the first frame update
    void Awake()
    {
        controller = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //物理检测，是否在地面上
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;//正好在地面上，可以为0f，但-2f更好一些
        }
        
        //跳跃  这里默认空格键是跳跃按键
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        //这里在unity中默认“W”为向前，“D”向右，即通过控制x轴方向来控制物体在水平方向的运动；控制z轴，来控制物体前后移动
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        float speed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = runSpeed;
        }
        //将控制的方向与速度相乘，就能实现物体的运动，同样，这里的Time.deltaTime也是解决帧率问题
        controller.Move(move * speed * Time.deltaTime);
        
        //三维向量组中的y   y=1/2*g*t*t
        //重力在每一帧的变化
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}