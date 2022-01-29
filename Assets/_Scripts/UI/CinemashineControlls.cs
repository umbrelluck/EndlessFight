using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Umbr.EF.UI
{
    public class CinemashineControlls : MonoBehaviour
    {

        [SerializeField] Cinemachine.CinemachineVirtualCamera mainCamera;
        // Start is called before the first frame update
        void Start()
        {
            mainCamera.Priority++;
        }

        // Update is called once per frame
        public void ChangeTargetCamera(Cinemachine.CinemachineVirtualCamera targetCamera)
        {
            mainCamera.Priority--;
            mainCamera = targetCamera;
            mainCamera.Priority++;
        }
    }
}