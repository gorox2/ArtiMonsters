using UnityEngine;

public class InstructionsUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(true);
    }


    public void onOKClick()
    {
        gameObject.SetActive(false);
    }
}
