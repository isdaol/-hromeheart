using UnityEngine;
using TMPro;

public class Movement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 300f;

    public TextMeshProUGUI coinText;
    public TextMeshProUGUI winText;

    public int totalCoins = 1;

    public AudioClip coinSound;
    private AudioSource audioSource;

    private int coins = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Скрываем надпись о победе в начале игры
        winText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Движение по WASD
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        transform.Translate(
            x * speed * Time.deltaTime,
            0,
            z * speed * Time.deltaTime
        );

        // Прыжок
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody>().AddForce(
                Vector3.up * jumpForce
            );
        }
    }

    // Сбор монеток
    void OnTriggerEnter(Collider coin)
    {
        if (coin.CompareTag("Coin"))
        {
            // Воспроизводим звук
            if (coinSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(coinSound);
            }

            // Удаляем монетку
            Destroy(coin.gameObject);

            // Увеличиваем счётчик
            coins++;
            coinText.text = "Монеты: " + coins;

            // Проверяем победу
            if (coins >= totalCoins)
            {
                winText.gameObject.SetActive(true);
            }
        }
    }
}