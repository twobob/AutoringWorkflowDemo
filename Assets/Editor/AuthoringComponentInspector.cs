using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class AuthoringComponentInspector : EditorWindow
{

    private List<IConvertGameObjectToEntity> AuthoringComponents = new List<IConvertGameObjectToEntity>();

    private Dictionary<string, AuthoringComponentMapping> TextFieldAuthoringComponentMap = new Dictionary<string, AuthoringComponentMapping>();


    private int cachedGoComponentCount;

    /// <summary>
    /// Should the data components be automaticaly synchronized ?
    /// </summary>
    private bool autoSync = false;

    private class AuthoringComponentMapping
    {
        public TextField tf;
        public ObjectField objectField;
        public IConvertGameObjectToEntity authoringComponent;
        public FieldInfo fieldInfo;
    }

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
        if ( cachedGoComponentCount != activeGoComponentCount)
        {
            RefreshInspector();
            cachedGoComponentCount = activeGoComponentCount;
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

        foreach (var tfem in TextFieldAuthoringComponentMap)
        {
            if(tfem.Value.tf != null)
            {
                    tfem.Value.tf.UnregisterValueChangedCallback(ChangeAuthoringComponentValue);
            }
            if(tfem.Value.objectField != null)
            {
                tfem.Value.objectField.UnregisterValueChangedCallback(ChangeAuthoringPrefab);
            }
        }

        TextFieldAuthoringComponentMap.Clear();
        AuthoringComponents.Clear();

        if (autoSync)
        {
            SyncAuthoringComponents();
        }
      

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
        if (!autoSync)
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
        var activeGo = Selection.activeGameObject;
        if (activeGo == null) return;

        // Get the list of required data components
        List<Type> RequiredAuthoringComponentsTypes = new List<Type>();
        var components = activeGo.GetComponents(typeof(MonoBehaviour));
        foreach (var mono in components)
        {
            Type requieringType = mono.GetType();
            foreach (var requiredAttribute in RequireComponent.GetCustomAttributes(requieringType, typeof(RequireComponent)))
            {
                Type requiredType = ((RequireComponent)requiredAttribute).m_Type0;
                if (requiredType != null) RegisterRequierment(RequiredAuthoringComponentsTypes,  requieringType, requiredAttribute, requiredType);
                requiredType = ((RequireComponent)requiredAttribute).m_Type1;
                if (requiredType != null) RegisterRequierment(RequiredAuthoringComponentsTypes,  requieringType, requiredAttribute, requiredType);
                requiredType = ((RequireComponent)requiredAttribute).m_Type2;
                if (requiredType != null) RegisterRequierment(RequiredAuthoringComponentsTypes,  requieringType, requiredAttribute, requiredType);
            }
        }

        // remove un necessary components
        var existingDataComponents = new List<Type>();
        foreach (var dataComponent in activeGo.GetComponents(typeof(IConvertGameObjectToEntity)))
        {
            if (!RequiredAuthoringComponentsTypes.Contains(dataComponent.GetType()))
            {
                DestroyImmediate(activeGo.GetComponent(dataComponent.GetType()));
            }
            else { 
                existingDataComponents.Add(dataComponent.GetType());
            }
        }

        // Add missing required data components
        foreach (var requiredComponent in RequiredAuthoringComponentsTypes)
        {

            if (!existingDataComponents.Contains(requiredComponent))
            {
                activeGo.AddComponent(requiredComponent);
            }
        }

        // Disable refresh if autoSync is active to avoid a infinite loop and stack overflow.
        if (!autoSync)
        {
            RefreshInspector();
        }
        
    }

    private static void RegisterRequierment(List<Type> RequiredAuthoringComponentsTypes, Type requieringType, Attribute requiredAttribute, Type requiredType)
    {
        if (((RequireComponent)requiredAttribute).m_Type0 != null)
        {
            RequiredAuthoringComponentsTypes.Add(requiredType);
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
                if(DisplayAuthoringComponentAttributes(authoringComponent, f))
                {
                    rootVisualElement.Add(f);
                }
            }
        }
    }

    private bool DisplayAuthoringComponentAttributes(IConvertGameObjectToEntity authoringComponent,Foldout foldout)
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
                        TextField tf = new TextField(prop.Name);
                        tf.name = GUID.Generate().ToString();
                        tf.SetValueWithoutNotify(fi.GetValue(authoringComponent).ToString());

                        tf.tooltip = fi.FieldType.ToString();
                         
                        AuthoringComponentMapping acm = new AuthoringComponentMapping();
                        acm.authoringComponent = authoringComponent;
                        acm.fieldInfo = fi;
                        acm.tf = tf;

                        TextFieldAuthoringComponentMap.Add(tf.name, acm);

                        tf.RegisterValueChangedCallback(ChangeAuthoringComponentValue);

                        foldout.Add(tf);
                        
                    }
                    else
                    {


                        ObjectField gameObjectPicker = new ObjectField(prop.Name);
                        gameObjectPicker.name = GUID.Generate().ToString();
                        gameObjectPicker.objectType = fi.FieldType;
                        gameObjectPicker.SetValueWithoutNotify((GameObject)fi.GetValue(authoringComponent));


                        AuthoringComponentMapping acm = new AuthoringComponentMapping();
                        acm.authoringComponent = authoringComponent;
                        acm.fieldInfo = fi;
                        acm.objectField = gameObjectPicker;

                        TextFieldAuthoringComponentMap.Add(gameObjectPicker.name, acm);

                        foldout.Add(gameObjectPicker);

                        gameObjectPicker.RegisterValueChangedCallback(ChangeAuthoringPrefab);

                    }
                    hasAttributes = true;
                }
            }
          
        }
        return hasAttributes;
    }

    private void ChangeAuthoringPrefab(ChangeEvent<UnityEngine.Object> evt)
    {
        var ofid = ((ObjectField)evt.target).name;
        if (!TextFieldAuthoringComponentMap.ContainsKey(ofid)) return;
        FieldInfo fi = TextFieldAuthoringComponentMap[ofid].fieldInfo;
        IConvertGameObjectToEntity authoringComponent = TextFieldAuthoringComponentMap[ofid].authoringComponent;

        fi.SetValue(authoringComponent, Convert.ChangeType(evt.newValue, fi.FieldType));
    }


    private void ChangeAuthoringComponentValue(ChangeEvent<string> evt)
    {
        var tfid = ((TextField)evt.target).name;
        if (!TextFieldAuthoringComponentMap.ContainsKey(tfid)) return;
        FieldInfo fi = TextFieldAuthoringComponentMap[tfid].fieldInfo;
        IConvertGameObjectToEntity authoringComponent = TextFieldAuthoringComponentMap[tfid].authoringComponent;
        try
        {
            fi.SetValue(authoringComponent, Convert.ChangeType(evt.newValue, fi.FieldType));
        }
#pragma warning disable 168
        catch (Exception e)
#pragma warning restore 168
        {
            TextFieldAuthoringComponentMap[tfid].tf.value = evt.previousValue;
        }
              
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