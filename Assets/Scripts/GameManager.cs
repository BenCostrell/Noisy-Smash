using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Player[] players;
    public Vector3[] playerSpawns;
    public Color[] playerColors;
    public GameObject[] playerDamageUI;
    public GameObject gameWinText;
    [HideInInspector]
    public bool fightActive;

	void Awake()
    {
        InitializeServices();
    }
    
    // Use this for initialization
	void Start () {
        InitializePlayers();
        fightActive = true;
        gameWinText.SetActive(false);
        Time.timeScale = 1;
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

    public void GameOver(Player losingPlayer)
    {
        Time.timeScale = 0;
        string winText;
        if (losingPlayer.playerNum == 1)
        {
            winText = "GREEN WINS";
        }
        else
        {
            winText = "BLUE WINS";
        }

        Text textComponent = gameWinText.GetComponent<Text>();
        textComponent.text = winText;
        textComponent.color = playerColors[2 - losingPlayer.playerNum];
        gameWinText.SetActive(true);
    }
}
