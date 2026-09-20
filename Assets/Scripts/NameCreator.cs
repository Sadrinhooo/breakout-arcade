using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NameCreator : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text TMP_Name;
    [SerializeField] private RectTransform arrowsTransform;
    [SerializeField] private RectTransform[] letterBoxTransforms;
    [SerializeField] private GameObject nameInputOverlay;

    private Controls controls;
    private InputActionMap inputActions;
    private InputAction joystickAction;
    private InputAction parryAction;

    string entryName = "AAAA";
    char[] nameChars;

    private int currentChar;

    private int currentIndex;

    private readonly int A_ASCII = 65;
    private readonly int Z_ASCII = 90;
    private const float REPEAT_DELAY = 0.2f;

    float joystickY;

    private Action<string> onNameCompleted;

    private void Awake()
    {
        controls = new Controls();  

        inputActions = controls.PlayerNormal;
        joystickAction = controls.PlayerNormal.PlayerMovement;
        parryAction = controls.PlayerNormal.Aim;

        nameInputOverlay.SetActive(false);
    }


    private void OnEnable()
    {
        inputActions.Enable();
        joystickAction.Enable();
        parryAction.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
        joystickAction.Disable();
        parryAction.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        joystickY = joystickAction.ReadValue<Vector2>().y;
        if (entryName != null) TMP_Name.text = entryName;
    }

    public void BeginNameEntry(Action<string> onCompleted)
    {
        onNameCompleted = onCompleted;
        nameChars = entryName.ToCharArray();
        nameInputOverlay.SetActive(true);
        StartCoroutine(TypeName());
    }

    IEnumerator TypeName()
    {
        currentChar = 0;
        arrowsTransform.position = letterBoxTransforms[currentChar].position;
        currentIndex = A_ASCII;

        float cooldown = 0;

        while (currentChar <= 3)
        {
            //Debug.Log(currentChar);

            if (cooldown > 0f)
            {
                cooldown -= Time.deltaTime;

            }
            else
            {
                if (joystickY > 0.5f)
                {
                    currentIndex = currentIndex >= Z_ASCII ? A_ASCII : currentIndex + 1;
                    cooldown = REPEAT_DELAY;
                }
                else if (joystickY < -0.5f)
                {
                    currentIndex = currentIndex <= A_ASCII ? Z_ASCII : currentIndex - 1;
                    cooldown = REPEAT_DELAY;
                }

                nameChars[currentChar] = (char)currentIndex;
                entryName = new string(nameChars);

                if (parryAction.WasPressedThisFrame())
                {
                    ++currentChar;
                    if (currentChar <= 3)
                    {
                        arrowsTransform.position = letterBoxTransforms[currentChar].position;
                        currentIndex = A_ASCII;
                    }
                }
            }
            yield return null;
        }

        onNameCompleted?.Invoke(entryName);
        nameInputOverlay.SetActive(false);
        yield return null;
    }
}
