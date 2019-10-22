using UnityEngine.Audio;
using UnityEditor;

namespace Goisagi
{

/// <summary>
/// .
/// </summary>
[CustomEditor(typeof(AudioResource))]
public class AudioResourceInspector : Editor
{
    AudioResource res;

    void OnEnable()
    {
        res = ( AudioResource )target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        res.group = ( AudioMixerGroup )EditorGUILayout.ObjectField( "AudioMixerGroup", res.group, typeof( AudioMixerGroup ), true );
        
        res.priority = EditorGUILayout.IntSlider( "Priority", res.priority, AudioManager.PRIORITY_HIGHEST, AudioManager.PRIORITY_LOWEST );

        EditorGUILayout.PropertyField( serializedObject.FindProperty( "clips" ), true );

        serializedObject.ApplyModifiedProperties();
    }

}   // End of class AudioResourceInspector.

} // namespace Goisagi.
