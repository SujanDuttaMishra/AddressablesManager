using AddressableManager.AddressableSetter.Editor;
using UnityEditor;
[CustomEditor(typeof(SetterList), true)]
public class SetterListEditor : Editor
{
    private GlobalSettersEditor<SetterList> GlobalSettersEditor { get; set; }
    internal SetterList Setter { get; private set; }

    private void OnEnable()
    {
        if (target == null) return;
        Setter = (SetterList)target;
        GlobalSettersEditor = new GlobalSettersEditor<SetterList>(this);
    }

    public override void OnInspectorGUI()
    {
        if (Setter == null) return;

        GlobalSettersEditor.Init();
        serializedObject.Update();
    }
}
