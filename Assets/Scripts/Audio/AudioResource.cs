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

}   // End of class AudioResource.

} // namespace Goisagi.
