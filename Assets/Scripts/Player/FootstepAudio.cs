using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FootstepAudio : MonoBehaviour
{
    [Header("Footstep Sounds")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float runStepInterval = 0.3f;
    [SerializeField] private float velocityThreshold = 2.0f;

    private CharacterController characterController;
    private float nextStepTime;
    private int lastPlayedIndex = -1;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleFootsteps();
    }

    private void HandleFootsteps()
    {
        if (!characterController.isGrounded || characterController.velocity.magnitude < velocityThreshold)
            return;

        float currentInterval = characterController.velocity.magnitude > velocityThreshold ? runStepInterval : walkStepInterval;

        if (Time.time >= nextStepTime)
        {
            PlayFootstepSound();
            nextStepTime = Time.time + currentInterval;
        }
    }

    private void PlayFootstepSound()
    {
        if (footstepSounds.Length == 0) return;

        int randomIndex = Random.Range(0, footstepSounds.Length);
        if (randomIndex == lastPlayedIndex)
            randomIndex = (randomIndex + 1) % footstepSounds.Length;

        lastPlayedIndex = randomIndex;
        footstepSource.clip = footstepSounds[randomIndex];
        footstepSource.Play();
    }
}
