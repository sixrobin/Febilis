namespace Templar.Boards
{
    using System.IO;
    using UnityEditor;

    public sealed class ScenesPassagesModificationProcessor : AssetModificationProcessor
    {
        // Workaround to make asset renaming work (https://www.youtube.com/watch?v=26Czu393xbQ).
        static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            ScenesPassagesHandler scenePassage = AssetDatabase.LoadMainAssetAtPath(sourcePath) as ScenesPassagesHandler;
            if (scenePassage == null)
                return AssetMoveResult.DidNotMove;

            string sourceDirectory = Path.GetDirectoryName(sourcePath);
            string destDirectory = Path.GetDirectoryName(destinationPath);
            if (sourceDirectory != destDirectory)
                return AssetMoveResult.DidNotMove;

            scenePassage.name = Path.GetFileNameWithoutExtension(destinationPath);
            return AssetMoveResult.DidNotMove;
        }
    }
}