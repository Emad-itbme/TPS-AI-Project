using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
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
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.playOnAwake = false;
        }

        if (charMove == null)
        {
            charMove = GetComponentInParent<CharacterMovement>();
        }
    }

    // Called by PLAYER attack animation - this damages ENEMIES
    public void PlayerAttack()
    {
        Debug.Log("Player Attack Animation Event!");
        if (charMove != null)
        {
            charMove.DoAttack();
        }
        if (LevelManager.instance != null &&
           LevelManager.instance.levelSounds != null &&
           LevelManager.instance.levelSounds.Length > 1 &&
           LevelManager.instance.levelSounds[1] != null)
        {
            LevelManager.instance.PlaySound(LevelManager.instance.levelSounds[1], transform.position);
        }
        else
        {
            Debug.LogWarning("Attack sound (levelSounds[1]) missing in LevelManager!");
        }
    }
    public void PlayJumpSound()
    {
        if (LevelManager.instance == null)
        {
            Debug.LogWarning("LevelManager instance not found!");
            return;
        }

        // Example: use index 0 for jump sound
        AudioClip jumpClip = null;

        if (LevelManager.instance.levelSounds != null &&
            LevelManager.instance.levelSounds.Length > 0)
        {
            jumpClip = LevelManager.instance.levelSounds[0];
        }

        if (jumpClip != null)
        {
            LevelManager.instance.PlaySound(jumpClip, transform.position);
        }
        else
        {
            Debug.LogWarning("Jump sound not assigned in LevelManager.levelSounds!");
        }
    }
    public void OnFootstep()
    {
        if (charMove != null && charMove.isGrounded)
        {
            PlayFootstepSound();
        }
    }

    public void OnLeftFootstep()
    {
        if (charMove != null && charMove.isGrounded)
        {
            PlayFootstepSound();
            if (leftFootTransform != null)
                SpawnFootstepEffect(leftFootTransform.position);
        }
    }

    public void OnRightFootstep()
    {
        if (charMove != null && charMove.isGrounded)
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

        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        audioSource.PlayOneShot(clip, footstepVolume);
    }

    void SpawnFootstepEffect(Vector3 position)
    {
        if (footstepParticlePrefab == null) return;

        if (Physics.Raycast(position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 1f))
        {
            GameObject effect = Instantiate(footstepParticlePrefab, hit.point, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }
    public void PlaySwordSwingSound()
    {
        if (LevelManager.instance == null)
        {
            Debug.LogWarning("LevelManager instance missing!");
            return;
        }

        var sounds = LevelManager.instance.swordSwingSounds;
        if (sounds == null || sounds.Length == 0)
        {
            Debug.LogWarning("No sword swing sounds assigned in LevelManager!");
            return;
        }

        AudioClip clip = sounds[Random.Range(0, sounds.Length)];
        LevelManager.instance.PlaySound(clip, transform.position);
    }
}