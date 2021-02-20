namespace DoomlikeEditor
{
	using UnityEngine;
	using UnityEditor;

	public class PixelPerfectEnabler
	{
		[MenuItem("Tools/Pixel Perfect/Enable Edit Mode")]
		public static void EnablePixelPerfectEditMode()
		{
			Debug.Log("Enabling Pixel Perfect edit mode...");

			UnityEngine.U2D.PixelPerfectCamera[] pixelPerfectComponents = GameObject.FindObjectsOfType<UnityEngine.U2D.PixelPerfectCamera>();
			for (int i = pixelPerfectComponents.Length - 1; i >= 0; --i)
				pixelPerfectComponents[i].runInEditMode = true;
		}

		[MenuItem("Tools/Pixel Perfect/Disable Edit Mode")]
		public static void DisablePixelPerfectEditMode()
		{
			Debug.Log("Disabling Pixel Perfect edit mode...");

			UnityEngine.U2D.PixelPerfectCamera[] pixelPerfectComponents = GameObject.FindObjectsOfType<UnityEngine.U2D.PixelPerfectCamera>();
			for (int i = pixelPerfectComponents.Length - 1; i >= 0; --i)
				pixelPerfectComponents[i].runInEditMode = false;
		}
	}
}