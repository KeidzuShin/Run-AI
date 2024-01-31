using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Text should be in camera/cinemachine")]
    public TextMeshProUGUI textHealth;

    [Header("Controlled by GameSetting.cs")]
    public float maxHealth;
    public float currentHealth;
    public float lowHealth;
    public float regenRate;    
    public GameSetting gameSetting;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameSetting = Resources.Load<GameSetting>("GameSetting");
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        maxHealth = gameSetting.maxHealth;
        currentHealth = gameSetting.currentHealth;
        lowHealth = gameSetting.lowHealth;
        regenRate = gameSetting.regenRate;
    }

    // Update is called once per frame
    void Update()
    {
        textHealth.SetText("Health : " + currentHealth + " / " + maxHealth);

        if (currentHealth < lowHealth && currentHealth != 0)
            currentHealth = Mathf.MoveTowards(currentHealth, lowHealth, regenRate * Time.deltaTime);

        if (currentHealth <= 0)
        {           
            currentHealth = 0;
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            Invoke(nameof(destroy), 3f);
        }            
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    public void destroy()
    {
        gameManager.GameOver();
    }
}
