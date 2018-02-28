using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamScript : MonoBehaviour
{
    [SerializeField] Transform target;
    //The following code has been adapted from https://gist.github.com/ftvs/5822103

    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public Transform camTransform;

    // How long the object should shake for.
    public float shakeDuration = 0f;
    private float currentShakeTimeLeft;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
        currentShakeTimeLeft = shakeDuration;
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    void Update()
    {
        if (target != null)
        {
            if (currentShakeTimeLeft > 0)
            {
                Vector3 shakyShake = Random.insideUnitSphere;
                shakyShake = new Vector3(shakyShake.x, shakyShake.y, 0f);
                transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z) + shakyShake * shakeAmount;
                currentShakeTimeLeft -= Time.deltaTime * decreaseFactor;
            }
            else transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z);
        }
    }

    public void shake()
    {
        currentShakeTimeLeft += shakeDuration;
    }
}


