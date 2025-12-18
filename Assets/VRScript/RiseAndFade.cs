// RiseAndFade.cs
using UnityEngine;

public class RiseAndFade : MonoBehaviour
{
    public float riseSpeed = 0.2f;
    public float lifeTime = 8.0f;

    void Start()
    {
        // Destroy the text object after its lifetime ends
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Move the text upwards
        transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);
    }
}