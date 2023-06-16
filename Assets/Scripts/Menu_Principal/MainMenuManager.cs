using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manager to controls all menus.
/// </summary>
public class MainMenuManager : Singleton<MainMenuManager>
{
    //==============================================================================================================
    #region PUBLIC
    //==============================================================================================================


    #endregion
    //==============================================================================================================

    //==============================================================================================================
    #region REFERENCE
    //==============================================================================================================

    [Space(30)]
#pragma warning disable CS0414 // field assigned but never used.
    [SerializeField, NaughtyAttributes.ReadOnly] private string REFERENCE = "All reference for the script to work.";
#pragma warning restore CS0414 // field assigned but never used.

    [Space(2), Header("Menu")]
    [Tooltip("Main menu GameObject.")]
    [SerializeField] private GameObject _MainMenu;
    [Tooltip("Options GameObject.")]
    [SerializeField] private GameObject _Options;
    [Tooltip("Credits GameObject.")]
    [SerializeField] private GameObject _Credits;

    [Space(5), Header("Buttons")]
    [Tooltip("Button to continue the last game save.")]
    [SerializeField] private Button _ContinueButton;
    [Tooltip("Button to start a new game.")]
    [SerializeField] private Button _NewGameButton;
    [Tooltip("Button to load a specific game.")]
    [SerializeField] private Button _LoadButton;
    [Tooltip("Button to quit game.")]
    [SerializeField] private Button _QuitButton;
    [Tooltip("Buttons to return on main menu")]
    [SerializeField] private Button _MainMenuButton;
    [Tooltip("Button to open options.")]
    [SerializeField] private Button _OptionsButton;
    [Tooltip("Button to open credits.")]
    [SerializeField] private Button _CreditsButton;

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

    [Space(5), Header("Exemple")]
    [SerializeField] private Image _BlackScreen;


    #endregion
    //==============================================================================================================

    //==============================================================================================================
    #region PRIVATE
    //==============================================================================================================

    [Space(30)]
#pragma warning disable CS0414 // field assigned but never used.
    [SerializeField, NaughtyAttributes.ReadOnly] private string PRIVATE = "All private var of script.";
#pragma warning restore CS0414 // field assigned but never used.

    private GameManager gameManager;

    [Space(2), Header("Menu")]
    [SerializeField] private GameObject currentMenu;

    [Space(2), Header("Option")]
    [SerializeField] string pathAudioTest;

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

    [Space(5), Header("Exemple")]
    [Tooltip("Speed of fade out")]
    [SerializeField, Range(0.1f, 3f)] private float blackScreenFadeOutSpeed;
    private bool isInTransition;


    #endregion
    //==============================================================================================================

    //==============================================================================================================
    #region MONOBEHAVIOUR
    //==============================================================================================================

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        // set listener to button
        SetButtonListener();
    }

    private void Start()
    {
        Intro();

        SetUpFMOD();
        SetListener();

        UpdateResolutionDropdown();

        LoadSettings();
    }

    #endregion
    //==============================================================================================================

    //==============================================================================================================
    #region PUBLIC FONCTION
    //==============================================================================================================



    #endregion
    //==============================================================================================================

    //==============================================================================================================
    #region PRIVATE FONCTION
    //==============================================================================================================
        
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
    }

    /// <summary>
    /// intro when player enter the main menu.
    /// </summary>
    private void Intro()
    {
        // cinematic when you goes to main menu.

        // exemple here:
        StartCoroutine(ExempleFadeOut());
    }

    /// <summary>
    /// exemple of a fade out for intro.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExempleFadeOut()
    {
        _BlackScreen.gameObject.SetActive(true);
        float _alpha = 1;
        while (_BlackScreen.color.a > 0.01f)
        {
            _alpha -= Time.deltaTime * blackScreenFadeOutSpeed;
            _BlackScreen.color = new Color(0, 0, 0, _alpha);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        _BlackScreen.gameObject.SetActive(false);
    }

    /// <summary>
    /// Exemple of Transition between menu.
    /// obj1 is exiting object, obj2 is entering object.
    /// </summary>
    private void Transition(GameObject obj1, GameObject obj2)
    {
        isInTransition = true;
        StateInteractable(false);
        Animator anim1 = obj1.GetComponent<Animator>();
        anim1.SetTrigger("Out");
        obj2.SetActive(true);
        StartCoroutine(TransitionWait(obj1));
    }
    private IEnumerator TransitionWait(GameObject obj1) 
    { 
        yield return new WaitForSeconds(1f); 
        obj1.SetActive(false); 
        StateInteractable(true); 
        isInTransition = false; 
    }

    /// <summary>
    /// set all button listener to their action.
    /// </summary>
    private void SetButtonListener()
    {
        if (_ContinueButton != null) _ContinueButton.onClick.AddListener(Continue);
        if (_NewGameButton != null) _NewGameButton.onClick.AddListener(NewGame);
        if (_LoadButton != null) _LoadButton.onClick.AddListener(Load);
        if (_QuitButton != null) _QuitButton.onClick.AddListener(Quit);
        if (_MainMenuButton != null) _MainMenuButton.onClick.AddListener(MainMenu);
        if (_OptionsButton != null) _OptionsButton.onClick.AddListener(Options);
        if (_CreditsButton != null) _CreditsButton.onClick.AddListener(Credits);
    }

    /// <summary>
    /// Enable/Disable interaction with certain UI.
    /// </summary>
    /// <param name="value"></param>
    private void StateInteractable(bool value)
    {
        _QuitButton.interactable = value;
        _FullscreenButton.interactable = value;
        _ResolutionDropdown.interactable = value;
        _MasterVolumeSlider.interactable = value;
        _MasterVolumeToggle.interactable = value;
        _MusicVolumeSlider.interactable = value;
        _MusicVolumeToggle.interactable = value;
        _SoundVolumeSlider.interactable = value;
        _SoundVolumeToggle.interactable = value;
        _TestAudioButton.interactable = value;
    }

    /// <summary>
    /// Open main menu.
    /// </summary>
    private void MainMenu()
    {
        if (currentMenu == _MainMenu) return;
        if (isInTransition) return;
        Transition(currentMenu, _MainMenu);
        currentMenu = _MainMenu.gameObject;
    }

    /// <summary>
    /// Open option menu.
    /// </summary>
    private void Options()
    {
        if (currentMenu == _Options) return;
        if (isInTransition) return;
        Transition(currentMenu, _Options);
        currentMenu = _Options.gameObject;
    }

    /// <summary>
    /// Open credits menu.
    /// </summary>
    private void Credits()
    {
        if (currentMenu == _Credits) return;
        if (isInTransition) return;
        Transition(currentMenu, _Credits);
        currentMenu = _Credits.gameObject;
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
#pragma warning disable CS0618 // Le type ou le membre est obsolète
        currentRefreshRate = Screen.currentResolution.refreshRate;
#pragma warning restore CS0618 // Le type ou le membre est obsolète

        for (int i = 0; i < resolutions.Length; i++)
        {
#pragma warning disable CS0618 // Le type ou le membre est obsolète
            if (resolutions[i].refreshRate == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
#pragma warning restore CS0618 // Le type ou le membre est obsolète
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

    #endregion
    //==========**



    //==================***
    // NEED TO SET THING HERE
    //==================***

    #region ButtonsActions
    private void Continue()
    {
        // continue game here
    }
    private void NewGame()
    {
        // start a new game with a new save
    }
    private void Load()
    {
        // choice wich one to load
    }
    private void Quit()
    {
        // save thing.

        Application.Quit();
    }
    #endregion

    //==================***




    #endregion
    //==============================================================================================================
}