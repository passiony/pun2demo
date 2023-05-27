using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviourPun
{
    private CharacterController m_Controller;
    private Animator m_Animator;
    private GameObject m_Helmet;
    private GameObject m_Camera;

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
    public float groundDistance = 0.1f;
    public LayerMask groundMask;

    //人物是否在地面上
    bool isGrounded;
    public float jumpHeight = 2f;

    //定义鼠标移动速度
    public Transform gunRoot;
    public float mouseSpeed = 100f;
    float yRotation = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        m_Controller = this.GetComponent<CharacterController>();
        m_Animator = this.GetComponent<Animator>();
        m_Helmet = transform.Find("Helmet").gameObject;
        m_Camera = transform.Find("GunRoot/Camera").gameObject;
        
        m_Helmet.SetActive(!photonView.IsMine);
        m_Camera.SetActive(photonView.IsMine);
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            UpdatePosition();
            UpdateRotation();
        }
    }

    void UpdatePosition()
    {
        //物理检测，是否在地面上
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; //正好在地面上，可以为0f，但-2f更好一些
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
        float scale = 1;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = runSpeed;
            scale = 2;
        }

        m_Animator.SetFloat("HorizontalMovement", x * scale);
        m_Animator.SetFloat("ForwardMovement", z * scale);

        //将控制的方向与速度相乘，就能实现物体的运动，同样，这里的Time.deltaTime也是解决帧率问题
        m_Controller.Move(move * speed * Time.deltaTime);

        //三维向量组中的y   y=1/2*g*t*t
        //重力在每一帧的变化
        velocity.y += gravity * Time.deltaTime;
        m_Controller.Move(velocity * Time.deltaTime);
    }

    void UpdateRotation()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSpeed * Time.deltaTime;
        //这里的Mouse X和Mouse Y是鼠标所控制的X，Y，
        //这里在前面新定义了一个鼠标移动的速度mouseSpeed，用来控制鼠标移动速度，Time.deltaTime是为了解决帧率问题
        yRotation -= mouseY; //不能为+=，会让鼠标控制的摄像机方向颠倒
        yRotation = Mathf.Clamp(yRotation, -80f, 50f); //将摄像机上下可调节范围控制在-90到90度之间

        gunRoot.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX); //绕着y轴旋转
    }
}