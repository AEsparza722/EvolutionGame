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

    public IEnumerator DamagePostProcess(float seconds, Vector3 attackPosition)
    {
        if (vignetteDamage != null)
        {
            Vector2 direction = Camera.main.transform.position - attackPosition;
            //20 = 0.6
            float distance = direction.magnitude;
            float normalizedDistance = 0f - (distance / 50f) * 1f; //Algoritmo cambiar tama;o de indicador
            float normalizedReal = (1 + normalizedDistance) / 2f;
            float normalizedRealClamped = Mathf.Clamp(normalizedReal,0.35f,1f);
            

            vignetteDamage.color.value = Color.red;
            vignetteDamage.intensity.value = normalizedRealClamped; //.45f
            vignetteDamage.smoothness.value = .215f;
        }
        
        yield return new WaitForSeconds(seconds);
        PostProcessDefault();

    }

}
