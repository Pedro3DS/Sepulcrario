using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    private Rigidbody2D _rb2d;
    [SerializeField] private GameObject shoot;
    [SerializeField] private GameObject shoot2;
    private bool _usingWapon2 = false;
    [SerializeField] private float moveSpeed;
    public float shootSpeed;
    [SerializeField] private float shootCadence = 0.5f;
    private float _nextShoot = 0f;
    public bool canTakeDamage = true;

    [SerializeField] private int maxHealth = 3; 
    private int currentHealth;
    
    [SerializeField] private Sprite activeLife; 
    [SerializeField] private Sprite disableLife; 
    [SerializeField] private Image[] heatsField; 
    private bool isInvulnerable = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;

    private GameManager gameManager;

    [SerializeField] private Camera mainCamera;
    private Vector2 _screenBounds;
    private Vector2 lastMoveDirection; 
    [SerializeField] private float boundsLimit;

    [SerializeField] private GameObject gameOverCanvas;
    private bool _isDied = false;
    private bool _canShoot = true;

    [Header("Power Ups")]
    private bool _canUseSuper = true;
    [SerializeField] private float superDuration;
    [SerializeField] private Slider superSlider;
    [SerializeField] private Image powerUpField;
    [SerializeField] private GameObject shield;
    [SerializeField] private Sprite shieldSprite;
    [Header("Sound Effects")]
    private AudioSource _audioSource;
    [SerializeField] private AudioClip lifeEffect;
    [SerializeField] private AudioClip defEffect;
    [SerializeField] private AudioClip damageEffect;
    [SerializeField] private AudioClip weaponEffect;



    void Start() {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        _rb2d = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        playerCollider = gameObject.GetComponent<Collider2D>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        currentHealth = maxHealth;
        UpdateHearts(); 
        gameManager = FindObjectOfType<GameManager>();
        lastMoveDirection = Vector2.right; 
    }
    void UpdateHearts() {
        if(currentHealth <= 3){

            for (int i = 0; i <= heatsField.Length-1; i++) {

                heatsField[i].sprite = disableLife;
                // heartSprites[i].SetActive(i < currentHealth);
            }
            for (int i = 0; i <= currentHealth -1; i++) {
                heatsField[i].sprite = activeLife;
                // Debug.Log(i);
                // heartSprites[i].SetActive(i < currentHealth);
            }
        }else{
            currentHealth = 3;
        }
    }

    void Update()
    {
        
        FixScreenBounds();
        Movement();
        Shoot();
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0)){
            StartCoroutine(UseSuper());
        }
    }
    void Movement() {
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(horizontal  ,vertical);

        _rb2d.velocity = move * moveSpeed;
    
        // _rb2d.velocity = move * moveSpeed;

        if (move != Vector2.zero) {
            lastMoveDirection = move; 
        }

        if (horizontal > 0) {
            spriteRenderer.flipX = true;
        } else if (horizontal < 0) {
            spriteRenderer.flipX = false;
        }


        if (horizontal != 0 || vertical != 0) {
            if(_usingWapon2){
                gameObject.GetComponent<Animator>().SetBool("walkF", true);

            }else{

                gameObject.GetComponent<Animator>().SetBool("walk", true);
            }
        } else {
            if(_usingWapon2){
                gameObject.GetComponent<Animator>().SetBool("walkF", false);

            }else{

                gameObject.GetComponent<Animator>().SetBool("walk", false);
            }
        }
    }
    void Shoot() {
        if(_canShoot){

            if ((Time.time >= _nextShoot && Input.GetKeyDown(KeyCode.P)) || (Time.time >= _nextShoot && Input.GetKeyDown(KeyCode.Joystick1Button3)) ) {
                _nextShoot = Time.time + shootCadence;

                
                GameObject newShoot = Instantiate(shoot, transform.position, Quaternion.identity);
                newShoot.GetComponent<Rigidbody2D>().velocity = lastMoveDirection * shootSpeed;
            }
        }
    }
    void FixScreenBounds(){
        _screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        Vector3 playerPosition = transform.position;
        playerPosition.x = Mathf.Clamp(playerPosition.x, ((_screenBounds.x * -1) + boundsLimit) + spriteRenderer.bounds.extents.x, (_screenBounds.x - boundsLimit) - spriteRenderer.bounds.extents.x);
        playerPosition.y = Mathf.Clamp(playerPosition.y, ((_screenBounds.y * -1) + boundsLimit) + spriteRenderer.bounds.extents.y, (_screenBounds.y - boundsLimit) - spriteRenderer.bounds.extents.y);
        _rb2d.transform.position = playerPosition;
    }

    IEnumerator UseSuper(){
        if(_canUseSuper){
            _canUseSuper = false;
            superSlider.maxValue = superDuration;
            superSlider.value = 0;
            foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("InimigoEspecial")){
                enemy.GetComponent<Enemy>().Die();
            }
            for(int i = 0; i <= superDuration; i++){
                superSlider.value = i;
                yield return new WaitForSeconds(1);
            }
            _canUseSuper = true;

        }

    }

    void CreateShield(){
        Instantiate(shield, gameObject.transform);

    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(canTakeDamage){
            if (collision.gameObject.CompareTag("InimigoEspecial") && !isInvulnerable) {
                TakeDamage();
            }

        }
        
    }
    
    void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.CompareTag("Shield")) {
            _audioSource.clip = defEffect;
            _audioSource.Play();
            Destroy(other.gameObject);
            CreateShield();

        }
        if (other.gameObject.CompareTag("Heart")) {
            _audioSource.clip = lifeEffect;
            _audioSource.Play();
            Destroy(other.gameObject);
            GetLife();

        }
        if (other.gameObject.CompareTag("Timer")) {
            _audioSource.clip = lifeEffect;
            _audioSource.Play();
            Destroy(other.gameObject);
            if(shootCadence >= 0.1f){
                shootCadence -= 0.1f;
            }

        }
        if (other.gameObject.CompareTag("Weapon2")) {
            _audioSource.clip = weaponEffect;
            _audioSource.Play();
            gameObject.GetComponent<Animator>().SetBool("walk", false);
            gameObject.GetComponent<Animator>().SetBool("idleF", true);
            _usingWapon2 = true;
            Destroy(other.gameObject);
            shoot = shoot2;
            shootSpeed += 4f;

        }
    }

    void GetLife(){
        currentHealth ++;
        UpdateHearts();
    }

    void TakeDamage() {
        if(canTakeDamage){
            _audioSource.clip = damageEffect;
            _audioSource.Play();
            currentHealth--;
            UpdateHearts();

            if (currentHealth <= 0) {
                if(!_isDied){
                    Die(); 

                }
            } else {
                StartCoroutine(BecomeInvulnerable()); 
            }
        }
    }
    void ChangePowerUpImage(Sprite powerUpSprite){
        powerUpField.preserveAspect = true;
        powerUpField.sprite = powerUpSprite;
    }
    IEnumerator BecomeInvulnerable() {
        isInvulnerable = true;
        playerCollider.enabled = false;

        for (int i = 0; i < 5; i++) {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f); 
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f); 
            yield return new WaitForSeconds(0.2f);
        }

        playerCollider.enabled = true;
        isInvulnerable = false;
    }
    void Die() {
        _isDied = true;
        _canShoot = false;
        _canUseSuper = false;
        gameManager.SaveScore(); 
        canTakeDamage = false;
        _rb2d.bodyType = RigidbodyType2D.Static;
        gameObject.GetComponent<CapsuleCollider2D>().isTrigger = true;
        gameOverCanvas.SetActive(true);
        gameOverCanvas.GetComponent<GameOver>().gameOver = true;
        // SceneManager.LoadScene("Menu"); 
    }
    public void TakeDamage(int amount) {
        if (!isInvulnerable) {
            currentHealth -= amount;
            UpdateHearts();

            if (currentHealth <= 0) {
                Die();
            } else {
                StartCoroutine(BecomeInvulnerable());
            }
        }
    }

}