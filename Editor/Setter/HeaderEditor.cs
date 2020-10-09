using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace AddressableManager.AddressableSetter.Editor
{
    internal class HeaderEditor<T> where T : ScriptableObject
    {
        private UnityEditor.Editor MainEditor { get; set; }
        private T Target => (T)MainEditor.target;
        internal HeaderEditor(UnityEditor.Editor editor) { MainEditor = editor; }
        internal bool Init( out string assetPath)
        {
            var style = new GUIStyle { richText = true };

            var asset = Utilities.GetAsset<T>(Target.name);
            assetPath = AssetDatabase.GetAssetPath(asset);
            assetPath = Path.ChangeExtension(assetPath, null);
           
            BeginVertical("box");
            BeginHorizontal();
            
            if (Utilities.UButton(Target.name, 3)) EditorGUIUtility.PingObject(asset);
            var isCorrectPath = Target.name == asset.name;
            var header = isCorrectPath ?
                $"<size=12> <color=grey>  =>  {assetPath} </color> </size>" :
                $"<size=12> <color=red>  =>  {Target.name} != {asset.name} </color> <color=grey> press button to locate then select asset to continue </color> </size>";
            GUILayout.Label(header, style, Utilities.MaxWidth(2));

            EndHorizontal();
            EndVertical();
            return isCorrectPath;
        }


    }
}
