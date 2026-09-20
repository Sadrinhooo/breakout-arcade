using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance {get ; private set;}

    private float timeElapsed = 0;
    private bool isShaking = false;

    [SerializeField] private float duration = 0.25f;
    [SerializeField] private float amplitude = 0.2f;
    [SerializeField] private float frequency = 40f;

    Vector3 basePosition; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        basePosition = transform.position;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isShaking)
        {
            timeElapsed += Time.deltaTime; 
            float t = timeElapsed / duration;
            float fallof = 1f - t;

            Vector3 shakeOffset = new Vector3(
                Mathf.Cos(timeElapsed * frequency) * amplitude * fallof, //amplitude is constant, fallof is closer to 0 overtime
                Mathf.Sin(timeElapsed * frequency) * amplitude * fallof,
                0f);

            transform.position = basePosition + shakeOffset;

            if (t >= 1f)
            {
                isShaking = false;
                transform.position = basePosition;
                return;
            }
        }
        
    }

    public void ScreenShake(float intensity)
    {
        if (!isShaking)
        {
            basePosition = transform.position;
        }

        amplitude = intensity;
        timeElapsed = 0;
        isShaking = true;
    }
}
