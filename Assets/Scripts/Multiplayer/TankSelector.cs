using UnityEngine;
using UnityEngine.UI;

public class TankSelector : MonoBehaviour
{
    public Sprite[] tankPreviews;
    public string[] tankNames;
    private Image display;
    static int selected;

    private void Start()
    {
        display = GetComponent<Image>();
        display.sprite = tankPreviews[selected];
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

        display.sprite = tankPreviews[selected];
        PlayerPrefs.SetString("selectedTank", tankNames[selected]);
    }
}
