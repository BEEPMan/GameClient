using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager
{
    public int PlayerId { get; set; }
    GameObject _player;
    Dictionary<int, GameObject> _players = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject> _ghosts = new Dictionary<int, GameObject>();

    public const int DUMMY_COUNT = 49;

    float _meanDistance = 0.0f;
    public int finishedSimulationCount = 0;

    public void OnUpdate()
    {
        if(finishedSimulationCount >= DUMMY_COUNT)
        {
            for(int i=1; i<=DUMMY_COUNT; i++)
            {
                _meanDistance += _ghosts[i].GetComponent<GhostController>().GetMeanDistance();
                WriteGhostCoordinate(i, _ghosts[i].GetComponent<GhostController>().coordinates);
            }
            for (int i = 1; i <= DUMMY_COUNT; i++)
            {
                WriteUserCoordinate(i, _players[i].GetComponent<UserController>().coordinates);
            }
            _meanDistance /= DUMMY_COUNT;
            WriteLog($"-,{_meanDistance},-,-", true);
            Debug.Log($"All simulations ended. Average Distance: {_meanDistance}");
            finishedSimulationCount = -1;
        }
    }

    public GameObject GetPlayer() { return _player; }

    public GameObject SpawnPlayer(int playerId, Vector3 position)
    {
        if(playerId == PlayerId)
        {
            _player = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/MyPlayer"), position, Quaternion.identity) as GameObject;
            _player.GetComponent<Stat>().ID = playerId;
            _players.Add(playerId, _player);
            return _player;
        }
        
        GameObject go = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Player"), position, Quaternion.identity) as GameObject;
        go.GetComponent<Stat>().ID = playerId;
        _players.Add(playerId, go);
        return go;
    }

    public void DespawnPlayer(int playerId)
    {
        if(_players.ContainsKey(playerId))
        {
            GameObject go = _players[playerId];
            _players.Remove(playerId);
            UnityEngine.Object.Destroy(go);
        }
    }

    public bool FindPlayer(int playerId)
    {
        return _players.ContainsKey(playerId);
    }

    public void MovePlayer(int playerId, Vector3 dest)
    {
        _players[playerId].transform.position = dest;
    }

    public bool isOverThreshold(int playerId, Vector3 position)
    {
        Vector3 currentPos = _players[playerId].transform.position;
        if(Vector3.Distance(currentPos, position) > 2.0f)
        {
            return true;
        }
        return false;
    }

    int count = 0;

    public void SetPlayerPosition(int playerId, Vector3 position)
    {
        _players[playerId].transform.position = position;
        count++;
    }

    public void SetPlayerVelocity(int playerId, Vector3 velocity)
    {
        float y = _players[playerId].GetComponent<Rigidbody>().velocity.y;
        _players[playerId].GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, y, velocity.z);
    }

    public void InterpolatePlayerPosition(int playerId, Vector3 position, Vector3 velocity, long delay)
    {
        //Vector3 diff = (position - _players[playerId].transform.position) * ((1000 + delay) / 250);
        Vector3 diff = (position - _players[playerId].transform.position) * 4;
        float y = _players[playerId].GetComponent<Rigidbody>().velocity.y;
        _players[playerId].GetComponent<Rigidbody>().velocity = velocity * (250 + delay) / 250 + new Vector3(diff.x, y, diff.z);
        //_players[playerId].GetComponent<Rigidbody>().velocity = velocity + new Vector3(diff.x, y, diff.z);
    }

    public float CalcDistance(int playerId)
    {
        if (!_players.ContainsKey(playerId) || !_ghosts.ContainsKey(playerId))
        {
            return 0.0f;
        }
        return Vector3.Distance(_players[playerId].transform.position, _ghosts[playerId].transform.position);
    }

    public float CalcSpeedDiff(int playerId)
    {
        if (!_players.ContainsKey(playerId) || !_ghosts.ContainsKey(playerId))
        {
            return 0.0f;
        }
        return Vector3.Distance(_players[playerId].GetComponent<Rigidbody>().velocity, _ghosts[playerId].GetComponent<Rigidbody>().velocity);
    }

    public void SimulateMoveEvents()
    {
        WriteLog("ID,MeanDistance,Variance,StandardDeviation", false);
        
        for (int i = 1; i <= DUMMY_COUNT; i++)
        {
            _ghosts[i].SetActive(true);
            _ghosts[i].transform.position = _players[i].transform.position;
            _ghosts[i].GetComponent<GhostController>().Simulate();
        }
    }

    public void CreateMoveEvents()
    {
        for (int i = 1; i <= DUMMY_COUNT; i++)
        {
            GameObject go = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GhostPlayer"), new Vector3(0, 1, 0), Quaternion.identity) as GameObject;
            go.GetComponent<Stat>().ID = i;
            _ghosts.Add(i, go);
            _ghosts[i].SetActive(false);
        }

        string path = Application.dataPath + "/Data/";

        for (int i = 1; i <= DUMMY_COUNT; i++)
        {
            //_ghosts[i].GetComponent<GhostController>()._moveEvents = ReadTimeline(path + $"Player_{i}.csv");
            _ghosts[i].GetComponent<GhostController>()._moveEvents = ReadTimeline($"Data/Player_{i}");
            _ghosts[i].GetComponent<Stat>().ID = i;
        }
    }

    public Queue<Define.MoveEvent> ReadTimeline(string path)
    {
        TextAsset textAsset = Resources.Load(path) as TextAsset;
        StringReader stringReader = new StringReader(textAsset.text);

        Queue<Define.MoveEvent> playerMoveEvents = new Queue<Define.MoveEvent>();

        string line;
        stringReader.ReadLine();
        while ((line = stringReader.ReadLine()) != null)
        {
            string[] split = line.Split(',');

            float time = float.Parse(split[0]);
            float startTime = float.Parse(split[1]);

            playerMoveEvents.Enqueue(new Define.MoveEvent()
            {
                startTime = startTime,
                time = time,
                velX = float.Parse(split[3]),
                velY = 0,
                velZ = float.Parse(split[4])
            });
        }
        stringReader.Close();

        //sr.ReadLine();
        //while (sr.EndOfStream == false)
        //{
        //    string line = sr.ReadLine();
        //    string[] split = line.Split(',');

        //    float time = float.Parse(split[0]);
        //    float startTime = float.Parse(split[1]);

        //    playerMoveEvents.Enqueue(new Define.MoveEvent()
        //    {
        //        startTime = startTime,
        //        time = time,
        //        velX = float.Parse(split[3]),
        //        velY = 0,
        //        velZ = float.Parse(split[4])
        //    });
        //}
        //sr.Close();

        return playerMoveEvents;
    }

    public void WriteLog(string msg, bool append)
    {
        string path = Application.dataPath + "/../../Log_v1.2";
        DirectoryInfo di = new DirectoryInfo(path);
        if(di.Exists == false)
        {
            di.Create();
        }
        using (StreamWriter sw = new StreamWriter(path + "/Statics.csv", append))
        {
            sw.WriteLine(msg);
        }
    }

    public void WriteGhostCoordinate(int playerId, Queue<Vector2> coordinates)
    {
        string path = Application.dataPath + "/../../Log_v1.2";

        using (StreamWriter sw = new StreamWriter(path + "/GhostCoordinates_" + playerId + ".csv", false))
        {
            sw.WriteLine("X,Z");
            while (coordinates.Count > 0)
            {
                Vector2 coord = coordinates.Dequeue();
                sw.WriteLine($"{coord.x},{coord.y}");
            }
        }
    }

    public void WriteUserCoordinate(int playerId, Queue<Vector2> coordinates)
    {
        string path = Application.dataPath + "/../../Log_v1.2";

        using (StreamWriter sw = new StreamWriter(path + "/UserCoordinates_" + playerId + ".csv", false))
        {
            sw.WriteLine("X,Z");
            while (coordinates.Count > 0)
            {
                Vector2 coord = coordinates.Dequeue();
                sw.WriteLine($"{coord.x},{coord.y}");
            }
        }
    }
}
