using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private Camera myCam;

    private void Awake()
    {
        myCam = GetComponent<Camera>();
    }

    public void ZoomPerspectiveCamera(float w, float h)
    {
        float _height = 2.0f * ((w > h ? w : h + 0.5f) / 2) * Mathf.Atan(myCam.fieldOfView);

        if(_height < 5.5f)
        {
            _height = 5.5f;
        }

        transform.position = new Vector3((w-1)/2f, _height, myCam.transform.position.z);
    }

    public void ZoomOrthographicSizeCamera(float w, float h)
    {
        myCam.orthographicSize = (w > h ? w : h + 0.5f)/2 + 0.25f;
    }
}
