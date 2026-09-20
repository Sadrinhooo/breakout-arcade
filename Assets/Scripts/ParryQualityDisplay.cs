using UnityEngine;
using DG.Tweening;
using TMPro;

public class ParryQualityDisplay : MonoBehaviour
{
    public static ParryQualityDisplay instance;
    [SerializeField] private GameObject textObject;
    [SerializeField] private Vector2 textOffset = new Vector2(3, 2);

    [Header("AnimProperties")]
    [SerializeField] private float riseDistance = 4;
    [SerializeField] private float duration = 1.5f;

    private Sequence seq;

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

        textObject.SetActive(false);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayParryText(ParryData data)
    {
        Color textColor = data.textColor;
        Vector2 padlePosition = data.parriedPlayerPosition;
        string text = data.feedbackText;

        seq?.Kill();

        textObject.transform.position = padlePosition + textOffset;
        textObject.transform.localScale = Vector3.one;
        textObject.SetActive(true);

        TMP_Text TMPText = textObject.GetComponent<TMP_Text>();
        TMPText.color = textColor;
        TMPText.text = text;
        TMPText.alpha = 1f;

        Vector3 startPosition = textObject.transform.position;
        Vector3 endPosition = startPosition + new Vector3(0f, riseDistance, 0f);

        seq = DOTween.Sequence();

        seq.Append(textObject.transform.DOMove(endPosition, duration).SetEase(Ease.OutQuad));
        seq.Join(TMPText.DOFade(0f, duration).SetEase(Ease.InQuad));
        seq.Join(textObject.transform.DOScale(Vector3.one * 1.4f, duration).SetEase(Ease.OutSine));

        seq.OnComplete(() => textObject.SetActive(false));
    }
}
