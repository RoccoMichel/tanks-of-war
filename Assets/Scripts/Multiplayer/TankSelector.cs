using UnityEngine;

public class TankSelector : MonoBehaviour
{
    public GameObject[] tankPreviews;
    public string[] tankNames;
    static int selected;

    private void Start()
    {
        // Pre-Selected a random tank to encourage using others than the first pick
        selected = Random.Range(0, tankPreviews.Length);
        ChangeTankPreview();
        PlayerPrefs.SetString("selectedTank", tankNames[selected]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) Next();
    }

    public void Next()
    {
        selected++;
        if (selected >= tankNames.Length) selected = 0;

        ChangeTankPreview();
        PlayerPrefs.SetString("selectedTank", tankNames[selected]);
    }

    void ChangeTankPreview()
    {
        foreach(GameObject preview in tankPreviews) preview.SetActive(false);
        tankPreviews[selected].SetActive(true);
    }
}
