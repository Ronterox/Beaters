using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Plugins.Tools;

namespace Gacha{
    public class GlowRotate : MonoBehaviour
{

    public float speed;
    public PostProcessVolume volume;
    private Bloom m_Bloom;
    private bool m_flag;
  
      void Start(){
          if (volume != null){
             volume.profile.TryGetSettings(out m_Bloom);
          }
          
      }
  
      void Update(){
          if(m_Bloom.intensity.value.Approximates(0, 0.1f))
          {
              ChangeBloomIntensity(speed);
            
          } else if(m_Bloom.intensity.value.Approximates(20, 0.1f))
              {
                  ChangeBloomIntensity(-speed);
              } 
      } 

    private void ChangeBloomIntensity(float increment)
    {
        if(m_flag){
            return;
        }

        bool isIncrement = increment > 0;
        float target = isIncrement? 20 : 0;
        

        IEnumerator incrementIntensity = UtilityMethods.FunctionCycleCoroutine(
            () => !m_Bloom.intensity.value.Approximates(target, 0.1f) && isIncrement? m_Bloom.intensity.value < target : m_Bloom.intensity.value > target,
            () => m_Bloom.intensity.value += increment * Time.deltaTime,
            null,
            () => m_flag = true,
            () => m_flag = false
        );
        StartCoroutine(incrementIntensity);
    }
}
}
