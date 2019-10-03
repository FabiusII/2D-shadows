using SpriteShadows.Scripts;
using UnityEngine;

namespace SpriteShadows.Example
{
    public class PlayerController : PhysicsObject
    {
        [SerializeField] private float maxSpeed = 7;
        [SerializeField] private float jumpTakeOffSpeed = 7;
        [SerializeField] private float lightFocusSpeed = 2.5f;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private Spotlight2D spotlight;
        [SerializeField] private Camera camera;

        private int _animatorWalkHash;

        private void Awake()
        {
            _animatorWalkHash = Animator.StringToHash("Walk");
        }

        protected override void CalculateVelocity()
        {
            Vector2 move = Vector2.zero;

            if (Input.GetKey(KeyCode.D))
            {
                move.x = 1;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                move.x = -1;
            }

            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
            {
                Velocity.y = jumpTakeOffSpeed;
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                if (Velocity.y > 0)
                {
                    Velocity.y *= 0.5f;
                }
            }

            if (Input.GetMouseButton(0))
            {
                spotlight.DecreaseAngle(lightFocusSpeed);
            }

            if (Input.GetMouseButton(1))
            {
                spotlight.IncreaseAngle(lightFocusSpeed);
            }

            var mousePosition = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                spotlight.transform.position.z));
            var mouseDirection = mousePosition - spotlight.transform.position;
            var mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
            spotlight.transform.rotation = Quaternion.Euler(0, 0, mouseAngle);

            if (mouseDirection.x < -0.01f)
            {
                spriteRenderer.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (mouseDirection.x > 0.01f)
            {
                spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            animator.SetBool(_animatorWalkHash, Mathf.Abs(move.x) > 0.01f);
            TargetVelocity = move * maxSpeed;
        }
    }
}