using UnityEngine;
using Mirror;

namespace Initiation
{
    [RequireComponent(typeof(CharacterController))]    
    public class PlayerController : NetworkBehaviour
    {
        public CharacterController characterController;

        public Camera mainCamera;
        public Vector3 cameraOffset = new Vector3(2, 4, -2);
        public Vector3 cameraLookAtOffset = new Vector3(0, 2, 0);

        public string groundLayer = "Ground";

        public float moveSpeed = 7.5f;
        public float jumpHeight = 1.2f;

        private Vector3 forward = Vector3.forward;
        private Vector3 right = Vector3.right;

        private Vector3 gravitySpeed = Vector3.zero;

        private Animator animator = null;

        private CharacterStats characterStats;
        private AbilityManager abilityManager;

        const float SQRT_OF_2 = 1.41421356237f;

        public override void OnStartLocalPlayer()
        {
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }
            characterController.enabled = isLocalPlayer;

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            UpdateCamera();

            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }

            if (characterStats == null)
            {
                characterStats = GetComponent<CharacterStats>();
            }

            if (abilityManager == null)
            {
                abilityManager = GetComponent<AbilityManager>();
            }
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

            float movementSpeed = Mathf.Sqrt(Mathf.Abs(inputHorizontal) + Mathf.Abs(inputVertical)) / SQRT_OF_2;
            animator.SetFloat("MovementSpeed", movementSpeed);
        }

        void Rotate()
        {
            // Adapted from https://learn.unity.com/project/survival-shooter-tutorial
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit floorHit;
            if (Physics.Raycast(camRay, out floorHit, Mathf.Infinity, LayerMask.GetMask(groundLayer)))
            {
                Vector3 playerToMouse = floorHit.point - transform.position;
                playerToMouse.y = 0f;
                Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
                transform.rotation = newRotation;
            }
        }

        void Update()
        {
            if (!isLocalPlayer || !characterController.enabled)
            {
                return;
            }
            animator.SetBool("IsDead", characterStats.dead);
            if (characterStats.dead)
            {
                return;
            }

            Abilities();
            Move();
            UpdateCamera();
        }

        void Abilities()
        {
            if (Input.GetKey(KeyCode.F))
            {
                abilityManager.healingRangeIndicator.SetActive(abilityManager.remainingCooldownHealing <= 0);
            }
            if (Input.GetKeyUp(KeyCode.F) && !Input.GetKey(KeyCode.Escape))
            {
                Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit floorHit;
                if (Physics.Raycast(camRay, out floorHit, Mathf.Infinity, LayerMask.GetMask(groundLayer)))
                {
                    abilityManager.CmdHeal(floorHit.point);
                }
                abilityManager.healingRangeIndicator.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                GetComponent<AbilityManager>().CmdCastFireball();
            }

        }

        void FixedUpdate()
        {
            if (!isLocalPlayer || !characterController.enabled || characterStats.dead)
            {
                return;
            }
            Rotate();
        }
    }
}