using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;

public class RoadGenerator : MonoBehaviour
{
    [Header("Road Pieces")]
    [SerializeField] private GameObject[] roadPrefabs;
    [SerializeField] private float scrollSpeed = 5f;
    [SerializeField] private int initialSegments = 7;

    private List<GameObject> activeSegments = new List<GameObject>();
    private float nextSpawnY = 0f;

    void Start()
    {
        // Start spawning from the current position of the manager
        nextSpawnY = transform.position.y;

        for (int i = 0; i < initialSegments; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        MoveRoad();

        // Check if the bottom-most segment is off screen
        if (activeSegments.Count > 0 && activeSegments[0].transform.position.y < -15f)
        {
            RemoveOldestSegment();
            SpawnSegment();
        }

        if (Keyboard.current.pKey.isPressed) ChangeSpeed(10f, 2.0f);
    }

    void MoveRoad()
    {
        foreach (GameObject segment in activeSegments)
        {
            segment.transform.position += Vector3.down * scrollSpeed * Time.deltaTime;
        }

        // The "virtual" spawn point also needs to move down with the road!
        nextSpawnY -= scrollSpeed * Time.deltaTime;
    }

    void SpawnSegment()
    {
        int randomIndex = Random.Range(0, roadPrefabs.Length);
        GameObject prefab = roadPrefabs[randomIndex];

        // 1. Calculate the height of the prefab before spawning
        // We use the SpriteRenderer bounds for this
        float currentHeight = prefab.GetComponent<SpriteRenderer>().bounds.size.y;

        // 2. Spawn it. 
        // We add half the height because Unity spawns from the CENTER of the sprite
        Vector3 spawnPos = new Vector3(0, nextSpawnY + (currentHeight / 2), 0);
        GameObject newSegment = Instantiate(prefab, spawnPos, Quaternion.identity);

        activeSegments.Add(newSegment);

        // 3. Set the spawn point for the NEXT piece at the TOP edge of this one
        nextSpawnY += currentHeight;
    }

    void RemoveOldestSegment()
    {
        GameObject oldest = activeSegments[0];
        activeSegments.RemoveAt(0);
        Destroy(oldest);
    }

    private Coroutine speedCoroutine; // To track if a transition is already happening

    // The public function you can call from other scripts
    public void ChangeSpeed(float targetSpeed, float duration)
    {
        // If we are already transitioning speed, stop it so they don't fight
        if (speedCoroutine != null) StopCoroutine(speedCoroutine);

        speedCoroutine = StartCoroutine(TransitionSpeed(targetSpeed, duration));
    }

    private IEnumerator TransitionSpeed(float targetSpeed, float duration)
    {
        float startSpeed = scrollSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Lerp (Linear Interpolation) calculates the value between 
            // start and end based on a percentage (0 to 1)
            float percentage = elapsed / duration;
            scrollSpeed = Mathf.Lerp(startSpeed, targetSpeed, percentage);

            yield return null; // Wait until the next frame
        }

        scrollSpeed = targetSpeed; // Ensure it's exactly the target speed at the end
    }
}