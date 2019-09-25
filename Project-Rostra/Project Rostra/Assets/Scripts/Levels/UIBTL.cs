﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIBTL : MonoBehaviour
{
    //Instances
    private BattleManager btlManager;
    private MainInventory inventory;

    public GameObject playerTurnIndicator;
    public GameObject enemyToAttackIndicator;
    public GameObject playerIndicatorPos0;
    public GameObject playerIndicatorPos1;
    public GameObject playerIndicatorPos2;
    public GameObject playerIndicatorPos3;
    public GameObject[] enemyIndicatorPosArray = new GameObject[5];
    public GameObject controlsPanel; //Needs to be disabled after choosing a command and re-enabled when it's a player's turn
    public GameObject rageModeIndicator1;
    public GameObject rageModeIndicator2;
    public GameObject highlighter;
    public GameObject highlighterPos0;
    public GameObject highlighterPos1;
    public GameObject highlighterPos2;
    public GameObject highlighterPos3;
    public GameObject highlighterPos4; //Used to go back to the basic menu when inside the skills/items menu
    private int controlsIndicator; //Used to know which command has been chosen
    private int enemyIndicatorIndex;//Used to know which enemy is being chosen to be attacked
    private int activeRange; // Are we using the player's standard range of a skill's range?
    private int previousEnemyIndicatorIndex; //Used to limit calls to become less visible
    [HideInInspector]
    public int currentPlayerTurnIndex; //Updated from the btl manager to know which player turn it is

    //Control Panel
    public Text rageText; //Needs to be disabled when it's not usable
    public Text playerName;
    public Text attackText;
    public Text skillsText;
    public Text itemsText;
    public Text guardText;
    public Text currentHP;
    public Text maxHP;
    public Image hpBar;
    public Text currentMP;
    public Text maxMP;
    public Image mpBar;

    //Skills Control Panel
    public GameObject skillsPanel;
    public Text skillsTextDescription;

    //Items Control Panel
    public GameObject itemsPanel;
    public Text itemDescription;
    public Sprite[] itemIcons; //Stores the icon of each item --> Temporary until the itemIcons array is ready in the MainInventory
    public Image[] itemIconsInPanel; //Stores references to the icons inside the panel --> They change if we scroll down
    public Text[] itemNames; //Displayes item name
    public Text[] itemCount; //Displays item count
    public Image itemsHighlighter; //Moves with the itemsPanelIndex to tell which item the player is currently using
    public GameObject itemsHPos0;
    public GameObject itemsHPos1;
    public GameObject itemsHPos2;
    private int itemHPosIndex;
    public GameObject upArrow;
    public GameObject downArrow;
    private int itemsPanelIndex; //Which item are we at now? Also represents the item ID since items will be ordered based on their IDs

    //Q UI Images

    private List<Sprite> imagesQ; //Filled by the BTL manager
    public Image[] images = new Image[9];
    private Vector2 imageRecyclePos; //To which position do images go when recycled?
    private Vector2 targetPos; //Used to calculate the distance each image travels in the Q
    private float imageMovementSpeed;
    private float imageMaxDistance; //Distance to be moved by each image
    private bool moveImagesNow; //Toggled on end turn and when the first image hits the recycler

    //States
    private enum btlUIState
    {
        choosingBasicCommand, //Player still choosing which command to use
        choosingSkillsCommand, //Player chooses between skills
        choosingItemsCommand, //Player chooses items
        choosingEnemy, //Player has chosen an offense command
        choosingPlayer, //Player has chosen a supporting command
        battleEnd
    }

    private btlUIState currentState;
    private btlUIState previousState; //Needed to know if we're choosing a player to use an item or use a skill

    //Enemies
    public Enemy[] enemies;
    public bool [] enemiesDead;
    public int numberOfEnemies;

    //Players
    private Player playerInControl;
    private int playerIndicatorIndex; //Used to keep track where the playerindicator is when using items or skills

    //End Battle Screen
    private bool battleHasEnded;
    public GameObject endScreenPanel;
    public Text fargasLevelUpBack;
    public Text fargasLevelUpFore;
    public Image fargasHP;
    public Image fargasMP;
    public Image fargasExp;
    private float fargasCurrentExp;
    private float fargasMaxExp;
    private float fargasExpStep; //Used to know by how much to increase the exp bar == 1/maxExp
    private int fargasExpGain;
    private bool fargasAddinExp; //Used in update to increase EXP bar
   
    public Text freaLevelUpBack;
    public Text freaLevelUpFore;
    public Image freaHP;
    public Image freaMP;
    public Image freaExp;
    private float freaCurrentExp;
    private float freaMaxExp;
    private float freaExpStep;
    private int freaExpGain;
    private bool freaAddinExp;

    public Text arcelusLevelUpBack;
    public Text arcelusLevelUpFore;
    public Image arcelusHP;
    public Image arcelusMP;
    public Image arcelusExp;
    private float arcelusCurrentExp;
    private float arcelusMaxExp;
    private float arcelusExpStep;
    private int arcelusExpGain;
    private bool arcelusAddinExp;

    public Text oberonLevelUpBack;
    public Text oberonLevelUpFore;
    public Image oberonHP;
    public Image oberonMP;
    public Image oberonExp;
    private float oberonCurrentExp;
    private float oberonMaxExp;
    private float oberonExpStep;
    private int oberonExpGain;
    private bool oberonAddinExp;

    //Dialogue after battle
    public static bool conversationAfterBattle = false; //If true, a conversation will start right after the battle ends
    private DialogueManager dialogueManager;
    private DialogueContainer dialogueContainer;

    #region singleton
    public static UIBTL instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


    }
    #endregion

    void Start()
    {
        btlManager = BattleManager.instance;
        inventory = MainInventory.invInstance;

        enemies = new Enemy[5]; //Filled by the BTL Manager in Add Enemy
        enemiesDead = new bool[5]; //Every entry is turned to true by the enemy that dies
        controlsIndicator = 0; //Start at Attack
        highlighter.transform.position = highlighterPos0.transform.position;
       previousState = currentState = btlUIState.choosingBasicCommand;
        playerName.text = "";
        enemyToAttackIndicator.gameObject.SetActive(false);
        playerTurnIndicator.SetActive(false);
        controlsPanel.gameObject.SetActive(false);
        enemyIndicatorIndex = 0;
        previousEnemyIndicatorIndex = 0;
        activeRange = 0;

        imagesQ = new List<Sprite>();
        imageRecyclePos = images[8].gameObject.transform.localPosition;
        targetPos = images[0].transform.localPosition;

        imageMovementSpeed = 250.0f;
        imageMaxDistance = 149.0f;
        moveImagesNow = false;

        //Rage mode is not available when the battle starts
        rageText.color = Color.gray;
        rageModeIndicator1.gameObject.SetActive(false);
        rageModeIndicator2.gameObject.SetActive(false);

        //All the enemies are alive at the beginning
        for(int i=0;i<enemiesDead.Length;i++)
        {
            enemiesDead[i] = false;
        }

        //End Battle
        battleHasEnded = false;
        endScreenPanel.SetActive(false);
        fargasLevelUpBack.gameObject.SetActive(false);
        fargasLevelUpFore.gameObject.SetActive(false);
        freaLevelUpBack.gameObject.SetActive(false);
        freaLevelUpFore.gameObject.SetActive(false);
        arcelusLevelUpBack.gameObject.SetActive(false);
        arcelusLevelUpFore.gameObject.SetActive(false);
        oberonLevelUpBack.gameObject.SetActive(false);
        oberonLevelUpFore.gameObject.SetActive(false);

        //Skills
        skillsPanel.gameObject.SetActive(false);

        //Items
        itemsPanel.gameObject.SetActive(false);
        itemHPosIndex = 0;

        playerIndicatorIndex = 0;

        //Dialogue after battle
        dialogueManager = DialogueManager.instance;
        dialogueContainer = DialogueContainer.instance;
    }


    void Update()
    {
        if(moveImagesNow)
        {
            //Called on End Turn
            moveQImages();
            
        }

        switch(currentState)
        {
            case btlUIState.choosingBasicCommand:
                choosingBasicCommand();
                break;
            case btlUIState.choosingSkillsCommand:
                choosingSkillsCommand();
                break;
            case btlUIState.choosingItemsCommand:
                choosingItemsCommand();
                break;
            case btlUIState.choosingEnemy:
                choosingEnemy();
                break;
            case btlUIState.choosingPlayer:
                choosingPlayer();
                break;
            case btlUIState.battleEnd:
                EndBattleUI();
                break;
        }
    }

    public void AddImageToQ(Sprite nextOnQImage)
    {
        //Called from the BTL manager when adding characters to the Q

        //Debug.Log("Adding image on Q");

        imagesQ.Add(nextOnQImage);
    }

    public void QueueIsReady()
    {
        //Called from the BTL manager when the Q has been built

        //Fill up the Q until its of size 9. Only 6 will be on screen at a time however.
        switch(imagesQ.Count)
        {
            //Minimum size of Q is 5 since we will not be removing the images when characters die
            //Change the image recycler position depending on the size of the Q
            case 5:
                imageRecyclePos = images[4].transform.localPosition;
                images[5].gameObject.SetActive(false);
                images[6].gameObject.SetActive(false);
                images[7].gameObject.SetActive(false);
                images[8].gameObject.SetActive(false);

                for(int i =0;i<5;i++)
                {
                    //0 - 4
                    images[i].sprite = imagesQ[i];
                }


                break;
            case 6:
                imageRecyclePos = images[5].transform.localPosition;
                images[6].gameObject.SetActive(false);
                images[7].gameObject.SetActive(false);
                images[8].gameObject.SetActive(false);

                for (int i = 0; i < 6; i++)
                {
                    //0 - 5
                    images[i].sprite = imagesQ[i];
                }
                break;
            case 7:
                imageRecyclePos = images[6].transform.localPosition;
                images[7].gameObject.SetActive(false);
                images[8].gameObject.SetActive(false);

                for (int i = 0; i < 7; i++)
                {
                    //0 - 6
                    images[i].sprite = imagesQ[i];
                }


                break;
            case 8:
                imageRecyclePos = images[7].transform.localPosition;
                images[8].gameObject.SetActive(false);
                for (int i = 0; i < 8; i++)
                {
                    //0 - 7
                    images[i].sprite = imagesQ[i];
                }
                break;
        }

    }

    public void moveQImages()
    {

        //Once the images start moving, turn off the indicator next to the "RAGE" word and return the text color to normal if the previous player was in rage
        if (playerInControl.currentState == Player.playerState.Rage)
        {
            rageModeIndicator1.gameObject.SetActive(false);
            rageModeIndicator2.gameObject.SetActive(false);
            skillsText.color = Color.white;
            itemsText.color = Color.white;
            guardText.color = Color.white;
        }

        //Move all the images an amount of imageMaxDistance to the right

        targetPos.x = images[0].transform.localPosition.x + imageMaxDistance;
        images[0].transform.localPosition = Vector2.MoveTowards(images[0].transform.localPosition, targetPos, imageMovementSpeed * Time.deltaTime);

        targetPos.x = images[1].transform.localPosition.x + imageMaxDistance;
        images[1].transform.localPosition = Vector2.MoveTowards(images[1].transform.localPosition, targetPos , imageMovementSpeed * Time.deltaTime);

        targetPos.x = images[2].transform.localPosition.x + imageMaxDistance;
        images[2].transform.localPosition = Vector2.MoveTowards(images[2].transform.localPosition, targetPos , imageMovementSpeed * Time.deltaTime);

        targetPos.x = images[3].transform.localPosition.x + imageMaxDistance;
        images[3].transform.localPosition = Vector2.MoveTowards(images[3].transform.localPosition, targetPos , imageMovementSpeed * Time.deltaTime);

        targetPos.x = images[4].transform.localPosition.x + imageMaxDistance;
        images[4].transform.localPosition = Vector2.MoveTowards(images[4].transform.localPosition, targetPos , imageMovementSpeed * Time.deltaTime);

        targetPos.x = images[5].transform.localPosition.x + imageMaxDistance;
        images[5].transform.localPosition = Vector2.MoveTowards(images[5].transform.localPosition, targetPos , imageMovementSpeed * Time.deltaTime);

        targetPos.x = images[6].transform.localPosition.x + imageMaxDistance;
        images[6].transform.localPosition = Vector2.MoveTowards(images[6].transform.localPosition, targetPos , imageMovementSpeed * Time.deltaTime);

        targetPos.x = images[7].transform.localPosition.x + imageMaxDistance;
        images[7].transform.localPosition = Vector2.MoveTowards(images[7].transform.localPosition, targetPos , imageMovementSpeed * Time.deltaTime);

        targetPos.x = images[8].transform.localPosition.x + imageMaxDistance;
        images[8].transform.localPosition = Vector2.MoveTowards(images[8].transform.localPosition, targetPos , imageMovementSpeed * Time.deltaTime);

    }

    //Called when the image at the far right of the Q collides with the recycle image collider
    public void imageRecycle(int imageIndex)
    {
        //We've hit the recycler, stop moving!
        moveImagesNow = false;

        switch (imageIndex) //Which image hit the recycler?
        {
            case 0:
                images[0].gameObject.transform.localPosition = imageRecyclePos;
                break;
            case 1:
                images[1].gameObject.transform.localPosition = imageRecyclePos;
                break;
            case 2:
                images[2].gameObject.transform.localPosition = imageRecyclePos;
                break;
            case 3:
                images[3].gameObject.transform.localPosition = imageRecyclePos;
                break;
            case 4:
                images[4].gameObject.transform.localPosition = imageRecyclePos;
                break;
            case 5:
                images[5].gameObject.transform.localPosition = imageRecyclePos;
                break;
            case 6:
                images[6].gameObject.transform.localPosition = imageRecyclePos;
                break;
            case 7:
                images[7].gameObject.transform.localPosition = imageRecyclePos;
                break;
            case 8:
                images[8].gameObject.transform.localPosition = imageRecyclePos;
                break;

        }
    }

    //Called from the BTL Manager to update the UI based on which player's turn it is
    public void showThisPlayerUI(int playerIndex, string name, Player playerReference)
    {
        if(playerReference.currentState != Player.playerState.Waiting && !battleHasEnded)
        {
            currentState = btlUIState.choosingBasicCommand;
        }

        //Debug.Log("Player index " + playerIndex);

        playerTurnIndicator.SetActive(true);
        controlsPanel.gameObject.SetActive(true);

        playerName.text = name;
        playerInControl = playerReference;
        UpdatePlayerHPControlPanel();
        UpdatePlayerMPControlPanel();
        playerInControl.PlayerTurn();
        RageOptionTextColor();

       
        //Turn on the indicator if the player is in rage mode
        if(playerInControl.currentState==Player.playerState.Rage)
        {
            rageModeIndicator1.gameObject.SetActive(true);
            rageModeIndicator2.gameObject.SetActive(true);
            skillsText.color = Color.grey;
            itemsText.color = Color.grey;
            guardText.color = Color.grey;
            rageText.color = Color.yellow;
        }
        else
        {
            rageModeIndicator1.gameObject.SetActive(false);
            rageModeIndicator2.gameObject.SetActive(false);
            skillsText.color = Color.white;
            itemsText.color = Color.white;
            guardText.color = Color.white;
            RageOptionTextColor();
        }

        switch (playerIndex)
        {
            case 0:
                playerTurnIndicator.transform.position = playerIndicatorPos0.transform.position;
                break;
            case 1:
                playerTurnIndicator.transform.position = playerIndicatorPos1.transform.position;
                break;
            case 2:
                playerTurnIndicator.transform.position = playerIndicatorPos2.transform.position;
                break;
            case 3:
                playerTurnIndicator.transform.position = playerIndicatorPos3.transform.position;
                break;
        }
    }

    private void choosingBasicCommand()
    {
        //Debug.Log("Choosing Basic Commands");
        if (!moveImagesNow) //Don't allow the player to choose a command until the Q has settled down
        {
            switch (controlsIndicator)
            {
                //If the player is in rage mode, only "Attack" can be chosen
                case 0://Highlighter is at attack
                    if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow)) && playerInControl.currentState!=Player.playerState.Rage)
                    {
                        controlsIndicator = 1;
                        highlighter.transform.position = highlighterPos1.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow) && playerInControl.currentState != Player.playerState.Rage)
                    {
                        controlsIndicator = 2;
                        highlighter.transform.position = highlighterPos2.transform.position;
                    }
                    else if(Input.GetKeyDown(KeyCode.LeftArrow) && playerInControl.canRage && playerInControl.currentState != Player.playerState.Rage)
                    {
                        controlsIndicator = 4;
                        highlighter.transform.position = highlighterPos4.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.Space)) //Player has chosen attack
                    {
                        currentState = btlUIState.choosingEnemy;
                        enemyToAttackIndicator.SetActive(true);
                        activeRange = playerInControl.range;

                        //Make sure the indicator starts at an alive enemy
                        for(int i =0;i<enemiesDead.Length;i++)
                        {
                            if(enemiesDead[i] == false)
                            {
                                enemyToAttackIndicator.transform.position = enemyIndicatorPosArray[i].transform.position;
                                enemyIndicatorIndex = i;
                                break;
                            }
                        }

                        MakeChosenEnemyMorePrompt(enemyIndicatorIndex);
                    }
                    break;

                case 1://Highlighter is at Guard
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        controlsIndicator = 0;
                        highlighter.transform.position = highlighterPos0.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        controlsIndicator = 3;
                        highlighter.transform.position = highlighterPos3.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.Space)) //Player has chosen Guard
                    {
                        playerInControl.Guard();
                    }
                    break;

                case 2:
                    //Highlighter is at Skills
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        controlsIndicator = 3;
                        highlighter.transform.position = highlighterPos3.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        controlsIndicator = 0;
                        highlighter.transform.position = highlighterPos0.transform.position;
                    }
                    else if(Input.GetKeyDown(KeyCode.RightArrow) && playerInControl.canRage)
                    {
                        controlsIndicator = 4;
                        highlighter.transform.position = highlighterPos4.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.Space)) //Player has chosen Skills
                    {
                        skillsPanel.gameObject.SetActive(true);
                        skillsTextDescription.text = "Your skills are in another castle...ughh In another build 0.015 ;)";
                        currentState = btlUIState.choosingSkillsCommand;
                    }
                    break;
                case 3:
                    //Highlighter is at Items
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        controlsIndicator = 2;
                        highlighter.transform.position = highlighterPos2.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        controlsIndicator = 1;
                        highlighter.transform.position = highlighterPos1.transform.position;
                    }
                    else if(Input.GetKeyDown(KeyCode.RightArrow) && playerInControl.canRage)
                    {
                        controlsIndicator = 4;
                        highlighter.transform.position = highlighterPos4.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.Space)) //Player has chosen Items
                    {
                        //Reset the items panel
                        itemsPanel.gameObject.SetActive(true);
                        itemsHighlighter.gameObject.SetActive(true);
                        itemsHighlighter.transform.position = itemsHPos0.transform.position;
                        itemDescription.text = inventory.ItemDescription(0);
                        itemsHighlighter.gameObject.transform.position = itemsHPos0.transform.position;
                        itemHPosIndex = 0;
                        upArrow.gameObject.SetActive(false);
                        downArrow.gameObject.SetActive(true);
                        itemsPanelIndex = 0;
                        if (inventory.ItemType(inventory.invItem[itemsPanelIndex, 0]) != (int)ITEM_TYPE.EQUIPABLE)
                        {
                            itemIconsInPanel[0].sprite = itemIcons[inventory.invItem[0, 0]];
                            itemNames[0].text = inventory.ItemName(inventory.invItem[0, 0]);
                            itemCount[0].text = inventory.invItem[0, 1].ToString();
                        }
                        else
                        {
                            itemIconsInPanel[0].sprite = itemIcons[0];
                            itemNames[0].text = "Unusable in battle";
                            itemCount[0].text = "0";
                        }
                        if (inventory.ItemType(inventory.invItem[itemsPanelIndex + 1, 0]) != (int)ITEM_TYPE.EQUIPABLE)
                        {
                            itemIconsInPanel[1].sprite = itemIcons[inventory.invItem[1, 0]];
                            itemNames[1].text = inventory.ItemName(inventory.invItem[1, 0]);
                            itemCount[1].text = inventory.invItem[1, 1].ToString();
                        }
                        else
                        {
                            itemIconsInPanel[1].sprite = itemIcons[0];
                            itemNames[1].text = "Unusable in battle";
                            itemCount[1].text = "0";
                        }
                        if (inventory.ItemType(inventory.invItem[itemsPanelIndex + 2, 0]) != (int)ITEM_TYPE.EQUIPABLE)
                        {
                            itemIconsInPanel[2].sprite = itemIcons[inventory.invItem[2, 0]];
                            itemNames[2].text = inventory.ItemName(inventory.invItem[2, 0]);
                            itemCount[2].text = inventory.invItem[2, 1].ToString();
                        }
                        else
                        {
                            itemIconsInPanel[2].sprite = itemIcons[0];
                            itemNames[2].text = "Unusable in battle";
                            itemCount[2].text = "0";
                        }
                        currentState = btlUIState.choosingItemsCommand;
                    }
                    break;
                case 4://Hilighter is at RAGE
                    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        controlsIndicator = 3;
                        highlighter.transform.position = highlighterPos3.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        controlsIndicator = 2;
                        highlighter.transform.position = highlighterPos2.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        controlsIndicator = 0;
                        highlighter.transform.position = highlighterPos0.transform.position;
                    }
                    else if (Input.GetKeyDown(KeyCode.Space)) //Player has chosen Rage
                    {
                        rageText.color = Color.yellow;
                        skillsText.color = Color.gray;
                        itemsText.color = Color.gray;
                        guardText.color = Color.gray;
                        playerInControl.Rage(); //Go into rage mode
                        rageModeIndicator1.gameObject.SetActive(true);
                        rageModeIndicator2.gameObject.SetActive(true);
                        highlighter.transform.position = highlighterPos0.transform.position;
                        controlsIndicator = 0;
                    }
                    break;
            }
        }
    }

    private void choosingSkillsCommand()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            skillsPanel.gameObject.SetActive(false);
            currentState = btlUIState.choosingBasicCommand;
        }

        switch (controlsIndicator)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }

    private void choosingItemsCommand()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            itemsPanel.gameObject.SetActive(false);
            currentState = btlUIState.choosingBasicCommand;
        }
        else if(Input.GetKeyDown(KeyCode.Space))//Player has chosen an item
        {
            //Make sure you choose an item that is usable and not equipable
            if (inventory.ItemType(inventory.invItem[itemsPanelIndex, 0]) != (int)ITEM_TYPE.EQUIPABLE)
            {
                previousState = btlUIState.choosingItemsCommand; //Needed to know what to reference when choosing the player
                playerTurnIndicator.transform.position = playerIndicatorPos0.transform.position; //Move the player indicator on top of Fargas for now
                playerIndicatorIndex = 0;
                currentState = btlUIState.choosingPlayer;
            }
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow) && itemsPanelIndex < 29)
        {
            //Keep track of where the highlighter is
            itemHPosIndex++;
            if(itemHPosIndex==1)
            {
                itemsHighlighter.transform.position = itemsHPos1.transform.position;
            }
            else if(itemHPosIndex==2)
            {
                itemsHighlighter.transform.position = itemsHPos2.transform.position;
            }
            else
            {
                itemHPosIndex = 0;
                itemsHighlighter.transform.position = itemsHPos0.transform.position;
            }

            //Know which items to display
            itemsPanelIndex++;

            itemDescription.text = inventory.ItemDescription(itemsPanelIndex);
            if(itemsPanelIndex%3 == 0)
            {
                //We're mirroring the order of the items in the inventory UI and only showing usable items
                if (inventory.ItemType(inventory.invItem[itemsPanelIndex, 0]) != (int)ITEM_TYPE.EQUIPABLE)
                {
                    itemIconsInPanel[0].sprite = itemIcons[inventory.invItem[itemsPanelIndex, 0]];
                    itemNames[0].text = inventory.ItemName(inventory.invItem[itemsPanelIndex, 0]);
                    itemCount[0].text = inventory.invItem[itemsPanelIndex, 1].ToString();
                }
                else
                {
                    itemIconsInPanel[0].sprite = itemIcons[0];
                    itemNames[0].text = "Unusable in battle";
                    itemCount[0].text = "0";
                }
                if (inventory.ItemType(inventory.invItem[itemsPanelIndex + 1, 0]) != (int)ITEM_TYPE.EQUIPABLE)
                {
                    itemIconsInPanel[1].sprite = itemIcons[inventory.invItem[itemsPanelIndex + 1, 0]];
                    itemNames[1].text = inventory.ItemName(inventory.invItem[itemsPanelIndex + 1, 0]);
                    itemCount[1].text = inventory.invItem[itemsPanelIndex + 1, 1].ToString();
                }
                else
                {
                    itemIconsInPanel[1].sprite = itemIcons[0];
                    itemNames[1].text = "Unusable in battle";
                    itemCount[1].text = "0";
                }
                if (inventory.ItemType(inventory.invItem[itemsPanelIndex + 2, 0]) != (int)ITEM_TYPE.EQUIPABLE)
                {
                    itemIconsInPanel[2].sprite = itemIcons[inventory.invItem[itemsPanelIndex + 2, 0]];
                    itemNames[2].text = inventory.ItemName(inventory.invItem[itemsPanelIndex + 2, 0]);
                    itemCount[2].text = inventory.invItem[itemsPanelIndex + 2, 1].ToString();
                }
                else
                {
                    itemIconsInPanel[2].sprite = itemIcons[0];
                    itemNames[2].text = "Unusable in battle";
                    itemCount[2].text = "0";
                }

                //Check if we're at the last three items -->This should be changed to work with the inventory count. Will change once we decide on the inventory count
                if (itemsPanelIndex == 27)
                {
                    downArrow.gameObject.SetActive(false);
                    upArrow.gameObject.SetActive(true);
                }
                else
                {
                    downArrow.gameObject.SetActive(true);
                    upArrow.gameObject.SetActive(true);
                }
            }

        }
        else if(Input.GetKeyDown(KeyCode.UpArrow) && itemsPanelIndex > 0)
        {
            itemHPosIndex--;

            //Know which items to display
            itemsPanelIndex--;

            itemDescription.text = inventory.ItemDescription(itemsPanelIndex);
            //Check if we're back to the previous 3 items
            if (itemsPanelIndex % 3 != 0 && itemHPosIndex < 0)
            {
                if (inventory.ItemType(inventory.invItem[itemsPanelIndex - 2, 0]) != (int)ITEM_TYPE.EQUIPABLE)
                {
                    itemIconsInPanel[0].sprite = itemIcons[inventory.invItem[itemsPanelIndex - 2, 0]];
                    itemNames[0].text = inventory.ItemName(inventory.invItem[itemsPanelIndex - 2, 0]);
                    itemCount[0].text = inventory.invItem[itemsPanelIndex - 2, 1].ToString();
                }
                else
                {
                    itemIconsInPanel[0].sprite = itemIcons[0];
                    itemNames[0].text = "Unusable in battle";
                    itemCount[0].text = "0";
                }
                if (inventory.ItemType(inventory.invItem[itemsPanelIndex - 1, 0]) != (int)ITEM_TYPE.EQUIPABLE)
                {
                    itemIconsInPanel[1].sprite = itemIcons[inventory.invItem[itemsPanelIndex - 1, 0]];
                    itemNames[1].text = inventory.ItemName(inventory.invItem[itemsPanelIndex - 1, 0]);
                    itemCount[1].text = inventory.invItem[itemsPanelIndex - 1, 1].ToString();
                }
                else
                {
                    itemIconsInPanel[1].sprite = itemIcons[0];
                    itemNames[1].text = "Unusable in battle";
                    itemCount[1].text = "0";
                }
                if (inventory.ItemType(inventory.invItem[itemsPanelIndex, 0]) != (int)ITEM_TYPE.EQUIPABLE)
                {
                    itemIconsInPanel[2].sprite = itemIcons[inventory.invItem[itemsPanelIndex, 0]];
                    itemNames[2].text = inventory.ItemName(inventory.invItem[itemsPanelIndex, 0]);
                    itemCount[2].text = inventory.invItem[itemsPanelIndex, 1].ToString();
                }
                else
                {
                    itemIconsInPanel[2].sprite = itemIcons[0];
                    itemNames[2].text = "Unusable in battle";
                    itemCount[2].text = "0";
                }

                itemsHighlighter.transform.position = itemsHPos2.transform.position;
                itemHPosIndex = 2;

                //Check if we're back to the very first three
                if (itemsPanelIndex == 2)
                {
                    downArrow.gameObject.SetActive(true);
                    upArrow.gameObject.SetActive(false);
                }
                else
                {
                    downArrow.gameObject.SetActive(true);
                    upArrow.gameObject.SetActive(true);
                }
            }
            else if (itemHPosIndex == 1)
            {
                itemsHighlighter.transform.position = itemsHPos1.transform.position;
            }
            else if (itemHPosIndex == 0)
            {
                itemsHighlighter.transform.position = itemsHPos0.transform.position;
            }
            
        }
    }


    private void choosingPlayer()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            switch(playerInControl.playerIndex)
            {
                case 0:
                    playerTurnIndicator.transform.position = playerIndicatorPos0.transform.position;
                    break;
                case 1:
                    playerTurnIndicator.transform.position = playerIndicatorPos1.transform.position;
                    break;
                case 2:
                    playerTurnIndicator.transform.position = playerIndicatorPos2.transform.position;
                    break;
                case 3:
                    playerTurnIndicator.transform.position = playerIndicatorPos3.transform.position;
                    break;
            }
            currentState = btlUIState.choosingItemsCommand;
        }
        if(playerIndicatorIndex == 0)
        {
            if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                playerIndicatorIndex = 1;//Go to Oberon
                playerTurnIndicator.transform.position = playerIndicatorPos1.transform.position;

            }
            else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                playerIndicatorIndex = 3;//Go to Arcelus
                playerTurnIndicator.transform.position = playerIndicatorPos3.transform.position;
            }

        }
        else if (playerIndicatorIndex == 1)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                playerIndicatorIndex = 0;//Go to Fargas
                playerTurnIndicator.transform.position = playerIndicatorPos0.transform.position;

            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                playerIndicatorIndex = 2;//Go to Frea
                playerTurnIndicator.transform.position = playerIndicatorPos2.transform.position;
            }
        }
        else if (playerIndicatorIndex == 2)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                playerIndicatorIndex = 3;//Go to Arcelus
                playerTurnIndicator.transform.position = playerIndicatorPos3.transform.position;

            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                playerIndicatorIndex = 1;//Go to Oberon
                playerTurnIndicator.transform.position = playerIndicatorPos1.transform.position;
            }
        }
        else if (playerIndicatorIndex == 3)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                playerIndicatorIndex = 2;//Go to Frea
                playerTurnIndicator.transform.position = playerIndicatorPos2.transform.position;

            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                playerIndicatorIndex = 0;//Go to Fargas
                playerTurnIndicator.transform.position = playerIndicatorPos0.transform.position;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Debug.Log("itemsPanelindex " + itemsPanelIndex);
            // Debug.Log("hilighterPos " + itemHPosIndex);
            inventory.curOption = itemsPanelIndex; //Update the cursor in the inventory UI as that's used in the itemUseFunction to decrease the count of the used item
            inventory.ItemUseFunction(inventory.invItem[itemsPanelIndex, 0], playerIndicatorIndex);
            //Debug.Log(inventory.invItem[itemsPanelIndex, 1]);
           // Debug.Log(inventory.ItemName(itemsPanelIndex));
            itemCount[itemHPosIndex].text = inventory.invItem[itemsPanelIndex, 1].ToString();
            UpdatePlayerStats(playerIndicatorIndex);
            inventory.curOption = itemsPanelIndex = 0; //Reset the itemsPanelIndex
            EndTurn();
        }
    }

    private void choosingEnemy()
    {
        //Leave choosing enemy
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ResetVisibilityForAllEnemies();
            enemyToAttackIndicator.gameObject.SetActive(false);
            currentState = btlUIState.choosingBasicCommand;
        }
        //If we're at the same enemy, don't call the visible function again
        if (previousEnemyIndicatorIndex != enemyIndicatorIndex)
        {
            MakeChosenEnemyMorePrompt(enemyIndicatorIndex);
        }

        switch (enemyIndicatorIndex)
        {
            case 0:
                if(Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (enemies[1] != null && enemiesDead[1]==false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 1;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPosArray[1].transform.position;
                    }
                }
                else if((Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.LeftArrow)) && (activeRange + playerInControl.initialPos >= 2))
                {
                    if (enemies[3] != null && enemiesDead[3] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 3;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPosArray[3].transform.position;
                    }
                }
                else if(Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (enemies[2] != null && enemiesDead[2] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 2;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPosArray[2].transform.position;
                    }
                }
                break;
            case 1:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (enemies[2] != null && enemiesDead[2] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 2;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPosArray[2].transform.position;
                    }
                }
                else if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) && (activeRange + playerInControl.initialPos >= 2))
                {
                    if (enemies[4] != null && enemiesDead[4] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 4;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPosArray[4].transform.position;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (enemies[0] != null && enemiesDead[0] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 0;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPosArray[0].transform.position;
                    }
                }
                break;
            case 2:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (enemies[0] != null && enemiesDead[0] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 0;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPosArray[0].transform.position;
                    }
                }
                else if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) && (activeRange + playerInControl.initialPos >= 2))
                {
                    if (enemies[3] != null && enemiesDead[3] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 3;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPosArray[3].transform.position;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (enemies[1] != null && enemiesDead[1] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 1;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPosArray[1].transform.position;
                    }
                }
                break;
            case 3:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (enemies[4] != null && enemiesDead[4] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 4;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPosArray[4].transform.position;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (enemies[0] != null && enemiesDead[0] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 0;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPosArray[0].transform.position;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (enemies[4] != null  && enemiesDead[4] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 4;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPosArray[4].transform.position;
                    }
                }
                break;
            case 4:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (enemies[3] != null && enemiesDead[3] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 3;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPosArray[3].transform.position;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (enemies[2] != null && enemiesDead[2] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 2;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPosArray[2].transform.position;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (enemies[3] != null && enemiesDead[3] == false)
                    {
                        previousEnemyIndicatorIndex = enemyIndicatorIndex;
                        enemyIndicatorIndex = 3;
                        enemyToAttackIndicator.transform.position = enemyIndicatorPosArray[3].transform.position;
                    }
                }
                break;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            playerInControl.Attack(enemies[enemyIndicatorIndex]);
        }
    }

    public void MakeChosenEnemyMorePrompt(int enemyIndex) //When the player chooses an enemy to attack, the other enemies should be less visible
    {
        if (enemies[enemyIndex] != null)
        {
            enemies[enemyIndex].resetVisibility();
        }

        for(int i =0;i<5;i++)
        {
            if(i!=enemyIndex && enemies[i]!=null)
            {
                enemies[i].becomeLessVisbile();
            }
        }
    }

    public void ResetVisibilityForAllEnemies()
    {
        //Reset the visibility of all enemies
        for (int i = 0; i < 5; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].resetVisibility();
            }
        }

    }


    public void RageOptionTextColor()
    {
        if (playerInControl.canRage)
        {
            rageText.color = Color.white;
        }
        else
        {
            rageText.color = Color.gray;
        }

    }

    public void UpdatePlayerHPControlPanel()
    {
        currentHP.text = Mathf.RoundToInt(playerInControl.currentHP).ToString();
        maxHP.text = " / " + Mathf.RoundToInt(playerInControl.maxHP).ToString();
        hpBar.fillAmount = playerInControl.hpImage.fillAmount;
    }

    public void UpdatePlayerMPControlPanel()
    {
        currentMP.text = Mathf.RoundToInt(playerInControl.currentMP).ToString();
        maxMP.text = " / " + Mathf.RoundToInt(playerInControl.maxMP).ToString();
        mpBar.fillAmount = playerInControl.currentMP / playerInControl.maxMP;
    }

    public void EndTurn()
    {
        if(playerInControl.currentState==Player.playerState.Rage)
        {
            //Make sure to turn off the indicators at the end of the turn, this is to make sure the end screen does not show the indicators
            rageModeIndicator1.gameObject.SetActive(false);
            rageModeIndicator2.gameObject.SetActive(false);
        }
        playerTurnIndicator.SetActive(false);
        enemyToAttackIndicator.SetActive(false);
        controlsPanel.gameObject.SetActive(false);
        itemsPanel.gameObject.SetActive(false);
        moveImagesNow = true;
    }

    public void EnemyIsDead(int enemyIndex)
    {
        enemiesDead[enemyIndex] = true;

        for(int i = 0, j=0; j<enemies.Length;j++)
        {
            if(enemies[j]!=null && enemiesDead[j]==true)
            {
                i++;
                if(i>=numberOfEnemies)
                {

                    //Enable the screen and start increasing the EXP
                    //Must check if all the players are still alive but for now they are
                    endScreenPanel.gameObject.SetActive(true);

                    fargasAddinExp = true;
                    fargasHP.fillAmount = btlManager.players[0].playerReference.hpImage.fillAmount;
                    fargasMP.fillAmount = btlManager.players[0].playerReference.currentMP/ btlManager.players[0].playerReference.maxMP;
                    fargasCurrentExp = btlManager.players[0].exp;
                    fargasMaxExp = btlManager.players[0].expNeededForNextLevel;
                    Debug.Log("Fargas current EXP: " + fargasCurrentExp);
                    Debug.Log("Fargas max EXP: " + fargasMaxExp);
                    fargasExp.fillAmount = btlManager.players[0].exp / fargasMaxExp;
                    fargasExpStep = 1.0f/ fargasMaxExp;
                    fargasExpGain = btlManager.expGain;


                    oberonAddinExp = true;
                    oberonHP.fillAmount = btlManager.players[1].playerReference.hpImage.fillAmount;
                    oberonMP.fillAmount = btlManager.players[1].playerReference.currentMP / btlManager.players[1].playerReference.maxMP;
                    oberonCurrentExp = btlManager.players[1].exp;
                    oberonMaxExp = btlManager.players[1].expNeededForNextLevel;
                    oberonExp.fillAmount = btlManager.players[1].exp / oberonMaxExp;
                    oberonExpStep = 1.0f/ oberonMaxExp;
                    oberonExpGain = btlManager.expGain; 

                    freaAddinExp = true;
                    freaHP.fillAmount = btlManager.players[2].playerReference.hpImage.fillAmount;
                    freaMP.fillAmount = btlManager.players[2].playerReference.currentMP / btlManager.players[2].playerReference.maxMP;
                    freaCurrentExp = btlManager.players[2].exp;
                    freaMaxExp = btlManager.players[2].expNeededForNextLevel;
                    freaExp.fillAmount = btlManager.players[2].exp / freaMaxExp;
                    freaExpStep = 1.0f/ freaMaxExp;
                    freaExpGain = btlManager.expGain;

                    arcelusAddinExp = true;
                    arcelusHP.fillAmount = btlManager.players[3].playerReference.hpImage.fillAmount;
                    arcelusMP.fillAmount = btlManager.players[3].playerReference.currentMP / btlManager.players[3].playerReference.maxMP;
                    arcelusCurrentExp = btlManager.players[3].exp;
                    arcelusMaxExp = btlManager.players[3].expNeededForNextLevel;
                    arcelusExp.fillAmount = btlManager.players[3].exp / arcelusMaxExp;
                    arcelusExpStep = 1.0f / arcelusMaxExp;
                    arcelusExpGain = btlManager.expGain;

                    currentState = btlUIState.battleEnd;
                    //Make sure to turn off the indicators at the end of the turn, this is to make sure the end screen does not show the indicators
                    rageModeIndicator1.gameObject.SetActive(false);
                    rageModeIndicator2.gameObject.SetActive(false);
                    battleHasEnded = true;
                    btlManager.EndOfBattle();
                }
            }
        }
    }

    private void EndBattleUI()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !fargasAddinExp && !freaAddinExp && !oberonAddinExp && !arcelusAddinExp)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("Queue Scene"));
            //If there's a conversation right after the battle, invoke the function in the dialogue manager
            if(conversationAfterBattle)
            {
                dialogueManager.StartConversation(dialogueContainer.doneFight);
            }
        }
        
        //Fargas
        if(fargasAddinExp)
        {
            if(fargasExp.fillAmount < 1.0f && fargasCurrentExp < fargasMaxExp && fargasExpGain>0)
            {
                fargasCurrentExp++;
                fargasExpGain--;
                fargasExp.fillAmount += fargasExpStep;
            }
            //If the player has leveled up, get the new max exp and 
            else if(fargasCurrentExp >= fargasMaxExp || fargasExp.fillAmount>=1.0f)
            {
                btlManager.LevelUp(0);
                fargasMaxExp = btlManager.players[0].expNeededForNextLevel;
                Debug.Log("UI stuff: Current: " + fargasCurrentExp + "  Max:  " + fargasMaxExp);
                fargasExpStep = 1.0f / fargasMaxExp;
                fargasExp.fillAmount = 0.0f;
                fargasLevelUpBack.gameObject.SetActive(true);
                fargasLevelUpFore.gameObject.SetActive(true);
            }
            //If we have reached the exp gain, stop
            else if(fargasExpGain<=0)
            {
                fargasAddinExp = false;
            }
        }

        //Oberon
        if (oberonAddinExp)
        {
            if (oberonExp.fillAmount < 1.0 && oberonCurrentExp < oberonMaxExp && oberonExpGain > 0)
            {
                oberonCurrentExp++;
                oberonExpGain--;
                oberonExp.fillAmount += oberonExpStep;
            }
            //If the player has leveled up, get the new max exp and 
            else if (oberonCurrentExp >= oberonMaxExp || oberonExp.fillAmount >= 1.0f)
            {
                btlManager.LevelUp(1);
                oberonMaxExp = btlManager.players[1].expNeededForNextLevel;
                oberonExpStep = 1.0f / oberonMaxExp;
                oberonExp.fillAmount = 0.0f;
                oberonLevelUpBack.gameObject.SetActive(true);
                oberonLevelUpFore.gameObject.SetActive(true);
            }
            else if (oberonExpGain <= 0)
            {
                oberonAddinExp = false;
            }
        }

        //Frea
        if (freaAddinExp)
        {
            if (freaExp.fillAmount < 1.0f && freaCurrentExp < freaMaxExp && freaExpGain > 0)
            {
                freaCurrentExp++;
                freaExpGain--;
                freaExp.fillAmount += freaExpStep;
            }
            //If the player has leveled up, get the new max exp and 
            else if (freaCurrentExp >= freaMaxExp || freaExp.fillAmount >= 1.0f)
            {
                btlManager.LevelUp(2);
                freaMaxExp = btlManager.players[2].expNeededForNextLevel;
                freaExpStep = 1.0f / freaMaxExp;
                freaExp.fillAmount = 0.0f;
                freaLevelUpBack.gameObject.SetActive(true);
                freaLevelUpFore.gameObject.SetActive(true);
            }
            else if (freaExpGain <= 0 )
            {
                freaAddinExp = false;
            }
        }
        
        //Arcelus
        if (arcelusAddinExp)
        {
            if (arcelusExp.fillAmount < 1.0f && arcelusCurrentExp < arcelusMaxExp && arcelusExpGain > 0)
            {
                arcelusCurrentExp++;
                arcelusExpGain--;
                arcelusExp.fillAmount += arcelusExpStep;
            }
            //If the player has leveled up, get the new max exp and 
            else if (arcelusCurrentExp >= arcelusMaxExp || arcelusExp.fillAmount >= 1.0f)
            {
                btlManager.LevelUp(3);
                arcelusMaxExp = btlManager.players[3].expNeededForNextLevel;
                arcelusExpStep = 1.0f / arcelusMaxExp;
                arcelusExp.fillAmount = 0.0f;
                arcelusLevelUpBack.gameObject.SetActive(true);
                arcelusLevelUpFore.gameObject.SetActive(true);
            }
            else if (arcelusExpGain <= 0)
            {
                arcelusAddinExp = false;
            }
        }

    }

    private void UpdatePlayerStats(int playerIndex)
    {
        btlManager.UpdatePlayerStats(playerIndex);
        playerInControl.ForcePlayerTurnAnimationOff();
    }


}
