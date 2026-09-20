using UnityEngine;

public class PlaySoundAtStart : MonoBehaviour
{
    [Header("Аудио")]
    public AudioClip soundClip;   // Сюда перетащите ваш файл raian-gosling.mp3

    private AudioSource audioSource;

    void Start()
    {
        // 1. Пытаемся получить компонент AudioSource на этом же объекте
        audioSource = GetComponent<AudioSource>();

        // 2. Если его нет – создаём новый
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 3. Назначаем клип и воспроизводим
        audioSource.clip = soundClip;
        audioSource.Play();
    }
}