using UnityEngine;

public class BuildingCompleteSFX : MonoBehaviour
{
    [Header("SFX")]
    public AudioClip buildCompleteSFX;
    public float volume = 1f;

    private bool hasPlayed = false;

    // Call this when the building is finished
    public void PlayCompleteSFX()
    {
        if (hasPlayed)
            return;

        hasPlayed = true;

        if (buildCompleteSFX != null)
            AudioSource.PlayClipAtPoint(buildCompleteSFX, transform.position, volume);
    }
}
