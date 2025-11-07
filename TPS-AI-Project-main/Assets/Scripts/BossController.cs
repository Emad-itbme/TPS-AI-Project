using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;

public class BossController : EnemyController
{
    [Header("Boss Settings")]
    public int bossScoreReward = 1000;        // higher reward
    public float bossAttackRadius = 25f;      // larger detection range
    public float bossAttackCooldown = 2f;     // faster attacks
    public float bossDeathAnimationDuration = 6f;
    public AudioClip bossRoarSound;           // intro roar
    public AudioClip bossDeathSound;          // death sound

    private bool hasRoared = false;

    protected new void Start()
    {
        base.Start();

        // Override EnemyController defaults with boss settings
        attackRaduis = bossAttackRadius;
        deathAnimationDuration = bossDeathAnimationDuration;

        // Optional: shorter cooldown for more aggressive behavior
        typeof(EnemyController)
            .GetField("attackCooldown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(this, bossAttackCooldown);

        // Play roar once when spawned
        if (!hasRoared && bossRoarSound != null && LevelManager.instance != null)
        {
            hasRoared = true;
            LevelManager.instance.PlaySound(bossRoarSound, transform.position);
        }

        // Boost stats
        if (stats != null)
        {
            stats.maxHealth = 100f;  // 5× tougher
            stats.power *= 3f;      // 3× stronger
            stats.ChangeHealth(stats.maxHealth - stats.currentHealth);
        }
    }

    public override void PlayDeathAnimation()
    {
        if (isDead) return;
        isDead = true;

        agent.isStopped = true;
        agent.enabled = false;

        anim.ResetTrigger("Attack");
        anim.SetTrigger("Death");

        // Stop the animator from blending back to idle
        anim.applyRootMotion = false;
        anim.updateMode = AnimatorUpdateMode.Normal;
        anim.SetBool("IsDead", true);

        if (LevelManager.instance != null && LevelManager.instance.bossDeathSound != null)
            LevelManager.instance.PlaySound(LevelManager.instance.bossDeathSound, transform.position);

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        StartCoroutine(DestroyAfterBossDeath());
    }

    IEnumerator DestroyAfterBossDeath()
    {
        // Return camera focus, play death anim, etc...
        var camController = Object.FindObjectOfType<BossCameraController>();
        if (camController != null) camController.ReturnToPlayer();

        // Wait for boss death animation to finish
        yield return new WaitForSeconds(bossDeathAnimationDuration);

        // ✅ Tell LevelManager that one enemy (the boss) died
        if (LevelManager.instance != null)
        {
            Debug.Log("Boss defeated! Notifying LevelManager.OnEnemyDied.");
            LevelManager.instance.OnEnemyDied(gameObject);
        }

        // Give LM one frame to process the list update, then destroy
        yield return null;
        Destroy(gameObject);
    }
}
