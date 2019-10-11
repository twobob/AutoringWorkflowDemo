using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class AuthoringComponentInspector : EditorWindow
{
    private List<IConvertGameObjectToEntity> AuthoringComponents = new List<IConvertGameObjectToEntity>();

 
    private bool AutoSync { get => AuthoringComponentInspectorSettings.GetSerializedSettings().FindProperty("autoSync").boolValue; }

    private bool CachedAutoSync = false;

    private int cachedGoComponentCount;



    
    [MenuItem("Window/AuthoringComponentInspector")]
    public static void ShowExample()
    {
        AuthoringComponentInspector wnd = GetWindow<AuthoringComponentInspector>();
        wnd.titleContent = new GUIContent("Authoring Component Inspector");

    }

    private void OnFocus()
    {
        RefreshInspector();
    }

    private void OnProjectChange()
    {
        RefreshInspector();
    }

    private void OnInspectorUpdate()
    {


        if (Selection.activeGameObject == null) return;

        int activeGoComponentCount = Selection.activeGameObject.GetComponents(typeof(MonoBehaviour)).Length;
        if (cachedGoComponentCount != activeGoComponentCount)
        {
            RefreshInspector();
            cachedGoComponentCount = activeGoComponentCount;
        }

        if (AutoSync != CachedAutoSync)
        {
            CachedAutoSync = AutoSync;
            RefreshInspector();
        }

    }


    private void OnSelectionChange()
    {
        RefreshInspector();
    }
    public void OnEnable()
    {
        RefreshInspector();
    }

    private void Clean()
    {
        rootVisualElement.Clear();

        AuthoringComponents.Clear();

    }

    private void RefreshInspector()
    {
        // Clear Display and deregister callbacks
        Clean();

        // Check we are targeting a GameObject
        var activeGo = Selection.activeGameObject;
        if (activeGo == null) return;

        // Refresh the list of authoring components on the game object
        GetAuthoringComponents(activeGo);



        Label label = new Label(Selection.activeGameObject.name);

        rootVisualElement.Add(label);

        // Allow manaul sync if autoSync is disabled
        if (!AutoSync)
        {
            Button cleanButeon = new Button(SyncAuthoringComponents);
            cleanButeon.text = "Synch Authoring Components";
            rootVisualElement.Add(cleanButeon);
        }

        // Display a foldout for each data component of the game object.
        DisplayAuthoringComponents();


    }

    private void SyncAuthoringComponents()
    {
        DataComponentHelper.ActualizeRequieredDataComponentsOnGameObject();
        if (!AutoSync)
        {
            RefreshInspector();
        }
    }

    private void DisplayAuthoringComponents()
    {

        if (AuthoringComponents.Count != 0)
        {
            foreach (var authoringComponent in AuthoringComponents)
            {
                Foldout f = new Foldout();
                f.text = authoringComponent.GetType().ToString();

                // Add a text field for each attribute of the authoring component
                if (DisplayAuthoringComponentAttributes(authoringComponent, f))
                {
                    rootVisualElement.Add(f);
                }
            }
        }
    }

    private bool DisplayAuthoringComponentAttributes(IConvertGameObjectToEntity authoringComponent, Foldout foldout)
    {
        bool hasAttributes = false;
        foreach (MemberInfo prop in authoringComponent.GetType().GetTypeInfo().GetMembers())
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            if (prop.DeclaringType.Equals(authoringComponent.GetType()) && MemberTypes.Field.Equals(prop.MemberType))
            {
                if (!properties.ContainsKey(prop.Name))
                {

                    FieldInfo fi = authoringComponent.GetType().GetField(prop.Name);
                    if (fi.FieldType != typeof(GameObject))
                    {
                        DataComponentAttributeTextField tf = new DataComponentAttributeTextField(fi, authoringComponent, prop.Name);
                        foldout.Add(tf);
                        DataComponentHelper.AddAttributeMapping(fi, tf);

                    }
                    else
                    {

                        DataComponentAttributeObjectField gameObjectPicker = new DataComponentAttributeObjectField(fi, authoringComponent, prop.Name);
                        foldout.Add(gameObjectPicker);
                        DataComponentHelper.AddAttributeMapping(fi, gameObjectPicker);

                    }
                    hasAttributes = true;
                }
            }

        }
        return hasAttributes;
    }


    private void GetAuthoringComponents(GameObject activeGo)
    {
        IConvertGameObjectToEntity[] comps = activeGo.GetComponents<IConvertGameObjectToEntity>();
        List<IConvertGameObjectToEntity> ECSComponents = new List<IConvertGameObjectToEntity>(comps);

        for (int i = 0; i < ECSComponents.Count; ++i)
        {

            if (!AuthoringComponents.Exists(c => c.GetType() == ECSComponents[i].GetType()))
            {
                AuthoringComponents.Add(ECSComponents[i]);
            }
        }

        for (int i = AuthoringComponents.Count - 1; i >= 0; i--)
        {
            if (!ECSComponents.Exists(c => c.GetType() == AuthoringComponents[i].GetType()))
            {
                AuthoringComponents.Remove(AuthoringComponents[i]);
            }
        }
        AuthoringComponents.Sort(AlphabeticOrder);
    }

    private int AlphabeticOrder(IConvertGameObjectToEntity x, IConvertGameObjectToEntity y)
    {
        return x.GetType().Name.CompareTo(y.GetType().Name);
    }
}
