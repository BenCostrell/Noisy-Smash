using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    [HideInInspector]
    public int playerNum;
    private Rigidbody2D rb;
    private GameObject punchPivot;
    private SpriteRenderer sr;
    private Collider2D col;

    public float baseAccel;
    private float accel;

    public float baseMaxSpeed;
    private float maxSpeed;

    public float aerialDriftFactor;

    public float baseJumpVelocity;
    private float jumpVelocity;

    public float punchCastDuration;
    public float punchActiveTime;
    public int punchBaseDamage;
    private int punchDamage;
    public float baseKnockback;
    [HideInInspector]
    public float knockback;
    public float stunFactor;
    public float dmgKnockbackRatio;

    public float deathHeight;

    public LayerMask groundLayer;

    public float noiseFactor;
    public float noiseSpeed;

    private Vector3 baseScale;
    private int doubleJumps;
    private bool grounded;
    private bool actionable;
    private float powerLevel;
    private float randomSeed;
    private int damage;

    [HideInInspector]
    public TaskManager taskManager;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        punchPivot = transform.GetChild(0).gameObject;
        baseScale = transform.localScale;
        DeactivatePunch();
        randomSeed = UnityEngine.Random.Range(0f, 1000f);
    }

    // Use this for initialization
    void Start () {
        taskManager = new TaskManager();
        Actionable();
	}

    // Update is called once per frame
    void Update()
    {
        CheckIfDead();
        taskManager.Update();
        UpdatePower();
        CheckIfGrounded();
        if (actionable)
        {
            Move();

            if (Input.GetButtonDown("A_P" + playerNum))
            {
                if (grounded || doubleJumps > 0)
                {
                    Jump();
                }
            }

            if (Input.GetButtonDown("B_P" + playerNum))
            {
                Punch();
            }
        }

	}

    void UpdatePower()
    {
        powerLevel = -1f + 2*Mathf.PerlinNoise(Time.time*noiseSpeed + (playerNum * 1000), randomSeed);
        transform.localScale = (1 + (noiseFactor * powerLevel)) * baseScale;
        knockback = (1 + (noiseFactor * powerLevel)) * baseKnockback;
        accel = (1 - (noiseFactor * powerLevel)) * baseAccel;
        maxSpeed = (1 - (noiseFactor * powerLevel)) * baseMaxSpeed;
        jumpVelocity = (1 - (noiseFactor * powerLevel)) * baseJumpVelocity;
        punchDamage = Mathf.CeilToInt((1 + (noiseFactor * powerLevel)) * punchBaseDamage);

    }

    void Move()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal_P" + playerNum), 0);
        
        if (rb.velocity.magnitude > maxSpeed && grounded)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        else
        {
            if (grounded)
            {
                rb.AddForce(accel * inputVector);
            }
            else
            {
                rb.AddForce(accel * aerialDriftFactor * inputVector);
            }
        }
    }

    void Jump()
    {
        if (!grounded)
        {
            doubleJumps -= 1;
        }
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        grounded = false;
    }

    void Punch()
    {
        taskManager.AddTask(new PunchTask(this, punchCastDuration, punchActiveTime));
    }

    void CheckIfGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, col.bounds.extents.y, groundLayer);
        if (hit){
            grounded = true;
            doubleJumps = 1;
            if (!actionable)
            {
                Bounce(hit.normal);
            }
        }
        else
        {
            grounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        GameObject obj = col.gameObject;
        if (obj.tag == "Punch")
        {
            float rot = obj.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            Player player = obj.GetComponentInParent<Player>();
            if (player != this)
            {
                GetHit(player, rot);
            }
        }
    }

    public void Unactionable()
    {
        actionable = false;
    }

    public void Actionable()
    {
        actionable = true;
    }

    public void ActivatePunch()
    {
        punchPivot.SetActive(true);
        punchPivot.transform.localRotation =
            Quaternion.Euler(Mathf.Atan2(Input.GetAxis("Vertical_P" + playerNum), Input.GetAxis("Horizontal_P" + playerNum)) 
            * Mathf.Rad2Deg * Vector3.forward);
    }

    public void DeactivatePunch()
    {
        punchPivot.SetActive(false);
    }

    public void GetHit(Player enemy, float rot)
    {
        damage += enemy.punchDamage;
        UpdateDamageUI();
        float kb = enemy.knockback * (1 + (damage * dmgKnockbackRatio));
        rb.velocity = kb * new Vector3(Mathf.Cos(rot), Mathf.Sin(rot), 0);
        DeactivatePunch();
        taskManager.Clear();
        taskManager.AddTask(new StunTask(this, stunFactor * kb));
        CheckIfGrounded();
    }

    void Bounce(Vector2 normal)
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity = Vector2.Reflect(rb.velocity, normal);
        }
    }

    void UpdateDamageUI()
    {
        Services.GameManager.playerDamageUI[playerNum - 1].GetComponent<Text>().text = damage + "%";
    }

    void CheckIfDead()
    {
        if (transform.position.y < deathHeight) Die();
    }

    void Die()
    {
        Services.GameManager.GameOver(this);
    }
}

