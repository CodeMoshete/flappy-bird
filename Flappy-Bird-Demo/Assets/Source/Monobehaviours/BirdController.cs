using UnityEngine;

public class BirdController : MonoBehaviour
{
    private readonly Vector3 START_POS = new Vector3(0f, 1.6f, 2f);
    private const float FLAP_FRAME_TIME = 0.25f;

    public Sprite GlideSprite;
    public Sprite FlapSprite;

    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private float flapFrameCountdown;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.simulated = false;

        spriteRenderer = GetComponent<SpriteRenderer>();

        Service.EventManager.AddListener(EventId.StartGame, OnGameStart);
        Service.EventManager.AddListener(EventId.GameOver, OnGameOver);
        Service.EventManager.AddListener(EventId.FlapPressed, OnFlapPressed);
    }

    private bool OnGameStart(object cookie)
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.simulated = true;
        return false;
    }

    private bool OnGameOver(object cookie)
    {
        rigidBody.simulated = false;
        transform.position = START_POS;
        spriteRenderer.sprite = GlideSprite;
        return false;
    }

    private bool OnFlapPressed(object cookie)
    {
        Vector3 velocity = rigidBody.velocity;
        velocity.y = 5f;
        rigidBody.velocity = velocity;
        spriteRenderer.sprite = FlapSprite;
        flapFrameCountdown = FLAP_FRAME_TIME;
        return true;
    }

    private void Update()
    {
        if (rigidBody.simulated)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Service.EventManager.SendEvent(EventId.FlapPressed, null);
            }

            if (flapFrameCountdown > 0f)
            {
                flapFrameCountdown -= Time.deltaTime;
            
                if (flapFrameCountdown <= 0f)
                {
                    spriteRenderer.sprite = GlideSprite;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Pipe")
        {
            Service.EventManager.SendEvent(EventId.GameOver, null);
        }
        else if (collision.gameObject.tag == "Gate")
        {
            Service.EventManager.SendEvent(EventId.GatePassed, null);
        }
    }
}
