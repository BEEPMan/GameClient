using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{
    Stat _stat;

    Rigidbody _rigid;

    public Queue<Vector2> coordinates = new Queue<Vector2>();

    void Start()
    {
        _stat = GetComponent<Stat>();
        _rigid = GetComponent<Rigidbody>();

        if (gameObject.GetComponentInChildren<UI_PlayerName>() == null)
        {
            Managers.UI.MakeWorldSpaceUI<UI_PlayerName>(transform);
        }
    }

    void FixedUpdate()
    {
        coordinates.Enqueue(new Vector2(transform.position.x, transform.position.z));
    }
}
