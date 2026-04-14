using UnityEngine;

public class MakeANewUI : MonoBehaviour
{
    [SerializeField] GameObject UI;

    private GameObject validUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        validUI = Instantiate(UI);
    }

    // Update is called once per frame
    void Update()
    {
        if (validUI == null)
        {
             validUI = Instantiate(UI);
        }
    }
}
