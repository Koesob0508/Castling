using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip GameStart;
    public AudioClip Move1;
    public AudioClip Move2;
    public AudioClip Catch;
    public AudioSource AudioSource;

    public static SoundManager Instance;

    private void Start()
    {
        Instance = this;
    }

    public void PlayGameStart()
    {
        AudioSource.clip = GameStart;
        AudioSource.Play();
    }

    public void PlayMove1()
    {
        AudioSource.clip = Move1;
        AudioSource.Play();
    }

    public void PlayMove2()
    {
        AudioSource.clip = Move2;
        AudioSource.Play();
    }

    public void PlayCatch()
    {
        AudioSource.clip = Catch;
        AudioSource.Play();
    }
}
