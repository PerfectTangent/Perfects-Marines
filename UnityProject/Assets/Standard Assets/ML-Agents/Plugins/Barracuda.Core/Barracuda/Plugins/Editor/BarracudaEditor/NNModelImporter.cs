using System.IO;
using UnityEditor;
using UnityEngine;


namespace Barracuda
{
    /// <summary>
    /// Asset Importer of barracuda models.
    /// </summary>
    [UnityEditor.AssetImporters.ScriptedImporter(1, new[] {"nn"})]
    public class NNModelImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        private const string k_IconName = "NNModelIcon";

        private Texture2D m_IconTexture;

        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
        {
            var model = File.ReadAllBytes(ctx.assetPath);
            var asset = ScriptableObject.CreateInstance<NNModel>();
            asset.Value = model;

            ctx.AddObjectToAsset("main obj", asset, LoadIconTexture());
            ctx.SetMainObject(asset);
        }

        private Texture2D LoadIconTexture()
        {
            if (m_IconTexture == null)
            {
                var allCandidates = AssetDatabase.FindAssets(k_IconName);

                if (allCandidates.Length > 0)
                {
                    m_IconTexture = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(allCandidates[0]), typeof(Texture2D)) as Texture2D;
                }
            }
            return m_IconTexture;
        }
    }
}
