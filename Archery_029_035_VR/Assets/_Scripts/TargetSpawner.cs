using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour, IHittable
{
    [SerializeField]
    private int health = 1;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private float repositionRadius = 2f; // Radius for repositioning the Spawner

    [SerializeField]
    private Difficulty difficulty = Difficulty.Easy; // Difficulty selection in Inspector

    private Transform spawnerTransform; // Reference to the Spawner parent
    private Vector3 originPosition; // Original position of the Spawner
    private GameObject lastArrow; // Store the last arrow that hit

    private void Awake()
    {
        // Get the Spawner transform (parent)
        spawnerTransform = transform.parent;
        if (spawnerTransform == null || spawnerTransform.name != "Spawner")
        {
            Debug.LogWarning("TargetSpawner requires a parent GameObject named 'Spawner'!");
        }

        // Store the original position of the Spawner
        originPosition = spawnerTransform != null ? spawnerTransform.position : transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arrow"))
        {
            Debug.Log("Collision detected with Arrow!");
            audioSource.Play();
            lastArrow = collision.gameObject; // Store the arrow
            GetHit(); // Call the public GetHit method
        }
    }

    public void GetHit()
    {
        health--;
        if (health <= 0)
        {
            ScoreManager.Instance?.AddScore(1);
            Debug.Log("Score updated, attempting to destroy arrow.");

            // Destroy the last arrow
            if (lastArrow != null)
            {
                Destroy(lastArrow);
                Debug.Log("Arrow should be destroyed!");
                lastArrow = null; // Clear the reference
            }

            // Rotate Spawner on Z-axis based on difficulty
            if (spawnerTransform != null)
            {
                float minRotation = 0f;
                float maxRotation = 0f;

                switch (difficulty)
                {
                    case Difficulty.Easy:
                        minRotation = 0f;
                        maxRotation = 45f;
                        break;
                    case Difficulty.Medium:
                        minRotation = 30f;
                        maxRotation = 60f;
                        break;
                    case Difficulty.Advanced:
                        minRotation = 45f;
                        maxRotation = 90f;
                        break;
                }

                float randomZRotation = Random.Range(minRotation, maxRotation);
                spawnerTransform.rotation = Quaternion.Euler(0f, 0f, randomZRotation);

                // Reposition Spawner to a new random position within radius
                Vector3 newPosition = originPosition + (Vector3)Random.insideUnitCircle * repositionRadius;
                spawnerTransform.position = newPosition;

                Debug.Log("Spawner rotated and repositioned!");
            }

            // Reset health to allow multiple hits
            health = 1;
        }
    }

    // Enum for difficulty selection in Inspector
    public enum Difficulty
    {
        Easy,
        Medium,
        Advanced
    }
}

public interface IHittable
{
    void GetHit();
}