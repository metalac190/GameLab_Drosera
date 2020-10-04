using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetLock : MonoBehaviour
{
    PlayerBase _player;

    [Header("Targeting properties")]
    [SerializeField] float _maxRange = 20f;
    [SerializeField] float _turnSpeed = 5;

    [Header("Set this to an empty GameObject")]
    [SerializeField] GameObject _aimingReticle;

    [Header("Set this to the UI element for the lock-on")]
    [SerializeField] RectTransform _targetingReticle;

    [HideInInspector] public GameObject _currentTarget;
    float _inputCooldown;
    List<Collider> _visibleEnemies = new List<Collider>();
    LayerMask _mask;
    bool _stickRelease = false;
    bool _mouseAiming = true;
    bool _offsetSet = false;
    Vector3 _offset = Vector3.zero;
    Vector3 _lastMousePosition = Vector3.zero;

    private void Awake()
    {
        //get reference to player and set layer mask for enemies
        _player = GetComponent<PlayerBase>();
        _mask = LayerMask.GetMask("Enemy");
        
        GameObject canvas = FindObjectOfType<Canvas>().gameObject;
        if (canvas != null)
        {
            _targetingReticle = Instantiate(_targetingReticle, canvas.transform);
        }
        _targetingReticle.gameObject.SetActive(false);
        _currentTarget = _aimingReticle;
    }

    private void Update()
    {
        //if player changes target and no target is selected, target nearest enemy
        if (_player.AimToggle && _currentTarget == _aimingReticle)
        {
            GetNearestEnemy();
        }
        else if (_player.AimToggle && _currentTarget != _aimingReticle)
        {
            _currentTarget = _aimingReticle;
        }

        if (_currentTarget == null)
        {
            if (!GetNearestEnemy())
            {
                _currentTarget = _aimingReticle;
            }
        }

        if (_currentTarget != _aimingReticle && _stickRelease)
        {
            if (_player.CycleTargetRight && Time.fixedTime - _inputCooldown > 0.1f)
            {
                GetNextEnemy(1);
                _inputCooldown = Time.fixedTime;
                _stickRelease = false;
            }
            else if (_player.CycleTargetLeft && Time.fixedTime - _inputCooldown > 0.1f)
            {
                GetNextEnemy(-1);
                _inputCooldown = Time.fixedTime;
                _stickRelease = false;
            }
        }

        if (!(_player.CycleTargetLeft || _player.CycleTargetRight))
        {
            _stickRelease = true;
        }

        if (_currentTarget != _aimingReticle)
        {
            _targetingReticle.gameObject.SetActive(true);
            _targetingReticle.position = Camera.main.WorldToScreenPoint(_currentTarget.transform.position);
        }
        else
        {
            _targetingReticle.gameObject.SetActive(false);
        }

        //if currently targeted enemy moves outside of range, target nearest enemy
        if (_currentTarget != _aimingReticle && Vector3.Distance(_currentTarget.transform.position, transform.position) > _maxRange)
        {
            GetNearestEnemy();
        }

        //if no enemy is locked, aim with controller or mouse
        if (_currentTarget == _aimingReticle)
        {
            ManageAiming();
        }

        //Orient player towards current target
        LookAtTarget();
    }

    private void LateUpdate()
    {
        _lastMousePosition = Input.mousePosition;
    }

    //sets target to nearest enemy
    bool GetNearestEnemy()
    {
        float minDistance = 9999f;

        GetVisibleEnemies();

        foreach (var enemy in _visibleEnemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if (distance < minDistance)
            {
                _currentTarget = enemy.gameObject;
                minDistance = distance;
            }
        }

        if (!(_currentTarget == _aimingReticle || _currentTarget == null))
        {
            return true;
        }
        return false;
    }

    void GetNextEnemy(float input)
    {
        float smallestAngle = 0f;
        float largestAngle = 0f;
        bool moreThan180 = true;

        GetVisibleEnemies();

        //If the input is positive, cycles through visible targets clockwise
        if (input > 0)
        {
            smallestAngle = 9999f;
            foreach (var enemy in _visibleEnemies)
            {
                float angle = GetAngleToEnemy(enemy.transform);
                if (angle > 0 && angle < smallestAngle)
                {
                    smallestAngle = angle;
                    _currentTarget = enemy.gameObject;
                    moreThan180 = false;
                }
            }
            if (moreThan180)
            {
                foreach (var enemy in _visibleEnemies)
                {
                    float angle = GetAngleToEnemy(enemy.transform);
                    if (angle < 0 && angle < smallestAngle)
                    {
                        smallestAngle = angle;
                        _currentTarget = enemy.gameObject;
                    }
                }
            }
        }
        //if the input is negative, cycles through visible targets counter-clockwise
        else if (input < 0)
        {
            largestAngle = -9999f;
            foreach (var enemy in _visibleEnemies)
            {
                float angle = GetAngleToEnemy(enemy.transform);
                if (angle < 0 && angle > largestAngle)
                {
                    largestAngle = angle;
                    _currentTarget = enemy.gameObject;
                    moreThan180 = false;
                }
            }
            if (moreThan180)
            {
                foreach (var enemy in _visibleEnemies)
                {
                    float angle = GetAngleToEnemy(enemy.transform);
                    if (angle > 0 && angle > largestAngle)
                    {
                        largestAngle = angle;
                        _currentTarget = enemy.gameObject;
                    }
                }
            }
        }
    }

    //sets a list with all enemies within range and within line of sight
    void GetVisibleEnemies()
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, _maxRange, _mask);

        _visibleEnemies.Clear();

        foreach (var enemy in nearbyEnemies)
        {
            RaycastHit hit;
            Vector3 rayDirection = enemy.transform.position - transform.position;
            Physics.Raycast(transform.position, rayDirection, out hit);
            if (hit.collider == enemy && enemy.gameObject != _currentTarget)
            {
                _visibleEnemies.Add(enemy);
            }
        }
    }

    float GetAngleToEnemy(Transform enemy)
    {
        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 target = new Vector3(enemy.position.x, 0, enemy.position.z);
        float angle = Vector3.SignedAngle(transform.forward, target - pos, Vector3.up);

        return angle;
    }

    void LookAtTarget()
    {
        float angle = GetAngleToEnemy(_currentTarget.transform);
        float angleToTurn = Mathf.Lerp(0, angle, _turnSpeed * Time.deltaTime);
        transform.RotateAround(transform.position, Vector3.up, angleToTurn);
    }

    void ManageAiming()
    {
        Vector3 mousePosition = Input.mousePosition;
        float xInput = Input.GetAxis("Controller Right Stick X");
        float yInput = Input.GetAxis("Controller Right Stick Y");

        if (!(xInput == 0 && yInput == 0))
        {
            _mouseAiming = false;
        }
        else if (mousePosition != _lastMousePosition)
        {
            _mouseAiming = true;
        }

        if (_mouseAiming)
        {
            MouseAim(mousePosition);
        }
        else
        {
            ControllerAim(xInput, yInput);
        }
    }

    void ControllerAim(float xInput, float yInput)
    {
        Vector2 input = new Vector2(xInput, yInput);
        if (input.magnitude > .25)
        {
            _offsetSet = false;
            Vector3 direction = (transform.position - Camera.main.transform.position).normalized;
            _aimingReticle.transform.position = transform.position + direction;

            float angle = Mathf.Atan2(xInput, yInput) * Mathf.Rad2Deg;

            _aimingReticle.transform.RotateAround(transform.position, Vector3.up, angle);
        }
        else
        {
            if (!_offsetSet)
            {
                _offset = (transform.position - _aimingReticle.transform.position);
                _offsetSet = true;
            }
            _aimingReticle.transform.position = transform.position - _offset;
        }
    }

    void MouseAim(Vector3 mousePosition)
    {
        mousePosition.z = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
        //mousePosition = Camera.main.ScreenToWorldPoint(mousePosition); //Use this line for perspective camera

        Vector3 direction = mousePosition - Camera.main.transform.position;
        Plane xzPlane = new Plane(Vector3.up, new Vector3(0, 1, 0));
        //Ray ray = new Ray(Camera.main.transform.position, direction);  //Use this line for perspective camera
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);           //Use this line for orthographic camera
        float distance;

        xzPlane.Raycast(ray, out distance);
        //Debug.DrawRay(Camera.main.transform.position, direction * 20);

        Vector3 hitPoint = ray.GetPoint(distance);

        _aimingReticle.transform.position = hitPoint;
    }
}
