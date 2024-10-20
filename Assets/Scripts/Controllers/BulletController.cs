using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float _speed = 30.0f;
    public int _damage = 10;
    public float _lifeTime = 3.0f;

    private Stat _stat;

    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * _speed, ForceMode.Impulse);
    }

    void Update()
    {
        _lifeTime -= Time.deltaTime;
        if(_lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            Stat targetStat = other.gameObject.GetComponent<Stat>();
            Stat myStat = GetComponent<Stat>();

            if (targetStat != null)
            {
                targetStat.OnAttacked(myStat);
            }
        }
    }
}
