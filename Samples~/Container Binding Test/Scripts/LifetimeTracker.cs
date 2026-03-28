using UnityEngine;

public class LifetimeTracker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"starting {gameObject.name}");
    }

    void OnDestroy()
    {
        Debug.Log($"destroying {gameObject.name}");
    }
}
