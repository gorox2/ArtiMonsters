using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class PickupFeedUI : MonoBehaviour
{
    public static PickupFeedUI Instance { get; private set; }

    [SerializeField] private GameObject root;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text text;
    [SerializeField] private float visibleDuration = 1.4f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;
        root.SetActive(false);
    }

    public void Show(ItemData item, int amount)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ShowRoutine(item, amount));
    }

    private IEnumerator ShowRoutine(ItemData item, int amount)
    {
        icon.sprite = item.icon;
        text.text = $"x{amount}  {item.itemName}";
        root.SetActive(true);

        yield return new WaitForSeconds(visibleDuration);
        root.SetActive(false);
    }
}
