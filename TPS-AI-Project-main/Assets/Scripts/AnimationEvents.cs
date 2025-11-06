using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public CharacterMovement charMove;

    [Header("Footstep Audio")]
    public AudioClip[] footstepSounds;
    public AudioSource audioSource;
    [Range(0f, 1f)] public float footstepVolume = 0.5f;
    public float pitchVariation = 0.1f;

    [Header("Footstep Effects (Optional)")]
    public GameObject footstepParticlePrefab;
    public Transform leftFootTransform;
    public Transform rightFootTransform;

    void Start()
    {
        // Create audio source if not assigned
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.playOnAwake = false;
        }
    }

    public void PlayerAttack()
    {
        Debug.Log("Player Attacked!");
        charMove.DoAttack();
    }

    // Called by animation event - generic footstep
    public void OnFootstep()
    {
        // Only play footstep if grounded (stops sound when jumping)
        if (charMove.isGrounded)
        {
            PlayFootstepSound();
        }
    }

    // Called by animation event - left foot
    public void OnLeftFootstep()
    {
        if (charMove.isGrounded)
        {
            PlayFootstepSound();
            if (leftFootTransform != null)
                SpawnFootstepEffect(leftFootTransform.position);
        }
    }

    // Called by animation event - right foot
    public void OnRightFootstep()
    {
        if (charMove.isGrounded)
        {
            PlayFootstepSound();
            if (rightFootTransform != null)
                SpawnFootstepEffect(rightFootTransform.position);
        }
    }

    void PlayFootstepSound()
    {
        if (footstepSounds == null || footstepSounds.Length == 0 || audioSource == null)
            return;

        // Pick random sound
        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];

        // Add pitch variation for realism
        audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);

        // Play sound
        audioSource.PlayOneShot(clip, footstepVolume);
    }   

    void SpawnFootstepEffect(Vector3 position)
    {
        if (footstepParticlePrefab == null) return;

        // Raycast down to find ground position
        if (Physics.Raycast(position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 1f))
        {
            GameObject effect = Instantiate(footstepParticlePrefab, hit.point, Quaternion.identity);
            Destroy(effect, 2f); // Auto cleanup after 2 seconds
        }
    }
    public void PlayerDamage()
    {
        transform.GetComponentInParent <EnemyController>().DamagePlayer();
    }
}