using TMPro;
using UnityEngine;

public class NameInput : MonoBehaviour {
    public TextMeshProUGUI[] letterTexts;
    public TextMeshProUGUI rankingText;   
    private char[] letters = new char[5]; 
    private int currentIndex = 0;
    bool verticalAxisInUse = false;
    bool horizontalAxisInUse = false;


    private void Start() {
        LoadPlayerName(); 
        UpdateLetters();
        HighlightCurrentLetter();
        DisplayRanking(); 
    }

    private void Update() {
        if (((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxisRaw("Vertical") >0) && !verticalAxisInUse) && Input.GetKey(KeyCode.Joystick1Button2) ) {
            PlayerPrefs.DeleteAll();
        }
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxisRaw("Vertical") >0) && !verticalAxisInUse) {
            ChangeLetter(1);
            verticalAxisInUse = true;
        } else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxisRaw("Vertical") <0) && !verticalAxisInUse) {
            ChangeLetter(-1);
            verticalAxisInUse = true;
        }
        if (Input.GetAxisRaw("Vertical") == 0) {
            verticalAxisInUse = false;
        }

        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetAxisRaw("Horizontal") >0) && !horizontalAxisInUse) {
            MoveToNextLetter();
            horizontalAxisInUse = true;
        } else if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetAxisRaw("Horizontal") <0) && !horizontalAxisInUse) {
            MoveToPreviousLetter();
            horizontalAxisInUse = true;
        }
        if (Input.GetAxisRaw("Horizontal") == 0) {
        horizontalAxisInUse = false;
    }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button9)) {
            SavePlayerName(); 
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game"); 
        }
    }

    private void ChangeLetter(int direction) {
        char currentLetter = letters[currentIndex];
        currentLetter = (char)(currentLetter + direction);

        if (currentLetter > 'Z') {
            currentLetter = 'A';
        } else if (currentLetter < 'A') {
            currentLetter = 'Z';
        }

        letters[currentIndex] = currentLetter;
        UpdateLetters();
    }

    private void UpdateLetters() {
        for (int i = 0; i < letterTexts.Length; i++) {
            letterTexts[i].text = letters[i].ToString();
        }

        HighlightCurrentLetter();
    }

    private void HighlightCurrentLetter() {
        for (int i = 0; i < letterTexts.Length; i++) {
            letterTexts[i].fontSize = i == currentIndex ? 85 : 60;
        }
    }

    private void MoveToNextLetter() {
        currentIndex = (currentIndex + 1) % letters.Length;
        HighlightCurrentLetter();
    }

    private void MoveToPreviousLetter() {
        currentIndex = (currentIndex - 1 + letters.Length) % letters.Length;
        HighlightCurrentLetter();
    }

    private void SavePlayerName() {
        string playerName = new string(letters);
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
        Debug.Log("Nome salvo: " + playerName); 
    }

    private void LoadPlayerName() {
        string savedName = PlayerPrefs.GetString("PlayerName", "AAAAA"); 
        letters = savedName.ToCharArray();
    }

    public void DisplayRanking() {
        rankingText.text = ""; // Limpa o texto atual
        var highScores = HighScoreManager.LoadHighScores(); // Carrega os high scores
        for (int i = 0; i < highScores.Count; i++) {
            rankingText.text += (i + 1) + ". " + highScores[i].name + ": " + highScores[i].score + "\n";
        }
    }

}
