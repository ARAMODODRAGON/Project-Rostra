﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farea : Enemy
{

    private float attackChance = 0; //Used to determine whether a skil or a normal attack will be used.
    private bool isThereADeadPlayer = false; //One of the Farea's skills relies on one of the players being dead
    private int bossPhase = 1; //Used to know which skills are available for the boss to use at which phase
    private float totalDamageSustained = 0.0f; //Used for Mother's Pain Skill. Becomes zero after the skill is used
    private float totalDamageThreshold = 400.0f; //When the Farea has sustained 300 or more damage and is in phase 2, it will do Mother's Pain Next
    private float skillChanceModifier = 1.0f; //Used in phase 2 to prioritize certain skills depending on the situation
    private Player thisPlayerIsDead; //Used as a reference for You Are Not Mine


    private enum fareaSkills //Only the skills that will require waiting for a number of turns before executing
    {
        none,
        wails,
        lullabyOfDepsair,
        mothersPain,
        youAreNotMine

    }
    private fareaSkills chosenSkill = fareaSkills.none;

    //J&W --> (The official competitor of A&W)
    public GameObject jObj;
    public GameObject wObj;

    //Wail
    public GameObject wailWait;
    private string[] statToDebuff; //Used to store all possible stats that WoF can debuff
    private string chosenStat; //Used to store the choice of the stat to debuff

    //Lullaby
    public GameObject lullWait;

    //MothersPain
    public GameObject mothersPainWait;


    protected override void Start()
    {
        battleManager = BattleManager.instance;
        objPooler = ObjectPooler.instance;
        uiBTL = UIBTL.instance;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteColor = spriteRenderer.color;
        animator = gameObject.GetComponent<Animator>();

        damageText.gameObject.SetActive(false);
        healText.gameObject.SetActive(false);

        haveAddedMyself = false;
        hit = false;
        dead = false;
        
        currentState = EnemyState.idle;
        waitTurnsText.gameObject.SetActive(false);
        jObj.gameObject.SetActive(false);
        wObj.gameObject.SetActive(false);
        wailWait.gameObject.SetActive(false);
        lullWait.gameObject.SetActive(false);
        mothersPainWait.gameObject.SetActive(false);

        statToDebuff = new string[2];
        statToDebuff[0] = "Defense";
        statToDebuff[1] = "Attack";
    }


    protected override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(float playerAttack, int numberOfAttacks)
    {
       
        Debug.Log("Received player attack: " + playerAttack);
        float damage = playerAttack - ((eDefence / (20.0f + eDefence)) * playerAttack);
        currentHP -= damage;
        totalDamageSustained += damage;
        damageText.gameObject.SetActive(true);
        damageText.text = Mathf.RoundToInt(damage).ToString();
        battleManager.enemies[enemyIndexInBattleManager].currentHP = currentHP; //Update the BTL manager with the new health
        HP.fillAmount = currentHP / maxHP;

        if (currentState != EnemyState.waiting)
        {
            animator.SetBool("Hit", true);
        }

        if (currentHP <= 0.0f)
        {
            if(bossPhase == 1)
            {
                //Reset and get ready for phase 2
                currentState = EnemyState.idle;
                waitTime = 0;
                waitTurnsText.text = "0";
                chosenSkill = fareaSkills.none;
                lullWait.gameObject.SetActive(false);
                wailWait.gameObject.SetActive(false);
                animator.SetBool("Phase2", true);
            }
            animator.SetBool("Death", true);
        }
        else
        {
            if (numberOfAttacks <= 0)
            {
                uiBTL.EndTurn(); //Only end the turn after the damage has been taken
            }
        }
    }

    private void StartPhase2()
    {
        uiBTL.EndTurn(); //End the player's turn
        bossPhase = 2;
        maxHP *= 2;
        currentHP = maxHP;
        eAttack *= 1.2f;
        eDefence *= 1.2f;
        eAgility *= 1.2f;
        eStrength *= 1.2f;
        
        HP.fillAmount = 1.0f;
        animator.SetBool("Hit", false);
        animator.SetBool("Death", false);
        animator.SetInteger("WaitingIndex", 0);
        waitTurnsText.gameObject.SetActive(false);
    }

    public override void EnemyTurn()
    {
        //Check if we're waiting on a skill first
        if (currentState == EnemyState.waiting)
        {
            
            waitQTurns--;
            waitTurnsText.text = waitQTurns.ToString(); //Update the UI
            if (waitQTurns <= 0)
            {
                waitTurnsText.gameObject.SetActive(false); //Turn off the text. Don't forget to enable it when the enemy goes to waiting state
                currentState = EnemyState.idle; //Change the state
                animator.SetInteger("WaitingIndex", 1); //After waiting, the index is always going to be 1
       }
            else
            {
                //End the turn
                uiBTL.EndTurn();
            }

        }
        else
        {
                //Only update the attackChance when no skill is on the waiting list
                attackChance = Random.Range(0.0f, 100.0f);
                //attackChance = 30; //Testing

            if (bossPhase == 1)
            {
                if (attackChance >= 0.0f && attackChance < 10.0f) //Normal attack
                {
                    DumbAttack();
                }
                else if (attackChance >= 10.0f && attackChance < 40.0f) //Wails of Frailty
                {
                    GoToWaitState(fareaSkills.wails, 2, 2);
                    //Summon skill effect
                    wailWait.gameObject.SetActive(true);
                    uiBTL.UpdateActivityText("Wails of Frailty");
                }
                else if(attackChance >= 40.0f && attackChance < 70.0f) //Judgment and Wrath
                {
                    attackThisPlayer = battleManager.players[Random.Range(0, 4)].playerReference;
                    animator.SetBool("JudgementAndWrath", true);
                    uiBTL.UpdateActivityText("Judgement & Wrath");
                }
                else if (attackChance >= 70.0f && attackChance <= 100.0f) // Lullaby Of Despair
                {
                    GoToWaitState(fareaSkills.lullabyOfDepsair, 1, 2);
                    //Summon skill effect
                    lullWait.gameObject.SetActive(true);
                    uiBTL.UpdateActivityText("Lullaby Of Despair");
                }
                    
            }
            else if(bossPhase == 2)
            {
                if (totalDamageSustained >= totalDamageThreshold)
                {
                    //If the total damage sustained is over the threshold, Mother's Pain must be done in 2 turns rathe than 3
                    GoToWaitState(fareaSkills.mothersPain, 1, 3);
                    //Summon skill effect
                    mothersPainWait.gameObject.SetActive(true);
                    uiBTL.UpdateActivityText("Mother's Pain");
                }
                else
                {
                    //Check if one of the players is dead --> if yes, there's a chance to use You Are Not Mine
                    for (int i = 0; i < uiBTL.playersDead.Length; i++)
                    {
                        if (uiBTL.playersDead[i] == true)
                        {
                            isThereADeadPlayer = true;
                            thisPlayerIsDead = battleManager.players[i].playerReference;
                            break;
                        }
                    }
                    //attackChance = Random.Range(0.0f, 100.0f);
                    attackChance = 30; //Testing
                    if(attackChance>= 0.0f && attackChance < 20.0f * skillChanceModifier && isThereADeadPlayer)
                    {
                        //You are not mine
                    }
                    else if(attackChance >= 20.0f && attackChance < 40.0f * skillChanceModifier)
                    {
                        GoToWaitState(fareaSkills.mothersPain, 1, 3);
                        //Summon skill effect
                        mothersPainWait.gameObject.SetActive(true);
                        uiBTL.UpdateActivityText("Mother's Pain");
                    }
                    else if(attackChance >= 40.0f && attackChance < 60.0f * skillChanceModifier)
                    {
                        //Deadly Ties
                    }
                    else if (attackChance >= 60.0f && attackChance < 70.0f) //Wails of Frailty
                    {
                        GoToWaitState(fareaSkills.wails, 2, 2);
                        //Summon skill effect
                        wailWait.gameObject.SetActive(true);
                        uiBTL.UpdateActivityText("Wails of Frailty");
                    }
                    else if (attackChance >= 70.0f && attackChance < 80.0f) //Judgment and Wrath
                    {
                        attackThisPlayer = battleManager.players[Random.Range(0, 4)].playerReference;
                        animator.SetBool("JudgementAndWrath", true);
                        uiBTL.UpdateActivityText("Judgement & Wrath");
                    }
                    else if (attackChance >= 80.0f && attackChance <= 90.0f) // Lullaby Of Despair
                    {
                        GoToWaitState(fareaSkills.lullabyOfDepsair, 1, 2);
                        //Summon skill effect
                        lullWait.gameObject.SetActive(true);
                        uiBTL.UpdateActivityText("Lullaby Of Despair");
                    }
                    else if (attackChance >= 90.0f && attackChance <= 100.0f) //Normal attack
                    {
                        DumbAttack();
                    }
                }
            }
        }
    }


    //-----------------------------Skills------------------------------//
    private void GoToWaitState(fareaSkills skill, int turnsToWait, int waitingIndex)
    {
        chosenSkill = skill;
        waitQTurns = turnsToWait;
        waitTurnsText.gameObject.SetActive(true);
        waitTurnsText.text = waitQTurns.ToString();
        animator.SetInteger("WaitingIndex", waitingIndex);
        currentState = EnemyState.waiting;
        uiBTL.EndTurn();
    }


    //Judgement & Wrath
    private void Judgement()
    {
        //Disable J and apply the damage.
        //Should change this to use animations
        objPooler.SpawnFromPool("JudgeAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);
        jObj.gameObject.SetActive(false);
        attackThisPlayer.TakeDamage(eAttack);
    }

    private void Wrath()
    {
        //Disable W and apply the damage.
        //Should change this to use animations
        objPooler.SpawnFromPool("WrathAttack", attackThisPlayer.gameObject.transform.position, gameObject.transform.rotation);
        wObj.gameObject.SetActive(false);
        attackThisPlayer.TakeDamage(eAttack * 1.2f); //Wrath does a little more damage
        animator.SetBool("JudgementAndWrath", false);
        uiBTL.DisableActivtyText();
        uiBTL.EndTurn();
    }

    private void JudgementAndWrathEffect()
    {
        jObj.gameObject.SetActive(true);
        wObj.gameObject.SetActive(true);
    }

    //Called from the animator to check which skill to execute after waiting

    private void SkillEffect()
    {
        switch (chosenSkill)
        {
            case fareaSkills.wails:
                //Choose at random whether you want to damage attack or defense
                chosenStat = statToDebuff[Random.Range(0, statToDebuff.Length)];
                //Update the UI
                uiBTL.UpdateActivityText("Wails of Frailty");
                //Damage all players
                for (int i = 0; i < battleManager.players.Length; i++)
                {
                    if(!battleManager.players[i].playerReference.dead)
                    {
                        battleManager.players[i].playerReference.TakeDamage(eAttack, 1, chosenStat, Player.playerAilments.none, null, 0.3f, 3, true);
                        //Summon debuff object
                        objPooler.SpawnFromPool("WailAttack", battleManager.players[i].playerReference.transform.position, gameObject.transform.rotation);
                    }
                }
                wailWait.gameObject.SetActive(false);

                break;
            case fareaSkills.lullabyOfDepsair:
                //Update the UI
                uiBTL.UpdateActivityText("Lullaby Of Despair");
                //Choose a player at random
                attackThisPlayer = battleManager.players[Random.Range(0, battleManager.players.Length)].playerReference;

                //If the player has no ailments, then affect thme with fear
                if(attackThisPlayer.currentAilment == Player.playerAilments.none) //Make sure you target 
                {
                    attackThisPlayer.TakeDamage(0.0f, 0, "", Player.playerAilments.fear, null, 0, 3, false);
                }
                else
                {
                    //Otherwise just damage the player
                    attackThisPlayer.TakeDamage(eAttack * 1.5f);
                }
                lullWait.gameObject.SetActive(false);

                break;
            case fareaSkills.mothersPain:
                for (int i = 0; i < battleManager.players.Length; i++)
                {
                    if (!battleManager.players[i].playerReference.dead)
                    {
                        battleManager.players[i].playerReference.TakeDamage(0.5f* totalDamageSustained); //Damage the players with half the totalDamageSustained

                        objPooler.SpawnFromPool("MothersPain", gameObject.transform.position, gameObject.transform.rotation);
                    }
                }
                totalDamageSustained = 0.0f; //Reset the variable
                mothersPainWait.gameObject.SetActive(false);
                break;
            case fareaSkills.youAreNotMine:
                break;
        }

        //Reset the skill and the animator
        chosenSkill = fareaSkills.none;
        animator.SetInteger("WaitingIndex", 0);
        uiBTL.EndTurn();

    }
}
