using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPoint : MonoBehaviour
    {
    public string nextSceneName; // Assign the name of the next scene in the inspector

    private void OnCollisionEnter(Collision collision)
        {
        if (collision.gameObject.CompareTag("Car")) // Ensure your car has the "Car" tag
            {
            SceneManager.LoadScene(nextSceneName);
            }
        }
    }
