using UnityEngine;

public class Destructible : Entity
{
    [Header("Destructible Attributes")]
    public bool dieOnTouch = true;
    public GameObject[] effects;
    public AudioClip[] sounds;
    private AudioSource audioSource;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (dieOnTouch && collision.gameObject.CompareTag("Player")) Die();
    }

    public override void Die()
    {
        PlayAllEffect();

        base.Die();
    }

    protected void PlayAllEffect()
    {
        // Instantiate effects and destroy when done
        foreach (GameObject effect in effects)
        {
            Destroy(Instantiate(effect, transform.position, transform.rotation), effect.GetComponent<ParticleSystem>().main.duration);
        }

        // If sounds exist just if there is a source
        if (sounds != null && audioSource == null)
        {
            try { audioSource = GetComponent<AudioSource>(); }
            catch { audioSource = gameObject.AddComponent<AudioSource>(); }
        }

        // Play sounds
        foreach (AudioClip clip in sounds)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
