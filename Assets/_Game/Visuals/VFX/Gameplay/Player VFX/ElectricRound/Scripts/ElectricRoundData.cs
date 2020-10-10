using UnityEngine;

public struct ElectricRoundData
{
    /// <summary>
    /// construct an ElectricRoundData using floats for scale
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="radiusThickness"></param>
    /// <param name="emissionRate"></param>
    /// <param name="scaleX"></param>
    /// <param name="scaleY"></param>
    /// <param name="scaleZ"></param>
    public ElectricRoundData(float radius, float radiusThickness, float emissionRate, Vector2 particleMinMax, float scaleX, float scaleY, float scaleZ, float trailLifetime, float trailWidth)
    {
        this.radius = radius;
        this.radiusThickness = radiusThickness;
        this.emissionRate = emissionRate;
        particleSize = particleMinMax;
        this.trailLifetime = trailLifetime;
        this.trailWidth = trailWidth;
        scale = new Vector3(scaleX, scaleY, scaleZ);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="radiusThickness"></param>
    /// <param name="emissionRate"></param>
    /// <param name="scale"></param>
    public ElectricRoundData(float radius, float radiusThickness, float emissionRate, Vector2 particleMinMax, Vector3 scale, float trailLifetime, float trailWidth)
    {
        this.radius = radius;
        this.radiusThickness = radiusThickness;
        this.emissionRate = emissionRate;
        particleSize = particleMinMax;
        this.trailLifetime = trailLifetime;
        this.trailWidth = trailWidth;
        this.scale = scale;
    }
    public float radius;
    public float radiusThickness;
    public float emissionRate;
    public Vector2 particleSize;
    public float trailLifetime;
    public float trailWidth;
    public Vector3 scale;
}
