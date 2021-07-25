namespace Templar.Tmp
{
    using System.IO;
    using UnityEditor;

    public class ScenePassagesModificationProcessor : AssetModificationProcessor
    {
        static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            ScenePassages scenePassage = AssetDatabase.LoadMainAssetAtPath(sourcePath) as ScenePassages;
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