using UnityEngine;

public class VRAvatar : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer m_Renderer;

    [SerializeField] private Material m_BodyMat;
    [SerializeField] public Material[] m_Materials;
    
    [SerializeField] private Transform m_Head;
    public Transform Head => m_Head;
    
    public void SetHeadVisible(bool visible)
    {
        m_Renderer.materials = visible ? new[] { m_BodyMat, m_Materials[0] } : new[] { m_BodyMat, m_Materials[1] };
    }
}