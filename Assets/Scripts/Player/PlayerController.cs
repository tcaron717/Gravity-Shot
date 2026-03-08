using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Collider2D _playerCollider;
    private LineRenderer _lineRenderer;
    private InputAction _aimPlayer;
    private InputAction _aimPress;
    private PlayerStats _stats;
    private bool _wasAimPressed;

    private Vector2 _dragStartPos;
    private bool _isDragging; 

    private float _launchPower = 1f;
    private float _maxLaunchForce = 5f;
    [SerializeField] private Transform arenaRoot;
    private float _onDragVelocityMultiplier;
    private bool _isInitialized;
    [ColorUsage(true, true)] [SerializeField] private Color primaryTrajectoryColor = Color.cyan;
    [ColorUsage(true, true)] [SerializeField] private Color bounceTrajectoryColor = new Color(1f, 0.5f, 0f, 1f);

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerCollider = GetComponent<Collider2D>();
        _lineRenderer = GetComponent<LineRenderer>();
        _stats = GetComponent<PlayerStats>();

        if (_rb == null || _playerCollider == null || _lineRenderer == null || _stats == null)
        {
            Debug.LogError(
                "PlayerController is missing one or more required components (Rigidbody2D, Collider2D, LineRenderer, PlayerStats). Disabling script.",
                this);
            enabled = false;
            return;
        }

        _lineRenderer.enabled = false;
        _aimPlayer = GameManager.aimPlayer;
        _aimPress = GameManager.aimPress;
        
        //set base stats from StatSheet
        _launchPower = _stats.launchPower;
        _maxLaunchForce = _stats.maxLaunchForce;
        _onDragVelocityMultiplier = _stats.onDragVelocityMultiplier;

        if (arenaRoot == null)
        {
            GameObject arenaObj = GameObject.Find("Arena");
            if (arenaObj != null)
            {
                arenaRoot = arenaObj.transform;
            }
        }

        _isInitialized = true;
    }

    void Update()
    {
        if (!_isInitialized)
        {
            return;
        }

        HandleInput();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isInitialized)
        {
            return;
        }

        if (collision == null || collision.gameObject == null)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player collided with the enemy");
            DamageModel.TakeDamage(gameObject.name, collision.gameObject.name);
        }
    }

    void HandleInput()
    {
        if (_aimPlayer == null)
        {
            _aimPlayer = GameManager.aimPlayer;
        }

        if (_aimPress == null)
        {
            _aimPress = GameManager.aimPress;
        }

        if (_aimPlayer == null || _aimPress == null)
        {
            return;
        }

        if (!TryGetPointerWorldPosition(out Vector2 pointerWorldPos))
        {
            return;
        }

        bool isAimPressed = IsPointerPressed();

        if (isAimPressed && !_wasAimPressed)
        {
            StartDrag(pointerWorldPos);
        }

        if (isAimPressed && _isDragging)
        {
            UpdateDrag(pointerWorldPos);
        }

        if (!isAimPressed && _wasAimPressed && _isDragging)
        {
            EndDrag(pointerWorldPos);
        }

        _wasAimPressed = isAimPressed;
    }

    void UpdateAimLine(Vector2 force)
    {
        Vector2 start = transform.position;
        float maxDistance = force.magnitude;

        _lineRenderer.SetPosition(0, start);

        if (maxDistance <= Mathf.Epsilon)
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(1, start);
            return;
        }

        Vector2 direction = force / maxDistance;

        if (TryGetFirstArenaWallHit(start, direction, maxDistance, out RaycastHit2D hit))
        {
            float remainingDistance = Mathf.Max(0f, maxDistance - hit.distance);
            float bouncePointT = Mathf.Clamp01(hit.distance / maxDistance);

            _lineRenderer.positionCount = 3;
            _lineRenderer.SetPosition(1, hit.point);

            Vector2 reflectedDirection = Vector2.Reflect(direction, hit.normal);
            Vector2 bounceEnd = hit.point + reflectedDirection * remainingDistance;
            _lineRenderer.SetPosition(2, bounceEnd);
            ApplyTrajectoryColor(true, bouncePointT);
            return;
        }

        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(1, start + force);
        ApplyTrajectoryColor(false, 1f);
    }

    void OnDisable()
    {
        _isDragging = false;
        _wasAimPressed = false;
        if (_lineRenderer != null)
        {
            _lineRenderer.enabled = false;
        }
        Time.timeScale = 1f;
    }

    bool TryGetPointerWorldPosition(out Vector2 worldPosition)
    {
        worldPosition = default;
        if (Camera.main == null)
        {
            return false;
        }

        if (Touchscreen.current != null)
        {
            Vector2 touchScreenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            worldPosition = Camera.main.ScreenToWorldPoint(touchScreenPos);
            return true;
        }

        if (Pointer.current != null)
        {
            Vector2 pointerScreenPos = Pointer.current.position.ReadValue();
            worldPosition = Camera.main.ScreenToWorldPoint(pointerScreenPos);
            return true;
        }

        if (_aimPlayer == null)
        {
            return false;
        }

        Vector2 actionScreenPos = _aimPlayer.ReadValue<Vector2>();
        worldPosition = Camera.main.ScreenToWorldPoint(actionScreenPos);
        return true;
    }

    bool IsPointerPressed()
    {
        if (Touchscreen.current != null)
        {
            return Touchscreen.current.primaryTouch.press.isPressed;
        }

        if (_aimPress != null)
        {
            return _aimPress.IsPressed();
        }

        return false;
    }

    void StartDrag(Vector2 startWorldPos)
    {
        if (_lineRenderer == null || _rb == null)
        {
            return;
        }

        _dragStartPos = startWorldPos;
        _isDragging = true;
        _lineRenderer.enabled = true;
        Time.timeScale = 0.2f;
        _rb.angularVelocity = 0f;
        Vector2 currentVelocity = _rb.linearVelocity;
        _rb.linearVelocity = currentVelocity * _onDragVelocityMultiplier;

    }

    void UpdateDrag(Vector2 currentWorldPos)
    {
        if (_lineRenderer == null)
        {
            return;
        }

        Vector2 launchDir = _dragStartPos - currentWorldPos;
        Vector2 clampedForce = Vector2.ClampMagnitude(launchDir * _launchPower, _maxLaunchForce);
        UpdateAimLine(clampedForce);
    }

    void EndDrag(Vector2 endWorldPos)
    {
        if (_lineRenderer == null || _rb == null)
        {
            return;
        }

        Vector2 launchDir = _dragStartPos - endWorldPos;
        Vector2 force = Vector2.ClampMagnitude(launchDir * _launchPower, _maxLaunchForce);
        _rb.AddForce(force, ForceMode2D.Impulse);

        _isDragging = false;
        _lineRenderer.enabled = false;
        Time.timeScale = 1f;
    }

    bool TryGetFirstArenaWallHit(Vector2 origin, Vector2 direction, float distance, out RaycastHit2D firstWallHit)
    {
        firstWallHit = default;
        if (arenaRoot == null)
        {
            return false;
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance);
        float closestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.collider == null)
            {
                continue;
            }

            if (_playerCollider != null && hit.collider == _playerCollider)
            {
                continue;
            }

            if (!hit.collider.transform.IsChildOf(arenaRoot))
            {
                continue;
            }

            if (hit.distance < 0.0001f || hit.distance >= closestDistance)
            {
                continue;
            }

            closestDistance = hit.distance;
            firstWallHit = hit;
        }

        return closestDistance < float.MaxValue;
    }

    void ApplyTrajectoryColor(bool hasBounce, float bouncePointT)
    {
        Gradient gradient = new Gradient();

        if (hasBounce)
        {
            GradientColorKey[] colorKeys =
            {
                new GradientColorKey(primaryTrajectoryColor, 0f),
                new GradientColorKey(primaryTrajectoryColor, bouncePointT),
                new GradientColorKey(bounceTrajectoryColor, 1f)
            };
            GradientAlphaKey[] alphaKeys =
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            };
            gradient.SetKeys(colorKeys, alphaKeys);
        }
        else
        {
            GradientColorKey[] colorKeys =
            {
                new GradientColorKey(primaryTrajectoryColor, 0f),
                new GradientColorKey(primaryTrajectoryColor, 1f)
            };
            GradientAlphaKey[] alphaKeys =
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            };
            gradient.SetKeys(colorKeys, alphaKeys);
        }

        _lineRenderer.colorGradient = gradient;
    }
}
