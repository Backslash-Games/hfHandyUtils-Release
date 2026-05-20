using UnityEngine;
using UnityEngine.VFX;
using HFHandyUtils.Effects;

namespace HFHandyUtils.DebugScripts
{
    /// <summary>
    ///     Debug script for Custom Effects... Should only be used in Unity Inspector 
    ///     <br></br>
    ///     <br>Luke Wittbrodt :: lwittbrodt87@gmail.com :: halfhand870</br>
    ///     <br><a href="https://halfhand870.notion.site/ds_EffectTester-34ad086035d38095a2a7e5dc6e900a38">Documentation</a></br>
    /// </summary>
    [AddComponentMenu("HFHandyUtils/Debug/ds_EffectTester")]
    public class ds_EffectTester : MonoBehaviour
    {
        public enum EffectTesterStates { state_1, state_2, state_3 };

        public EffectLibrary<EffectTesterStates, AudioClip, EffectComponent_Audio.AudioParameters> audioLibrary;
        public EffectLibrary<EffectTesterStates, VisualEffectAsset, EffectComponent_Visual.VisualParameters> particleLibrary;

        public void PlayState(int value)
        {
            // Provide library and state
            EffectManager.Instance.Play(audioLibrary, (EffectTesterStates)value);
            EffectManager.Instance.Play(particleLibrary, (EffectTesterStates)value);
        }

        public void PlayStateContinuous(int value)
        {
            // Provide library and state
            EffectManager.Instance.Play(audioLibrary, (EffectTesterStates)value, EffectManager.PlayMode.Continuous);
            EffectManager.Instance.Play(particleLibrary, (EffectTesterStates)value, EffectManager.PlayMode.Continuous);
        }

        public void StopStateContinuous(int value)
        {
            // Provide library and state
            EffectManager.Instance.Stop(audioLibrary, (EffectTesterStates)value);
            EffectManager.Instance.Stop(particleLibrary, (EffectTesterStates)value);
        }
    }
}