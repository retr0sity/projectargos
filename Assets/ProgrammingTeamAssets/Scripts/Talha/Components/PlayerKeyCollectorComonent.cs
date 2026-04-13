using UnityEngine;
using Game.Managers;
using UnityEngine.InputSystem;

namespace Game.Components
{
    public class PlayerKeyCollectorComonent : MonoBehaviour
    {
        private InputManager InputManager => DependencyManager.Instance.InputManager;

        float VerticalInput = 0;
        float HorizontalInput = 0;
        public string CollectedKey;
        public float MoveSpeed = 5f;

        private Rigidbody2D mRigidbody2D;
        private Rigidbody2D Rigidbody2D
        {
            get 
            {  
                if (mRigidbody2D == null)
                {
                    mRigidbody2D = GetComponent<Rigidbody2D>();
                }
                return mRigidbody2D; 
            }
        }

        private void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            transform.position = Vector3.zero;
        }

        void FixedUpdate()
        {
            CheckInputs();
            Vector2 movement = new Vector2(HorizontalInput, VerticalInput);
            Rigidbody2D.MovePosition(Rigidbody2D.position + movement * MoveSpeed * Time.fixedDeltaTime);
        }

        private void CheckInputs()
        {
            VerticalInput = 0;
            HorizontalInput = 0;

            var Up = InputManager.InputButtonUp.ReadValue<float>();
            var Down = InputManager.InputButtonDown.ReadValue<float>();
            var Left = InputManager.InputButtonLeft.ReadValue<float>();
            var Right = InputManager.InputButtonRight.ReadValue<float>();

            if (Up > 0)
            {
                VerticalInput = 1;
            }
            else if (Down > 0)
            {
                VerticalInput = -1;
            }

            if (Left > 0)
            {
                HorizontalInput = -1;
            }
            else if (Right > 0)
            {
                HorizontalInput = 1;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Key"))
            {
                Debug.Log("Key Collected");
                collision.GetComponent<KeyComponent>().CollectKey();
            }
            else if (collision.CompareTag("OutCast"))
            {
                collision.GetComponentInParent<OutcastComponent>().ShowDialogue();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Door"))
            {
                Debug.Log("Door Reached");
                collision.gameObject.GetComponent<DoorComponent>().CheckDoorOpening();
            }
        }
    }
}