using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colectable : MonoBehaviour
{

    [SerializeField] private float destroyTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(DestroyObject());
    }

    IEnumerator DestroyObject(){
        
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}
