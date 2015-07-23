using UnityEngine;
using System.Collections;
using RTS;

/* 
 * Script that handles all available game objects in the game
 * that a player can spawn
 * 
 * */

public class GameObjectList : MonoBehaviour {

	// Lists of available objects
	public GameObject[] buildings;
	public GameObject[] units;
	public GameObject[] worldObjects;
	public GameObject player;

	// Is the game object list spawned yet?
	private static bool created = false;

	void Awake() {
		if(!created) {
			DontDestroyOnLoad(transform.gameObject);
			ResourceManager.SetGameObjectList(this);
			created = true;
		} else {
			Destroy(this.gameObject);
		}
	}
	
	public GameObject GetPlayer() {
		return player;
	}

	public GameObject GetBuilding(string name) {
		for(int i = 0; i < buildings.Length; i++) {
			Building building = buildings[i].GetComponent< Building >();
			if(building && building.name == name) return buildings[i];
		}
		return null;
	}
	
	public GameObject GetUnit(string name) {
		for(int i = 0; i < units.Length; i++) {
			Unit unit = units[i].GetComponent< Unit >();
			if(unit && unit.name == name) return units[i];
		}
		return null;
	}
	
	public GameObject GetWorldObject(string name) {
		foreach(GameObject worldObject in worldObjects) {
			if(worldObject.name == name) return worldObject;
		}
		return null;
	}
	
	public Texture2D GetBuildImage(string name) {
		for(int i = 0; i < buildings.Length; i++) {
			Building building = buildings[i].GetComponent< Building >();
			if(building && building.name == name) return building.buildImage;
		}
		for(int i = 0; i < units.Length; i++) {
			Unit unit = units[i].GetComponent< Unit >();
			if(unit && unit.name == name) return unit.buildImage;
		}
		return null;
	}
}
