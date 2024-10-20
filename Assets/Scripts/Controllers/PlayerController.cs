using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    float _speed = 10.0f;
    Vector3 _destPos;
    Vector3 _shootPos;

    public Stat myStat;
    public GameObject _bullet;

    int _mask = (1 << (int)6);

    void Start()
    {
        _destPos = transform.position;
        myStat = GetComponent<Stat>();

        if(gameObject.GetComponentInChildren<UI_PlayerName>() == null)
        {
            Managers.UI.MakeWorldSpaceUI<UI_PlayerName>(transform);
        }

        Camera.main.GetComponent<CameraController>().SetPlayer(gameObject);
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hitInfo;

        //    if (Physics.Raycast(ray, out hitInfo, 100.0f, _mask))
        //    {
        //        _shootPos = hitInfo.point;
        //        _shootPos.y = transform.position.y;
        //        Instantiate(_bullet, transform.position, Quaternion.LookRotation(_shootPos - transform.position));
        //    }
        //}

        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 100.0f, _mask))
            {
                _destPos = hitInfo.point;
                _destPos.y = transform.position.y;
            }
        }

        Vector3 dir = _destPos - transform.position;

        if (dir.magnitude > 0.1f)
        {
            float moveDist = Mathf.Clamp(_speed * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10.0f * Time.deltaTime);
        }
    }
}
