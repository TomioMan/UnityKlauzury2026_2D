using UnityEngine;
using System.Collections;

public class FloorFader : MonoBehaviour
{
    private SpriteRenderer[] sprites;
    private Collider2D[] colliders; // Store all colliders here

    void Awake()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
        // Grab every collider on this floor and its children
        colliders = GetComponentsInChildren<Collider2D>();
    }

    public void SetCollisions(bool enabled)
    {
        foreach (var col in colliders)
        {
            if (col == null) continue;
            col.enabled = enabled;
        }
    }

    public void SetAlpha(float alpha)
    {
        foreach (var s in sprites)
        {
            // The NULL CHECK: Skip this sprite if it was destroyed
            if (s == null) continue;

            Color c = s.color;
            c.a = alpha;
            s.color = c;
        }
    }

    public IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        // kill collisions immediately
        if (targetAlpha <= 0)
        {
            SetCollisions(false);
        }
        // enable collisions so we don't fall through
        else
        {
            SetCollisions(true);
        }

        float startAlpha = (sprites.Length > 0) ? sprites[0].color.a : 0;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float lerpAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            SetAlpha(lerpAlpha);
            yield return null;
        }

        SetAlpha(targetAlpha);

        if (targetAlpha <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}