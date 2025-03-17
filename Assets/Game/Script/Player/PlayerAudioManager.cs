using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _footstepSFX;
    [SerializeField]
    private AudioSource _landingSFX;
    [SerializeField]
    private AudioSource _punchSFX;
    [SerializeField]
    private AudioSource _glideSFX;

    public void playGlideSFX()
    {
        _glideSFX.Play();
    }

    public void stopGlideSFX()
    {
        _glideSFX.Stop();
    }

    public void PlayFootstepSFX()
    {
        _footstepSFX.volume = Random.Range(0.8f, 1f);
        _footstepSFX.pitch = Random.Range(0.8f, 1.5f);
         _footstepSFX.Play();
    }

    public void PlayLandingSFX()
    {
        _landingSFX.volume = Random.Range(0.8f, 1f);
        _landingSFX.pitch = Random.Range(0.8f, 1.5f);
        _landingSFX.Play();
    }

    public void PlayPunchSFX()
    {
        _punchSFX.volume = Random.Range(0.8f, 1f);
        _punchSFX.pitch = Random.Range(0.8f, 1.5f);
        _punchSFX.Play();
    }

    public void PlayGlideSFX()
    {
        _glideSFX.Play();
    }
}
