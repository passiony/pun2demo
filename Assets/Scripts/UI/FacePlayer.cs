using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private Transform m_Target;

    public void SetTarget(Transform target)
    {
        m_Target = target;
    }

    void Update()
    {
        if (m_Target)
        {
            var forward =transform.position - m_Target.position;
            forward.y = 0;
            transform.forward = forward;
        }
    }
}
