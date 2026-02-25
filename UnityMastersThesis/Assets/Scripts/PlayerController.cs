using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Tilemap _collisionTilemap;
    [SerializeField] private float _walkSpeed = 4f;
    [SerializeField] private float _runSpeed = 7f;
    [SerializeField] private Animator _animator;
    [SerializeField] private InputActionReference _move;
    [SerializeField] private InputActionReference _run;

    private enum State { Idle, Walking, Running }
    private State _state = State.Idle;

    private enum Direction { Up, Down, Left, Right }
    private Direction _direction = Direction.Down;
    
    private Vector2 _moveInput;
    private bool _runHeld;

    private Vector3 _targetPos;
    
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    
    private void OnEnable()
    {
        _move.action.Enable();
        _move.action.performed += c => _moveInput = c.ReadValue<Vector2>();
        _move.action.canceled += _ => _moveInput = Vector2.zero;

        _run.action.Enable();
        _run.action.performed += _ => _runHeld = true;
        _run.action.canceled += _ => _runHeld = false;
    }

    private void Update()
    {
        if (_state == State.Idle)
            TryStartMove();

        if (_state != State.Idle)
            MoveStep();

        UpdateAnimator();
    }

    private void TryStartMove()
    {
        Vector2 dir = new Vector2(
            Mathf.Round(_moveInput.x),
            Mathf.Round(_moveInput.y)
        );

        if (dir == Vector2.zero) return;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            dir.y = 0;
        else
            dir.x = 0;

        Vector3Int gridDir = Vector3Int.zero;

        if (dir == Vector2.up)    { _direction = Direction.Up;    gridDir = Vector3Int.up; }
        if (dir == Vector2.down)  { _direction = Direction.Down;  gridDir = Vector3Int.down; }
        if (dir == Vector2.left)  { _direction = Direction.Left;  gridDir = Vector3Int.left; }
        if (dir == Vector2.right) { _direction = Direction.Right; gridDir = Vector3Int.right; }

        Vector3Int current = _collisionTilemap.WorldToCell(transform.position);
        Vector3Int next = current + gridDir;

        if (!_collisionTilemap.HasTile(next)) return;

        _targetPos = _collisionTilemap.GetCellCenterWorld(next) + new Vector3(0f, 0.5f, 0f);
        _state = _runHeld ? State.Running : State.Walking;
    }

    private void MoveStep()
    {
        float speed = _state == State.Running ? _runSpeed : _walkSpeed;

        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetPos,
            speed * Time.deltaTime
        );

        if ((transform.position - _targetPos).sqrMagnitude < 0.001f)
        {
            transform.position = _targetPos;
            _state = State.Idle;
        }
    }

    private void UpdateAnimator()
    {
        Vector2 dir = DirectionToVector(_direction);

        _animator.SetFloat(MoveX, dir.x);
        _animator.SetFloat(MoveY, dir.y);
        _animator.SetBool(IsMoving, _state != State.Idle);
        _animator.SetBool(IsRunning, _state == State.Running);
    }

    private Vector2 DirectionToVector(Direction d)
    {
        return d switch
        {
            Direction.Up => Vector2.up,
            Direction.Down => Vector2.down,
            Direction.Left => Vector2.left,
            Direction.Right => Vector2.right,
            _ => Vector2.zero
        };
    }
}