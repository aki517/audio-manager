using UnityEngine;
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

        AnimationCurve cv = EditorGUILayout.CurveField( "Curve Fade In", res.curveFadeIn );
        res.curveFadeIn = AdjustFadeCurve( cv, 0.0f, 1.0f );

        cv = EditorGUILayout.CurveField( "Curve Fade Out", res.curveFadeOut );
        res.curveFadeOut = AdjustFadeCurve( cv, 1.0f, 0.0f );

        serializedObject.ApplyModifiedProperties();
    }

    private AnimationCurve AdjustFadeCurve( AnimationCurve cv, float fromValue, float toValue )
    {
        int numKey = cv.length;
        if( numKey >= 2 )
        {
            Keyframe[] keys = cv.keys;
            keys[ 0 ].value = fromValue;
            keys[ 0 ].time = 0.0f;
            
            int end = (numKey - 1);
            keys[ end ].value = toValue;
            keys[ end ].time = 1.0f;

            cv.keys = keys;
        }
        else{
            cv = AnimationCurve.Linear( 0, fromValue, 1, toValue );
        }

        return cv;
    }

}   // End of class AudioResourceInspector.

} // namespace Goisagi.
