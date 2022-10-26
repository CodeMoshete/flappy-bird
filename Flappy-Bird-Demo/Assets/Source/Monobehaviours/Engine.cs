using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Engine : MonoBehaviour
{
    private enum GameState
    {
        Menu,
        Playing
    }

    private const int MAX_PIPES = 3;

    public float WORLD_MOVE_RATE = 1.75f;
    public float PIPE_SPAWN_TIME = 2.5f;

    public GameObject PipePrefab;
    public GameObject StartButton;
    public Text ScoreText;
    public Button FlapButton;

    public List<Transform> GroundPool;

    private GameState currentGameState;
    private int currentScore;
    private float pipeSpawnCountdown;

    private List<Transform> PipePool = new List<Transform>();
    private int numPipes;


    private void Start()
    {
        Service.EventManager.AddListener(EventId.StartGame, OnGameStart);
        Service.EventManager.AddListener(EventId.GameOver, OnGameOver);
        Service.EventManager.AddListener(EventId.GatePassed, IncreaseScore);
    }
    
    private bool OnGameStart(object cookie)
    {
        currentScore = 0;
        ScoreText.text = currentScore.ToString();
        StartButton.SetActive(false);
        FlapButton.gameObject.SetActive(true);
        numPipes = 0;
        currentGameState = GameState.Playing;
        return false;
    }

    private bool OnGameOver(object cookie)
    {
        StartButton.SetActive(true);
        FlapButton.gameObject.SetActive(false);
        currentGameState = GameState.Menu;

        for (int i = 0, count = PipePool.Count; i < count; ++i)
        {
            Destroy(PipePool[i].gameObject);
        }
        PipePool.Clear();

        return false;
    }

    private bool IncreaseScore(object cookie)
    {
        currentScore++;
        ScoreText.text = currentScore.ToString();
        return true;
    }

    private void Update()
    {
        if (currentGameState == GameState.Playing)
        {
            float dt = Time.deltaTime;
            pipeSpawnCountdown -= dt;
            if (pipeSpawnCountdown <= 0f)
            {
                Transform newPipe;
                if (numPipes < MAX_PIPES)
                {
                    newPipe = Instantiate(PipePrefab).transform;
                    numPipes++;
                }
                else
                {
                    newPipe = PipePool[0];
                    PipePool.RemoveAt(0);
                }
                
                newPipe.position = new Vector3(4f, Random.Range(-2.5f, 2.5f), 1f);
                PipePool.Add(newPipe);
                pipeSpawnCountdown = PIPE_SPAWN_TIME;
            }


            for (int i = 0, count = PipePool.Count; i < count; ++i)
            {
                Vector3 currentPos = PipePool[i].position;
                currentPos.x -= (dt * WORLD_MOVE_RATE);
                PipePool[i].position = currentPos;
            }

            for (int i = 0, count = GroundPool.Count; i < count; ++i)
            {
                Vector3 currentPos = GroundPool[i].position;
                currentPos.x -= (dt * WORLD_MOVE_RATE);
                GroundPool[i].position = currentPos;
            }

            Transform leadingGround = GroundPool[0];
            if (leadingGround.position.x < -10f)
            {
                Vector3 endPosition = GroundPool[GroundPool.Count - 1].position;
                endPosition.x += 10f;
                leadingGround.position = endPosition;
                GroundPool.RemoveAt(0);
                GroundPool.Add(leadingGround);
            }
        }
    }
}
