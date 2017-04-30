using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public Player[] players;
    public Vector3[] playerSpawns;
    public Color[] playerColors;

	void Awake()
    {
        InitializeServices();
    }
    
    // Use this for initialization
	void Start () {
        InitializePlayers();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Reset"))
        {
            Reset();
        }
	}

    void InitializeServices()
    {
        Services.GameManager = this;
        Services.EventManager = new EventManager();
        Services.TaskManager = new TaskManager();
        Services.Prefabs = Resources.Load<PrefabDB>("Prefabs/Prefabs");
    }

    void InitializePlayers()
    {
        players = new Player[2]
        {
            InitializePlayer(1),
            InitializePlayer(2)
        };
    }

    Player InitializePlayer(int playerNum)
    {
        GameObject playerObj = Instantiate(Services.Prefabs.Player, playerSpawns[playerNum - 1], Quaternion.identity);
        Player player = playerObj.GetComponent<Player>();
        player.playerNum = playerNum;
        player.GetComponent<SpriteRenderer>().color = playerColors[playerNum - 1];
        return player;
    }

    void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
