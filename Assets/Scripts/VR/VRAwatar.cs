using UnityEngine;

public class VRAwatar : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer m_Renderer;

    [SerializeField] private Material m_BodyMat;
    [SerializeField] public Material[] m_Materials;

    public void SetHeadVisible(bool visible)
    {
        m_Renderer.materials = new[] { m_BodyMat, m_Materials[1] };
    }
}