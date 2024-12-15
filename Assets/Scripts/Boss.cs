using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour {
    public float entrySpeed = 2f; 
    public float dashSpeed = 10f; 
    public float shootingInterval = 2f;
    public GameObject projectile; 
    public int projectileCount = 10; 
    public float waitTimeAfterShot = 1f; 
    public Transform firePoint; 
    public Vector3 leftEdge = new Vector3(-12, 0, 0); 
    public Vector3 rightEdge = new Vector3(12, 0, 0);
    public Vector3 stopLeft = new Vector3(-7, 0, 0); 
    public Vector3 stopRight = new Vector3(7, 0, 0); 
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f; 
    private Color originalColor;
    private SpriteRenderer spriteRenderer; 
    private bool isShooting = false;
    private bool isFacingRight = false; 

    void Start() {
       
        transform.position = rightEdge;


        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        
        StartCoroutine(BossBehavior());
    }

   
    public void TakeDamage() {
        StartCoroutine(FlashRed());
    }


    IEnumerator FlashRed() {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    IEnumerator BossBehavior() {
       
        while (Vector3.Distance(transform.position, stopRight) > 0.1f) {
            transform.position = Vector3.MoveTowards(transform.position, stopRight, entrySpeed * Time.deltaTime);
            yield return null;
        }

      
        while (true) {

            StartCoroutine(ShootProjectiles());

      
            yield return new WaitForSeconds(waitTimeAfterShot + shootingInterval);

            while (transform.position.x > leftEdge.x) {
                transform.position = Vector3.MoveTowards(transform.position, leftEdge, dashSpeed * Time.deltaTime);
                yield return null;
            }

     
            Flip();

           
            while (Vector3.Distance(transform.position, stopLeft) > 0.1f) {
                transform.position = Vector3.MoveTowards(transform.position, stopLeft, entrySpeed * Time.deltaTime);
                yield return null;
            }

        
            StartCoroutine(ShootProjectiles());

            
            yield return new WaitForSeconds(waitTimeAfterShot + shootingInterval);

       
            while (transform.position.x < rightEdge.x) {
                transform.position = Vector3.MoveTowards(transform.position, rightEdge, dashSpeed * Time.deltaTime);
                yield return null;
            }

          
            Flip();

          
            while (Vector3.Distance(transform.position, stopRight) > 0.1f) {
                transform.position = Vector3.MoveTowards(transform.position, stopRight, entrySpeed * Time.deltaTime);
                yield return null;
            }
        }
    }

    IEnumerator ShootProjectiles() {
        if (isShooting) yield break;
        isShooting = true;

        
        float angleStep = 360f / projectileCount;
        float angle = 0f;

        for (int i = 0; i < projectileCount; i++) {
          
            float projectileDirXPosition = firePoint.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float projectileDirYPosition = firePoint.position.y + Mathf.Cos((angle * Mathf.PI) / 180);

            Vector3 projectileVector = new Vector3(projectileDirXPosition, projectileDirYPosition, 0);
            Vector3 projectileMoveDirection = (projectileVector - firePoint.position).normalized;

           
            GameObject proj = Instantiate(projectile, firePoint.position, Quaternion.identity);
            proj.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileMoveDirection.x, projectileMoveDirection.y) * 5f; 

            angle += angleStep;
        }

        yield return new WaitForSeconds(shootingInterval);

        isShooting = false;
    }

    
    private void Flip() {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Player player = collision.GetComponent<Player>();
            if (player != null) {
                player.TakeDamage(1); 
            }
         
        }
    }
}
