using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System;

public class Camera : MonoBehaviour {

    new UnityEngine.Camera camera;
    [SerializeField]PixelPerfectCamera pixelPerfectCamera;

    // Use this for initialization
    void Start () {
        camera = this.gameObject.GetComponent<UnityEngine.Camera>();
        #if UNITY_STANDALONE_WIN
            pixelPerfectCamera.refResolutionX = 265;
            pixelPerfectCamera.refResolutionY = 349;
        #endif

        #if UNITY_ANDROID
            pixelPerfectCamera.refResolutionX = 265;
            pixelPerfectCamera.refResolutionY = 570;
        #endif
    }

    // Update is called once per frame
    void Update () {
        #if UNITY_STANDALONE_WIN
            gameObject.transform.position = new Vector3(0, camera.orthographicSize, -10);
        #endif

        #if UNITY_ANDROID
            gameObject.transform.position = new Vector3(0, camera.orthographicSize - 7, -10);
        #endif
    }
}
