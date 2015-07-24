using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using RTS;

public class MinimapController : NetworkBehaviour {

	private Resolution ScreenResolution;
	
	private Player player;

	private static Material lineMaterial;
	private static void CreateLineMaterial() {
		if( !lineMaterial ) {
			lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
			                            "SubShader { Pass { " +
			                            "    Blend SrcAlpha OneMinusSrcAlpha " +
			                            "    ZWrite Off Cull Off Fog { Mode Off } " +
			                            "    BindChannels {" +
			                            "      Bind \"vertex\", vertex Bind \"color\", color }" +
			                            "} } }" );
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	void Awake() {
		player = GetComponent<Player> ();
	}

	public void RenderViewportBox() {

		Ray topLeftRay = player.playCam.ViewportPointToRay(new Vector3(0, 1, 0));
		Ray topRightRay = player.playCam.ViewportPointToRay(new Vector3(1, 1, 0));
		Ray bottomLeftRay = player.playCam.ViewportPointToRay(new Vector3(0, 0, 0));
		Ray bottomRightRay = player.playCam.ViewportPointToRay(new Vector3(1, 0, 0));

		RaycastHit topLeftHit, topRightHit, bottomLeftHit, bottomRightHit;
		Vector2 topLeftMinimap, topRightMinimap, bottomLeftMinimap, bottomRightMinimap;

		// set the current material
		CreateLineMaterial();
		lineMaterial.SetPass( 0 );
		GL.Begin( GL.LINES );
		GL.Color( Color.white );

		Physics.Raycast (topLeftRay, out topLeftHit);
		Vector3 topLeftWorld = topLeftHit.point;
		topLeftMinimap = Common.CalculateMinimapPosFromWorldCoordinate (topLeftWorld);

		Physics.Raycast (topRightRay, out topRightHit);
		Vector3 topRightWorld = topRightHit.point;
		topRightMinimap = Common.CalculateMinimapPosFromWorldCoordinate (topRightWorld);

		Physics.Raycast (bottomRightRay, out bottomRightHit);
		Vector3 bottomRightWorld = bottomRightHit.point;
		bottomRightMinimap = Common.CalculateMinimapPosFromWorldCoordinate (bottomRightWorld);

		Physics.Raycast (bottomLeftRay, out bottomLeftHit);
		Vector3 bottomLeftWorld = bottomLeftHit.point;
		bottomLeftMinimap = Common.CalculateMinimapPosFromWorldCoordinate (bottomLeftWorld);
		
		GL.Vertex3( topLeftMinimap.x, topLeftMinimap.y, 0f );
		GL.Vertex3( topRightMinimap.x, topRightMinimap.y, 0f );
		GL.Vertex3( topRightMinimap.x, topRightMinimap.y, 0f );
		GL.Vertex3( bottomRightMinimap.x, bottomRightMinimap.y, 0f );
		GL.Vertex3( bottomRightMinimap.x, bottomRightMinimap.y, 0f );
		GL.Vertex3( bottomLeftMinimap.x, bottomLeftMinimap.y, 0f );
		GL.Vertex3( bottomLeftMinimap.x, bottomLeftMinimap.y, 0f );
		GL.Vertex3( topLeftMinimap.x, topLeftMinimap.y, 0f );

		GL.End();

	}
	
	// Use this for initialization
	public void ShowMinimap () {
		
		Resolution currentResolution = Screen.currentResolution;
		
		if (Application.isEditor) {
			ScreenResolution = Screen.resolutions [0];
		} else {
			ScreenResolution = currentResolution;
		}

		Screen.SetResolution (ScreenResolution.width, ScreenResolution.height, true);

		// Adjust the camera to center the map (world coordinates)
		Vector3 minimapOffset = new Vector3 (0f, 350f, 0f);
		player.miniCam.transform.position = minimapOffset;
		
		Debug.Log (player.playCam);
		Debug.Log (player.miniCam);
		
		//		miniCam.orthographicSize = ScreenResolution.height / 2;
		//		miniCam.orthographic = true;
		
		player.miniCam.rect = new Rect (
			ResourceManager.MinimapOffsetX,
			ResourceManager.MinimapOffsetZ,
			ResourceManager.MinimapSizeX,
			ResourceManager.MinimapSizeZ
			);
	}

	public void HideMinimap() {

		player.miniCam.gameObject.SetActive (false);

	}

//
//	public GameObject MinimapCamera;
//
//	private Resolution ScreenResolution;
//	public LayerMask UICameraLayerMask;
//
//	public static MinimapController Instance;
//
//	// Use this for initialization
//	void Start () {
//		Instance = this;
//
//		Resolution currentResolution = Screen.currentResolution;
//
//		if (Application.isEditor) {
//			ScreenResolution = Screen.resolutions [0];
//		} else {
//			ScreenResolution = currentResolution;
//		}
//
//		Screen.SetResolution (ScreenResolution.width, ScreenResolution.height, true);
//
//		// UI Camera setup
//		MinimapCamera = new GameObject ("MinimapCamera");
//		MinimapCamera.AddComponent<Camera>();
//
//		Camera miniCam = MinimapCamera.GetComponent<Camera>();
//		miniCam.cullingMask = UICameraLayerMask;
//		miniCam.name = "Minimap";
//
//		miniCam.transform.position = new Vector3 (0f, 100f, 0f);
//		miniCam.transform.eulerAngles = new Vector3 (90f, 0f, 0f);
//
//		miniCam.orthographicSize = ScreenResolution.height / 2;
//		miniCam.orthographic = true;
//
//		miniCam.nearClipPlane = 0.1f;
//		miniCam.farClipPlane = 400f;
//		miniCam.clearFlags = CameraClearFlags.Depth;
//
//		miniCam.depth = 3;
//
//		miniCam.rect = new Rect (0.05f, 0.05f, 0.3f, 0.3f);
//		miniCam.renderingPath = RenderingPath.UsePlayerSettings;
//
//		miniCam.targetTexture = null;
//		miniCam.hdr = false;
//
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
}
