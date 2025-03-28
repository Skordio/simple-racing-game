using UnityEngine;

public class SettingsMenuKeyListener : MonoBehaviour
{
    public GameObject settingsMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingsMenu.SetActive(!settingsMenu.activeSelf);
        }
    }
}