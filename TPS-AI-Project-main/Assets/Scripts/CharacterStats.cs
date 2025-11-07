using System;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class CharacterStats : MonoBehaviour
{

    public static bool playerIsDead = false;

    [SerializeField]
    public float maxHealth = 100;
    public float power = 10;
  
    public float currentHealth { get; protected set; }
    private bool isDead = false;
    private Animator animator;
    private CharacterMovement charMove;
    private Transform rootT;
    public AudioClip[] damageSounds;



    void Awake()
    {
        rootT = transform.root;                  // use the root for tag checks
        animator = GetComponentInChildren<Animator>(); // player often has Animator on "GFX"
        charMove = GetComponentInParent<CharacterMovement>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (LevelManager.instance.playerStartSound != null)
            LevelManager.instance.PlaySound(LevelManager.instance.playerStartSound, transform.position);

    }

    public void ChangeHealth(float value)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);
        Debug.Log("Current Health: " + currentHealth + "/" + maxHealth);

     
        if (transform.CompareTag("Enemy"))
        {
            transform.Find("Canvas").GetChild(0).GetComponent<UnityEngine.UI.Image>().fillAmount = currentHealth / maxHealth;
        }
        else if (transform.CompareTag("Player"))
        {
            LevelManager.instance.MainCanvas.Find("PnlStats").Find("ImgHealthBar").GetComponent<UnityEngine.UI.Image>().fillAmount = currentHealth / maxHealth;
            LevelManager.instance.MainCanvas.Find("PnlStats").Find("TxtHealth").GetComponent<TextMeshProUGUI>().text =
                string.Format("{0:0.##}", (currentHealth / maxHealth) * 100);
        }

        if (value < 0) // only play when taking damage
        {
            if (transform.CompareTag("Player"))
            {
            
                if (LevelManager.instance != null && LevelManager.instance.playerDamageSounds != null && LevelManager.instance.playerDamageSounds.Length > 0)
                {
                    AudioClip randomClip = LevelManager.instance.playerDamageSounds[
                       UnityEngine.Random.Range(0, LevelManager.instance.playerDamageSounds.Length)
                    ];
                    LevelManager.instance.PlaySound(randomClip, transform.position);
                }
            }
            else if (transform.CompareTag("Enemy"))
            {
               
                if (LevelManager.instance != null && LevelManager.instance.enemyDamageSounds != null && LevelManager.instance.enemyDamageSounds.Length > 0)
                {
                    AudioClip randomClip = LevelManager.instance.enemyDamageSounds[
                        UnityEngine.Random.Range(0, LevelManager.instance.enemyDamageSounds.Length)
                    ];
                    LevelManager.instance.PlaySound(randomClip, transform.position);
                }
            }
        }

        if (currentHealth <= 0f)
            Die();
    }
    void Die()
    {
        if (isDead) return;
        isDead = true;

        bool isPlayer = rootT.CompareTag("Player");
        bool isEnemy = rootT.CompareTag("Enemy");
        bool isBoss = rootT.GetComponent<BossController>() != null;

        Debug.Log($"Die() on '{name}'. isPlayer={isPlayer}, isEnemy={isEnemy}, isBoss={isBoss}");

        if (isPlayer)
        {
            playerIsDead = true;
            if (charMove != null) charMove.enabled = false;

            if (animator != null)
            {
                animator.ResetTrigger("Die");
                animator.SetTrigger("Die");
                Debug.Log("Player death trigger 'Die' sent to Animator.");
            }
            if (LevelManager.instance != null)
                LevelManager.instance.ShowDefeatScreen();

            return;
        }
        else if (isBoss)
        {
            //  Boss-specific handling
            BossController boss = rootT.GetComponent<BossController>();
            if (boss != null)
            {
                boss.PlayDeathAnimation();
                boss.enabled = false;
            }

            // Notify LevelManager so it can end boss fight
            LevelManager.instance.OnEnemyDied(rootT.gameObject);
           
        }
        else if (isEnemy)
        {
            //  Normal enemies
            EnemyController enemy = GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.PlayDeathAnimation();
            }

            // Score + notify LevelManager
            LevelManager.instance.AddScore(100);
            LevelManager.instance.OnEnemyDied(rootT.gameObject);
        }
    }
    public void PlayAttackSound()
    {
        if (LevelManager.instance == null)
        {
            Debug.LogWarning("LevelManager instance missing!");
            return;
        }

        if (LevelManager.instance.levelSounds != null && LevelManager.instance.levelSounds.Length > 1)
        {
            AudioClip attackClip = LevelManager.instance.levelSounds[1];
            if (attackClip != null)
                LevelManager.instance.PlaySound(attackClip, transform.position);
        }
        else
        {
            Debug.LogWarning("Attack sound (levelSounds[1]) not assigned in LevelManager!");
        }
    }

    public void PlayDamageSound()
    {
        if (damageSounds == null || damageSounds.Length == 0) return;

        AudioClip clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
        LevelManager.instance.PlaySound(clip, transform.position);
    }
}