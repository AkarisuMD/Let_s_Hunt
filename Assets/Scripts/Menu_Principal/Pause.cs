using DG.Tweening.Core.Easing;
using FMOD;
using GameplayIngredients;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    private GameManager gameManager;
    public Controls _controls;
    [SerializeField] private GameObject pauseObj, optionObj;
    [SerializeField]
    private JoinGame_Manager joinGame_Manager;
    private bool canOpen => joinGame_Manager.isTheGameStarted.Value;
    [SerializeField] private DevicesSelector deviceSelector;

    [Space(5), Header("Options")]
    [Tooltip("Button to set fullscreen.")]
    [SerializeField] private Toggle _FullscreenButton;
    [Tooltip("Dropdown to set up resolution settings.")]
    [SerializeField] private TMP_Dropdown _ResolutionDropdown;
    [Tooltip("slider of audio master volume.")]
    [SerializeField] private Slider _MasterVolumeSlider;
    [Tooltip("toogle to enable/disable audio.")]
    [SerializeField] private Toggle _MasterVolumeToggle;
    [Tooltip("slider of audio music volume.")]
    [SerializeField] private Slider _MusicVolumeSlider;
    [Tooltip("toogle to enable/disable audio.")]
    [SerializeField] private Toggle _MusicVolumeToggle;
    [Tooltip("slider of audio sound volume.")]
    [SerializeField] private Slider _SoundVolumeSlider;
    [Tooltip("toogle to enable/disable audio.")]
    [SerializeField] private Toggle _SoundVolumeToggle;
    [Tooltip("Button to test audio.")]
    [SerializeField] private Button _TestAudioButton;

    [SerializeField] string pathAudioTest;

    [SerializeField] private Slider _VoiceVolumeSlider;

    private bool _fullscreen
    {
        get { return gameManager.gameData.gameSettingsParameters.fullscreen; }
        set { gameManager.gameData.gameSettingsParameters.fullscreen = value; }
    }

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private string currentResolution
    {
        get { return gameManager.gameData.gameSettingsParameters.resolution; }
        set { gameManager.gameData.gameSettingsParameters.resolution = value; }
    }
    [SerializeField, NaughtyAttributes.ReadOnly] private float currentRefreshRate;
    private int currentResolutionIndex = 0;

    //======**
    // audio
    //======**
    FMOD.Studio.Bus master;
    FMOD.Studio.Bus music;
    FMOD.Studio.Bus sound;
    private bool masterVolumeIsOff
    {
        get { return gameManager.gameData.gameSettingsParameters.masterVolumeIsOff; }
        set { gameManager.gameData.gameSettingsParameters.masterVolumeIsOff = value; }
    }
    private float masterVolume
    {
        get { return gameManager.gameData.gameSettingsParameters.masterVolume; }
        set { gameManager.gameData.gameSettingsParameters.masterVolume = value; }
    }
    private bool musicVolumeIsOff
    {
        get { return gameManager.gameData.gameSettingsParameters.musicVolumeIsOff; }
        set { gameManager.gameData.gameSettingsParameters.musicVolumeIsOff = value; }
    }
    private float musicVolume
    {
        get { return gameManager.gameData.gameSettingsParameters.musicVolume; }
        set { gameManager.gameData.gameSettingsParameters.musicVolume = value; }
    }
    private bool soundVolumeIsOff
    {
        get { return gameManager.gameData.gameSettingsParameters.soundVolumeIsOff; }
        set { gameManager.gameData.gameSettingsParameters.soundVolumeIsOff = value; }
    }
    private float soundVolume
    {
        get { return gameManager.gameData.gameSettingsParameters.soundVolume; }
        set { gameManager.gameData.gameSettingsParameters.soundVolume = value; }
    }

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    private void Start()
    {
        _controls = new();
        _controls.Enable();
        _controls.Menu.Pause.started += ctx => OpenPause();
    }

    private void _OnEnableOption()
    {
        SetUpFMOD();
        SetListener();

        UpdateResolutionDropdown();

        LoadSettings();
    }

    private void OpenPause()
    {
        if (!canOpen) return;

        pauseObj.SetActive(!pauseObj.active);
    }
    public void ClosePause()
    {
        pauseObj.SetActive(false);
    }

    public void OpenOption()
    {
        optionObj.SetActive(true);
        pauseObj.SetActive(false);
        _OnEnableOption();
    }
    public void CloseOption()
    {
        optionObj.SetActive(false);
        pauseObj.SetActive(true);
    }

    /// <summary>
    /// Load settings from gameData.
    /// </summary>
    private void LoadSettings()
    {
        _FullscreenButton.isOn = _fullscreen;
        SetFullscreen(_fullscreen);

        _MasterVolumeSlider.value = masterVolume;
        MasterVolumeChange(masterVolume);
        MasterVolumeState(masterVolumeIsOff);

        _MusicVolumeSlider.value = musicVolume;
        MusicVolumeChange(musicVolume);
        MusicVolumeState(musicVolumeIsOff);

        _SoundVolumeSlider.value = soundVolume;
        SoundVolumeChange(soundVolume);
        SoundVolumeState(soundVolumeIsOff);

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            if (filteredResolutions[i].ToString() == currentResolution)
            {
                _ResolutionDropdown.value = i;
                SetResolution(i);
                break;
            }
        }

        _VoiceVolumeSlider.value = LoginCredentials.Instance.client.AudioInputDevices.VolumeAdjustment;
    }

    //==========**
    #region Options
    //==========**

    /// <summary>
    /// Set listener to there action.
    /// </summary>
    private void SetListener()
    {
        _FullscreenButton.onValueChanged.AddListener(SetFullscreen);
        _ResolutionDropdown.onValueChanged.AddListener(SetResolution);

        _MasterVolumeSlider.onValueChanged.AddListener(MasterVolumeChange);
        _MasterVolumeToggle.onValueChanged.AddListener(MasterVolumeState);

        _MusicVolumeSlider.onValueChanged.AddListener(MusicVolumeChange);
        _MusicVolumeToggle.onValueChanged.AddListener(MusicVolumeState);

        _SoundVolumeSlider.onValueChanged.AddListener(SoundVolumeChange);
        _SoundVolumeToggle.onValueChanged.AddListener(SoundVolumeState);

        _TestAudioButton.onClick.AddListener(TestAudio);
    }

    /// <summary>
    /// Enable/Disable fullscreen of the game.
    /// </summary>
    /// <param name="fullscreen"></param>
    private void SetFullscreen(bool value) => Screen.fullScreen = _fullscreen = value;

    /// <summary>
    /// Set the resolution of the game.
    /// </summary>
    /// <param name="resolutionIndex"></param>
    private void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        currentResolution = resolution.ToString();
        Screen.SetResolution(resolution.width, resolution.height, _fullscreen);
    }

    /// <summary>
    /// Change the volume of Master.
    /// </summary>
    /// <param name="value"></param>
    private void MasterVolumeChange(float value)
    {
        masterVolume = value;

        if (value < 0.05f) { MasterVolumeState(true); _MasterVolumeToggle.isOn = true; }
        else if (value > 0.05f) { MasterVolumeState(false); _MasterVolumeToggle.isOn = false; }
    }
    private void MasterVolumeChange()
    {
        if (masterVolumeIsOff) master.setVolume(0);
        else master.setVolume(masterVolume);
    }

    /// <summary>
    /// Switch state of Master volume.
    /// </summary>
    /// <param name="value"></param>
    private void MasterVolumeState(bool value)
    {
        if (value) { masterVolumeIsOff = true; }
        else { masterVolumeIsOff = false; }

        MasterVolumeChange();
    }

    /// <summary>
    /// Chnage the volume of Music.
    /// </summary>
    /// <param name="value"></param>
    private void MusicVolumeChange(float value)
    {
        musicVolume = value;

        if (value < 0.05f) { MusicVolumeState(true); _MusicVolumeToggle.isOn = true; }
        else if (value > 0.05f) { MusicVolumeState(false); _MusicVolumeToggle.isOn = false; }
    }
    private void MusicVolumeChange()
    {
        if (musicVolumeIsOff) music.setVolume(0);
        else music.setVolume(masterVolume);
    }

    /// <summary>
    /// Switch state of Music volume.
    /// </summary>
    /// <param name="value"></param>
    private void MusicVolumeState(bool value)
    {
        if (value) { musicVolumeIsOff = true; }
        else { musicVolumeIsOff = false; }

        MusicVolumeChange();
    }

    /// <summary>
    /// Change the volume of Sound.
    /// </summary>
    /// <param name="value"></param>
    private void SoundVolumeChange(float value)
    {
        soundVolume = value;

        if (value < 0.05f) { SoundVolumeState(true); _SoundVolumeToggle.isOn = true; }
        else if (value > 0.05f) { SoundVolumeState(false); _SoundVolumeToggle.isOn = false; }
    }
    private void SoundVolumeChange()
    {
        if (soundVolumeIsOff) sound.setVolume(0);
        else sound.setVolume(soundVolume);
    }

    /// <summary>
    /// Switch state of Sound volume.
    /// </summary>
    /// <param name="value"></param>
    private void SoundVolumeState(bool value)
    {
        if (value) { soundVolumeIsOff = true; }
        else { soundVolumeIsOff = false; }

        SoundVolumeChange();
    }

    /// <summary>
    /// set up all FMOD path.
    /// </summary>
    private void SetUpFMOD()
    {
        master = FMODUnity.RuntimeManager.GetBus("bus:/");
        music = FMODUnity.RuntimeManager.GetBus("bus:/Music");
        sound = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
    }

    /// <summary>
    /// Test of a sound audio.
    /// </summary>
    private void TestAudio() => FMODUnity.RuntimeManager.PlayOneShot(pathAudioTest, transform.position);

    /// <summary>
    /// Reset value inside the dropdown of the resolution settings.
    /// </summary>
    private void UpdateResolutionDropdown()
    {

        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        _ResolutionDropdown.ClearOptions();
#pragma warning disable CS0618 // Le type ou le membre est obsol�te
        currentRefreshRate = Screen.currentResolution.refreshRate;
#pragma warning restore CS0618 // Le type ou le membre est obsol�te

        for (int i = 0; i < resolutions.Length; i++)
        {
#pragma warning disable CS0618 // Le type ou le membre est obsol�te
            if (resolutions[i].refreshRate == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
#pragma warning restore CS0618 // Le type ou le membre est obsol�te
        }
        List<string> options = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height;
            options.Add(resolutionOption);
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        _ResolutionDropdown.AddOptions(options);
        _ResolutionDropdown.value = currentResolutionIndex;
        _ResolutionDropdown.RefreshShownValue();

    }

    public void VoiceInputVolume(float volume)
    {
        LoginCredentials.Instance.AdjustVolume(volume);
    }

    #endregion
    //==========**
}
