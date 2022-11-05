using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Hap;
using Lasp;


public class AudioControlVideo : MonoBehaviour

{
    [SerializeField] HapPlayer _player;
    [SerializeField, Range(0, 1)] float _time;

    [SerializeField] Vector2 _limits = new Vector2(17, 21);
    [SerializeField] AudioLevelTracker _audio;
    [SerializeField] float _videoFraction;
    [SerializeField, CurveRange()] AnimationCurve _animCurve;

    float movingAverageXn = 0;
    float PREVmovingAverageXn = 0;
    [SerializeField] float nSamples = 10;
    [SerializeField] float _velocity = 1;
    [SerializeField] float _decay = 0.1f;
    [SerializeField] float _bounceFraction = 0.1f;
    [SerializeField] float _bounceFrequency = 1.0f;

    [SerializeField] float _stock = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //Time.deltaTime: the time between two calls to update  
        _stock += _audio.normalizedLevel * _velocity * Time.deltaTime - _decay * Time.deltaTime;
        _stock = Mathf.Clamp01(_stock);
        float sample = Mathf.Clamp01(_stock);
        // float sample = _audio.normalizedLevel;

        //equetion for exponetial moving average: https://www.daycounter.com/LabBook/Moving-Average.phtml: MA*[i]= MA*[i-1] +X[i] - MA*[i-1]/N
        movingAverageXn = PREVmovingAverageXn + sample - PREVmovingAverageXn / nSamples;
        PREVmovingAverageXn = movingAverageXn;
        float movingAverage = movingAverageXn / nSamples;

        _videoFraction = _animCurve.Evaluate(movingAverage);
        // _player.time = Mathf.Lerp(_limits[0], _limits[1], _videoFraction * (1 - _bounceFraction) + Mathf.PerlinNoise(Time.time, 0) * (Mathf.Sin(Time.time * _bounceFrequency) + 1) * 0.5f * _bounceFraction);

        _player.time = Mathf.Lerp(_limits[0], _limits[1], _videoFraction * (1 - _bounceFraction) + Mathf.PerlinNoise(Time.time * _bounceFrequency, 0) * _bounceFraction);

    }

}
