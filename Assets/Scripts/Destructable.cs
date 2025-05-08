using UnityEngine;

public class Destructable : Entity
{
    [Header("Destructable Attributes")]
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
        // Instatiate effects and destroy when done
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

        base.Die();
    }
}
