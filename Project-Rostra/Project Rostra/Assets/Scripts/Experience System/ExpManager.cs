﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpManager : MonoBehaviour {
	//singleton
	public static ExpManager instance;

	#region Initialization & Destruction

	private void Awake() {
		//initialize singleton
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
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

	#region Getting required stats
	
	private int[] RequiredStats(SKILLS skill, int playerindex) {
		switch (playerindex) {
			case 0:
				switch (skill) {
					case SKILLS.TEST_SKILL1: return new int[] { 110, 15, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL2: return new int[] { 110, 15, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL3: return new int[] { 100, 20, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL4: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL5: return new int[] { 110, 15, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL6: return new int[] { 110, 15, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL7: return new int[] { 100, 20, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL8: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL9: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL10: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL11: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					default: return new int[0];
				}
			case 1:
				switch (skill) {
					case SKILLS.TEST_SKILL1: return new int[] { 110, 15, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL2: return new int[] { 110, 15, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL3: return new int[] { 100, 20, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL4: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL5: return new int[] { 110, 15, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL6: return new int[] { 110, 15, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL7: return new int[] { 100, 20, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL8: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL9: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL10: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL11: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					default: return new int[0];
				}
			case 2:
				switch (skill) {
					case SKILLS.TEST_SKILL1: return new int[] { 110, 15, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL2: return new int[] { 110, 15, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL3: return new int[] { 100, 20, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL4: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL5: return new int[] { 110, 15, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL6: return new int[] { 110, 15, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL7: return new int[] { 100, 20, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL8: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL9: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL10: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL11: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					default: return new int[0];
				}
			case 3:
				switch (skill) {
					case SKILLS.TEST_SKILL1: return new int[] { 110, 15, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL2: return new int[] { 110, 15, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL3: return new int[] { 100, 20, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL4: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL5: return new int[] { 110, 15, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL6: return new int[] { 110, 15, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL7: return new int[] { 100, 20, 200, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL8: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL9: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL10: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					case SKILLS.TEST_SKILL11: return new int[] { 100, 15, 210, 200, 16, 14, 16 };
					default: return new int[0];
				}
			default: return new int[0];
		}
	}

	#endregion

	private void UpdatePlayerSkills(int index) {
		int[] arr = new int[] { (int)PartyStats.chara[index].attack,        (int)PartyStats.chara[index].defence,   (int)PartyStats.chara[index].hitpoints,
								(int)PartyStats.chara[index].magicpoints,   (int)PartyStats.chara[index].strength,  (int)PartyStats.chara[index].agility,
								(int)PartyStats.chara[index].speed };
		for (int i = 0; i < PartySkills.skills[index].unlockableSkills.Length; i++) {
			
			if (CheckForMinimumVals(arr, RequiredStats((SKILLS)PartySkills.skills[index].unlockableSkills[i], index))) {
				SkillsInventory.invInstance.AddToUnlockedSkills((int)PartySkills.skills[index].unlockableSkills[i], index);
			}
		}
	}

	private bool CheckForMinimumVals(int[] cur, int[] min) {
		if (cur.Length != min.Length) return false;
		for (int i = 0; i < cur.Length; i++) {
			if (cur[i] < min[i]) {
				return false;
			}
		}
		return true;
	}


	#endregion

	#region GUI

#if DEBUG

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

		#region Draw left side

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

		#endregion

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
				//case 7: UsePointOnCritical(currentPlayer); break;
				case 8: UsePointOnAgility(currentPlayer); break;
				case 9: UsePointOnSpeed(currentPlayer); break;
				default: break;
			}
		}
		use = false;

		rect = new Rect(1440, 50, 400, 80);

		for (int j = 0; j < PartySkills.skills[currentPlayer].unlockableSkills.Length; j++) {
			GUI.Label(rect, "" + (SKILLS)PartySkills.skills[currentPlayer].unlockableSkills[j]);
			rect.y += 80;
		}

		rect = new Rect(840, 50, 400, 80);

		for (int i = 0; i < PartySkills.skills[currentPlayer].learnedSkills.Length; i++) {
			GUI.Label(rect, "" + (SKILLS)PartySkills.skills[currentPlayer].learnedSkills[i]);
			rect.y += 80;
		}
	}

	private void Update() {
		if (Input.GetButtonDown("expOpen")) showGUI = showGUI ? false : true;
		if (showGUI) use = Input.GetButtonDown("expConfirm");
		up = Input.GetButtonDown("expUp");
		down = Input.GetButtonDown("expDown");
	}

#endif

	#endregion
}
