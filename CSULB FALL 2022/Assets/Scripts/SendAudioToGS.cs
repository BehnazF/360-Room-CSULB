using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lasp;

public class SendAudioToGS : MonoBehaviour
{
    [SerializeField] string _shaderProperty;
    [SerializeField] Material _material;
    [SerializeField] AudioLevelTracker _audioLevelTracker;
    [SerializeField] AnimationCurve _animationCurve;
    // Start is called before the first frame update
    int _shaderPropertyID;
    void Start()
    {
        _shaderPropertyID = Shader.PropertyToID(_shaderProperty);

    }

    // Update is called once per frame
    void Update()
    {
        float adjustedLevel = _animationCurve.Evaluate(_audioLevelTracker.normalizedLevel);
        _material.SetFloat(_shaderPropertyID, adjustedLevel);
    }
}
