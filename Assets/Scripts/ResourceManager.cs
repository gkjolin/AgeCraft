using UnityEngine;
using System.Collections;

namespace RTS {
	public static class ResourceManager {
		public static float ClickTolerance { get { return 0.8f; } }

		public static float CameraScrollSpeed { get { return 100; } } // Not  adjusted by Time.deltaTime
		public static int CameraMoveTriggerPadding { get { return 15; } }
		
		public static float CameraMoveSpeed { get { return 100; } }
		public static float CameraShiftBonusSpeed { get { return 100; } }

		public static float MinCameraHeight { get { return 20; } }
		public static float MaxCameraHeight { get { return 60; } }
	}
}