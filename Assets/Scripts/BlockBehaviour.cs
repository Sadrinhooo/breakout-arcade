using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class BlockBehaviour : MonoBehaviour
{
    private SpriteRenderer spriteRend;
    private Color color = new Color(1, 1, 0, 1);

    private float health = 80f;

    [SerializeField] private float damageMultiplier = 5f;
    [SerializeField] private ParticleSystem destructPartciles;

    public static UnityEvent onDestroyedBlock = new UnityEvent();
   
    private void Awake()
    {
        spriteRend = GetComponent<SpriteRenderer>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        destructPartciles = Instantiate(destructPartciles, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool TakeDamage(float ballSpeed)
    {
        float screenShakeIntensity = ballSpeed * 0.015f;
        health -= ballSpeed * damageMultiplier;
        CameraShake.instance.ScreenShake(screenShakeIntensity);

        if (health <= 0)
        {
            destructPartciles.Play();
            onDestroyedBlock?.Invoke();
            Destroy(gameObject);
            Destroy(destructPartciles.gameObject, 2);

            return true;
        }

        CalculateColor();
        return false;
    }

    void CalculateColor()
    {
        color.g = health / 100;
        color.r = 1 - (health / 100);
        spriteRend.color = color;
    }
}
