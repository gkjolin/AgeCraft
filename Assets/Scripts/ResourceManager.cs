using UnityEngine;
using System.Collections;

namespace RTS {
	public static class ResourceManager {

		// Game object list instantiator
		private static GameObjectList gameObjectList;
		public static void SetGameObjectList(GameObjectList objectList) {
			gameObjectList = objectList;
		}
		
		public static GameObject GetPlayer() {
			return gameObjectList.GetPlayer();
		}

		public static GameObject GetBuilding(string name) {
			return gameObjectList.GetBuilding(name);
		}
		
		public static GameObject GetUnit(string name) {
			return gameObjectList.GetUnit(name);
		}
		
		public static GameObject GetWorldObject(string name) {
			return gameObjectList.GetWorldObject(name);
		}
		
		public static Texture2D GetBuildImage(string name) {
			return gameObjectList.GetBuildImage(name);
		}

		// Mouse variables
		public static float ClickTolerance { get { return 0.8f; } }

		// Camera variables
		public static float CameraScrollSpeed { get { return 100; } } // Not  adjusted by Time.deltaTime
		public static float CameraMoveTriggerPadding { get { return 15; } }
		
		public static float CameraMoveSpeed { get { return 100; } }
		public static float CameraShiftBonusSpeed { get { return 100; } }

		public static float MinCameraHeight { get { return 20; } }
		public static float MaxCameraHeight { get { return 60; } }

		// Minimap location
		public static float MinimapOffsetX { get { return 0.02f; } }
		public static float MinimapOffsetZ { get { return 0.02f; } }
		public static float MinimapSizeZ { get { return 0.30f; } }
		public static float MinimapSizeX { get { 
				float aspectRatio = (float) Screen.height / Screen.width;
				return MinimapSizeZ * aspectRatio; 
			} 
		}

		// Has to be sufficiently long (more than 350 at least)
		public static int Raylength { get { return 500; } }

		// HUD
		public static float HudHeight { get { return (float) Screen.height / 3.5f; } }

		// Buildings build speed
		public static int BuildSpeed { get { return 2; } }

	}
}