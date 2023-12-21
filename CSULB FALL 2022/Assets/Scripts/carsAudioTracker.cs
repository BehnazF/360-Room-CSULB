using UnityEngine;
using Unity.Collections;
using UnityEngine.Assertions;

// [RequireComponent(typeof(AudioSource))]
public class carsAudioTracker : MonoBehaviour
{
    Texture2D _spectrumTexture;
    NativeArray<float> _buffer;

    [SerializeField] string _audioPropertyName;
    [SerializeField] float _scale = 0.001f;
    [SerializeField] float maxDelta = 0.01f;
    [SerializeField] int _audioSampleOffset = 0;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Vector3 _graphScale = Vector3.one;
    [SerializeField] Vector3 _graphOffset = Vector3.one;
    [SerializeField] string _shaderProperty;
    int _shaderId;
    int _audioPropertyId;
    [SerializeField] Material _material;
    float[] spectrum = new float[256 * 2];
    float[] slowed = new float[256];
    float[] accumulated = new float[256];

    float[] spectrum2 = new float[256 * 2];
    float[] spectrum3 = new float[256 * 2];
    void Awake()
    {
        _audioPropertyId = Shader.PropertyToID(_audioPropertyName);
        _spectrumTexture = new Texture2D(spectrum.Length / 2, 2, TextureFormat.RFloat, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
    }
    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.white;
    //     Gizmos.DrawLine(Vector3.zero + _graphOffset, Vector3.Scale(new Vector3(spectrum2.Length, 0, 0), _graphScale) + _graphOffset);
    //     Gizmos.color = Color.blue;
    //     for (int i = 1; i < slowed.Length; i++)
    //     {

    //         Gizmos.DrawLine(Vector3.Scale(new Vector3(i - 1, slowed[i - 1], 3), _graphScale) + _graphOffset, Vector3.Scale(new Vector3(i, slowed[i], 3), _graphScale) + _graphOffset);

    //     }

    //     // Vector3 graphScale = new Vector3(0.1f, 20, 0);
    //     // // _audioSource.clip.GetData(spectrum2, _audioSampleOffset);
    //     // _audioSource.clip.GetData(spectrum2, (int)(_audioSource.time * _audioSource.clip.frequency));
    //     // Gizmos.color = Color.red;
    //     // for (int i = 1; i < spectrum2.Length; i++)
    //     // {
    //     //     Gizmos.DrawLine(Vector3.Scale(new Vector3(i - 1, spectrum2[i - 1], 0), _graphScale) + _graphOffset, Vector3.Scale(new Vector3(i, spectrum2[i], 0), _graphScale) + _graphOffset);
    //     //     Debug.Log(spectrum2[i]);
    //     // }
    //     // for (int i = 0; i < spectrum3.Length; i++)
    //     // {
    //     //     spectrum3[i] = 0f;
    //     // }
    //     // FFT(FFTDir.Forward, 9, spectrum2, spectrum3);
    //     // Gizmos.color = Color.green;
    //     // for (int i = 1; i < spectrum2.Length; i++)
    //     // {
    //     //     Gizmos.DrawLine(Vector3.Scale(new Vector3(i - 1, new Vector2(spectrum2[i - 1], spectrum3[i - 1]).magnitude, 0), _graphScale) + _graphOffset, Vector3.Scale(new Vector3(i, new Vector3(spectrum2[i], spectrum3[i]).magnitude, 0), _graphScale) + _graphOffset);
    //     // }
    // }

    void start()
    {
        _shaderId = Shader.PropertyToID(_shaderProperty);
    }
    void Update()
    {

        if (!_buffer.IsCreated)
        {
            _buffer = new NativeArray<float>(256 * 2, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        }

        // _audioSource.clip.samples
        // samples per sec

        // _audioSource.clip.GetData(spectrum, _audioSampleOffset);
        _audioSource.clip.GetData(spectrum, (int)(_audioSource.time * _audioSource.clip.frequency));
        for (int i = 0; i < spectrum3.Length; i++)
        {
            spectrum3[i] = 0f;
        }
        FFT(FFTDir.Forward, 9, spectrum, spectrum3);

        // AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        for (int i = 0; i < spectrum.Length / 2; i++)
        {
            accumulated[i] = new Vector2(spectrum[i], spectrum3[i]).magnitude * _scale + accumulated[i];
            slowed[i] = Mathf.MoveTowards(slowed[i], new Vector2(spectrum[i], spectrum3[i]).magnitude, maxDelta);
            _buffer[i] = slowed[i];
            _buffer[i + spectrum.Length / 2] = accumulated[i];
        }

        _spectrumTexture.LoadRawTextureData(_buffer);
        _spectrumTexture.Apply();
        _buffer.Dispose();

        _material.SetTexture(_audioPropertyId, _spectrumTexture);

        // for (int i = 1; i < spectrum.Length - 1; i++)
        // {
        //     Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
        //     Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
        //     Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
        //     Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
        // }


    }



    enum FFTDir
    {
        Forward,
        Inverse
    }

    // Adapted from http://paulbourke.net/miscellaneous/dft/
    /*
       This computes an in-place complex-to-complex FFT 
       x and y are the real and imaginary arrays of 2^m points.
       dir =  1 gives forward transform
       dir = -1 gives reverse transform 
    */
    void FFT(FFTDir dir, int m, float[] x, float[] y)
    {
        Assert.IsTrue(x.Length == y.Length && (1 << m) == x.Length);
        int n, i, i1, j, k, i2, l, l1, l2;
        float c1, c2, tx, ty, t1, t2, u1, u2, z;

        /* Calculate the number of points */
        n = 1;
        for (i = 0; i < m; i++)
        {
            n *= 2;
        }

        /* Do the bit reversal */
        i2 = n >> 1;
        j = 0;
        for (i = 0; i < n - 1; i++)
        {
            if (i < j)
            {
                tx = x[i];
                ty = y[i];
                x[i] = x[j];
                y[i] = y[j];
                x[j] = tx;
                y[j] = ty;
            }
            k = i2;
            while (k <= j)
            {
                j -= k;
                k >>= 1;
            }
            j += k;
        }

        /* Compute the FFT */
        c1 = -1.0f;
        c2 = 0.0f;
        l2 = 1;
        for (l = 0; l < m; l++)
        {
            l1 = l2;
            l2 <<= 1;
            u1 = 1.0f;
            u2 = 0.0f;
            for (j = 0; j < l1; j++)
            {
                for (i = j; i < n; i += l2)
                {
                    i1 = i + l1;
                    t1 = u1 * x[i1] - u2 * y[i1];
                    t2 = u1 * y[i1] + u2 * x[i1];
                    x[i1] = x[i] - t1;
                    y[i1] = y[i] - t2;
                    x[i] += t1;
                    y[i] += t2;
                }
                z = u1 * c1 - u2 * c2;
                u2 = u1 * c2 + u2 * c1;
                u1 = z;
            }
            c2 = Mathf.Sqrt((1.0f - c1) / 2.0f);
            if (dir == FFTDir.Forward)
            {
                c2 = -c2;
            }
            c1 = Mathf.Sqrt((1.0f + c1) / 2.0f);
        }

        /* Scaling for forward transform */
        if (dir == FFTDir.Forward)
        {
            for (i = 0; i < n; i++)
            {
                x[i] /= n;
                y[i] /= n;
            }
        }
    }
}