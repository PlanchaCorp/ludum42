﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusHandler : MonoBehaviour {
    /// <summary>
    /// Character game object
    /// </summary>
    private GameObject player;

    /// <summary>
    /// Dictionary of active bonuses
    /// </summary>
    private Dictionary<string, float> activeBonuses = new Dictionary<string, float>();

    /// <summary>
    /// Speed boost duration
    /// </summary>
    [SerializeField]
    private float speedBoostDuration = 10f;

    /// <summary>
    /// Digging boost duration
    /// </summary>
    [SerializeField]
    private float diggingBoostDuration = 15f;

    /// <summary>
    /// Respiration boost duration
    /// </summary>
    [SerializeField]
    private float respirationBoostDuration = 20f;

    /// <summary>
    /// Pickup distance from which a bonus can be picked up
    /// </summary>
    [SerializeField]
    private float pickupDistance = 1.0f;
    
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update ()
    {
		foreach(KeyValuePair<string, float> activeBonus in activeBonuses)
        {
            if (activeBonus.Value < Time.time)
            {
                TriggerBonus(false, activeBonus.Key);
            }
        }
        LookBeneath();
    }

    /// <summary>
    /// Activate speed boost bonus !
    /// </summary>
    public void ActivateSpeedBoost()
    {
        TriggerBonus(true, "speedBoost1", speedBoostDuration);
    }

    /// <summary>
    /// Activate digging boost bonus
    /// </summary>
    public void ActivateDiggingBoost()
    {
        TriggerBonus(true, "diggingBoost1", diggingBoostDuration);
    }

    /// <summary>
    /// Activate respiration boost bonus
    /// </summary>
    public void ActivateRespirationBoost()
    {
        TriggerBonus(true, "respirationBoost1", respirationBoostDuration);
    }

    /// <summary>
    /// Activate or deactivate a bonus
    /// </summary>
    /// <param name="state">Activation or deactivation</param>
    /// <param name="bonus">Name of the bonus</param>
    private void TriggerBonus(bool state, string bonus, float value = 0)
    {
        Debug.Log(((state) ? "Activated" : "Desactivated") + " bonus " + bonus);
        switch (bonus)
        {
            case "speedBoost1":
                player.GetComponent<PlayerMovement>().SetQuarterSpeedBonus(state);
                break;
            case "diggingBoost1":
                player.GetComponent<PlayerAction>().SetDiggingBoost(state);
                break;
            case "respirationBoost1":
                player.GetComponent<PlayerAction>().SetRespirationBoost(state);
                break;
        }
        if (state)
        {
            if (activeBonuses.ContainsKey(bonus))
            {
                activeBonuses.Remove(bonus);
            }
            activeBonuses.Add(bonus, Time.time + value);
        } else
        {
            activeBonuses.Remove(bonus);
        }
    }

    /// <summary>
    /// Check under the player if an object is present
    /// </summary>
    private void LookBeneath()
    {
        GameObject[] bonuses = GameObject.FindGameObjectsWithTag("Bonus");
        foreach (GameObject bonus in bonuses)
        {
            float distance = Mathf.Sqrt(Mathf.Pow(player.transform.position.x - bonus.transform.position.x, 2) + Mathf.Pow(player.transform.position.y - bonus.transform.position.y, 2));
            if (distance < pickupDistance)
            {
                if (bonus.name.Contains("TileBoots"))
                {
                    ActivateSpeedBoost();
                } else if (bonus.name.Contains("TileMarteauPiqueur"))
                {
                    ActivateDiggingBoost();
                } else if (bonus.name.Contains("TileScaphandre"))
                {
                    ActivateRespirationBoost();
                }
                Destroy(bonus);
            }
        }
    }
}
