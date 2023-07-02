using System;
using UnityEngine;

namespace MFPS
{
    [Serializable]
    public class FireAudio
    {
        [SerializeField] protected AudioClip m_FireAudioClip;
        [SerializeField] protected AudioClip m_DryFireAudioClip;
        [SerializeField] protected AudioClip m_EquipAudioClip;
        [SerializeField] protected AudioClip m_UnequipAudioClip;
        [SerializeField] protected AudioClip m_ReloadAudioClip;

        protected AudioSource m_AudioSource;

        public void Init(AudioSource audioSource)
        {
            m_AudioSource = audioSource;
        }

        public void PlayFire()
        {
            m_AudioSource.clip = m_FireAudioClip;
            m_AudioSource.Play();
        }

        public void PlayDryFire()
        {
            m_AudioSource.clip = m_DryFireAudioClip;
            m_AudioSource.Play();
        }

        public void PlayEquipAudio()
        {
            m_AudioSource.clip = m_EquipAudioClip;
            m_AudioSource.Play();
        }

        public void PlayUnequipAudio()
        {
            m_AudioSource.clip = m_UnequipAudioClip;
            m_AudioSource.Play();
        }

        public void PlayReloadAudio()
        {
            m_AudioSource.clip = m_ReloadAudioClip;
            m_AudioSource.Play();
        }
    }
}