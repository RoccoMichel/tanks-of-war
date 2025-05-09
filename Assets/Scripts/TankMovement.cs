using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankMovement : MonoBehaviour
{
    public float rotationSpeed = 5;
    public float driveSpeed = 5;
    public float roadMultiplier = 1.5f;
    public float boostPower = 2.5f;
    private new Rigidbody2D rigidbody;
    private InputAction moveAction;
    private bool onRoad;
    private float boostTimer;
    [SerializeField] private TrailRenderer trail;
    [HideInInspector] public PhotonView view;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        rigidbody = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        boostTimer = Mathf.Clamp(boostTimer -= Time.deltaTime, 0, int.MaxValue);
        if (boostTimer == 0) trail.enabled = false;

        if (!view.IsMine) return;

        if (moveAction.inProgress)
        {
            Vector2 moveValue = moveAction.ReadValue<Vector2>();

            // Rotation
            if (moveValue.x != 0)
            {
                float inputValue = moveValue.x * Time.deltaTime * -1f;
                Vector3 newRotation = new (0, 0, inputValue * rotationSpeed * 100);
                transform.Rotate(newRotation);
            }

            // Driving
            if (moveValue.y != 0)
            {
                float newSpeed = moveValue.y * Time.deltaTime * driveSpeed;
                newSpeed *= onRoad ? roadMultiplier : 1; // drive faster when on the road
                newSpeed *= boostTimer > 0 ? boostPower : 1; // apply possible boost
                rigidbody.AddForce(transform.up * newSpeed, ForceMode2D.Impulse);
            }
        }
    }

    public void Boost(float timeSeconds)
    {
        boostTimer += timeSeconds;
        trail.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Road"))
            onRoad = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Road"))
            onRoad = false;
    }
}
