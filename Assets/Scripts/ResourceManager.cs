using UnityEngine;
using System.Collections;

namespace RTS {
	public static class ResourceManager {
		public static float ClickTolerance { get { return 0.8f; } }

		public static float ScrollSpeed { get { return 40; } }
		public static float RotateSpeed { get { return 100; } }
		public static int ScrollWidth { get { return 15; } }

		public static float MinCameraHeight { get { return 20; } }
		public static float MaxCameraHeight { get { return 100; } }
	}
}