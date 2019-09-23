using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; // needed for mixer method.
using UnityEngine.UI; // needed to reference the dropdown component when determining resolutions.

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Dropdown resolutionDropdown;

    Resolution[] resolutions;

    // uses List<string> to capture PC's available resolutions
    // also sets the default resolution of the game
    void Start ()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // FOR THE FOLLOWING METHODS
    // YOU NEED TO TIE THEM INTO
    // THEIR RESPECTIVE UNITY ITEMS
    // BUT IMPORTANTLY TIE THEM WITH
    // THE DYNAMIC METHOD TO AUTO
    // UPDATE THE VALUES.
    
    // updates the resolution of the game
    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        // need to set up resolution dropdown in Unity
    }
    public void SetVolume (float volume)
    {
        audioMixer.SetFloat("volume", volume);
        // need to set up an audio mixer in Unity, call it "volume"
        // set volume bounds to [-80, 0] in Unity on the slider
    }

    public void SetQuality (int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        // need to set up video quality settings in Unity
    }

    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        // need to set up fullscreen toggle in Unity
    }
}
