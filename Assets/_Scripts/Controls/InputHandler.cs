using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Umbr.EF.Manager
{
    public class InputHandler : MonoBehaviour
    {
        private static InputHandler _instance;
        public static InputHandler Instance { get { return _instance; } }

        private GameManager gameManager;

        private float horizontalInput;
        private float verticallInput;
        private float rtSpeed;

        [SerializeField] private FloatingJoystick joyst;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            _instance = this;
        }

        private void Start()
        {
            gameManager = GameManager.Instance;
            rtSpeed = gameManager.cameraSpeedRotation;
        }

        public void RotateCamera()
        {
            horizontalInput = Input.GetAxis("Horizontal") + joyst.Horizontal;
            gameManager.pivot.Rotate(Vector3.up, rtSpeed * horizontalInput * Time.deltaTime);
            gameManager.playerLogic.gameObject.transform.Rotate(Vector3.up, rtSpeed * horizontalInput * Time.deltaTime);
        }

        public void HandleMovement()
        {
            verticallInput = Input.GetAxis("Vertical") + joyst.Vertical;
            horizontalInput = Input.GetAxis("Horizontal") + joyst.Horizontal;

            Vector3 direction = new Vector3(horizontalInput, 0, verticallInput).normalized;
            direction = Quaternion.Euler(0, 45, 0) * direction;
            gameManager.playerLogic.Move(direction);

            if (direction != Vector3.zero)
            {
                Quaternion toRot = Quaternion.LookRotation(direction);
                gameManager.playerLogic.transform.rotation = Quaternion.RotateTowards(gameManager.playerLogic.transform.rotation,
                    toRot, gameManager.playerRotation * Time.deltaTime);
            }


#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                gameManager.playerLogic.UseAbility();
            //if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            //    gameManager.playerLogic.ShowAbilityIndicator();
#else
            foreach (var touch in Input.touches)
                if (touch.phase == TouchPhase.Began)
                    gameManager.playerLogic.UseAbility();
                //else if (touch.phase == TouchPhase.Ended)
                //    gameManager.playerLogic.ShowAbilityIndicator();

#endif
        }
    }
}
