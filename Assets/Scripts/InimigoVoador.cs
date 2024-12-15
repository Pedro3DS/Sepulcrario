using UnityEngine;

public class InimigoVoador : MonoBehaviour {
    public float velocidade = 3.0f;
    public float distanciaAtaque = 5.0f;
    public GameObject projetilPrefab;
    public float intervaloTiro = 2.0f;
    public Transform pontoDisparo;

    private Transform player;
    private float tempoUltimoTiro = 0f;
    private Rigidbody2D _rb2d;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _rb2d = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (player != null) {
            float distanciaDoPlayer = Vector3.Distance(transform.position, player.position);

            if (distanciaDoPlayer > distanciaAtaque) {
                Vector3 direcao = (player.position - transform.position).normalized;
                _rb2d.velocity = direcao * velocidade * Time.deltaTime;
                // transform.position += direcao * velocidade * Time.deltaTime;
            } else {
                AtirarNoPlayer();
            }
        }
    }

    void AtirarNoPlayer() {
        if (Time.time >= tempoUltimoTiro + intervaloTiro) {
            Vector3 direcao = (player.position - pontoDisparo.position).normalized;
            GameObject projetil = Instantiate(projetilPrefab, pontoDisparo.position, Quaternion.identity);
            projetil.GetComponent<Rigidbody2D>().velocity = direcao * 5f;
            tempoUltimoTiro = Time.time;
        }
    }
}
