using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ToonShaderLightSettings : MonoBehaviour
{
    struct LightSet
    {
        public int id;
        public Light light;
        public Vector3 dir;
        public Color color;
        public float atten;
        public float inView;

        public LightSet(Light newLight)
        {
            light = newLight;
            id = newLight.GetInstanceID();
            dir = Vector3.zero;
            color = Color.black;
            color.a = 0.01f;
            atten = 0f;
            inView = 1.1f; // Range -0.1 to 1.1 which is clamped 0-1 for faster consistent fade
        }
    }

    // State
    Vector3 posAbs;
    Dictionary<int, LightSet> lightSets;

    // Refs
    Material materialInstance;
    SkinnedMeshRenderer skinRenderer;
    MeshRenderer meshRenderer;


    private Light light;

	void OnEnable()
	{
		light = GetComponent<Light>();
	}
	
	void Update ()
	{
        //Shader.SetGlobalVector("_ToonLightDirection", -transform.forward);
        //Shader.SetGlobalColor("_ToonLightColor", mainLight.color);
        // Shader.SetGlobalFloat("_ToonLightIntensity", mainLight.intensity);

        float dist;

        posAbs = new Vector3(0,1f,0);//this is the absolute world position that is the center of the object being lit

        if (!light.isActiveAndEnabled)
        {
            Shader.SetGlobalFloat("_ToonLightIntensity", 0f);
        }

        switch (light.type)
        {
            case LightType.Directional:
                Shader.SetGlobalVector("_ToonLightDirection", light.transform.forward * -1f);
                Shader.SetGlobalColor("_ToonLightColor", light.color * light.intensity);
                Shader.SetGlobalFloat("_ToonLightIntensity", 1f);
                break;

            case LightType.Point:
                Shader.SetGlobalVector("_ToonLightDirection", light.transform.position - posAbs);
                dist = Mathf.Clamp01((light.transform.position - posAbs).magnitude / light.range);
                Shader.SetGlobalFloat("_ToonLightIntensity", CalcAttenuation(dist));
                Shader.SetGlobalColor("_ToonLightColor", light.color * CalcAttenuation(dist) * light.intensity * 0.1f);
                break;

            case LightType.Spot:
                Shader.SetGlobalVector("_ToonLightDirection", light.transform.position - posAbs);
                dist = Mathf.Clamp01((light.transform.position - posAbs).magnitude / light.range);
                float angle = Vector3.Angle(light.transform.forward * -1f, (light.transform.position - posAbs).normalized);
                float inFront = Mathf.Lerp(0f, 1f, (light.spotAngle - angle * 2f) / (light.transform.position - posAbs).magnitude); // More edge fade when far away from light source
                Shader.SetGlobalFloat("_ToonLightIntensity", CalcAttenuation(dist));
                Shader.SetGlobalColor("_ToonLightColor", light.color * CalcAttenuation(dist) * light.intensity * 0.05f);
                break;

            default:
                Debug.Log("Lighting type '" + light.type + "' not supported by Awesome Toon Helper (" + light.name + ").");
                Shader.SetGlobalFloat("_ToonLightIntensity", 0f);
                break;
        }
    }

    // Ref - Light Attenuation calc: https://forum.unity.com/threads/light-attentuation-equation.16006/#post-3354254
    float CalcAttenuation(float dist)
    {
        return Mathf.Clamp01(1.0f / (1.0f + 25f * dist * dist) * Mathf.Clamp01((1f - dist) * 5f));
    }
}