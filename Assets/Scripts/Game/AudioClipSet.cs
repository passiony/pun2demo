/// ---------------------------------------------
/// Opsive Shared
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace MFPS
{
    [System.Serializable]
    public class AudioClipSet
    {
        [FormerlySerializedAs("m_AudioName")] [Tooltip("The AudioConfig for the set.")] [SerializeField]
        protected string m_Name;

        [FormerlySerializedAs("m_AudioClips")]
        [Tooltip("An array of AudioClips which belong to the set.")]
        [SerializeField]
        protected AudioClip[] m_Clips;

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public AudioClip[] Clips
        {
            get { return m_Clips; }
            set { m_Clips = value; }
        }

        public void PlayAudioClip(Vector3 point, AudioClip clip, float volume = 1)
        {
            AudioSource.PlayClipAtPoint(clip, point, volume);
        }

        public void PlayAudioClip(Vector3 point, int clipIndex = -1, float volume = 1)
        {
            var clip = GetAudioClipInfo(clipIndex);
            AudioSource.PlayClipAtPoint(clip, point, volume);
        }

        private AudioClip GetAudioClipInfo(int index = -1)
        {
            if (m_Clips == null || m_Clips.Length == 0)
            {
                return null;
            }

            if (index < 0 || index >= m_Clips.Length)
            {
                index = Random.Range(0, m_Clips.Length);
            }

            var audioClip = m_Clips[index];
            return audioClip;
        }
    }
}