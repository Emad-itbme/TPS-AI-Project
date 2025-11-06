
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
   //public Transform Player;
    protected CharacterStats stats;
    public float attackRaduis = 0;
    //ublic float attackCooldown = 1f;
    //public float rotationSpeed = 5f;
    public float deathAnimationDuration = 4f; // Adjust to match your animation length
    protected UnityEngine.AI.NavMeshAgent agent;
    protected Animator anim;
    protected bool canAttack = true;
    protected float attackCooldown = 3f;
    protected bool isDead = false;

    protected void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        stats = GetComponent<CharacterStats>();

        
    }

    void Update()
    {
        if (isDead) return; // Don't update if dead

        if (CharacterStats.playerIsDead)
        {
            // Stop attacking the player
            StopAttacking();
            return;
        }

        anim.SetFloat("Speed", agent.velocity.magnitude);
        float distance = Vector3.Distance(transform.position, LevelManager.instance.player.position);

        
       
        if (distance < attackRaduis)
        {

            agent.SetDestination(LevelManager.instance.player.position);
            if (distance <= agent.stoppingDistance)
            {
                if (canAttack)
                {
                    StartCoroutine(cooldown());
                    anim.SetTrigger("Attack");
                   
                }
            }
          
        }
    }
    IEnumerator cooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            

            stats.ChangeHealth(-other.GetComponentInParent<CharacterStats>().power);

        }
        
    }

    public void DamagePlayer()
    {
        if (!CharacterStats.playerIsDead)
        {
           
            LevelManager.instance.player.GetComponent<CharacterStats>().ChangeHealth(-stats.power);

            if (GetComponent<BossController>() != null)
            {
                if (LevelManager.instance != null && LevelManager.instance.bossHitPlayerSound != null)
                { LevelManager.instance.PlaySound(LevelManager.instance.bossHitPlayerSound, transform.position); }
            }
        }

    }
    public virtual void PlayDeathAnimation()
    {
        if (isDead) return; // Prevent multiple death calls

        isDead = true;

        // Stop the NavMeshAgent
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        // Play death animation
        if (anim != null)
        {
            anim.SetTrigger("Death");
        }
        // Disable collider so player doesn't collide with dead enemy
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Destroy the object after the animation finishes
        StartCoroutine(DestroyAfterAnimation());
    }
    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    void StopAttacking()
    {
        // Example: disable attack animation or behavior
        anim.ResetTrigger("Attack");
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }
    public void PlayDamageSound()
    {
        if (LevelManager.instance == null)
        {
            Debug.LogWarning("LevelManager instance missing!");
            return;
        }

        var sounds = LevelManager.instance.enemyDamageSounds;
        if (sounds == null || sounds.Length == 0)
        {
            Debug.LogWarning("Enemy damage sounds not assigned in LevelManager!");
            return;
        }

        // Pick random clip
        AudioClip randomClip = sounds[Random.Range(0, sounds.Length)];

        // Play it using LevelManager's pooling system
        LevelManager.instance.PlaySound(randomClip, transform.position);
    }
}