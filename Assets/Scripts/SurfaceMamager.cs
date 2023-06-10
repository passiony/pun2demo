using UnityEngine;
using Random = UnityEngine.Random;

public class SurfaceMamager : MonoBehaviour
{
    public static SurfaceMamager Instance;
    public bool m_Multe;

    [Header("Decal")] public float m_DecalLifetime = 5f;
    public GameObject[] m_DecalPrefabs;
    public AudioClip[] m_DecalClips;

    [Header("Shell")] public AudioClip[] m_ShellClips;
    // private AudioSource m_AudioSouce;

    private void Awake()
    {
        Instance = this;
        // m_AudioSouce = gameObject.GetComponent<AudioSource>();
    }

    public void ShowDecal(RaycastHit hit)
    {
        var decal = m_DecalPrefabs[Random.Range(0, m_DecalPrefabs.Length)];
        PlayAudio(hit);
        SpawnDecal(decal, hit);
    }

    public void PlayAudio(RaycastHit hit)
    {
        // Play the clip.
        var volume = Random.Range(0.1f, 0.3f);
        // var pitch = Random.Range(-1, 1) * Time.timeScale;
        var audioClip = m_DecalClips[Random.Range(0, m_DecalClips.Length)];
        if (!m_Multe)
        {
            AudioSource.PlayClipAtPoint(audioClip, hit.point, volume);
        }
    }

    private GameObject SpawnDecal(GameObject original, RaycastHit hit)
    {
        var rotation = Quaternion.LookRotation(hit.normal) *
                       Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);
        var position = hit.point + (hit.normal * 0.001f);
        var scale = Random.Range(0.15f, 0.25f);

        var decal = GameObject.Instantiate(original, position, rotation);
        if (MathUtility.IsUniform(hit.transform.localScale))
        {
            decal.transform.parent = hit.transform;
        }

        if (scale < 1)
        {
            var vectorScale = Vector3.one;
            vectorScale.x = vectorScale.y = scale;
            decal.transform.localScale = vectorScale;
        }

        Destroy(decal, m_DecalLifetime);
        return decal;
    }

    public void ShowShell(Vector3 point)
    {
        var clip = m_ShellClips[Random.Range(0, m_ShellClips.Length)];
        var volume = Random.Range(0.5f, 1f);
        AudioSource.PlayClipAtPoint(clip, point, volume);
    }
}