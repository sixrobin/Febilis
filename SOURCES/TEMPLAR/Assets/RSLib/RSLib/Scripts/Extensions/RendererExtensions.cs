namespace RSLib.Extensions
{
	using UnityEngine;

    public static class RendererExtensions
    {
		#region GENERAL

		/// <summary>Checks if the renderer is visible by a given camera.</summary>
		/// <param name="camera">Camera to check.</param>
		/// <returns>True if the renderer is visible by the specified camera.</returns>
		public static bool IsVisibleByCamera(this Renderer rend, Camera camera)
		{
			return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), rend.bounds);
		}

		#endregion

		#region MATERIALS

		/// <summary>Sets the renderer material at a given index.</summary>
		/// <param name="index">Index of the changed material.</param>
		/// <param name="newMaterial">Material to set.</param>
		/// <returns>True if change has been made successfully.</returns>
		public static void ChangeMaterialAtIndex(this Renderer renderer, int index, Material newMaterial)
		{
			Material[] materialsCopy = renderer.materials;
			materialsCopy[index] = newMaterial;
			renderer.materials = materialsCopy;
		}

		/// <summary>Sets the renderer shared material at a given index.</summary>
		/// <param name="index">Index of the changed shared material.</param>
		/// <param name="newMaterial">Material to set.</param>
		/// <returns>True if change has been made successfully.</returns>
		public static void ChangeSharedMaterialAtIndex(this Renderer rend, int index, Material newMaterial)
		{
			Material[] sharedMaterialsCopy = rend.sharedMaterials;
			sharedMaterialsCopy[index] = newMaterial;
			rend.sharedMaterials = sharedMaterialsCopy;
		}

		#endregion MATERIALS
	}
}