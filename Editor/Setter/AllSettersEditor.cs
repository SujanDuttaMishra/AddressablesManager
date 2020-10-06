using System.Linq;
using AddressableManager.AddressableSetter.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AllSetters), true)]
public class AllSettersEditor : Editor
{
    private GlobalSettersEditor<AllSetters> GlobalSettersEditor { get; set; }
    internal AllSetters Setter { get; private set; }

    private void OnEnable()
    {
        if (target == null) return;
        Setter = (AllSetters)target;
        GlobalSettersEditor = new GlobalSettersEditor<AllSetters>(this);
    }

    public override void OnInspectorGUI()
    {
        AllSetters.RemoveNullOrUnpopulated();
        serializedObject.ApplyModifiedProperties();
        GlobalSettersEditor.Init();
        serializedObject.Update();
       

    }
}
