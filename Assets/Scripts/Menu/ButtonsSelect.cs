using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsSelect : MonoBehaviour
{
    [SerializeField] private Color handleColor;
    [SerializeField] private Color normalColor;
    [SerializeField] private Button[] buttons;
    public int visibleButtonCount = 5;

    private string backImgsPath;
    private int currentIndex = 0;
    private int totalButtons = 0;
    private int selectedButtonIndex = 0;
    private Process currentProcess;
    private float gamepadInputDelay = 0.3f;
    private float nextInputTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        HighlightButton(selectedButtonIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            ButtonUp();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ButtonDown();
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button9))
        {
            buttons[selectedButtonIndex].onClick.Invoke();
       
        }
    }
    public void SetButtonsColor(TMP_Text currentText)
    {
        RemoveColors();
        // currentText.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        currentText.color = handleColor;
    }
    public void RemoveColors()
    {
        for (var i = 0; i < buttons.Length; i++)
        {
            if (i != selectedButtonIndex)
            {
                buttons[i].GetComponentInChildren<TMP_Text>().transform.localScale = new Vector3(1f, 1f, 1f);
                buttons[i].GetComponentInChildren<TMP_Text>().color = normalColor;

            }
        };
    }
    private void ButtonDown()
    {

            selectedButtonIndex++;
            if (selectedButtonIndex > buttons.Length-1)
            {
                selectedButtonIndex = 0;
            }
            HighlightButton(selectedButtonIndex);
        
    }
    private void ButtonUp()
    {
            selectedButtonIndex--;
            if (selectedButtonIndex < 0)
            {
                selectedButtonIndex = buttons.Length-1;
            }
            HighlightButton(selectedButtonIndex);
       
    }
    private void HighlightButton(int index)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            Transform button = buttons[i].transform;
            TMP_Text buttonImage = button.GetComponentInChildren<TMP_Text>();
            if (buttonImage != null)
            {
                buttonImage.transform.localScale = new Vector3(1f, 1f,1f);
                buttonImage.color = normalColor;
            }
        }

        Transform selectedButton = buttons[index].transform;
        TMP_Text selectedButtonImage = selectedButton.GetComponentInChildren<TMP_Text>();
        if (selectedButtonImage != null)
        {
            selectedButtonImage.transform.localScale = new Vector3(1.1f, 1.1f,1.1f);
            selectedButtonImage.color = handleColor;
        }

    }
}
