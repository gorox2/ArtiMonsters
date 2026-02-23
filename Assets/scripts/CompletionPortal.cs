using UnityEngine;
using UnityEngine.SceneManagement;

public class CompletionPortal : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private InteractPromptUI promptUI;

    private bool playerInside;
    private int promptToken;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

  
    void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(interactKey))
        {

            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInside = true;
        if (promptUI != null)
            promptToken = promptUI.Show("Enter Portal [E]");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInside = false;
        if (promptUI != null) promptUI.Hide(promptToken);
    }
}
