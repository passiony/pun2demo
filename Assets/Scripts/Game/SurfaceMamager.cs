using System.Collections.Generic;
using Opsive.Shared.Audio;
using UnityEngine;
using Random = UnityEngine.Random;

public class SurfaceMamager : MonoBehaviour
{
    public static SurfaceMamager Instance;
    public bool m_Multe;

    [Header("Decal")] 
    public float m_DecalLifetime = 5f;
    public GameObject[] m_DecalPrefabs;
    public AudioClipSet[] m_DecalClips;

    [Header("Shell")] 
    public AudioClipSet[] m_ShellClips;

    private Dictionary<string, AudioClipSet> m_DecalClipDic;
    private Dictionary<string, AudioClipSet> m_ShellClipDic;
    
    private void Awake()
    {
        Instance = this;
        // m_AudioSouce = gameObject.GetComponent<AudioSource>();
        m_DecalClipDic = new Dictionary<string, AudioClipSet>();
        m_ShellClipDic = new Dictionary<string, AudioClipSet>();
        foreach (var set in m_DecalClips)
        {
            m_DecalClipDic.Add(set.Name,set);
        }
        foreach (var set in m_ShellClips)
        {
            m_ShellClipDic.Add(set.Name,set);
        }
    }

    public void ShowDecal(RaycastHit hit)
    {
        var decal = m_DecalPrefabs[Random.Range(0, m_DecalPrefabs.Length)];
        PlayDecalAudio(hit);
        SpawnDecal(decal, hit);
    }

    public void PlayDecalAudio(RaycastHit hit)
    {
        // Play the clip.
        var volume = Random.Range(0.1f, 0.3f);
        // var pitch = Random.Range(-1, 1) * Time.timeScale;
        var audioSet = m_DecalClips[Random.Range(0, m_DecalClips.Length)];
        if (!m_Multe)
        {
            audioSet.PlayAudioClip(hit.point, -1, volume);
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
        var audioSet = m_ShellClips[Random.Range(0, m_ShellClips.Length)];
        var volume = Random.Range(0.5f, 1f);
        audioSet.PlayAudioClip(point, -1, volume);
    }
}