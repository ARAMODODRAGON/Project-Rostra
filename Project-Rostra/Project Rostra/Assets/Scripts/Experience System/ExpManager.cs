﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpManager : MonoBehaviour {
	//singleton
	public static ExpManager instance;

	#region skills per player

	private static int[][] playeroneskills = new int[4][];

	private static int[][] playertwoskills = new int[4][];

	private static int[][] playerthreeskills = new int[4][];

	private static int[][] playerfourskills = new int[4][];

	//temporary available skills lists
	private List<SKILLS> playeroneunlockedskills = new List<SKILLS>();
	private List<SKILLS> playertwounlockedskills = new List<SKILLS>();
	private List<SKILLS> playerthreeunlockedskills = new List<SKILLS>();
	private List<SKILLS> playerfourunlockedskills = new List<SKILLS>();

	#endregion

	#region Initialization & Destruction

	private void Awake() {
		//initialize singleton
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		} else {
			Destroy(gameObject);

		}

		playeroneskills[0] = new int[] { (int)SKILLS.TEST_SKILL1, 0, 0, 0, 0, 0, 0, 0 };
		playeroneskills[1] = new int[] { (int)SKILLS.TEST_SKILL1, 0, 0, 0, 0, 0, 0, 0 };
		playeroneskills[2] = new int[] { (int)SKILLS.TEST_SKILL1, 0, 0, 0, 0, 0, 0, 0 };
		playeroneskills[3] = new int[] { (int)SKILLS.TEST_SKILL1, 0, 0, 0, 0, 0, 0, 0 };
		playertwoskills[0] = new int[] { (int)SKILLS.TEST_SKILL1, 0, 0, 0, 0, 0, 0, 0 };
		playertwoskills[1] = new int[] { (int)SKILLS.TEST_SKILL1, 0, 0, 0, 0, 0, 0, 0 };
		playertwoskills[2] = new int[] { (int)SKILLS.TEST_SKILL1, 0, 0, 0, 0, 0, 0, 0 };
		playertwoskills[3] = new int[] { (int)SKILLS.TEST_SKILL1, 0, 0, 0, 0, 0, 0, 0 };
		playerthreeskills[0] = new int[] { (int)SKILLS.TEST_SKILL1, 0, 0, 0, 0, 0, 0, 0 };
		playerthreeskills[1] = new int[] { (int)SKILLS.TEST_SKILL1, 0, 0, 0, 0, 0, 0, 0 };
		playerthreeskills[2] = new int[] { (int)SKILLS.TEST_SKILL1, 0, 0, 0, 0, 0, 0, 0 };
		playerthreeskills[3] = new int[] { (int)SKILLS.TEST_SKILL1, 0, 0, 0, 0, 0, 0, 0 };
		playerfourskills[0] = new int[] { (int)SKILLS.TEST_SKILL1, 0, 0, 0, 0, 0, 0, 0 };
		playerfourskills[1] = new int[] { (int)SKILLS.TEST_SKILL1, 0, 0, 0, 0, 0, 0, 0 };
		playerfourskills[2] = new int[] { (int)SKILLS.TEST_SKILL1, 0, 0, 0, 0, 0, 0, 0 };
		playerfourskills[3] = new int[] { (int)SKILLS.TEST_SKILL1, 0, 0, 0, 0, 0, 0, 0 };
	}

	private void OnDestroy() {
		//remove singleton
		if (instance == this) instance = null;
	}

	private void Start() {
		//Give initial values for stats
		//Will be changed to use a load file instead

		//Fargas
		PartyStats.chara[0].hitpoints = 200.0f;
		PartyStats.chara[0].maxHealth = 200.0f;
		PartyStats.chara[0].magicpoints = 200.0f;
		PartyStats.chara[0].maxMana = 200.0f;
		PartyStats.chara[0].attack = 100.0f;
		PartyStats.chara[0].defence = 15.0f;
		PartyStats.chara[0].agility = 14.0f;
		PartyStats.chara[0].strength = 16.0f;
		PartyStats.chara[0].critical = 5.0f;
		PartyStats.chara[0].speed = 16.0f;
		PartyStats.chara[0].currentExperience = 0;
		PartyStats.chara[0].neededExperience = 150;

		//Oberon
		PartyStats.chara[1].hitpoints = 250.0f;
		PartyStats.chara[1].maxHealth = 250.0f;
		PartyStats.chara[1].magicpoints = 150.0f;
		PartyStats.chara[1].maxMana = 150.0f;
		PartyStats.chara[1].attack = 100.0f;
		PartyStats.chara[1].defence = 20.0f;
		PartyStats.chara[1].agility = 10.0f;
		PartyStats.chara[1].strength = 14.0f;
		PartyStats.chara[1].critical = 3.0f;
		PartyStats.chara[1].speed = 9.0f;
		PartyStats.chara[1].currentExperience = 0;
		PartyStats.chara[1].neededExperience = 150;

		//Frea
		PartyStats.chara[2].hitpoints = 180.0f;
		PartyStats.chara[2].maxHealth = 180.0f;
		PartyStats.chara[2].magicpoints = 200.0f;
		PartyStats.chara[2].maxMana = 200.0f;
		PartyStats.chara[2].attack = 100.0f;
		PartyStats.chara[2].defence = 14.0f;
		PartyStats.chara[2].agility = 13.0f;
		PartyStats.chara[2].strength = 15.0f;
		PartyStats.chara[2].critical = 5.0f;
		PartyStats.chara[2].speed = 14.0f;
		PartyStats.chara[2].currentExperience = 0;
		PartyStats.chara[2].neededExperience = 150;

		//Arcelus
		PartyStats.chara[3].hitpoints = 160.0f;
		PartyStats.chara[3].maxHealth = 160.0f;
		PartyStats.chara[3].magicpoints = 250.0f;
		PartyStats.chara[3].maxMana = 250.0f;
		PartyStats.chara[3].attack = 100.0f;
		PartyStats.chara[3].defence = 15.0f;
		PartyStats.chara[3].agility = 17.0f;
		PartyStats.chara[3].strength = 13.0f;
		PartyStats.chara[3].critical = 3.0f;
		PartyStats.chara[3].speed = 12.0f;
		PartyStats.chara[3].currentExperience = 0;
		PartyStats.chara[3].neededExperience = 150;
	}

	#endregion

	#region exp & leveling

	public void LevelUp(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) {
			Debug.LogError("Player number " + playerIndex + " does not exist!");
			return;
		}

		//level up
		PartyStats.chara[playerIndex].level++;
		//set current exp to 0
		PartyStats.chara[playerIndex].currentExperience = 0;

		//calculate the number of stat points gained
		///number of points increases by 1 each level
		///eg: level 30, points gained = 7;
		PartyStats.chara[playerIndex].statPoints += 1 + PartyStats.chara[playerIndex].level / 5; // WIP

		//changes the Exp needed to level up again
		PartyStats.chara[playerIndex].neededExperience = 500 + 250 * (PartyStats.chara[playerIndex].level - 1);
		// WIP
		///500 is the base exp needed
		///250 * (level - 1) adds 250 for each level gained
		///eg: level 30, exp needed is 7,750
	}

	public int ExpNeeded(int level) {
		//a calculation of how much exp is needed
		return 500 + 250 * (level - 1);
		// WIP
		///500 is the base exp needed
		///250 * (level - 1) adds 250 for each level gained
		///eg: level 30, exp needed is 7,750
	}


	#endregion

	#region Using upgrade points & unlocking skills

	#region Using points

	public void UsePointOnAttack(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) { Debug.LogError("Player number " + playerIndex + " does not exist!"); return; }
		if (PartyStats.chara[playerIndex].statPoints == 0) return;

		PartyStats.chara[playerIndex].statPoints--;
		PartyStats.chara[playerIndex].attack++;
		UpdatePlayerSkills(playerIndex);
	}

	public void UsePointOnDefence(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) { Debug.LogError("Player number " + playerIndex + " does not exist!"); return; }
		if (PartyStats.chara[playerIndex].statPoints == 0) return;

		PartyStats.chara[playerIndex].statPoints--;
		PartyStats.chara[playerIndex].defence += 1f;
		UpdatePlayerSkills(playerIndex);
	}

	public void UsePointOnHealth(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) { Debug.LogError("Player number " + playerIndex + " does not exist!"); return; }
		if (PartyStats.chara[playerIndex].statPoints == 0) return;

		PartyStats.chara[playerIndex].statPoints--;
		PartyStats.chara[playerIndex].maxHealth += 75f;
		PartyStats.chara[playerIndex].hitpoints += 75f;
		UpdatePlayerSkills(playerIndex);
	}

	public void UsePointOnMana(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) { Debug.LogError("Player number " + playerIndex + " does not exist!"); return; }
		if (PartyStats.chara[playerIndex].statPoints == 0) return;

		PartyStats.chara[playerIndex].statPoints--;
		PartyStats.chara[playerIndex].maxMana += 75f;
		PartyStats.chara[playerIndex].magicpoints += 75f;
		UpdatePlayerSkills(playerIndex);
	}

	public void UsePointOnStrength(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) { Debug.LogError("Player number " + playerIndex + " does not exist!"); return; }
		if (PartyStats.chara[playerIndex].statPoints == 0) return;

		PartyStats.chara[playerIndex].statPoints--;
		PartyStats.chara[playerIndex].strength += 1f;
		UpdatePlayerSkills(playerIndex);
	}

	public void UsePointOnCritical(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) { Debug.LogError("Player number " + playerIndex + " does not exist!"); return; }
		if (PartyStats.chara[playerIndex].statPoints == 0) return;

		PartyStats.chara[playerIndex].statPoints--;
		PartyStats.chara[playerIndex].critical += 1f;
		UpdatePlayerSkills(playerIndex);
	}

	public void UsePointOnAgility(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) { Debug.LogError("Player number " + playerIndex + " does not exist!"); return; }
		if (PartyStats.chara[playerIndex].statPoints == 0) return;

		PartyStats.chara[playerIndex].statPoints--;
		PartyStats.chara[playerIndex].agility += 1f;
		UpdatePlayerSkills(playerIndex);
	}

	public void UsePointOnSpeed(int playerIndex) {
		//checks if the playerIndex is valid
		if (playerIndex < 0 || playerIndex > 3) { Debug.LogError("Player number " + playerIndex + " does not exist!"); return; }
		if (PartyStats.chara[playerIndex].statPoints == 0) return;

		PartyStats.chara[playerIndex].statPoints--;
		PartyStats.chara[playerIndex].speed += 1f;
		UpdatePlayerSkills(playerIndex);
	}

	#endregion

	#region Unlock skills

	private void UpdatePlayerSkills(int index) {
		switch (index) {
			case 0: UpdatePlayerOneSkills(); break;
			case 1: UpdatePlayerTwoSkills(); break;
			case 2: UpdatePlayerThreeSkills(); break;
			case 3: UpdatePlayerFourSkills(); break;
			default: Debug.LogError("something happened but i dont know what"); break;
		}
	}

	private bool CheckForMinimumVals(int[] arr, int[] min) {
		if (arr.Length != min.Length + 1) { Debug.LogError("AAAAAAAAAAAAAAAA"); return false; }
		for (int i = 1; i < arr.Length; i++) {
			if (arr[i] < min[i - 1]) return false;
		}
		return true;
	}

	private void UpdatePlayerOneSkills() {
		for (int i = 0; i < playeroneskills.Length; i++) {
			if (playeroneunlockedskills.Contains((SKILLS)playeroneskills[i][0])) continue;
			if (CheckForMinimumVals(playeroneskills[i], new int[] { 0, 0, 0, 0, 0, 0, 0, 0 })) {
				/* TODO: Unlock skill in skills inventory */
				playeroneunlockedskills.Add((SKILLS)playeroneskills[i][0]);
			}
		}
	}

	private void UpdatePlayerTwoSkills() {

	}

	private void UpdatePlayerThreeSkills() {

	}

	private void UpdatePlayerFourSkills() {

	}

	#endregion

	#endregion

	#region GUI

	private bool showGUI = false;
	private bool use = false;
	private bool up = false;
	private bool down = false;
	private int currentItem = 0;
	private int currentPlayer = 0;

	private void OnGUI() {
		if (!showGUI) {
			use = false;
			return;
		}

		if (up) currentItem--;
		else if (down) currentItem++;
		if (currentItem == -1) currentItem = 10;
		if (currentItem == 11) currentItem = 0;
		up = false;
		down = false;

		GUI.skin.label.fontSize = 40;

		Rect rect = new Rect(40, 50, 400, 80);

		GUI.Label(rect, "   skillPoints " + PartyStats.chara[currentPlayer].statPoints);

		rect.y += 80;
		if (currentItem == 0) GUI.Label(rect, "> Current Player " + currentPlayer);
		else GUI.Label(rect, "   Current Player " + currentPlayer);

		rect.y += 80;
		if (currentItem == 1) GUI.Label(rect, "> Level " + PartyStats.chara[currentPlayer].level);
		else GUI.Label(rect, "   Level " + PartyStats.chara[currentPlayer].level);

		rect.y += 80;
		if (currentItem == 2) GUI.Label(rect, "> Attack " + PartyStats.chara[currentPlayer].attack);
		else GUI.Label(rect, "   Attack " + PartyStats.chara[currentPlayer].attack);

		rect.y += 80;
		if (currentItem == 3) GUI.Label(rect, "> Defence " + PartyStats.chara[currentPlayer].defence);
		else GUI.Label(rect, "   Defence " + PartyStats.chara[currentPlayer].defence);

		rect.y += 80;
		if (currentItem == 4) GUI.Label(rect, "> Health " + PartyStats.chara[currentPlayer].maxHealth);
		else GUI.Label(rect, "   Health " + PartyStats.chara[currentPlayer].maxHealth);

		rect.y += 80;
		if (currentItem == 5) GUI.Label(rect, "> Mana " + PartyStats.chara[currentPlayer].maxMana);
		else GUI.Label(rect, "   Mana " + PartyStats.chara[currentPlayer].maxMana);

		rect.y += 80;
		if (currentItem == 6) GUI.Label(rect, "> Strength " + PartyStats.chara[currentPlayer].strength);
		else GUI.Label(rect, "   Strength " + PartyStats.chara[currentPlayer].strength);

		rect.y += 80;
		if (currentItem == 7) GUI.Label(rect, "> Critical " + PartyStats.chara[currentPlayer].critical);
		else GUI.Label(rect, "   Critical " + PartyStats.chara[currentPlayer].critical);

		rect.y += 80;
		if (currentItem == 8) GUI.Label(rect, "> Agility " + PartyStats.chara[currentPlayer].agility);
		else GUI.Label(rect, "   Agility " + PartyStats.chara[currentPlayer].agility);

		rect.y += 80;
		if (currentItem == 9) GUI.Label(rect, "> Speed " + PartyStats.chara[currentPlayer].speed);
		else GUI.Label(rect, "   Speed " + PartyStats.chara[currentPlayer].speed);

		if (use) {
			switch (currentItem) {
				case 0:
					currentPlayer++;
					if (currentPlayer == 4) currentPlayer = 0;
					break;
				case 1: LevelUp(currentPlayer); break;
				case 2: UsePointOnAttack(currentPlayer); break;
				case 3: UsePointOnDefence(currentPlayer); break;
				case 4: UsePointOnHealth(currentPlayer); break;
				case 5: UsePointOnMana(currentPlayer); break;
				case 6: UsePointOnStrength(currentPlayer); break;
				case 7: UsePointOnCritical(currentPlayer); break;
				case 8: UsePointOnAgility(currentPlayer); break;
				case 9: UsePointOnSpeed(currentPlayer); break;
				default: break;
			}
		}
		use = false;
	}

	private void Update() {
		if (Input.GetButtonDown("expOpen")) showGUI = showGUI ? false : true;
		if (showGUI) use = Input.GetButtonDown("expConfirm");
		up = Input.GetButtonDown("expUp");
		down = Input.GetButtonDown("expDown");
	}

	#endregion
}
