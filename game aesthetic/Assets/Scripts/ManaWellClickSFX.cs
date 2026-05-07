using UnityEngine;

public class ManaWellClickSFX : MonoBehaviour
{
    public AudioClip clickSound;
    public float volume = 1f;

    private void OnMouseDown()
    {
        if (clickSound != null)
        {
            AudioSource.PlayClipAtPoint(clickSound, transform.position, volume);
        }
    }
}
