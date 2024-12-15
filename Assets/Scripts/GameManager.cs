using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public GameObject inimigoTerrestrePrefab;
    public GameObject inimigoVoadorPrefab;
    public GameObject inimigoEspecialPrefab;
    public GameObject bossPrefab;
    public TextMeshProUGUI name;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hordaText;
    public TextMeshProUGUI countdownText;
    public float initialWait = 0f;

    public int hordaAtual = 1;
    public int inimigosPorHorda = 5;
    public float spawnInterval = 1.0f;
    public float screenWidth = 20f;
    public float screenHeight = 10f;
   

    private bool hordaAtiva = false;
    private bool inimigoEspecialSpawnado = false;
    private int inimigosEspeciaisRestantes = 0;
    private int playerScore = 0;

    private int incrementoVida = 1;
    private int incrementoPontos = 1;
    private bool bossSpawnado = false;

    [SerializeField] private GameObject gameOverCanvas;
    
    private int bossIncrementoVida = 10; 
    private int bossIncrementoPontos = 100; 
    private bool _normalMusic = false;
    private float _groundEnemySpeed;


    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gameMusic;

    void Start() {
        LoadPlayerName();
        StartCoroutine(GerenciarHordas());
        UpdateScoreText();
        UpdateHordaText();
        countdownText.gameObject.SetActive(false); 
    }
    

    void LoadPlayerName() {
        if (PlayerPrefs.HasKey("PlayerName")) {
            string playerName = PlayerPrefs.GetString("PlayerName");
            name.text = playerName;
        }
    }

    public void AddScore(int points) {
        playerScore += points;
        // Debug.Log("Pontuacao atual: " + playerScore);
        UpdateScoreText();
    }
    void Update(){
        CheckBoss();
        ExitGame();
    }

    public void PlayerDied() {
        SaveScore();
        gameOverCanvas.SetActive(true);
        gameOverCanvas.GetComponent<GameOver>().gameOver = true;
        // SceneManager.LoadScene("Menu");
    }

    public void SaveScore() {
        string playerName = PlayerPrefs.GetString("PlayerName", "AAAAA");
        // Debug.Log("Salvando pontua��o: " + playerName + " - " + playerScore);
        HighScoreManager.SaveScore(playerName, playerScore);
    }

    private void UpdateScoreText() {
        if (scoreText != null) {
            scoreText.text = "Pontuação: " + playerScore;
        }
    }

    private void UpdateHordaText() {
        if (hordaText != null) {
            hordaText.text = "Horda: " + hordaAtual;
        }
    }

    IEnumerator GerenciarHordas() {
        yield return new WaitForSeconds(initialWait);
        while (true) {
            if(hordaAtual >= 10){
            _groundEnemySpeed +=10f;

            }else{

            _groundEnemySpeed +=5f;
            }
            hordaAtiva = true;

            
            if (hordaAtual % 10 == 0 && !bossSpawnado) {
               
                Enemy[] inimigos = FindObjectsOfType<Enemy>();
                foreach (Enemy inimigo in inimigos) {
                    Destroy(inimigo.gameObject);
                }
                SpawnBoss();
                yield return new WaitForSeconds(5.0f); 
                bossSpawnado = true; 
                continue; 
            }

           
            int terrestres = inimigosPorHorda;
            int voadores = 0;

            if (hordaAtual >= 4) {
                voadores = inimigosPorHorda / 2;
                terrestres = inimigosPorHorda - voadores;
            }

            for (int i = 0; i < terrestres; i++) {
                SpawnInimigoTerrestre();
                yield return new WaitForSeconds(spawnInterval);
            }

            for (int i = 0; i < voadores; i++) {
                SpawnInimigoVoador();
                yield return new WaitForSeconds(spawnInterval);
            }

            if (hordaAtual >= 7 && !inimigoEspecialSpawnado) {
                StartCoroutine(GerenciarInimigosEspeciais());
            }

            yield return new WaitForSeconds(0.1f);//TODO deixar em 5

            hordaAtual++; 

          
            inimigosPorHorda += 3;

            bossSpawnado = false; 

            UpdateHordaText();
        }
    }

    void CheckBoss(){
        if(!GameObject.FindGameObjectWithTag("Boss")){
            if(!_normalMusic){
                audioSource.clip = gameMusic;
                audioSource.loop = true;
                audioSource.Play();
                _normalMusic = true;

            }
        }
        else{
            _normalMusic = false;
            audioSource.Stop();
        }
    }
    void ExitGame(){
        if(Input.GetKey(KeyCode.Joystick1Button8) && Input.GetKeyDown(KeyCode.Joystick1Button9)){
            Application.Quit();
        }
    }


    void SpawnInimigoTerrestre() {
        float ladoSpawn = Random.Range(0, 2) == 0 ? -screenWidth / 2 : screenWidth / 2;
        float randomY = Random.Range(-screenHeight / 2, screenHeight / 2);
        Vector3 spawnPosition = new Vector3(ladoSpawn, randomY, 0);
        Enemy inimigo = Instantiate(inimigoTerrestrePrefab, spawnPosition, Quaternion.identity).GetComponent<Enemy>();
        if (hordaAtual >=10) {
            inimigo.GetComponent<InimigoTerrestre>().velocidade += 40f;
            inimigo.GetComponent<Enemy>().maxHealth ++;
             inimigo.GetComponent<Enemy>().scoreValue++;
             inimigo.GetComponent<Enemy>().scoreValue = Random.Range(inimigo.GetComponent<Enemy>().scoreValue, inimigo.GetComponent<Enemy>().scoreValue*2);
               
                
        }
        
        inimigo.AjustarVelocidade(_groundEnemySpeed); 
    }

    void SpawnInimigoVoador() {
        float randomX = Random.Range(-screenWidth / 2, screenWidth / 2);
        Vector3 spawnPosition = new Vector3(randomX, screenHeight / 2 + 2, 0);
        Enemy inimigo = Instantiate(inimigoVoadorPrefab, spawnPosition, Quaternion.identity).GetComponent<Enemy>();
        if (hordaAtual >=10) {
            inimigo.GetComponent<Enemy>().scoreValue = Random.Range(inimigo.GetComponent<Enemy>().scoreValue, inimigo.GetComponent<Enemy>().scoreValue*2);
               
                
        }
       
        inimigo.AjustarVidaInicial(); 
    }

    void SpawnInimigoEspecial() {
        float randomX = Random.Range(Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x, Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x);
        float randomY = Random.Range(Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y, Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y);
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0);
        Enemy inimigo = Instantiate(inimigoEspecialPrefab, spawnPosition, Quaternion.identity).GetComponent<Enemy>();

        inimigo.AjustarVidaInicial(); 
    }

    void SpawnBoss() {
        StopAllCoroutines();

        
        Vector3 spawnPosition = new Vector3(12, 0, 0); 
        Enemy boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity).GetComponent<Enemy>();

       
        boss.AumentarDificuldade(bossIncrementoVida, bossIncrementoPontos); 

        bossSpawnado = true;
    }
    public void BossDied() {
        Debug.Log("Boss foi derrotado! Iniciando contagem regressiva.");
        StartCoroutine(ContagemRegressivaParaReinicio());
        

        
        bossSpawnado = false;

      
        AjustarDificuldadeInimigos(); 
    }

    IEnumerator GerenciarInimigosEspeciais() {
        inimigoEspecialSpawnado = true;

        while (true) {
            if (inimigosEspeciaisRestantes == 0) {
                inimigosEspeciaisRestantes = Random.Range(1, 5);

                for (int i = 0; i < inimigosEspeciaisRestantes; i++) {
                    SpawnInimigoEspecial();
                    yield return new WaitForSeconds(spawnInterval);
                }
            }

            yield return null;

            if (GameObject.FindGameObjectsWithTag("InimigoEspecial").Length == 0) {
                yield return new WaitForSeconds(Random.Range(1, 5));
                inimigosEspeciaisRestantes = 0;
            }
        }
    }


    IEnumerator ContagemRegressivaParaReinicio() {
        Debug.Log("Iniciando contagem regressiva para rein�cio.");
        countdownText.gameObject.SetActive(true); 
        for (int i = 3; i > 0; i--) {
            countdownText.text = i.ToString(); 
            yield return new WaitForSeconds(1.0f);
        }
        countdownText.text = "Vai!"; 
        yield return new WaitForSeconds(1.0f); 
        countdownText.gameObject.SetActive(false); 

      
        hordaAtual++; 
        inimigosPorHorda += 3;
        UpdateHordaText(); 
        StartCoroutine(GerenciarHordas()); 
    }

    void ResetarHordas() {
        Debug.Log("Resetando hordas.");
        hordaAtual = 1;
        inimigosPorHorda = 5;

        
        Enemy[] inimigos = FindObjectsOfType<Enemy>();
        foreach (Enemy inimigo in inimigos) {
            Destroy(inimigo.gameObject);
        }

        
        bossSpawnado = false;
        UpdateHordaText();
    }
    void AjustarDificuldadeInimigos() {
       
        Enemy[] inimigos = FindObjectsOfType<Enemy>();
        foreach (Enemy inimigo in inimigos) {
            if (inimigo.enemyType == Enemy.EnemyType.Boss) {
                inimigo.AumentarDificuldade(10, 10);
            } else {
                inimigo.AumentarDificuldade(1, 1);
            }
        }
    }

}
