using TMPro;
using UnityEngine;
using DG.Tweening;

public class ParryStreakHandler : MonoBehaviour
{
    public static ParryStreakHandler instance { get; private set; }

    [Header("UIAnimations")]
    [SerializeField] private TMP_Text streakText;
    [SerializeField] private float animDuration = 0.4f;
    [SerializeField] private Vector3 punchStrength = new Vector3(0.4f, 0.4f, 0f);
    [SerializeField] private int punchVibrato = 8;
    [SerializeField] private float punchElasticity = 0.6f;
    [SerializeField] private float shakeStrength = 15f;
    [SerializeField] private int shakeVibrato = 20;
    [SerializeField] private float shakeRandomness = 90f;

    [SerializeField] private AudioClip parrySound;

    private Vector3 baseScale;
    private Vector3 basePosition;
    public int parryStreak { get; private set; }
    public int highestStreak { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
            return;
        }

        highestStreak = 0;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseScale = streakText.transform.localScale;
        basePosition = streakText.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This function is subscribed to the OnParryEvent in ballbehaviour, set in editor
    public void OnStreakUpdate(ParryData data)
    {
        bool shouldContinueStreak = data.shouldContinueStreak;

        if (shouldContinueStreak)
        {
            parryStreak++;
            SuccessfulParryAnimation();
            AudioManager.PlaySFX(parrySound, Mathf.Min(parryStreak, 7) * 1.05f);

            if (parryStreak > highestStreak) highestStreak = parryStreak;
        }
        else
        {
            if (parryStreak != 0)
            {
                parryStreak = 0;
                FailedParryAnimation();
            }
        }
        
        streakText.text = "streak : X" + parryStreak;
    }

    void SuccessfulParryAnimation()
    {
        streakText.transform.DOKill();
        streakText.transform.localScale = baseScale;

        streakText.transform.DOPunchScale(punchStrength, animDuration, punchVibrato, punchElasticity);
    }

    void FailedParryAnimation()
    {
        streakText.transform.DOKill();
        streakText.transform.localPosition = basePosition;

        streakText.transform.DOShakePosition(animDuration, shakeStrength, shakeVibrato, shakeRandomness, false, true);
    }

}
