using UnityEngine;

public class MoveLeftToRight : MonoBehaviour {
    public float speed = 5f;
    private float screenWidth;
    private Vector3 startPosition;

    void Start() {
      
        screenWidth = Camera.main.orthographicSize * Camera.main.aspect * 2f;


        startPosition = new Vector3(-screenWidth / 2 - transform.localScale.x, transform.position.y, transform.position.z);
        transform.position = startPosition;
    }

    void Update() {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (transform.position.x > screenWidth / 2 + transform.localScale.x) {

            transform.position = startPosition;
        }
    }
}
