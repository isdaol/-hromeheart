using UnityEngine;

public class Coin : MonoBehaviour
{
    public AudioClip coinSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (coinSound != null)
            {
                Debug.Log("Звук монетки запускается!");
                GameObject soundObject = new GameObject("CoinSound");
                AudioSource audioSource = soundObject.AddComponent<AudioSource>();

                audioSource.clip = coinSound;
                audioSource.Play();

                Destroy(soundObject, coinSound.length);
            }

            Destroy(gameObject);
        }
    }
}