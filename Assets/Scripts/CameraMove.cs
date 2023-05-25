using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//所有代码如下：
public class CameraMove : MonoBehaviour
{
    //定义鼠标移动速度
    public float mouseSpeed = 100f;
  
    public Transform playerBody;

    float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        //将鼠标隐藏
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSpeed * Time.deltaTime;
        //这里的Mouse X和Mouse Y是鼠标所控制的X，Y，
        //这里在前面新定义了一个鼠标移动的速度mouseSpeed，用来控制鼠标移动速度，Time.deltaTime是为了解决帧率问题
        xRotation -= mouseY;//不能为+=，会让鼠标控制的摄像机方向颠倒
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);//将摄像机上下可调节范围控制在-90到90度之间

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);//绕着y轴旋转

    }
}