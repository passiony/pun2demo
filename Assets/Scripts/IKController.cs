using UnityEngine;

/// <summary>
/// IK 控制
/// </summary>
public class IKController : MonoBehaviour
{
    // 身体
    public Transform bodyObj = null; //与相关球体对应

    // 左脚
    public Transform leftFootObj = null; //与相关球体对应

    // 右脚
    public Transform rightFootObj = null; //与相关球体对应

    // 左手
    public Transform leftHandObj = null; //与相关球体对应

    // 右手
    public Transform rightHandObj = null; //与相关球体对应

    // 头的观察点
    public Transform lookAtObj = null; //与相关球体对应

    // 右手肘
    public Transform rightElbowobj = null; //与相关球体对应

    // 左手肘
    public Transform leftElbowobj = null; //与相关球体对应

    // 右膝盖
    public Transform rightKneeobj = null; //与相关球体对应

    // 左膝盖
    public Transform leftKneeobj = null; //与相关球体对应


    // 动画机
    private Animator avatar;

    // 是否激活 IK
    private bool ikActive = false;

    void Start()
    {
        //获取动画机
        avatar = GetComponent<Animator>();
    }

    void Update()
    {
        // 如果 IK 没有激活
        // 把对应的控制部分附上动画自身的值
        if (!ikActive)
        {
            if (bodyObj != null)
            {
                bodyObj.position = avatar.bodyPosition;

                bodyObj.rotation = avatar.bodyRotation;
            }
            if (leftFootObj != null)
            {
                leftFootObj.position = avatar.GetIKPosition(AvatarIKGoal.LeftFoot);

                leftFootObj.rotation = avatar.GetIKRotation(AvatarIKGoal.LeftFoot);
            }
            if (rightFootObj != null)
            {
                rightFootObj.position = avatar.GetIKPosition(AvatarIKGoal.RightFoot);

                rightFootObj.rotation = avatar.GetIKRotation(AvatarIKGoal.RightFoot);
            }
            if (leftHandObj != null)
            {
                leftHandObj.position = avatar.GetIKPosition(AvatarIKGoal.LeftHand);

                leftHandObj.rotation = avatar.GetIKRotation(AvatarIKGoal.LeftHand);
            }
            if (rightHandObj != null)
            {
                rightHandObj.position = avatar.GetIKPosition(AvatarIKGoal.RightHand);

                rightHandObj.rotation = avatar.GetIKRotation(AvatarIKGoal.RightHand);
            }
            if (lookAtObj != null)
            {
                lookAtObj.position = avatar.bodyPosition + avatar.bodyRotation * new Vector3(0, 0.5f, 1);
            }
            if (rightElbowobj != null)
            {
                rightElbowobj.position = avatar.GetIKHintPosition(AvatarIKHint.RightElbow);
            }

            if (leftElbowobj != null)
            {
                leftElbowobj.position = avatar.GetIKHintPosition(AvatarIKHint.LeftElbow);
            }

            if (rightKneeobj != null)
            {
                rightKneeobj.position = avatar.GetIKHintPosition(AvatarIKHint.RightKnee);
            }

            if (leftKneeobj != null)
            {
                leftKneeobj.position = avatar.GetIKHintPosition(AvatarIKHint.LeftKnee);
            }
        }
    }


    /// <summary>
    /// IK 动画的控制专用函数
    /// </summary>
    /// <param name="layerIndex">动画层</param>
    void OnAnimatorIK(int layerIndex)
    {
        // 动画机为空，返回
        if (avatar == null)
            return;

        // 激活 IK 
        //1、 各部分权重赋值 1
        //2、 各部分位置赋值
        //3、 部分旋转赋值
        if (ikActive)
        {
            avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1.0f);

            avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1.0f);
            avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1.0f);

            avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1.0f);
            avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);

            avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
            avatar.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);

            avatar.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            avatar.SetLookAtWeight(1.0f, 0.3f, 0.6f, 1.0f, 0.5f);
            avatar.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1.0f);
            avatar.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1.0f);
            avatar.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 1.0f);
            avatar.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 1.0f);
            if (bodyObj != null)
            {
                avatar.bodyPosition = bodyObj.position;

                avatar.bodyRotation = bodyObj.rotation;
            }
            if (leftFootObj != null)
            {
                avatar.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootObj.position);

                avatar.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootObj.rotation);
            }
            if (rightFootObj != null)
            {
                avatar.SetIKPosition(AvatarIKGoal.RightFoot, rightFootObj.position);

                avatar.SetIKRotation(AvatarIKGoal.RightFoot, rightFootObj.rotation);
            }
            if (leftHandObj != null)
            {
                avatar.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);

                avatar.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
            }
            if (rightHandObj != null)

            {
                avatar.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);

                avatar.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
            }
            if (lookAtObj != null)

            {
                avatar.SetLookAtPosition(lookAtObj.position);
            }

            if (rightElbowobj != null)

            {
                avatar.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowobj.position);
            }

            if (leftElbowobj != null)

            {
                avatar.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowobj.position);
            }

            if (rightKneeobj != null)

            {
                avatar.SetIKHintPosition(AvatarIKHint.RightKnee, rightKneeobj.position);
            }

            if (leftKneeobj != null)

            {
                avatar.SetIKHintPosition(AvatarIKHint.LeftKnee, leftKneeobj.position);
            }
        }

        // 不激活 IK 
        //1、 各部分权重赋值 0    
        else
        {
            avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);

            avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
            avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);

            avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
            avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);

            avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            avatar.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);

            avatar.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            avatar.SetLookAtWeight(0.0f);
            avatar.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);
            avatar.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
            avatar.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 0);
            avatar.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 0);
        }
    }


    /// <summary>
    /// UI 
    /// </summary>
    void OnGUI()
    {
        GUILayout.Label("激活IK然后在场景中移动Effector对象观察效果");

        ikActive = GUILayout.Toggle(ikActive, "激活IK");
    }
}