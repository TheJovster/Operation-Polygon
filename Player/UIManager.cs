using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationPolygon.Combat;
using TMPro;
using UnityEngine.UI;
using OperationPolygon.Core;

public class UIManager : MonoBehaviour
{
    
    private Weapon currentWeapon; //probably not the best practice, but this is done for testing purposes and will be refactored later.
    [Header("TMPro Components")]
    [SerializeField] private TextMeshProUGUI ammoCount; //refactor names and everything later.
    [SerializeField] private TextMeshProUGUI ammoInInventoryCount;
    [SerializeField] private TextMeshProUGUI timer;
    [Header("Image Components")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image staminaBar;
    [Header("Menu Objects")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject victoryMenu;

    private AmmoInventory ammoInventory;
    private GameManager gameManager;
    private Health health;
    private Stamina stamina;

    private void Awake()
    {
        health = GetComponent<Health>();
        stamina = GetComponent<Stamina>();
        gameManager = FindObjectOfType<GameManager>();
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentWeapon = GameObject.FindGameObjectWithTag("PlayerWeapon").GetComponent<Weapon>();
        ammoInventory = GetComponent<AmmoInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        ammoCount.text = currentWeapon.GetCurrentAmmoInMag().ToString();
        ammoInInventoryCount.text = ammoInventory.GetCurrentAmmoInInventory().ToString();
        healthBar.fillAmount = health.GetHealthPercentage();
        staminaBar.fillAmount = stamina.GetStaminaPercentage();
        timer.text = gameManager.GetCurrentTime().ToString("0.0");
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
}
