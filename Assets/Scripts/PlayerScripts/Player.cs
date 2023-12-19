using Cinemachine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour, IDamageable, IResourceGauge
{
    [Header("Player's Current Info")]
    public int curHealth;
    public int maxHealth;
    public float maxGauge;
    public float curGauge;
    [SerializeField] private float knockbackForce;
    public playerState curPlayerState;
    public bool isGrounded;
    public bool isFacingRight;
    public bool isUnstoppable;
    public bool isDashing;
    public bool isInvincible;
    public bool canMove;
    public bool canDash;
    public bool canAtk;

    [Header("Player's KeyBinds")]
    public KeyCode upKey = KeyCode.UpArrow;
    public KeyCode downKey = KeyCode.DownArrow;
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode jmpKey = KeyCode.Z;
    public KeyCode atkKey = KeyCode.X;
    public KeyCode dashKey = KeyCode.C;
    public KeyCode timeSlowKey = KeyCode.V;
    public KeyCode healKey = KeyCode.A;

    [Header("Ground Check")]
    public LayerMask groundLayers;
    private float checkGroundDist;
    [Tooltip("Buffer distance for Ground Check")][SerializeField] private float checkGroundBuffer;
    [HideInInspector] public CapsuleCollider2D playerCollider;
    [HideInInspector] public Rigidbody2D playerRb;
    [HideInInspector] public Animator playerAnimator;

    [Header("Player VC")]
    public CinemachineVirtualCamera playerVC;
    public float playerVCFOV;
    public float playerVCFOVShrunk;
    public float playerVCZoomDuration;


    // TODO: Get this in Player combat?
    [SerializeField] private float invincibleTime;

    // Events to trigger
    public event Action<int> onHitUI;
    public event Action<int> onRecoverUI;
    public event Action<float> onEnergySpentUI;
    public event Action<float> onEnergyRecoverUI;
    public static event Action<float> onPlayerHurt;

    // SFX
    private PlayerAudioManager playerAudioManager;
    public UnityEvent onHurt;

    public enum playerState
    {
        IDLE,               // has to be Grounded
        RUNNING,            // has to be Grounded
        AIRBORNE,
        FOCUS,              // has to be Grounded
        ATTACK,
        HURT,
        DEAD
    }
    private void Awake()
    {
        playerCollider = GetComponent<CapsuleCollider2D>();
        playerRb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerAudioManager = GetComponent<PlayerAudioManager>();
    }
    private void Start()
    {
        // Bool
        isFacingRight = true;
        isDashing = false;
        canMove = true;
        canAtk = true;
        canDash = true;

        // Init Values
        curHealth = maxHealth;
        curGauge = maxGauge;

        // Init Events
        onHurt.AddListener(playerAudioManager.PlayHurtSFX);

        // Get FOV of VC
        playerVCFOV = playerVC.m_Lens.OrthographicSize;
        playerVCFOVShrunk = playerVCFOV * 0.6f;

    }
    private void Update()
    {
        checkGrounded();
    }

    #region Ground Check
    void checkGrounded()
    {
        checkGroundDist = playerCollider.size.y * 0.5f + checkGroundBuffer;
        RaycastHit2D hit = Physics2D.CapsuleCast(transform.position, new Vector2(playerCollider.size.x - 1, playerCollider.size.y), CapsuleDirection2D.Vertical, 90f, -Vector2.up, checkGroundDist, groundLayers);
        if (hit)
        {
            isGrounded = true;
            playerAnimator.SetBool("isGrounded", true);
        }
        else
        {
            isGrounded = false;
            playerAnimator.SetBool("isGrounded", false);
        }
    }

    #endregion


    #region Gain Health
    public void gainHealth(int health)
    {
        if (curHealth > 0 && curHealth < maxHealth)
        {
            curHealth += health;
            onRecoverUI?.Invoke(health);
        }
    }
    #endregion

    #region IDamageable Methods
    public void takeDamage(int damage, Vector2? from = null)
    {
        if (!isInvincible)
        {
            // Invoke UI and SFX
            onHitUI?.Invoke(damage);
            onHurt.Invoke();
            // Start invincible timer
            StartCoroutine(BecomeInvincible());
            // Knock Player back a little from the position of attacker
            if (!isUnstoppable)
            {
                Vector2 attackPos = from ?? Vector2.zero;
                playerRb.AddForce(knockbackForce * attackPos, ForceMode2D.Impulse);
                // Send Freeze signal
                Player.onPlayerHurt?.Invoke(0);
            }
            if (curHealth > damage)
            {
            
                curHealth -= damage;
            }
            else
            {
                //Application.LoadLevel(Application.loadedLevel);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                //Destroy(gameObject);
            }
        }
        else
        {

        }
    }

    
    private IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        yield return new WaitForSecondsRealtime(invincibleTime);
        isInvincible = false;
    }
    #endregion

    #region IResourceGauge Methods
    public void gainResource(float gain)
    {
        curGauge += gain;
        // Convert gain to percentage
        float gainPerc = gain / maxGauge;
        onEnergyRecoverUI?.Invoke(gainPerc);
    }

    public void useResource(float amount)
    {
        curGauge -= amount;
        // Convert amount to percentage
        float amountPerc = amount / maxGauge;
        onEnergySpentUI?.Invoke(amountPerc);
    }
    #endregion
}
