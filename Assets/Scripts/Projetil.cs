using UnityEngine;

public class Projetil : MonoBehaviour {
    private Renderer rend;

    void Start() {
        rend = GetComponent<Renderer>();
    }

    void Update() {
        // Destroi o projetil se ele sair da tela
        if (!rend.isVisible) {
            Destroy(gameObject);
        }
    }

    // Detecta colis�o com o jogador
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Player player = collision.GetComponent<Player>();
            if (player != null && player.canTakeDamage) {
                player.TakeDamage(1); // Causa 1 de dano ao jogador
            }
            Destroy(gameObject); // Destroi o projetil ap�s a colis�o
        }
        if (collision.CompareTag("escudo")) {
            
            Destroy(gameObject); // Destroi o projetil ap�s a colis�o
        }
    }
}
