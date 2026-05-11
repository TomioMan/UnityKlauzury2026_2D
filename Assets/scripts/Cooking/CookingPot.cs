using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem; // Added for Mouse input

public class CookingPot : MonoBehaviour
{
    [System.Serializable]
    public struct IngredientRequirement
    {
        public ItemData item;
        public int amount;
    }

    [System.Serializable]
    public struct Recipe
    {
        public string recipeName;
        public List<IngredientRequirement> requirements;
        public ItemData resultItem;
    }

    [Header("STIRRING SETTINGS")]
    public float stirRequired = 20f;
    private float currentStirAmount = 0f;
    private Vector3 lastSpoonPos;

    [Header("INGREDIENTS")]
    public List<ItemData> addedIngredients = new List<ItemData>();

    [Header("RECIPES")]
    public List<Recipe> recipes = new List<Recipe>();
    public Transform spawnPoint;

    [Header("DUMP SETTINGS")]
    public Transform dumpPoint;
    public float dumpClickRadius = 0.5f;

    [Header("SPAWN PHYSICS")]
    public Vector2 resultLaunchForce = new Vector2(0, 5f);
    public float collisionDisableTime = 0.5f;
    public float dumpDelay = 0.25f;

    [Header("SOUNDS")]
    public AudioSource sourceBubbling;
    public AudioClip bubblingSound;
    public AudioSource sourceSplash;
    public List<AudioClip> splashClips;
    public AudioSource sourceStir;
    public AudioClip stirSound;
    public AudioSource sourceResult;
    public AudioClip positiveResultSound;
    public AudioClip negativeResultSound;

    [Header("ALER SYSTEM")]
    public AlertSystem alertSystem;

    private float nextActionTime = 0f;
    private float cooldownDuration = 0.25f;

    void Start()
    {
        if (sourceBubbling != null && bubblingSound != null)
        {
            sourceBubbling.clip = bubblingSound;
            sourceBubbling.loop = true;
        }
    }

    void Update()
    {
        // 1. Handle Bubbling Sound Logic
        if (addedIngredients.Count > 0)
        {
            if (!sourceBubbling.isPlaying) sourceBubbling.Play();
        }
        else
        {
            if (sourceBubbling.isPlaying) sourceBubbling.Stop();
        }

        // 2. Handle Click-to-Dump Logic
        if (Mouse.current.leftButton.wasPressedThisFrame && dumpPoint != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0;

            float distance = Vector2.Distance(mousePos, dumpPoint.position);
            if (distance <= dumpClickRadius)
            {
                DumpIngredients();
            }
        }
    }

    public void DumpIngredients()
    {
        // Only dump if there's actually stuff in the pot
        if (addedIngredients.Count > 0)
        {
            StartCoroutine(DumpRoutine());
        }
    }

    private IEnumerator DumpRoutine()
    {
        List<ItemData> itemsToDump = new List<ItemData>(addedIngredients);
        addedIngredients.Clear();
        currentStirAmount = 0;

        foreach (ItemData data in itemsToDump)
        {
            if (data.itemPrefab != null)
            {
                GameObject spawned = Instantiate(data.itemPrefab, spawnPoint.position, Quaternion.identity);

                if (spawned.TryGetComponent<Rigidbody2D>(out var rb))
                {
                    rb.linearVelocity = new Vector2(resultLaunchForce.x, resultLaunchForce.y);
                }

                StartCoroutine(TemporaryDisableCollision(spawned));
            }

            yield return new WaitForSeconds(dumpDelay);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Ingredient item = other.GetComponent<Ingredient>();
        if (item != null)
        {
            PlayRandomSplash();
            addedIngredients.Add(item.ingredientData);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("MixingSpoon")) lastSpoonPos = other.transform.position;
    }

    private void PlayRandomSplash()
    {
        if (sourceSplash != null && splashClips.Count > 0)
        {
            sourceSplash.PlayOneShot(splashClips[Random.Range(0, splashClips.Count)]);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("MixingSpoon") && addedIngredients.Count > 0)
        {
            float movement = Vector3.Distance(other.transform.position, lastSpoonPos);
            if (movement > 0.1f)
            {
                currentStirAmount += movement;
                TryPlayingStirSound();
                if (currentStirAmount >= stirRequired) FinishCooking();
            }
            lastSpoonPos = other.transform.position;
        }
    }

    void TryPlayingStirSound()
    {
        if (Time.time >= nextActionTime)
        {
            if (sourceStir != null && stirSound != null)
            {
                sourceStir.pitch = Random.Range(0.9f, 1.1f);
                sourceStir.PlayOneShot(stirSound);
            }
            nextActionTime = Time.time + cooldownDuration;
        }
    }

    void FinishCooking()
    {
        Recipe matchedRecipe = default;
        bool foundMatch = false;

        foreach (Recipe recipe in recipes)
        {
            if (CheckRecipeMatch(recipe))
            {
                matchedRecipe = recipe;
                foundMatch = true;
                break;
            }
        }

        if (foundMatch)
        {
            sourceResult.PlayOneShot(positiveResultSound);
            SpawnResult(matchedRecipe.resultItem);

        }
        else
        {
            sourceResult.PlayOneShot(negativeResultSound);
            DumpIngredients();
            alertSystem.ShowAlert("Invalid recipe! Try something else.");
        }

        currentStirAmount = 0;
        addedIngredients.Clear();
    }

    private bool CheckRecipeMatch(Recipe recipe)
    {
        int totalRequired = 0;
        foreach (var req in recipe.requirements) totalRequired += req.amount;
        if (addedIngredients.Count != totalRequired) return false;

        foreach (var req in recipe.requirements)
        {
            int count = 0;
            foreach (ItemData i in addedIngredients) if (i == req.item) count++;
            if (count != req.amount) return false;
        }
        return true;
    }

    private void SpawnResult(ItemData result)
    {
        if (result != null && result.itemPrefab != null && spawnPoint != null)
        {
            GameObject spawnedItem = Instantiate(result.itemPrefab, spawnPoint.position, Quaternion.identity);
            if (spawnedItem.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.linearVelocity = resultLaunchForce;
            }
            StartCoroutine(TemporaryDisableCollision(spawnedItem));
        }
    }

    private IEnumerator TemporaryDisableCollision(GameObject target)
    {
        if (target == null) yield break;
        Collider2D[] colliders = target.GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders) col.enabled = false;
        yield return new WaitForSeconds(collisionDisableTime);
        if (target != null)
        {
            foreach (var col in colliders) col.enabled = true;
        }
    }

    // This helps you see where the click zone is in the Scene view
    private void OnDrawGizmosSelected()
    {
        if (dumpPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(dumpPoint.position, dumpClickRadius);
        }
    }
}