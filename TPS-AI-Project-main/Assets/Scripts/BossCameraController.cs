using UnityEngine;
using System.Collections;
using Unity.Cinemachine;


public class BossCameraController : MonoBehaviour
{
    public Transform player;
    public Transform boss;

    private CinemachineCamera cinemachineCam;
    private bool focusingBoss = false;

    void Start()
    {
        cinemachineCam = GetComponent<CinemachineCamera>();
    }

    public void FocusOnBoss(float duration = 3f)
    {
        if (focusingBoss || boss == null || player == null) return;
        StartCoroutine(FocusRoutine(duration));
    }

    IEnumerator FocusRoutine(float duration)
    {
        focusingBoss = true;

        // Temporarily make the camera look at the boss
        cinemachineCam.LookAt = boss;
        cinemachineCam.Follow = player;

        yield return new WaitForSeconds(duration);

        // Resume normal follow (look at player again)
        cinemachineCam.LookAt = player;
        focusingBoss = false;
    }

    // 🔹 Called when boss dies
    public void ReturnToPlayer()
    {
        cinemachineCam.LookAt = player;
        cinemachineCam.Follow = player;
        focusingBoss = false;
    }
}
