using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] [TextArea(3,10)] private string[] diePhrase;
    [SerializeField] private float endGameTimer;

    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private TMP_Text gameOverTimer;
    public bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameOver){
            gameOver = false;
            int randomPhrase = Random.Range(0, diePhrase.Length-0);
            gameOverText.text = diePhrase[randomPhrase];
            StartCoroutine(DecrementTimer());

        }

    }

    void OnEnable(){
    }

    IEnumerator DecrementTimer(){
        for(int i = 0; i <= endGameTimer; i++){
            yield return new WaitForSeconds(1);

            gameOverTimer.text = $"{endGameTimer - i}";
            if(endGameTimer - i <= 0){
                SceneManager.LoadScene("Menu");
            }
        }

    }
}
