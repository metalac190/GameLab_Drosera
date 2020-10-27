using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ToonHelper : MonoBehaviour
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

    [SerializeField] bool showRaycasts = true;
    [SerializeField] Vector3 meshCenter = Vector3.zero;
    [Range(0, 4)]
    [SerializeField] int maxLights = 4;

    [Header("Recieve Shadow Check")]
    [SerializeField] bool raycast = true;
    [SerializeField] LayerMask raycastMask = new LayerMask();
    [SerializeField] float raycastFadeSpeed = 10f;

    // State
    Vector3 posAbs;
    Dictionary<int, LightSet> lightSets;
    private bool initialized = false;

    // Refs
    Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    Dictionary<Renderer, Material[]> instancedMaterials = new Dictionary<Renderer, Material[]>();

    // Toggle
    public static bool toggleRaycasts = false;

    void Start()
    {
        if (Application.isPlaying) return;
        Initialize();
        GetLights();
        UpdateMaterial();
    }

    void OnValidate()
    {
        if (Application.isPlaying) return;
        Initialize();
        UpdateMaterial();
    }

    public void Initialize()
    {
        if (GetComponent<Renderer>() != null)
        {
            Renderer r = GetComponent<Renderer>();
            FixOld(r);
            Init(r);

        }
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            FixOld(r);
            Init(r);
        }
    }

    void FixOld(Renderer r)
    {
        AwesomeToonHelper ath = r.GetComponent<AwesomeToonHelper>();
        if (ath == null || (ath != null && ath.materials[0] == null)) return;
        r.sharedMaterials = ath.materials;
    }

    void Init(Renderer r)
    {
        if (!originalMaterials.ContainsKey(r))
        {
            originalMaterials.Add(r, r.sharedMaterials);
        }
        else
        {
            originalMaterials[r] = r.sharedMaterials;
        }

        Material[] temp = new Material[r.sharedMaterials.Length];
        for (int i = 0; i < r.sharedMaterials.Length; i++)
        {
            if (r.sharedMaterials[i] == null) continue;
            temp[i] = new Material(r.sharedMaterials[i]);
            temp[i].name = "Instance of " + temp[i].name;
        }
        if (!instancedMaterials.ContainsKey(r))
        {
            instancedMaterials.Add(r, temp);
        }
        else
        {
            instancedMaterials[r] = temp;
        }

        foreach (KeyValuePair<Renderer, Material[]> ren in instancedMaterials)
        {
            ren.Key.sharedMaterials = ren.Value;
        }
    }

    // NOTE: If your game loads lights dynamically, this should be called to init new lights
    public void GetLights()
    {
        if (lightSets == null)
        {
            lightSets = new Dictionary<int, LightSet>();
        }

        Light[] lights = FindObjectsOfType<Light>();
        List<int> newIds = new List<int>();

        // Initialise new lights
        foreach (Light light in lights)
        {
            int id = light.GetInstanceID();
            newIds.Add(id);
            if (!lightSets.ContainsKey(id))
            {
                lightSets.Add(id, new LightSet(light));
            }
        }

        // Remove old lights
        List<int> oldIds = new List<int>(lightSets.Keys);
        foreach (int id in oldIds)
        {
            if (!newIds.Contains(id))
            {
                lightSets.Remove(id);
            }
        }
    }

    void Update()
    {
        posAbs = transform.position + meshCenter;
        // Always update lighting while in editor
        if (Application.isEditor && !Application.isPlaying)
        {
            GetLights();
        }
        if (!gameObject.isStatic || gameObject.activeSelf || !initialized)
            UpdateMaterial();
    }

    public static void InitializeAllLighting()
    {
        foreach (ToonHelper s in FindObjectsOfType<ToonHelper>())
        {
            s.Initialize();
            s.GetLights();
            s.UpdateMaterial();
        }
    }

    public static void UpdateAllLighting()
    {
        foreach (ToonHelper s in FindObjectsOfType<ToonHelper>())
        {
            s.GetLights();
            s.UpdateMaterial();
        }
    }

    void UpdateMaterial()
    {
        if (instancedMaterials == null) return;

        // Refresh light data
        List<LightSet> sortedLights = new List<LightSet>();
        if (lightSets != null)
        {
            foreach (LightSet lightSet in lightSets.Values)
            {
                sortedLights.Add(CalcLight(lightSet));
            }
        }

        // Sort lights by brightness
        sortedLights.Sort((x, y) => {
            float yBrightness = y.color.grayscale * y.atten;
            float xBrightness = x.color.grayscale * x.atten;
            return yBrightness.CompareTo(xBrightness);
        });

        // Apply lighting
        int i = 1;
        foreach (LightSet lightSet in sortedLights)
        {
            if (i > maxLights) break;
            if (lightSet.atten <= Mathf.Epsilon) break;

            // Use color Alpha to pass attenuation data
            Color color = lightSet.color;
            color.a = Mathf.Clamp(lightSet.atten, 0.01f, 0.99f); // UV might wrap around if attenuation is >1 or 0<

            foreach (KeyValuePair<Renderer, Material[]> ren in instancedMaterials)
            {
                foreach (Material m in ren.Value)
                {
                    if (m != null)
                    {
                        m.SetVector($"_L{i}_dir", lightSet.dir.normalized);
                        m.SetColor($"_L{i}_color", color);
                    }
                }
            }
            i++;
        }

        // Turn off the remaining light slots
        while (i <= 4)
        {
            foreach (KeyValuePair<Renderer, Material[]> ren in instancedMaterials)
            {
                foreach (Material m in ren.Value)
                {
                    if (m != null)
                    {
                        m.SetVector($"_L{i}_dir", Vector3.up);
                        m.SetColor($"_L{i}_color", Color.black);
                    }
                }
            }
            i++;
        }

        // Store updated light data
        foreach (LightSet lightSet in sortedLights)
        {
            lightSets[lightSet.id] = lightSet;
        }
    }

    LightSet CalcLight(LightSet lightSet)
    {
        Light light = lightSet.light;
        float inView = 1.1f;
        float dist;

        if (!light.isActiveAndEnabled)
        {
            lightSet.atten = 0f;
            return lightSet;
        }

        switch (light.type)
        {
            case LightType.Directional:
                lightSet.dir = light.transform.forward * -1f;
                inView = TestInView(lightSet.dir, 100f);
                lightSet.color = light.color * light.intensity;
                lightSet.atten = 1f;
                break;

            case LightType.Point:
                lightSet.dir = light.transform.position - posAbs;
                dist = Mathf.Clamp01(lightSet.dir.magnitude / light.range);
                inView = TestInView(lightSet.dir, lightSet.dir.magnitude);
                lightSet.atten = CalcAttenuation(dist);
                lightSet.color = light.color * lightSet.atten * light.intensity * 0.1f;
                break;

            case LightType.Spot:
                lightSet.dir = light.transform.position - posAbs;
                dist = Mathf.Clamp01(lightSet.dir.magnitude / light.range);
                float angle = Vector3.Angle(light.transform.forward * -1f, lightSet.dir.normalized);
                float inFront = Mathf.Lerp(0f, 1f, (light.spotAngle - angle * 2f) / lightSet.dir.magnitude); // More edge fade when far away from light source
                inView = inFront * TestInView(lightSet.dir, lightSet.dir.magnitude);
                lightSet.atten = CalcAttenuation(dist);
                lightSet.color = light.color * lightSet.atten * light.intensity * 0.05f;
                break;

            default:
                Debug.Log("Lighting type '" + light.type + "' not supported by Awesome Toon Helper (" + light.name + ").");
                lightSet.atten = 0f;
                break;
        }

        // Slowly fade lights on and off
        float fadeSpeed = (Application.isEditor && !Application.isPlaying)
            ? raycastFadeSpeed / 60f
            : raycastFadeSpeed * Time.deltaTime;

        lightSet.inView = Mathf.Lerp(lightSet.inView, inView, fadeSpeed);
        lightSet.color *= Mathf.Clamp01(lightSet.inView);

        return lightSet;
    }

    float TestInView(Vector3 dir, float dist)
    {
        if (!raycast) return 1.1f;
        RaycastHit hit;
        if (Physics.Raycast(posAbs, dir, out hit, dist, raycastMask))
        {
            if (showRaycasts && toggleRaycasts)
            {
                Debug.DrawRay(posAbs, dir.normalized * hit.distance, Color.red);
            }
            return -0.1f;
        }
        else
        {
            if (showRaycasts && toggleRaycasts)
            {
                Debug.DrawRay(posAbs, dir.normalized * dist, Color.green);
            }
            return 1.1f;
        }
    }

    // Ref - Light Attenuation calc: https://forum.unity.com/threads/light-attentuation-equation.16006/#post-3354254
    float CalcAttenuation(float dist)
    {
        return Mathf.Clamp01(1.0f / (1.0f + 25f * dist * dist) * Mathf.Clamp01((1f - dist) * 5f));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(posAbs, 0.1f);
    }
}
