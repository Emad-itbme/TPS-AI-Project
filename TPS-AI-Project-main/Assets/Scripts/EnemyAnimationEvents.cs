using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private EnemyController enemyController;





    void Start()
    {
        enemyController = GetComponentInParent<EnemyController>();


    }

    // Called by ENEMY attack animation - this damages the PLAYER
    public void DamagePlayer()
    {
        Debug.Log("Enemy Attack Animation Event!");
        if (enemyController != null)
        {
            enemyController.DamagePlayer();
        }
    }



}