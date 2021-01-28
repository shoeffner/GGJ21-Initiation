using UnityEngine;
using Mirror;

namespace Initiation
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : NetworkBehaviour
    {
        public CharacterController characterController;

        public Camera mainCamera;
        public Vector3 cameraOffset = new Vector3(2, 4, -2);
        public Vector3 cameraLookAtOffset = new Vector3(0, 2, 0);

        public float moveSpeed = 7.5f;
        public float jumpHeight = 1.2f;

        private Vector3 forward = Vector3.forward;
        private Vector3 right = Vector3.right;

        private Vector3 gravitySpeed = Vector3.zero;

        private Animator animator = null;
        

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
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                UpdateCamera();
            }
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
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
            mainCamera.transform.position = transform.position + cameraOffset;
            mainCamera.transform.LookAt(transform.position + cameraLookAtOffset);
            forward = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;
            right = Vector3.ProjectOnPlane(mainCamera.transform.right, Vector3.up).normalized;
        }

        void Move()
        {
            float inputHorizontal = Input.GetAxis("Horizontal");
            float inputVertical = Input.GetAxis("Vertical");
            Vector3 forwardMovement = forward * inputVertical;
            Vector3 rightMovement = right * inputHorizontal;
            Vector3 moveDirection = (forwardMovement + rightMovement).normalized;

            if (characterController.isGrounded)
            {
                gravitySpeed = Vector3.zero;
            }
            else
            {
                gravitySpeed += Physics.gravity * Time.deltaTime;
            }

            characterController.Move((moveDirection * moveSpeed + gravitySpeed) * Time.deltaTime);

            if (Mathf.Abs(inputHorizontal + inputVertical) < 0.001f)
            {
                animator.SetFloat("Forward", 0);
                animator.SetFloat("Rightward", 0);
            } 
            else
            {
                Vector3 movement = Quaternion.AngleAxis(-Vector3.SignedAngle(moveDirection, transform.forward, Vector3.up), Vector3.up) * Vector3.forward;
                animator.SetFloat("Forward", movement.z * forwardMovement.magnitude);
                animator.SetFloat("Rightward", movement.x * rightMovement.magnitude);
            }
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