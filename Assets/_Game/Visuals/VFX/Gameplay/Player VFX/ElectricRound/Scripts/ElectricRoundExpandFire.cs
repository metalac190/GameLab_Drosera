using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class ElectricRoundExpandFire : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    private ParticleSystem electricParticles;
    private TrailRenderer trail;
    [SerializeField] private Vector3 defaultStartScale = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 defaultEndScale = new Vector3(5, 5, 2.5f);
    //[SerializeField] float speed = 10f;
    //[SerializeField] Vector3 fireDirection;
    //[SerializeField] float m_chargeTime = 2f;

    ElectricRoundData startingData;
    [SerializeField] ElectricRoundData endDataDefault = new ElectricRoundData(0.38f, 0.23f, 500, new Vector2(0.3f,0.4f), new Vector3(1.8f,1.8f,2f), 0.55f, 0.6f);
    
    //used in Charge() to keep track of how far we've gotten in the lerp
    float lerpProgress = 0f;
    private void Awake()
    {
        m_rigidbody = GetComponentInParent<Rigidbody>();
        electricParticles = GetComponentInChildren<ParticleSystem>();
        trail = GetComponentInChildren<TrailRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //make sure that the electric round isn't using gravity
        m_rigidbody.useGravity = false;
        //get our starting data from our particle and trail systems
        startingData.radius = electricParticles.shape.radius;
        startingData.radiusThickness = electricParticles.shape.radiusThickness;
        startingData.shapeScale = electricParticles.shape.scale;
        startingData.emissionRate = electricParticles.emission.rateOverTimeMultiplier;
        startingData.particleSize = new Vector2(electricParticles.main.startSize.constantMin, electricParticles.main.startSize.constantMax);
        startingData.trailLifetime = trail.time;
        startingData.trailWidth = trail.widthMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKey(KeyCode.Space))
        //{
        //    Charge(m_chargeTime);
        //}
        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    if (fireDirection == Vector3.zero)
        //    {
        //        Fire(speed);
        //    }
        //    else
        //    {
        //        Fire(speed, fireDirection);
        //    }
        //}
    }

    #region overloadedChargeMethods
    /// <summary>
    /// a Charge method using default values for every part of the effect except for the charge time
    /// </summary>
    /// <param name="chargeTime"></param>
    /// <returns></returns>
    public bool Charge(float chargeTime = 1f)
    {
        return Charge(defaultStartScale, defaultEndScale, chargeTime, endDataDefault);
    }
    /// <summary>
    /// Charge using set start and end scales and default values for electric round data
    /// returns true if done charging, false otherwise
    /// </summary>
    /// <param name="startScale"></param>
    /// <param name="endScale"></param>
    /// <param name="chargeTime"></param>
    public bool Charge(Vector3 startScale, Vector3 endScale, float chargeTime)
    {
        return Charge(startScale, endScale, chargeTime, endDataDefault);
    }
    /// <summary>
    /// charge using set endScale, chargeTime, default values for everything else
    /// returns true if done charging, false otherwise
    /// </summary>
    /// <param name="endScale"></param>
    /// <param name="chargeTime"></param>
    public bool Charge(Vector3 endScale, float chargeTime)
    {
        return Charge(new Vector3(1, 1, 1), endScale, chargeTime, endDataDefault);
    }
    /// <summary>
    /// call charge without using the ElectricRoundData struct
    /// returns true if done charging, false otherwise
    /// </summary>
    /// <param name="startScale"></param>
    /// <param name="endScale"></param>
    /// <param name="chargeTime"></param>
    /// <param name="endRadius"></param>
    /// <param name="endRadiusThickness"></param>
    /// <param name="endSize"> the min and max values for particle size at the end of charging</param>
    /// <param name="endEmissionRate"></param>
    /// <param name="endTrailLife"> the lifetime of the trail at the end of charging</param>
    /// <param name="endTrailWidth"> the width multiplier for the trail at the end of charging</param>
    /// <param name="psEndScale"></param>
    /// <returns></returns>
    public bool Charge(Vector3 startScale, 
        Vector3 endScale, float chargeTime, float endRadius, 
        float endRadiusThickness, Vector2 endSize, float endEmissionRate, 
        float endTrailLife, float endTrailWidth, Vector3 psEndScale)
    {
        ElectricRoundData endData = new ElectricRoundData(endRadius, endRadiusThickness, endEmissionRate, endSize, endScale, endTrailLife, endTrailWidth);
        return Charge(startScale, endScale, chargeTime, endData);
    }
    /// <summary>
    /// call charge with default scale and without using the ElectricRoundData struct
    /// returns true if done charging, false otherwise
    /// </summary>
    /// <param name="endScale"></param>
    /// <param name="chargeTime"></param>
    /// <param name="endRadius"></param>
    /// <param name="endRadiusThickness"></param>
    /// <param name="endSize"> the min and max values for particle size at the end of charging</param>
    /// <param name="endEmissionRate"></param>
    /// <param name="endTrailLife"> the lifetime of the trail at the end of charging</param>
    /// <param name="endTrailWidth"> the width multiplier for the trail at the end of charging</param>
    /// <param name="psEndScale"></param>
    /// <returns></returns>
    public bool Charge(Vector3 endScale, float chargeTime, float endRadius, float endRadiusThickness, Vector2 endSize, float endEmissionRate, float endTrailLife, float endTrailWidth, Vector3 psEndScale)
    {
        ElectricRoundData endData = new ElectricRoundData(endRadius, endRadiusThickness, endEmissionRate, endSize, endScale, endTrailLife, endTrailWidth);
        return Charge(new Vector3(1, 1, 1), endScale, chargeTime, endData);
    }
    /// <summary>
    /// Overloaded method for charge that allows us to call it with no argument for startScale
    /// assumes that starting scale is (1,1,1)
    /// returns true if done charging, false otherwise
    /// </summary>
    /// <param name="endScale"></param>
    /// <param name="chargeTime"></param>
    /// <param name="endData"></param>
    public bool Charge(Vector3 endScale, float chargeTime, ElectricRoundData endData)
    {
        return Charge(new Vector3(1, 1, 1), endScale, chargeTime, endData);
    }
    #endregion
    /// <summary>
    /// takes the start and end values and lerps between them over chargeTime
    /// returns true if done charging, false otherwise
    /// </summary>
    /// <param name="startScale"></param>
    /// <param name="endScale"></param>
    /// <param name="chargeTime"></param>
    /// <param name="endData"></param>
    public bool Charge(Vector3 startScale, Vector3 endScale, float chargeTime, ElectricRoundData endData)
    {
        //get our particle system's modules to alter them over time
        ParticleSystem.ShapeModule shapeModule = electricParticles.shape;
        ParticleSystem.EmissionModule emissionModule = electricParticles.emission;
        ParticleSystem.MainModule mainModule = electricParticles.main;
        //TODO: make an animation for this instead of doing it through code
        //(only necessary if we want to do more than lerp between start and end state)
        //get our lerp progress here so we can use it for each of the different things we're lerping between
        lerpProgress += (Time.deltaTime / chargeTime);
        lerpProgress = Mathf.Clamp(lerpProgress, 0, 1);
        //lerp all the stuff we need to lerp
        transform.localScale = Vector3.Lerp(startScale, endScale, lerpProgress);
        shapeModule.radius = Mathf.Lerp(startingData.radius, endData.radius, lerpProgress);
        shapeModule.radiusThickness = Mathf.Lerp(startingData.radiusThickness, endData.radiusThickness, lerpProgress);
        shapeModule.scale = Vector3.Lerp(startingData.shapeScale, endData.shapeScale, lerpProgress);
        emissionModule.rateOverTimeMultiplier = Mathf.Lerp(startingData.emissionRate, endData.emissionRate, lerpProgress);
        //create a new MinMaxCurve for particle size and update that based on time
        Vector2 particleSize = Vector2.Lerp(startingData.particleSize, endData.particleSize, lerpProgress);
        ParticleSystem.MinMaxCurve newSize = new ParticleSystem.MinMaxCurve(particleSize.x, particleSize.y);
        mainModule.startSize = newSize;

        trail.widthMultiplier = Mathf.Lerp(startingData.trailWidth, endData.trailWidth, lerpProgress);
        trail.time = Mathf.Lerp(startingData.trailLifetime, endData.trailLifetime, lerpProgress);

        if (lerpProgress == 1)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// make the projectile move forward at the speed given
    /// </summary>
    /// <param name="speed"></param>
    public void Fire(float speed)
    {
        m_rigidbody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }

    /// <summary>
    /// make the projectile move in the direction indicated at the speed given
    /// local bool controls if it's relative to the rotation of the object or not
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="direction"></param>
    public void Fire (float speed, Vector3 direction, bool local = false)
    {
        if (local)
        {
            m_rigidbody.AddRelativeForce(direction.normalized * speed, ForceMode.VelocityChange);
        }
        else
        {
            m_rigidbody.AddForce(direction.normalized * speed, ForceMode.VelocityChange);
        }
    }
}
