using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Move());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Move()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(5, 0, 0);
        yield return new WaitForSeconds(1.0f);
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        yield return null;
    }
}
