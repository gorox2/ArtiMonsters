using System.Collections;
using UnityEngine;

public class LevelCompletionUI : MonoBehaviour
{
    [SerializeField] private GameObject root; // panel root
    [SerializeField] float showTime = 2f;

    private void Awake()
    {
        if (root != null) root.SetActive(false);
    }

    public void Show()
    {
        if (root != null) root.SetActive(true);
        StartCoroutine(timer());
    }

    IEnumerator timer()
    {
        yield return new WaitForSeconds(showTime);
        Hide();
    }

    public void Hide()
    {
        if (root != null) root.SetActive(false);
    }
}
