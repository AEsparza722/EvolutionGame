using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcess : MonoBehaviour
{
    public static PostProcess instance;

    [SerializeField] Volume volume;
    Vignette vignetteDamage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        volume.profile.TryGet<Vignette>(out vignetteDamage); //Obtener componente vignette
    }

    public void PostProcessDefault()
    {
        if (vignetteDamage != null)
        {
            vignetteDamage.color.value = Color.black;
            vignetteDamage.intensity.value = .3f;
            vignetteDamage.smoothness.value = .2f;
        }
        
    }

    public IEnumerator DamagePostProcess(float seconds)
    {
        if (vignetteDamage != null)
        {
            vignetteDamage.color.value = Color.red;
            vignetteDamage.intensity.value = .45f;
            vignetteDamage.smoothness.value = .215f;
        }
        
        yield return new WaitForSeconds(seconds);
        PostProcessDefault();

    }

}
