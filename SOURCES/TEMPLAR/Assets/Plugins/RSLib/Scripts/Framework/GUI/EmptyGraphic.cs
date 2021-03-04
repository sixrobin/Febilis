namespace RSLib.Framework.GUI
{
    /// <summary>
    /// Class that allows the creation of an empty Graphic, avoiding overdraw.
    /// Can be used to set UI graphic raycast sizes.
    /// </summary>
    public class EmptyGraphic : UnityEngine.UI.Graphic
    {
        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vh)
        {
            vh.Clear();
        }
    }
}