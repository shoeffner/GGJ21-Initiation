using UnityEngine;
using Mirror;

namespace Initiation
{
    //[RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : NetworkBehaviour
    {
        public CharacterController characterController;

        public new Camera camera;
        public Vector3 cameraOffset = new Vector3(2, 4, -2);
        public Vector3 cameraLookAtOffset = new Vector3(0, 2, 0);

        public float moveSpeed = 7.5f;
        public float jumpHeight = 1.2f;

        private Vector3 forward = Vector3.forward;
        private Vector3 right = Vector3.right;

        private Vector3 gravitySpeed = Vector3.zero;
        

        void OnValidate()
        {
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }
        }

        void Start()
        {
            characterController.enabled = isLocalPlayer;
            if (camera == null)
            {
                camera = Camera.main;
                UpdateCamera();
            }
        }

        public override void OnStartLocalPlayer()
        {
        }

        void OnDisable()
        {

        }

        void UpdateCamera()
        {
            camera.transform.position = transform.position + cameraOffset;
            camera.transform.LookAt(transform.position + cameraLookAtOffset);
            forward = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up).normalized;
            right = Vector3.ProjectOnPlane(camera.transform.right, Vector3.up).normalized;
        }

        void Move()
        {
            float inputHorizontal = Input.GetAxis("Horizontal");
            float inputVertical = Input.GetAxis("Vertical");
            Vector3 forwardMovement = forward * inputVertical;
            Vector3 rightMovement = right * inputHorizontal;
            Vector3 moveDirection =(forwardMovement + rightMovement).normalized;

            if (characterController.isGrounded)
            {
                gravitySpeed = Vector3.zero;
            }
            else
            {
                gravitySpeed += Physics.gravity * Time.deltaTime;
            }

            characterController.Move((moveDirection * moveSpeed + gravitySpeed) * Time.deltaTime);
        }

        void Rotate()
        {
            // Adapted from https://learn.unity.com/project/survival-shooter-tutorial
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit floorHit;
            if (Physics.Raycast(camRay, out floorHit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                Vector3 playerToMouse = floorHit.point - transform.position;
                playerToMouse.y = 0f;
                Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);
                GetComponent<Rigidbody>().MoveRotation(newRotatation);
            }
        }

        void Update()
        {
            if (!isLocalPlayer || !characterController.enabled)
            {
                return;
            }
            Move();
            UpdateCamera();
        }

        void FixedUpdate()
        {
            Rotate();
        }
    }
}