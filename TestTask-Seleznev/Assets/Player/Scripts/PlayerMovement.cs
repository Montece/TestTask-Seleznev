using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float MoveSpeed = 8f;
    [SerializeField] private float RunSpeed = 14f;
    [SerializeField] private float Gravity = -9.81f;
    [SerializeField] private float JumpHeight = 3f;
    [Space]
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private float GroundDistance = 0.4f;
    [SerializeField] private LayerMask GroundMask;

    private Vector3 velocity;
    private bool isGrounded;
    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        //��������, ���� �� �����
        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

        //������� ���������
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        //���������� ������
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        //����� �� �����
        bool isRun = Input.GetKey(KeyCode.LeftShift) && move != Vector3.zero;

        if (isRun)
        {
            //���� ����� - ��������� �����
            controller.Move(RunSpeed * Time.deltaTime * move);
        }
        else
        {
            //����� ��������� ����������
            controller.Move(MoveSpeed * Time.deltaTime * move);
        }

        //���� ������� ����� � �� �� � �������
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            //���. ������� ��� �������� ������
            velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        velocity.y += Gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    ///<summary> ��������� ������� ������, ����������������� ������������ ����� ������ </summary>
    public WorldPos GetPosition()
    {
        return new WorldPos((int)(transform.position.x / WorldPos.CHUNK_SIZE), (int)(transform.position.z / WorldPos.CHUNK_SIZE));
    }
}