using UnityEngine;

public class ParryHitbox : MonoBehaviour
{
    [SerializeField] private PlayerMovement owner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ball")) return;

        if (!owner.isParryWindowActive) return;

        BallBehaviour ball = BallBehaviour.instance;

        if (Vector2.Dot(ball.normalizedVelocity, Vector2.down) < 0.05f) return;
        Debug.Log(Vector2.Dot(ball.normalizedVelocity, Vector2.down));

        float timeIntoWindow = Time.time - owner.parryWindowStartTime;

        float quality = Mathf.Clamp01(1f - (timeIntoWindow / owner.parryWindowDuration));

        Vector2 offset = other.transform.position - transform.position;

        Vector2 direction = (offset.normalized + Vector2.up).normalized;
        
        ball.HandleParryAttempt(quality, direction, transform.position);
    }
}
