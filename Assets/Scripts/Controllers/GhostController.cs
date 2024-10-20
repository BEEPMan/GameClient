using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    private float _speed = 10.0f;

    public Queue<Define.MoveEvent> _moveEvents = new Queue<Define.MoveEvent>();

    public Queue<Vector2> coordinates = new Queue<Vector2>();

    Stat _stat;

    private float _totalDistance = 0.0f;
    private float _meanDistance = 0.0f;
    private float _M2 = 0.0f;
    private int _count = 0;
    private float _remainDelay = 0.0f;

    bool _firstFlag = true;

    void Start()
    {
        _stat = GetComponent<Stat>();

        if (gameObject.GetComponentInChildren<UI_PlayerName>() == null)
        {
            Managers.UI.MakeWorldSpaceUI<UI_PlayerName>(transform);
        }
    }

    void FixedUpdate()
    {
        if (_stat.ID != 0)
        {
            coordinates.Enqueue(new Vector2(transform.position.x, transform.position.z));
            float dist = Managers.Game.CalcDistance(_stat.ID);
            _count++;
            _meanDistance += (dist - _meanDistance) / _count;
            _M2 += (dist - _meanDistance) * (dist - _meanDistance);
        }
    }

    public float GetTotalDistance()
    {
        return _totalDistance;
    }

    public int GetCount()
    {
        return _count;
    }

    public float GetMeanDistance()
    {
        return _meanDistance;
    }

    public float GetVariance()
    {
        return _M2 / (_count - 1);
    }

    public float GetStandardDeviation()
    {
        return Mathf.Sqrt(_M2 / (_count - 1));
    }

    public void Simulate()
    {
        _remainDelay = Managers.Network.Delay / 1000.0f;
        StartCoroutine(Move());
    }

    public IEnumerator Move()
    {
        if (_moveEvents.Count == 0)
        {
            Managers.Game.WriteLog($"{_stat.ID},{_meanDistance},{GetVariance()},{GetStandardDeviation()}", true);
            Debug.Log($"Player {_stat.ID} MoveEvents ended. Average Distance: {_meanDistance}, Variance: {GetVariance()}, Standard Deviation: {GetStandardDeviation()}");
            Managers.Game.finishedSimulationCount++;
            yield break;
        }
        if(_firstFlag)
        {
            // Debug.Log(DateTime.Now.Millisecond);
            _firstFlag = false;
        }
        Define.MoveEvent moveEvent = _moveEvents.Dequeue();
        if(moveEvent.time < _remainDelay)
        {
            transform.position = transform.position + new Vector3(moveEvent.velX, 0.0f, moveEvent.velZ) * _speed * moveEvent.time;
            _remainDelay = _remainDelay - moveEvent.time;
        }
        else
        {
            transform.position = transform.position + new Vector3(moveEvent.velX, 0.0f, moveEvent.velZ) * _speed * _remainDelay;
            float y = GetComponent<Rigidbody>().velocity.y;
            GetComponent<Rigidbody>().velocity = new Vector3(moveEvent.velX * _speed, y, moveEvent.velZ * _speed);
            yield return new WaitForSeconds(moveEvent.time - _remainDelay);
            _remainDelay = 0.0f;
        }
        StartCoroutine(Move());
    }
}
