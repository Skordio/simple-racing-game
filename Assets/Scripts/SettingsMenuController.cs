using TMPro;
using UnityEngine;

public class SettingsMenuController : MonoBehaviour
{
    public GameObject settingsMenu;

    public enum ControlType { Mouse, Keyboard }

    public TMP_Dropdown controlTypeDropdown;
    public static ControlType currentControlType = ControlType.Mouse;

    void Start()
    {
        // Load previous setting (optional)
        int savedValue = PlayerPrefs.GetInt("ControlType", 1);
        controlTypeDropdown.value = savedValue;
        UpdateControlType(savedValue);

        controlTypeDropdown.onValueChanged.AddListener(UpdateControlType);

        settingsMenu.SetActive(false);
    }

    void UpdateControlType(int index)
    {
        currentControlType = (ControlType)index;
        PlayerPrefs.SetInt("ControlType", index); // Save for next session
        Debug.Log("Control Type set to: " + currentControlType);
    }
}