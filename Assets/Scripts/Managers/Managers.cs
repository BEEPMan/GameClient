using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    static Managers Instance { get { Init(); return s_instance; } }

    NetworkManager _network = new NetworkManager();
    PacketManager _packet = new PacketManager();
    InputManager _input = new InputManager();
    UIManager _ui = new UIManager();
    GameManager _game = new GameManager();

    public static NetworkManager Network { get { return Instance._network; } }
    public static PacketManager Packet { get { return Instance._packet; } }
    public static InputManager Input { get { return Instance._input; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static GameManager Game { get { return Instance._game; } }

    void Start()
    {
        Init();
        Game.CreateMoveEvents();
    }

    void Update()
    {
        _input.OnUpdate();
        _network.OnUpdate();
        _game.OnUpdate();

        // Debug.Log(_game.CalcSpeedDiff(4));
    }

    private void OnApplicationQuit()
    {
        _network.Disconnect();
    }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
        }
    }

    public static void Clear()
    {
        Input.Clear();
        UI.Clear();
    }
}
