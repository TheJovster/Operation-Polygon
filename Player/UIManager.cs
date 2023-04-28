using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationPolygon.Combat;
using TMPro;
using UnityEngine.UI;
using OperationPolygon.Core;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set;}
    
    private Weapon currentWeapon; //probably not the best practice, but this is done for testing purposes and will be refactored later.
    private FragCount fragCount;
    [Header("TMPro Components")]
    [SerializeField] private TextMeshProUGUI ammoCount; //refactor names and everything later.
    [SerializeField] private TextMeshProUGUI ammoInInventoryCount;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI currentWeaponText;
    [SerializeField] private TextMeshProUGUI fragCountText;
    [SerializeField] private TextMeshProUGUI healthPacks;
    [Header("Image Components")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image staminaBar;
    [Header("Menu Objects")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject victoryMenu;

    private AmmoInventory ammoInventory;
    private ItemInventory itemInventory;
    private WeaponInventory weaponInventory;
    private GameManager gameManager;
    private Health health;
    private Stamina stamina;

    private void Awake()
    {
        Instance = this;
        health = GetComponent<Health>();
        stamina = GetComponent<Stamina>();
        gameManager = FindObjectOfType<GameManager>();
        fragCount = GetComponent<FragCount>();
        itemInventory = GetComponent<ItemInventory>();
        weaponInventory = GetComponent<WeaponInventory>();
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        victoryMenu.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentWeapon = weaponInventory.CurrentWeapon;
        ammoInventory = GetComponent<AmmoInventory>();
        GetWeaponName();
    }

    public void GetWeaponName()
    {
        currentWeaponText.text = weaponInventory.CurrentWeapon.WeaponName;
    }

    // Update is called once per frame
    void Update()
    {
        ammoCount.text = currentWeapon.GetCurrentAmmoInMag().ToString();
        ammoInInventoryCount.text = ammoInventory.GetCurrentAmmoInInventory().ToString();
        healthBar.fillAmount = health.GetHealthPercentage();
        staminaBar.fillAmount = stamina.GetStaminaPercentage();
        timer.text = gameManager.GetCurrentTime().ToString("0.0");
        fragCountText.text = fragCount.GetFragCount().ToString();
        healthPacks.text = itemInventory.NumberOfHealthPacks.ToString();
        //This may seem inconsistent - that's because I didn't set FragCount as a property, but instead
        //the int is being retrieved through a getter method.
    }

    public void ShowPauseMenu() 
    {
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }
    
    public void HidePauseMenu() 
    {
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowGameOverScreen() 
    {
        gameOverMenu.SetActive(true);
    }

    public void ShowVictoryScreen() 
    {
        victoryMenu.SetActive(true);
    }

    public void InitalizeUI() 
    {
        currentWeapon = weaponInventory.CurrentWeapon;
        ammoInventory = GetComponent<AmmoInventory>();
    }
}
