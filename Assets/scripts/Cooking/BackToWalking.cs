using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToWalking : MonoBehaviour
{
    // Capitalize the 'B' so it's easier for Unity to find
    public void GoBackToWalking()
    {
        SceneManager.LoadScene("Walking scene");
    }
}