using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private float duration;
    private GameObject _player;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if(_player != null){
            _player.GetComponent<Player>().canTakeDamage = false;
            StartCoroutine(DestoryShield());
        }
    }

    IEnumerator DestoryShield(){
        yield return new WaitForSeconds(duration);
        _player.GetComponent<Player>().canTakeDamage = true;
        Destroy(gameObject);
    }
}
