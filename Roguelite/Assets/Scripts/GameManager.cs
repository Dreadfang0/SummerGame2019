using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using TMPro;
/*
Original Author Janne Karhu
V:1.0 | 17.6.2019 Janne Karhu
Taken from another project and going through adjustments to fit into this one.
V:1.1 | 20.6.2019 Janne Karhu
Removed obsolote stuff and adjusted player health, also added healthbar functionality
V:1.2 | Unknown Date Saku Petlin
Perk system Compability
V:1.3 | 2.7.2019 Janne Karhu
Boss Health bar implementation
*/

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //public float playerMaxHealth;
    //[SerializeField]
    //float playerHealth;
    public Transform PlayerPosition;
    public GameObject Player;
    public bool pausedGame;
    [HideInInspector]
    public bool damagedPlayer = false;
    public float volume; // Sound volume that we can set and get from playerprefs
    public float damageCooldown; // How often is the player allowed to get damaged
    bool recentlyDamaged;
    public bool isDead;
    [SerializeField]
    GameObject BloodPartic;
    // UI stuff
    [SerializeField]
    GameObject Dead;
    [SerializeField]
    GameObject damaged;
    [SerializeField]
    GameObject Win;
    [SerializeField]
    Color dmgColor;
    [SerializeField]
    Color noColor;
    float colorFade = 100;

    [SerializeField]
    GameObject MainMenu;
    [SerializeField]
    GameObject SettingsMenu;
    [SerializeField]
    GameObject CreditsMenu;
    [SerializeField]
    GameObject PauseMenu;
    [SerializeField]
    GameObject SkillsMenu;
    [SerializeField]
    GameObject LevelUpMenu;

    [SerializeField]
    Button perk1Button, perk2Button, perk3Button;
    [SerializeField]
    TextMeshProUGUI perk1Text, perk2Text, perk3Text;
    [SerializeField]
    Image perk1Icon, perk2Icon, perk3Icon;
    public TextMeshProUGUI[] MenuSkillCount;
    [SerializeField]

    private PerkSystem perkSystem;
    private PlayerController playerController;
    private MouseLook mouseLook;

    //HUD things
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private TextMeshProUGUI healthText;
    [SerializeField]
    private Image armorBar;
    [SerializeField]
    private TextMeshProUGUI armorText;
    //[SerializeField]
    //private Image staminaBar;
    //[SerializeField]
    //private TextMeshProUGUI staminaText;
    //[SerializeField]
    //private Image experienceBar;
    //[SerializeField]
    //private TextMeshProUGUI experienceText;
    public Image reloading;
    [SerializeField]
    private TextMeshProUGUI ammoCount;
    public GameObject reload;
    [SerializeField]
    private TextMeshProUGUI reloadTime;
    [HideInInspector]
    public float t;

    [SerializeField]
    GameObject BossHealthHolder;
    [SerializeField]
    Image BossHealthBar;
    float BossMaxHp;
    float BossHp;
    bool BossActive;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    public TextMeshProUGUI masterNumber;
    public TextMeshProUGUI musicNumber;
    public TextMeshProUGUI sfxNumber;

    public Slider mouseSensitivity;
    public TextMeshProUGUI mouseSensitivityNumber;
    public Toggle mouseInversion;

    public AudioMixer masterMixer;
    public AudioMixerSnapshot paused, unpaused;
    public AudioMixer DOOTMixer;
    //public AudioMixer musicMixer;
    //public AudioMixer soundMixer;

    //float timer; // Used for regen.
    //[SerializeField]
    //private AudioSource audioSource;
    //[SerializeField]
    //private AudioSource audioSoundSource;
    public AudioSource playerDamageSource;
    

    // Fish Audio files
    public AudioClip MainMenuMusic;
    public AudioClip Area1Music;
    public AudioClip Area2Music;
    [SerializeField]
    AudioClip PlayerHurt;
    public Animator animator;
    public Animator endingAnimator;

    private Rigidbody selfbody;
    public float fadeTime;

    Vector3 CheckpointPos;

    void Awake()
    {
        Application.targetFrameRate = 60;
        if(instance == null)
        {
        instance = this;
        DontDestroyOnLoad(gameObject);
        }
        if (instance != this)
        {
            Destroy(gameObject);
        }
        Cursor.lockState = CursorLockMode.Locked;
        mouseLook = GameObject.FindGameObjectWithTag("Player").GetComponent<MouseLook>();
        mouseLook.sensitivity = PlayerPrefs.GetFloat("Sensitivity");
        mouseSensitivity.value = PlayerPrefs.GetFloat("MouseSlider");
        masterSlider.value = PlayerPrefs.GetFloat("MasterVol");
        musicSlider.value = PlayerPrefs.GetFloat("MusicVol");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol");
    }

    void Start()
    {
        //audioSource.clip = MainMenuMusic;
        //audioSource.Play();
        // Check if first run key exists, if it doesn't exist, create it or it is 1, initialize values. 0 = False, 1 = True.
        if (PlayerPrefs.HasKey("FirstRun") == false && PlayerPrefs.GetInt("FirstRun") == 1)
        {
            PlayerPrefs.SetInt("FirstRun", 0);

            //PlayerPrefs.SetFloat("SoundVolume", 0);
            //PlayerPrefs.SetFloat("MusicVolume", 0);

            //PlayerPrefs.SetFloat("Sensitivity",1);
            //mouseLook.sensitivity = PlayerPrefs.GetFloat("Sensitivity");
            //mouseSensitivity.value = PlayerPrefs.GetFloat("MouseSlider");
            SetAudio();
            PlayerPrefs.Save();
        }
        else // If first run is not found or is 1, get values since they already exist.
        {
            //soundMixer.SetFloat("Volume", PlayerPrefs.GetFloat("SoundVolume"));
            //musicMixer.SetFloat("Volume", PlayerPrefs.GetFloat("MusicVolume"));
            //soundSlider.value = PlayerPrefs.GetFloat("SoundVolume");
            //musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            //mouseLook.sensitivity = PlayerPrefs.GetFloat("Sensitivity");
            //mouseSensitivity.value = PlayerPrefs.GetFloat("MouseSlider");
            SetAudio();

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (perkSystem == null)
        {
            perkSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<PerkSystem>();
        }

        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        if(mouseLook == null)
        {
            mouseLook = GameObject.FindGameObjectWithTag("Player").GetComponent<MouseLook>();
        }

        if(pausedGame == true)
        {
            mouseSensitivityNumber.text = "" + (int)(mouseSensitivity.value * 10f)/ 10f;
            SetAudio();
            masterNumber.text = "" + (int)(masterSlider.value * 100f) + "%";
            musicNumber.text = "" + (int)(musicSlider.value * 100f) + "%";
            sfxNumber.text = "" + (int)(sfxSlider.value * 100f) + "%";
        }

        if (playerController == null)
            playerController = Player.GetComponent<PlayerController>();
        if (BossActive == true) //BossHealthBar Updater
        {
            BossHealthHolder.SetActive(true);
            float BossPercentage = BossHp / BossMaxHp;
            BossHealthBar.fillAmount = BossPercentage;
            //Debug.Log("% " + BossPercentage);
            //Debug.Log("cur " + BossHp);
            //Debug.Log("Max " + BossMaxHp);
            if (BossHp <= 0)
            {
                BossHealthHolder.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            LevelUp();
        }
        float currentHealthPercentage = (float)playerController.health / (float)playerController.healthMax;
        healthBar.fillAmount = currentHealthPercentage;
        healthText.text = "Health: " + playerController.health + " / " + playerController.healthMax;

        float currentArmorPercentage = (float)playerController.armor / (float)playerController.armorMax;
        armorBar.fillAmount = currentArmorPercentage;
        armorText.text = "Armor: " + playerController.armor + " / " + playerController.armorMax;

        //float currentStaminaPercentage = playerController.stamina / playerController.staminaMax;
        //staminaBar.fillAmount = currentStaminaPercentage;
        //staminaText.text = "Stamina: " + (int)playerController.stamina + " / " + playerController.staminaMax;

        //float currentExperiencePercentage = playerController.experience / playerController.experienceMax;
        //experienceBar.fillAmount = currentExperiencePercentage;
        //experienceText.text = "Experience: " + playerController.experience + " / " + playerController.experienceMax;

        if (playerController.reloading == true)
        {
            reload.SetActive(true);
            reloading.fillAmount += 1f / playerController.reloadSpeed * Time.deltaTime;
            reloadTime.text = "" + Mathf.Round((playerController.reloadSpeed - reloading.fillAmount * playerController.reloadSpeed) * 10f) / 10f;
        }

        ammoCount.text = "Ammo: " + playerController.ammo + " / " + playerController.ammoMax;

        if (PlayerPosition == null)
        {
            PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        //timer += Time.deltaTime; // Used by regen.

        if (Input.GetKeyDown(KeyCode.P)) // Used for pausing the game
        {
            if (pausedGame == false)
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                pauseMenu();
            }
            else
            {

                continueGame();
                closeSettingsMenu();
            }
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
            restartGame();

        if (playerController.health <= 0 && isDead == false)
        {
            PlayerDeath();
            isDead = true;
            //restartGame();
        }
        if (recentlyDamaged == false) // If player is damaged and has not taken damage in a moment they regenerate.
        {
            /*if (timer > 0.2f && playerHealth < 100)
            {
                //playerHealth += 1;
                colorFade += 1f;
                timer = 0;
            }
            else if (timer > 0.2f && colorFade < 100)
            {
                colorFade += 1f;
                timer = 0;
            }*/
        }

        //damaged.GetComponent<Image>().color = Color.Lerp(dmgColor, noColor,colorFade / 100);
        //ammoCount.text = collectiblesCollected.ToString();

    }
    public void damagePlayer(int damage) // |-----DAMAGE PLAYER-----|
    {
        if (playerController.health > 0)
        {
            playerDamageSource.Play();
            if (playerController.armor > 0)
            {
                if (damage > playerController.armor)
                {
                    damage -= playerController.armor;
                    playerController.armor = playerController.armorMin;
                    playerController.health -= damage;
                }
                else
                {
                    playerController.armor -= Mathf.RoundToInt((float)(damage * playerController.armorAbsorption));
                    playerController.health -= Mathf.RoundToInt((float)(damage * playerController.armorSpill));
                    //Debug.Log(Mathf.RoundToInt((float)(damage * playerController.armorSpill)));
                }

            }

            else
            {
                //audioSoundSource.PlayOneShot(PlayerHurt, 1);
                playerController.health -= damage;
                colorFade = playerController.health - 30;
            }

        }
    }
    public void healPlayer(int healAmount) // |-----HEAL PLAYER-----|
    {
        if (playerController.health > 0)
        {
            //audioSoundSource.PlayOneShot(PlayerHurt, 1);  
            playerController.health += healAmount;

        }
    }
    public void armorPlayer(int armorAmount) // |-----ARMORIZE PLAYER-----|
    {
        if (playerController.armor < playerController.armorMax)
        {
            //audioSoundSource.PlayOneShot(PlayerHurt, 1);  
            playerController.armor += armorAmount;

        }
    }
    public void startGame() // |-----START GAME-----|
    {
        MainMenu.SetActive(false);
        SceneManager.LoadScene(2);
        //audioSource.clip = Area1Music;
    }
    public void openSettingsMenu() // |-----OPEN SETTINGS-----|
    {
        SettingsMenu.SetActive(true);
    }
    public void closeSettingsMenu() // |-----CLOSE SETTINGS-----|
    {
        SettingsMenu.SetActive(false);
    }
    public void openSkillsMenu() // |-----OPEN SKILLS-----|
    {
        SkillsMenu.SetActive(true);
    }
    public void closeSkillsMenu() // |-----CLOSE SKILLS-----|
    {
        SkillsMenu.SetActive(false);
    }
    public void OpenCreditsMenu() // |-----OPEN CREDITS-----|
    {
        CreditsMenu.SetActive(true);
    }
    public void CloseCreditsMenu() // |-----CLOSE CREDITS-----|
    {
        CreditsMenu.SetActive(false);
    }
    public void quitGame() // |-----QUIT GAME-----|
    {
        Application.Quit();
    }
    public void pauseMenu() // |-----PAUSE GAME-----|
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
        pausedGame = true;
        Cursor.visible = true;
        paused.TransitionTo(0);
        // Add menu stuff here

    }
    public void continueGame() // |-----UNPAUSE GAME-----|
    {
        // Unpauses game
        PauseMenu.SetActive(false);
        closeSettingsMenu();
        closeSkillsMenu();
        Time.timeScale = 1;
        pausedGame = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        MouseSensitivity();
        saveSettings();
        unpaused.TransitionTo(0);
    }
    //public void SetSoundVolume(float newVolume) // |-----VOLUME CHANGING-----|
    //{
    //    soundMixer.SetFloat("Volume", newVolume);
    //}
    //public void SetMusicVolume(float newVolume)
    //{
    //    musicMixer.SetFloat("Volume", newVolume);
    //}

    public void saveSettings() // |-----SAVE SETTINGS-----|
    {
        //float sv;
        //soundMixer.GetFloat("Volume", out sv);
        //PlayerPrefs.SetFloat("SoundVolume", sv);
        //float mv;
        //musicMixer.GetFloat("Volume", out mv);
        //PlayerPrefs.SetFloat("MusicVolume", mv);

        PlayerPrefs.SetFloat("Sensitivity", mouseLook.sensitivity);
        PlayerPrefs.SetFloat("MouseSlider", mouseSensitivity.value);

        PlayerPrefs.SetFloat("MasterVol", masterSlider.value);
        PlayerPrefs.SetFloat("MusicVol", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVol", sfxSlider.value);

        PlayerPrefs.Save();
    }
    public void ChangeMusic(int trackNumber) // |-----CHANGE MUSIC-----|
    {
        if (trackNumber == 1)
        {
            //audioSource.clip = Area1Music;
        }
        else if (trackNumber == 2)
        {
            //audioSource.clip = Area2Music;
        }
    }
    public void WinGame() // |-----WIN GAME-----|
    {
        Win.SetActive(true);
    }
    public void PlayerDeath() // |-----DEATH-----|
    {
        //Debug.Log("DEAD");
        //Dead.SetActive(true);
        //GameObject.Instantiate(BloodPartic, PlayerPosition.position, BloodPartic.transform.rotation);
        Player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        Player.GetComponent<PlayerController>().enabled = false;
        for (int i = 0; i < MenuSkillCount.Length; i++)
        {
            MenuSkillCount[i].text = "";
        }
        StartCoroutine(DeathFade());
    }
    public void restartGame() // |-----RESTART GAME-----|
    {
        //Win.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //playerController.health = 100; // Resets health
        Player.GetComponent<PlayerController>().enabled = true; // Activates Player
        isDead = false; // Allows player to be damaged again
                        // Player.GetComponent<PlayerController>().dashUsed = false; // Prevents bug that causes the player being unable to dash
        for (int i = 0; i < MenuSkillCount.Length; i++)
        {
            MenuSkillCount[i].text = "";
        }
    }

    public void BossHealthBarSetup(float MaxHp) // |----BOSS HEALTH----|
    {
        BossActive = true;
        BossMaxHp = MaxHp;
    }
    public void BossHealthUpdater(float curHp) // |----BOSS CURRENT HEALTH UPDATER----|
    {
        BossHp = curHp;
    }
    IEnumerator DamageCooldown() // |-----DAMAGE ON COOLDOWN-----|
    {
        damagedPlayer = true;
        recentlyDamaged = true;
        yield return new WaitForSeconds(damageCooldown);
        //audioSource.Stop();
        recentlyDamaged = false;
        damagedPlayer = false;
    }
    public void burnPlayer(int burn)
    {
        StartCoroutine(burning(burn));
    }
    IEnumerator burning(int fireDamage) // |----- BURNING -----|
    {
        //BurningParticle.GetComponent<ParticleSystem>().Play();
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1);
            damagePlayer(fireDamage);
        }
        //BurningParticle.GetComponent<ParticleSystem>().Stop();
    }
    IEnumerator DeathFade()
    {
        float rate = 1.0f / fadeTime;
        float progress = 1.0f;

        Dead.GetComponent<Image>().color = Color.clear;


        while (progress > 0.0f)
        {
            Dead.GetComponent<Image>().color = Color.Lerp(Color.white, Color.clear, progress);
            progress -= rate * Time.deltaTime;
            yield return null;
        }
        Dead.GetComponent<Image>().color = Color.white;

        yield return new WaitForSeconds(4);
        restartGame();

        Dead.GetComponent<Image>().color = Color.white;
        progress = 0.0f;

        while (progress < 1.0f)
        {
            Dead.GetComponent<Image>().color = Color.Lerp(Color.white, Color.clear, progress);
            progress += rate * Time.deltaTime;
            yield return null;
        }
        Dead.GetComponent<Image>().color = Color.clear;

    }

    public void MouseSensitivity()
    {
        //mouseLook.sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1);
        mouseLook.sensitivity = mouseSensitivity.value;

        if(mouseInversion.isOn)
        {
            mouseLook.invert = true;
        }

        else
        {
            mouseLook.invert = false;
        }
    }

    public void SetAudio()
    {
        masterMixer.SetFloat("MasterVol", Mathf.Log10(masterSlider.value) * 20);
        masterMixer.SetFloat("MusicVol", Mathf.Log10(musicSlider.value) * 20);
        masterMixer.SetFloat("SFXVol", Mathf.Log10(sfxSlider.value) * 20);
    }

    public void LevelUp()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if (perkSystem.GlassCannon == true)
        {
            playerController.healthMax += playerController.healthLvlUp / 2;
            playerController.health += playerController.healthLvlUp / 2;
        }
        else
        {
            playerController.healthMax += playerController.healthLvlUp;
            playerController.health += playerController.healthLvlUp;
        }

        perkSystem.UpdateTexts();
        LevelUpMenu.SetActive(true);
        Time.timeScale = 0;
        pausedGame = true;
        Cursor.visible = true;
        Perk1();
        Perk2();
        Perk3();
    }

    public void continueLevelUp()
    {
        LevelUpMenu.SetActive(false);
        Time.timeScale = 1;
        pausedGame = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        perk1Button.onClick.RemoveAllListeners();
        perk2Button.onClick.RemoveAllListeners();
        perk3Button.onClick.RemoveAllListeners();
    }
    
    void Perk1()
    {
        switch (Random.Range(0, 13))
        {
            case 0:
                if(perkSystem.multishotUpgrade == 1)
                {
                    perk1Button.onClick.AddListener(perkSystem.MultishotPerk);
                    perk1Text.text = perkSystem.multiShotText;
                    perk1Icon.sprite = perkSystem.perkIcons.multiShotIcon2;
                    perk1Button.onClick.AddListener(continueLevelUp);
                    break;
                }

                if (perkSystem.multishotUpgrade == 2)
                {
                    Perk1();
                    break;
                }

                else
                {
                    perk1Button.onClick.AddListener(perkSystem.MultishotPerk);
                    perk1Text.text = perkSystem.multiShotText;
                    perk1Icon.sprite = perkSystem.perkIcons.multiShotIcon;
                    perk1Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 1:
                if (perkSystem.FireProjectiles == true)
                {
                    Perk1();
                    break;
                }

                else
                {
                    perk1Button.onClick.AddListener(perkSystem.FireProjectilesPerk);
                    perk1Text.text = perkSystem.fireProjectilesText;
                    perk1Icon.sprite = perkSystem.perkIcons.fireProjectilesIcon;
                    perk1Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 2:
                if (perkSystem.ExplosiveProjectiles == true)
                {
                    Perk1();
                    break;
                }

                else
                {
                    perk1Button.onClick.AddListener(perkSystem.ExplosiveProjectilesPerk);
                    perk1Text.text = perkSystem.explosiveProjectilesText;
                    perk1Icon.sprite = perkSystem.perkIcons.explosiveProjectilesIcon;
                    perk1Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 3:
                if (perkSystem.PiercingProjectiles == true)
                {
                    Perk1();
                    break;
                }

                else
                {
                    perk1Button.onClick.AddListener(perkSystem.PiercingProjectilesPerk);
                    perk1Text.text = perkSystem.piercingProjectilesText;
                    perk1Icon.sprite = perkSystem.perkIcons.piercingProjectilesIcon;
                    perk1Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 4:
                perk1Button.onClick.AddListener(perkSystem.AttackSpeedPerk);
                perk1Text.text = perkSystem.attackSpeedText;
                perk1Icon.sprite = perkSystem.perkIcons.attackSpeedIcon;
                perk1Button.onClick.AddListener(continueLevelUp);
                break;
            case 5:
                perk1Button.onClick.AddListener(perkSystem.DamagePerk);
                perk1Text.text = perkSystem.damageText;
                perk1Icon.sprite = perkSystem.perkIcons.damageIcon;
                perk1Button.onClick.AddListener(continueLevelUp);
                break;
            //case 6:
            //    perk1Button.onClick.AddListener(perkSystem.HealthUpgradePerk);
            //    perk1Text.text = perkSystem.healthUpgradeText;
            //    perk1Icon.sprite = perkSystem.perkIcons.healthUpgradeIcon;
            //    perk1Button.onClick.AddListener(continueLevelUp);
            //    break;
            //case 7:
            //    perk1Button.onClick.AddListener(perkSystem.HealthRestorePerk);
            //    perk1Text.text = perkSystem.healthRestoreText;
            //    perk1Icon.sprite = perkSystem.perkIcons.healthRestoreIcon;
            //    perk1Button.onClick.AddListener(continueLevelUp);
            //    break;
            case 6:
                if (perkSystem.LifeSteal == true)
                {
                    Perk1();
                    break;
                }

                else
                {
                    perk1Button.onClick.AddListener(perkSystem.LifeStealPerk);
                    perk1Text.text = perkSystem.lifeStealText;
                    perk1Icon.sprite = perkSystem.perkIcons.lifeStealIcon;
                    perk1Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 7:
                perk1Button.onClick.AddListener(perkSystem.AmmoReloadSpeedPerk);
                perk1Text.text = perkSystem.ammoReloadSpeedText;
                perk1Icon.sprite = perkSystem.perkIcons.ammoReloadSpeedIcon;
                perk1Button.onClick.AddListener(continueLevelUp);
                break;
            case 8:
                perk1Button.onClick.AddListener(perkSystem.CriticalMasteryPerk);
                perk1Text.text = perkSystem.criticalMasteryText;
                perk1Icon.sprite = perkSystem.perkIcons.criticalMasteryIcon;
                perk1Button.onClick.AddListener(continueLevelUp);
                break;
            case 9:
                if (perkSystem.GlassCannon == true)
                {
                    Perk1();
                    break;
                }
                else
                {
                    perk1Button.onClick.AddListener(perkSystem.GlassCannonPerk);
                    perk1Text.text = perkSystem.glassCannonText;
                    perk1Icon.sprite = perkSystem.perkIcons.glassCannonIcon;
                    perk1Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 10:
                if (perkSystem.burstUpgrade == 2)
                {
                    Perk1();
                    break;
                }
                else
                {
                    perk1Button.onClick.AddListener(perkSystem.BurstPerk);
                    perk1Text.text = perkSystem.burstText;
                    perk1Icon.sprite = perkSystem.perkIcons.burstIcon;
                    perk1Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 11:
                if(perkSystem.FrostProjectiles == true)
                {
                    Perk1();
                    break;
                }
                else
                {
                    perk1Button.onClick.AddListener(perkSystem.FrostProjectilesPerk);
                    perk1Text.text = perkSystem.frostProjectilesText;
                    perk1Icon.sprite = perkSystem.perkIcons.frostProjectilesIcon;
                    perk1Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 12:
                if (perkSystem.Berserk == true)
                {
                    Perk1();
                    break;
                }
                else
                {
                    perk1Button.onClick.AddListener(perkSystem.BerserkPerk);
                    perk1Text.text = perkSystem.berserkText;
                    perk1Icon.sprite = perkSystem.perkIcons.berserkIcon;
                    perk1Button.onClick.AddListener(continueLevelUp);
                    break;
                }
        }
    }

    void Perk2()
    {
        switch (Random.Range(0, 13))
        {
            case 0:
                if (perkSystem.multishotUpgrade == 1)
                {
                    perk2Button.onClick.AddListener(perkSystem.MultishotPerk);
                    perk2Text.text = perkSystem.multiShotText;
                    perk2Icon.sprite = perkSystem.perkIcons.multiShotIcon2;
                    perk2Button.onClick.AddListener(continueLevelUp);
                    break;
                }

                if (perkSystem.multishotUpgrade == 2)
                {
                    Perk2();
                    break;
                }

                else
                {
                    perk2Button.onClick.AddListener(perkSystem.MultishotPerk);
                    perk2Text.text = perkSystem.multiShotText;
                    perk2Icon.sprite = perkSystem.perkIcons.multiShotIcon;
                    
                    perk2Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 1:
                if (perkSystem.FireProjectiles == true)
                {
                    Perk2();
                    break;
                }

                else
                {
                    perk2Button.onClick.AddListener(perkSystem.FireProjectilesPerk);
                    perk2Text.text = perkSystem.fireProjectilesText;
                    perk2Icon.sprite = perkSystem.perkIcons.fireProjectilesIcon;
                    perk2Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 2:
                if (perkSystem.ExplosiveProjectiles == true)
                {
                    Perk2();
                    break;
                }

                else
                {
                    perk2Button.onClick.AddListener(perkSystem.ExplosiveProjectilesPerk);
                    perk2Text.text = perkSystem.explosiveProjectilesText;
                    perk2Icon.sprite = perkSystem.perkIcons.explosiveProjectilesIcon;
                    perk2Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 3:
                if (perkSystem.PiercingProjectiles == true)
                {
                    Perk2();
                    break;
                }

                else
                {
                    perk2Button.onClick.AddListener(perkSystem.PiercingProjectilesPerk);
                    perk2Text.text = perkSystem.piercingProjectilesText;
                    perk2Icon.sprite = perkSystem.perkIcons.piercingProjectilesIcon;
                    perk2Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 4:
                perk2Button.onClick.AddListener(perkSystem.AttackSpeedPerk);
                perk2Text.text = perkSystem.attackSpeedText;
                perk2Icon.sprite = perkSystem.perkIcons.attackSpeedIcon;
                perk2Button.onClick.AddListener(continueLevelUp);
                break;
            case 5:
                perk2Button.onClick.AddListener(perkSystem.DamagePerk);
                perk2Text.text = perkSystem.damageText;
                perk2Icon.sprite = perkSystem.perkIcons.damageIcon;
                perk2Button.onClick.AddListener(continueLevelUp);
                break;
            //case 6:
            //    perk2Button.onClick.AddListener(perkSystem.HealthUpgradePerk);
            //    perk2Text.text = perkSystem.healthUpgradeText;
            //    perk2Icon.sprite = perkSystem.perkIcons.healthUpgradeIcon;
            //    perk2Button.onClick.AddListener(continueLevelUp);
            //    break;
            //case 7:
            //    perk2Button.onClick.AddListener(perkSystem.HealthRestorePerk);
            //    perk2Text.text = perkSystem.healthRestoreText;
            //    perk2Icon.sprite = perkSystem.perkIcons.healthRestoreIcon;
            //    perk2Button.onClick.AddListener(continueLevelUp);
            //    break;
            case 6:
                if (perkSystem.LifeSteal == true)
                {
                    Perk2();
                    break;
                }

                else
                {
                    perk2Button.onClick.AddListener(perkSystem.LifeStealPerk);
                    perk2Text.text = perkSystem.lifeStealText;
                    perk2Icon.sprite = perkSystem.perkIcons.lifeStealIcon;
                    perk2Button.onClick.AddListener(continueLevelUp);
                    break;
                }
            case 7:
                perk2Button.onClick.AddListener(perkSystem.AmmoReloadSpeedPerk);
                perk2Text.text = perkSystem.ammoReloadSpeedText;
                perk2Icon.sprite = perkSystem.perkIcons.ammoReloadSpeedIcon;
                perk2Button.onClick.AddListener(continueLevelUp);
                break;
            case 8:
                perk2Button.onClick.AddListener(perkSystem.CriticalMasteryPerk);
                perk2Text.text = perkSystem.criticalMasteryText;
                perk2Icon.sprite = perkSystem.perkIcons.criticalMasteryIcon;
                perk2Button.onClick.AddListener(continueLevelUp);
                break;
            case 9:
                if (perkSystem.GlassCannon == true)
                {
                    Perk2();
                    break;
                }
                else
                {
                    perk2Button.onClick.AddListener(perkSystem.GlassCannonPerk);
                    perk2Text.text = perkSystem.glassCannonText;
                    perk2Icon.sprite = perkSystem.perkIcons.glassCannonIcon;
                    perk2Button.onClick.AddListener(continueLevelUp);
                    break;
                }
            case 10:
                if (perkSystem.burstUpgrade == 2)
                {
                    Perk2();
                    break;
                }
                else
                {
                    perk2Button.onClick.AddListener(perkSystem.BurstPerk);
                    perk2Text.text = perkSystem.burstText;
                    perk2Icon.sprite = perkSystem.perkIcons.burstIcon;
                    perk2Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 11:
                if (perkSystem.FrostProjectiles == true)
                {
                    Perk2();
                    break;
                }
                else
                {
                    perk2Button.onClick.AddListener(perkSystem.FrostProjectilesPerk);
                    perk2Text.text = perkSystem.frostProjectilesText;
                    perk2Icon.sprite = perkSystem.perkIcons.frostProjectilesIcon;
                    perk2Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 12:
                if (perkSystem.Berserk == true)
                {
                    Perk2();
                    break;
                }
                else
                {
                    perk2Button.onClick.AddListener(perkSystem.BerserkPerk);
                    perk2Text.text = perkSystem.berserkText;
                    perk2Icon.sprite = perkSystem.perkIcons.berserkIcon;
                    perk2Button.onClick.AddListener(continueLevelUp);
                    break;
                }
        }
    }

    void Perk3()
    {
        switch (Random.Range(0, 13))
        {
            case 0:
                if (perkSystem.multishotUpgrade == 1)
                {
                    perk3Button.onClick.AddListener(perkSystem.MultishotPerk);
                    perk3Text.text = perkSystem.multiShotText;
                    perk3Icon.sprite = perkSystem.perkIcons.multiShotIcon2;
                    perk3Button.onClick.AddListener(continueLevelUp);
                    break;
                }

                if (perkSystem.multishotUpgrade == 2)
                {
                    Perk3();
                    break;
                }

                else
                {
                    perk3Button.onClick.AddListener(perkSystem.MultishotPerk);
                    perk3Text.text = perkSystem.multiShotText;
                    perk3Icon.sprite = perkSystem.perkIcons.multiShotIcon;
                    perk3Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 1:
                if (perkSystem.FireProjectiles == true)
                {
                    Perk3();
                    break;
                }

                else
                {
                    perk3Button.onClick.AddListener(perkSystem.FireProjectilesPerk);
                    perk3Text.text = perkSystem.fireProjectilesText;
                    perk3Icon.sprite = perkSystem.perkIcons.fireProjectilesIcon;
                    perk3Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 2:
                if (perkSystem.ExplosiveProjectiles == true)
                {
                    Perk3();
                    break;
                }

                else
                {
                    perk3Button.onClick.AddListener(perkSystem.ExplosiveProjectilesPerk);
                    perk3Text.text = perkSystem.explosiveProjectilesText;
                    perk3Icon.sprite = perkSystem.perkIcons.explosiveProjectilesIcon;
                    perk3Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 3:
                if (perkSystem.PiercingProjectiles == true)
                {
                    Perk3();
                    break;
                }

                else
                {
                    perk3Button.onClick.AddListener(perkSystem.PiercingProjectilesPerk);
                    perk3Text.text = perkSystem.piercingProjectilesText;
                    perk3Icon.sprite = perkSystem.perkIcons.piercingProjectilesIcon;
                    perk3Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 4:
                perk3Button.onClick.AddListener(perkSystem.AttackSpeedPerk);
                perk3Text.text = perkSystem.attackSpeedText;
                perk3Icon.sprite = perkSystem.perkIcons.attackSpeedIcon;
                perk3Button.onClick.AddListener(continueLevelUp);
                break;
            case 5:
                perk3Button.onClick.AddListener(perkSystem.DamagePerk);
                perk3Text.text = perkSystem.damageText;
                perk3Icon.sprite = perkSystem.perkIcons.damageIcon;
                perk3Button.onClick.AddListener(continueLevelUp);
                break;
            //case 6:
            //    perk3Button.onClick.AddListener(perkSystem.HealthUpgradePerk);
            //    perk3Text.text = perkSystem.healthUpgradeText;
            //    perk3Icon.sprite = perkSystem.perkIcons.healthUpgradeIcon;
            //    perk3Button.onClick.AddListener(continueLevelUp);
            //    break;
            //case 7:
            //    perk3Button.onClick.AddListener(perkSystem.HealthRestorePerk);
            //    perk3Text.text = perkSystem.healthRestoreText;
            //    perk3Icon.sprite = perkSystem.perkIcons.healthRestoreIcon;
            //    perk3Button.onClick.AddListener(continueLevelUp);
            //    break;
            case 6:
                if (perkSystem.LifeSteal == true)
                {
                    Perk3();
                    break;
                }

                else
                {
                    perk3Button.onClick.AddListener(perkSystem.LifeStealPerk);
                    perk3Text.text = perkSystem.lifeStealText;
                    perk3Icon.sprite = perkSystem.perkIcons.lifeStealIcon;
                    perk3Button.onClick.AddListener(continueLevelUp);
                    break;
                }
            case 7:
                perk3Button.onClick.AddListener(perkSystem.AmmoReloadSpeedPerk);
                perk3Text.text = perkSystem.ammoReloadSpeedText;
                perk3Icon.sprite = perkSystem.perkIcons.ammoReloadSpeedIcon;
                perk3Button.onClick.AddListener(continueLevelUp);
                break;
            case 8:
                perk3Button.onClick.AddListener(perkSystem.CriticalMasteryPerk);
                perk3Text.text = perkSystem.criticalMasteryText;
                perk3Icon.sprite = perkSystem.perkIcons.criticalMasteryIcon;
                perk3Button.onClick.AddListener(continueLevelUp);
                break;
            case 9:
                if (perkSystem.GlassCannon == true)
                {
                    Perk3();
                    break;
                }
                else
                {
                    perk3Button.onClick.AddListener(perkSystem.GlassCannonPerk);
                    perk3Text.text = perkSystem.glassCannonText;
                    perk3Icon.sprite = perkSystem.perkIcons.glassCannonIcon;
                    perk3Button.onClick.AddListener(continueLevelUp);
                    break;
                }
            case 10:
                if (perkSystem.burstUpgrade == 2)
                {
                    Perk3();
                    break;
                }
                else
                {
                    perk3Button.onClick.AddListener(perkSystem.BurstPerk);
                    perk3Text.text = perkSystem.burstText;
                    perk3Icon.sprite = perkSystem.perkIcons.burstIcon;
                    perk3Button.onClick.AddListener(continueLevelUp);
                    break;
                }
            case 11:
                if (perkSystem.FrostProjectiles == true)
                {
                    Perk3();
                    break;
                }
                else
                {
                    perk3Button.onClick.AddListener(perkSystem.FrostProjectilesPerk);
                    perk3Text.text = perkSystem.frostProjectilesText;
                    perk3Icon.sprite = perkSystem.perkIcons.frostProjectilesIcon;
                    perk3Button.onClick.AddListener(continueLevelUp);
                    break;
                }

            case 12:
                if (perkSystem.Berserk == true)
                {
                    Perk3();
                    break;
                }
                else
                {
                    perk3Button.onClick.AddListener(perkSystem.BerserkPerk);
                    perk3Text.text = perkSystem.berserkText;
                    perk3Icon.sprite = perkSystem.perkIcons.berserkIcon;
                    perk3Button.onClick.AddListener(continueLevelUp);
                    break;
                }
        }
    }
}
