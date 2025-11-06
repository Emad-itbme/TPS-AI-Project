using System.Runtime.CompilerServices;
using UnityEngine;

public class UI_LookAt : MonoBehaviour
{

    Camera cam;
    void Start()
    {
      cam = Camera.main;
    }
    void Update()
    {
        
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
            cam.transform.rotation * Vector3.up);
    }

}
