using System.Linq;
using AddressableManager.AddressableSetter.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AllSetters), true)]
public class AllSettersEditor : Editor
{
    private ListDisplayEditor<AllSetters> ListDisplayEditor { get; set; }
    internal AllSetters Setter { get; private set; }

    private void OnEnable()
    {
        if (target == null) return;
        Setter = (AllSetters)target;
        ListDisplayEditor = new ListDisplayEditor<AllSetters>(this);
    }

    public override void OnInspectorGUI()
    {
       
        AllSetters.RemoveNullOrUnpopulated();
        serializedObject.ApplyModifiedProperties();
        ListDisplayEditor.Init();
        serializedObject.Update();


    }
}
