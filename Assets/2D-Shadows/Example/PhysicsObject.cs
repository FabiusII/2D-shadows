using System.Collections.Generic;
using UnityEngine;

namespace SpriteShadows.Example
{
    public class PhysicsObject : MonoBehaviour
    {
        protected const float MinMoveDistance = 0.001f;
        protected const float ShellRadius = 0.01f;

        [SerializeField] protected float minGroundNormalY = .65f;
        [SerializeField] protected float gravityModifier = 1f;
        [SerializeField] protected Rigidbody2D rigidBody;

        protected Vector2 TargetVelocity;
        protected bool IsGrounded;
        protected Vector2 GroundNormal;
        protected Vector2 Velocity;
        protected RaycastHit2D[] HitBuffer = new RaycastHit2D[16];
        protected List<RaycastHit2D> HitBufferList = new List<RaycastHit2D>(16);

        private void Update()
        {
            TargetVelocity = Vector2.zero;
            CalculateVelocity();
        }

        protected virtual void CalculateVelocity()
        {
        }

        private void FixedUpdate()
        {
            Velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
            Velocity.x = TargetVelocity.x;

            IsGrounded = false;

            Vector2 deltaPosition = Velocity * Time.deltaTime;

            Vector2 moveAlongGround = new Vector2(GroundNormal.y, -GroundNormal.x);

            Vector2 move = moveAlongGround * deltaPosition.x;

            Movement(move, false);

            move = Vector2.up * deltaPosition.y;

            Movement(move, true);
        }

        private void Movement(Vector2 move, bool yMovement)
        {
            float distance = move.magnitude;

            if (distance > MinMoveDistance)
            {
                int count = rigidBody.Cast(move, HitBuffer, distance + ShellRadius);
                HitBufferList.Clear();
                for (int i = 0; i < count; i++)
                {
                    HitBufferList.Add(HitBuffer[i]);
                }

                for (int i = 0; i < HitBufferList.Count; i++)
                {
                    Vector2 currentNormal = HitBufferList[i].normal;
                    if (currentNormal.y > minGroundNormalY)
                    {
                        IsGrounded = true;
                        if (yMovement)
                        {
                            GroundNormal = currentNormal;
                            currentNormal.x = 0;
                        }
                    }

                    float projection = Vector2.Dot(Velocity, currentNormal);
                    if (projection < 0)
                    {
                        Velocity = Velocity - projection * currentNormal;
                    }

                    float modifiedDistance = HitBufferList[i].distance - ShellRadius;
                    distance = modifiedDistance < distance ? modifiedDistance : distance;
                }
            }

            rigidBody.position = rigidBody.position + move.normalized * distance;
        }
    }
}