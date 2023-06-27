using System;
using UnityEngine;

[Serializable]
public struct MinMaxFloat
{
    [Tooltip("The minimum Vector3 value.")] [SerializeField]
    private float m_MinValue;

    [Tooltip("The maximum Vector3 value.")] [SerializeField]
    private float m_MaxValue;

    public float MinValue
    {
        get { return m_MinValue; }
        set { m_MinValue = value; }
    }

    public float MaxValue
    {
        get { return m_MaxValue; }
        set { m_MaxValue = value; }
    }

    public float RandomValue
    {
        get { return UnityEngine.Random.Range(m_MinValue, m_MaxValue); }
    }

    /// <summary>
    /// MinMaxFloat constructor which can specify the min and max values.
    /// </summary>
    /// <param name="minValue">The minimum float value.</param>
    /// <param name="maxValue">The maximum float value.</param>
    public MinMaxFloat(float minValue, float maxValue)
    {
        m_MinValue = minValue;
        m_MaxValue = maxValue;
    }
}

/// <summary>
/// A container for a min and max Vector3 value.
/// </summary>
[Serializable]
public struct MinMaxVector3
{
    [Tooltip("The minimum Vector3 value.")] [SerializeField]
    private Vector3 m_MinValue;

    [Tooltip("The maximum Vector3 value.")] [SerializeField]
    private Vector3 m_MaxValue;

    [Tooltip("The minimum magnitude value when determining a random value.")] [SerializeField]
    private Vector3 m_MinMagnitude;

    public Vector3 MinValue
    {
        get { return m_MinValue; }
        set { m_MinValue = value; }
    }

    public Vector3 MaxValue
    {
        get { return m_MaxValue; }
        set { m_MaxValue = value; }
    }

    public Vector3 MinMagnitude
    {
        get { return m_MinMagnitude; }
        set { m_MinMagnitude = value; }
    }

    public Vector3 RandomValue
    {
        get
        {
            var value = Vector3.zero;
            value.x = GetRandomFloat(m_MinValue.x, m_MaxValue.x, m_MinMagnitude.x);
            value.y = GetRandomFloat(m_MinValue.y, m_MaxValue.y, m_MinMagnitude.y);
            value.z = GetRandomFloat(m_MinValue.z, m_MaxValue.z, m_MinMagnitude.z);
            return value;
        }
    }

    /// <summary>
    /// MinMaxVector3 constructor which can specify the min and max values.
    /// </summary>
    /// <param name="minValue">The minimum Vector3 value.</param>
    /// <param name="maxValue">The maximum Vector3 value.</param>
    public MinMaxVector3(Vector3 minValue, Vector3 maxValue)
    {
        m_MinValue = minValue;
        m_MaxValue = maxValue;
        m_MinMagnitude = Vector3.zero;
    }

    /// <summary>
    /// MinMaxVector3 constructor which can specify the min and max values.
    /// </summary>
    /// <param name="minValue">The minimum Vector3 value.</param>
    /// <param name="maxValue">The maximum Vector3 value.</param>
    /// <param name="minMagnitude">The minimum magnitude of the random value.</param>
    public MinMaxVector3(Vector3 minValue, Vector3 maxValue, Vector3 minMagnitude)
    {
        m_MinValue = minValue;
        m_MaxValue = maxValue;
        m_MinMagnitude = minMagnitude;
    }

    /// <summary>
    /// Returns a random float between the min and max value with the specified minimum magnitude.
    /// </summary>
    /// <param name="minValue">The minimum float value.</param>
    /// <param name="maxValue">The maximum float value.</param>
    /// <param name="minMagnitude">The minimum magnitude of the random value.</param>
    /// <returns>A random float between the min and max value.</returns>
    private float GetRandomFloat(float minValue, float maxValue, float minMagnitude)
    {
        if (minMagnitude != 0 && Mathf.Sign(m_MinValue.x) != Mathf.Sign(m_MaxValue.x))
        {
            if (Mathf.Sign(UnityEngine.Random.Range(m_MinValue.x, m_MaxValue.x)) > 0)
            {
                return UnityEngine.Random.Range(minMagnitude, Mathf.Max(minMagnitude, maxValue));
            }

            return UnityEngine.Random.Range(-minMagnitude, Mathf.Min(-minMagnitude, minValue));
        }
        else
        {
            return UnityEngine.Random.Range(minValue, maxValue);
        }
    }
}