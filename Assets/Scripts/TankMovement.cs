using UnityEngine;
using UnityEngine.InputSystem;

public class TankMovement : MonoBehaviour
{
    public float rotationSpeed = 5;
    public float driveSpeed = 3;
    private new Rigidbody2D rigidbody;
    private InputAction moveAction;
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (moveAction.inProgress)
        {
            Vector2 moveValue = moveAction.ReadValue<Vector2>();

            // Rotation
            if (moveValue.x != 0)
            {
                float inputValue = moveValue.x * Time.deltaTime * -1f;
                Vector3 newRotation = new Vector3(0, 0, inputValue * rotationSpeed * 100);
                transform.Rotate(newRotation);
            }

            // Driving
            if (moveValue.y != 0)
            {
                float newSpeed = moveValue.y * Time.deltaTime * driveSpeed;
                rigidbody.AddForce(transform.up * newSpeed, ForceMode2D.Impulse);
            }
        }
    }
}
