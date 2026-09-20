using UnityEngine;
using UnityEngine.Events;

public struct ParryData
{
    public Color textColor;
    public Vector2 parriedPlayerPosition;
    public string feedbackText;
    public float quality;
    public bool shouldContinueStreak;
}

[System.Serializable]
public class OnParryEvent: UnityEvent<ParryData> { }

public class BallBehaviour : MonoBehaviour
{
    public static BallBehaviour instance { get; private set; }

    public float speed { get; private set; }
    public Vector2 normalizedVelocity { get; private set; }
    private Vector2 lastVelocity;

    [Header("Speed")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float startSpeed;
    [SerializeField] private float increasePerParry;
    [SerializeField] private float missParrySpeed;

    [SerializeField] private AudioClip paldeSound;
    [SerializeField] private AudioClip parrySound;

    private Rigidbody2D rb2D;

    public OnParryEvent onParryEvent;
    private ParryData parryData = new();

    //Add a small random offset to the vector direction after each hit
    private float randomRotationOffset;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        //Apply Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        //:::::::::::::::::::

        rb2D = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        speed = startSpeed;
       
    }

    // Update is called once per frame
    void Update()
    {
        normalizedVelocity = rb2D.linearVelocity.normalized;
    }

    private void FixedUpdate()
    {
        if (rb2D.linearVelocity.sqrMagnitude > 0.001f)
        {
            rb2D.linearVelocity = rb2D.linearVelocity.normalized * speed;
        }

        lastVelocity = rb2D.linearVelocity;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent<BlockBehaviour>(out var block))
        {
            bool wasDestroyed = block.TakeDamage(speed);

            if (wasDestroyed)
            {
                rb2D.linearVelocity = lastVelocity;
                return;
            }
        }

        randomRotationOffset = Random.Range(1f, 5f);
        Vector2 reflectedAngle = Vector2.Reflect(lastVelocity, other.GetContact(0).normal);
        rb2D.linearVelocity = Quaternion.Euler(0f, 0f, randomRotationOffset) * reflectedAngle;
    }

    public void HandleParryAttempt(float quality, Vector2 direction, Vector2 padelPosition)
    {
        
        if (quality >= 0.7f) //good parry
        {
            speed = Mathf.Min(speed + increasePerParry, maxSpeed);
            rb2D.linearVelocity = direction * speed;
            parryData.textColor = Color.green;
            parryData.feedbackText = "perfect";
            AudioManager.PlaySFX(paldeSound);
            
        }
        else if (quality >= 0.3f) //meh parry
        {
            speed = missParrySpeed;
            rb2D.linearVelocity = direction * speed;
            parryData.textColor = Color.orange;
            parryData.feedbackText = "good";
            AudioManager.PlaySFX(paldeSound);
        }
        else
        {
            parryData.textColor= Color.darkRed;
            parryData.feedbackText = "miss";
        }

        parryData.quality = quality;
        parryData.parriedPlayerPosition = padelPosition;
        parryData.shouldContinueStreak = quality > 0.7f;

        onParryEvent.Invoke(parryData);
    }

    public void BeginGame()
    {
        rb2D.linearVelocity = Vector2.right * startSpeed;
    }
}
