using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class CanExecuteInEditor : Attribute
{
    public string name;
    public bool onlyRuntime;

    public CanExecuteInEditor() : this(null, false) { }
    public CanExecuteInEditor(bool onlyRuntime) : this(null, onlyRuntime) { }
    public CanExecuteInEditor(string name) : this(name, false) { }
    public CanExecuteInEditor(string name, bool onlyRuntime)
    {
        this.name = name;
        this.onlyRuntime = onlyRuntime;
    }
}
public class WaitFor
{
    public static IEnumerator EndOfFrameEditor()
    {
#if UNITY_EDITOR
        if (Application.isFocused)
        {
            yield return new WaitForEndOfFrame();
        }
        else
        {
            yield return null;
        }
#else
        yield return new WaitForEndOfFrame();
#endif
    }
}

#if UNITY_EDITOR

[CanEditMultipleObjects]
[CustomEditor(typeof(ScriptableObject), true)]
public class UltimateScriptableObjectInspector : UltimateInspector
{

}
[CanEditMultipleObjects]
[CustomEditor(typeof(MonoBehaviour), true)]
public class UltimateInspector : Editor
{
    bool ultimateInspector;

    Dictionary<string, object[]> args = new Dictionary<string, object[]>();

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MethodInfo[] allMethods = target.GetType().GetMethods();
        foreach (MethodInfo methodInfo in allMethods)
        {
            CanExecuteInEditor attribute = Attribute.GetCustomAttribute(methodInfo, typeof(CanExecuteInEditor)) as CanExecuteInEditor;

            if (attribute != null)
            {
                ParameterInfo[] parametersInfo = methodInfo.GetParameters();
                string key = GetUniqueMethodName(methodInfo);
                if (!args.ContainsKey(key)) args[key] = new object[parametersInfo.Length];

                args[key] = DrawArgs(args[key], parametersInfo);

                if (GUILayout.Button(String.IsNullOrEmpty(attribute.name) ? methodInfo.Name : attribute.name))
                {
                    Type[] types = new Type[parametersInfo.Length];
                    for (int i = 0; i < parametersInfo.Length; i++) types[i] = parametersInfo[i].ParameterType;
                    object response = methodInfo.Invoke(target, args[key]);

                    if (response is UnityEngine.Object)
                        Debug.Log("Result " + methodInfo.Name + "= " + response, (UnityEngine.Object)response);
                    else
                        Debug.Log("Result " + methodInfo.Name + "= " + response);


                }
            }
        }


    }


    public static object[] DrawArgs(object[] args, ParameterInfo[] pars)
    {
        if (args == null || args.Length != pars.Length) args = new object[pars.Length];
        int i = 0;
        foreach (ParameterInfo p in pars)
        {

            EditorGUILayout.BeginHorizontal();
            args[i] = DrawArg(args[i], p.ParameterType, p.Name);
            EditorGUILayout.EndHorizontal();
            i++;
        }

        return args;
    }

    private static object DrawArg(object arg, Type type, string name)
    {
        if (type.IsSubclassOf(typeof(UnityEngine.Object)))
        {
            arg = EditorGUILayout.ObjectField(name, (UnityEngine.Object)arg, type, true);
        }
        else if (type == typeof(int))
        {
            if (arg == null) arg = 0;
            arg = EditorGUILayout.IntField(name, (int)arg);
        }
        else if (type.IsEnum)
        {
            if (arg == null) arg = 0;
            arg = (int)Convert.ChangeType(EditorGUILayout.EnumPopup(name, (Enum)Enum.GetValues(type).GetValue((int)arg)), type);
        }

        else if (type == typeof(bool))
        {
            if (arg == null) arg = false;
            arg = EditorGUILayout.Toggle(name, (bool)arg);
        }
        else if (type == typeof(float))
        {
            if (arg == null) arg = 0f;
            arg = EditorGUILayout.FloatField(name, (float)arg);
        }
        else if (type == typeof(Vector3))
        {
            if (arg == null) arg = Vector3.zero;
            arg = EditorGUILayout.Vector3Field(name, (Vector3)arg);
        }
        else if (type == typeof(string))
        {
            if (arg == null) arg = "";
            arg = EditorGUILayout.TextField(name, (string)arg);
        }
        else if (type == typeof(object[]))
        {
            GUILayout.Label("Arguments dynamiques");
        }
        else if (type.IsSubclassOf(typeof(SerializedProperty)))
        {
            Debug.Log("Yes subclass of it ! ");
        }
        else
        {
            GUILayout.Label("Unkown arg: " + type);
        }
        return arg;
    }

    public static string GetUniqueMethodName(MethodInfo methodInfo)
    {
        string uniqueId = methodInfo.Name;
        foreach (ParameterInfo parameter in methodInfo.GetParameters())
        {
            uniqueId += parameter.Name + parameter.ParameterType;
        }
        return uniqueId;
    }
}
#endif