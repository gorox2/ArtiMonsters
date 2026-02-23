using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class InteractPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject root;     
    [SerializeField] private TMP_Text promptText;

    private int showToken = 0;

    private void Awake()
    {
        Hide();
    }

    public int Show(string message)
    {
        showToken++;
        if (root != null) root.SetActive(true);
        if (promptText != null) promptText.text = message;
        return showToken;
    }

    public void Hide(int token)
    {
        if (token != showToken) return;

        if (promptText != null) promptText.text = "";
        if (root != null) root.SetActive(false);
    }

    public void Hide()
    {
        showToken++;
        if (promptText != null) promptText.text = "";
        if (root != null) root.SetActive(false);
    }
}
