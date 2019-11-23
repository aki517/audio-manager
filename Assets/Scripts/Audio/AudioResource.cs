using UnityEngine;
using UnityEngine.Audio;

namespace Goisagi
{

/// <summary>
/// オーディオリソースクラス.
/// </summary>
[CreateAssetMenu( menuName = "Goisagi/Audio/Create AudioResource" )]
public class AudioResource : ScriptableObject
{
    public AudioMixerGroup group;
    public int priority = AudioManager.PRIORITY_MIDDLE;
    public AudioClip[] clips;
    public AnimationCurve curveFadeIn = AnimationCurve.Linear( 0, 0, 1, 1 );
    public AnimationCurve curveFadeOut = AnimationCurve.Linear( 0, 1, 1, 0 );

}   // End of class AudioResource.

} // namespace Goisagi.
