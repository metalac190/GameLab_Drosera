using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLock : MonoBehaviour
{
    PlayerBase _player;

    [SerializeField] float _maxRange = 20f;

    [HideInInspector] public GameObject _currentTarget;
    float _inputCooldown;
    List<Collider> _visibleEnemies = new List<Collider>();
    LayerMask _mask;

    private void Awake()
    {
        //get reference to player and set layer mask for enemies
        _player = GetComponent<PlayerBase>();
        _mask = LayerMask.GetMask("Enemy");
    }

    private void Update()
    {
        //if player changes target and no target is selected, target nearest enemy
        if (_player.AimToggle && _currentTarget == null)
        {
            GetNearestEnemy();
 
            Debug.Log(_currentTarget.name);
        }
        else if (_player.AimToggle && _currentTarget != null)
        {
            _currentTarget = null;
        }

        if (_currentTarget != null)
        {
            if (_player.CycleTargetRight && Time.fixedTime - _inputCooldown > 0.05f)
            {
                GetNextEnemy(1);
                _inputCooldown = Time.fixedTime;
                Debug.Log(_currentTarget.name);
            }
            else if (_player.CycleTargetLeft && Time.fixedTime - _inputCooldown > 0.05f)
            {
                GetNextEnemy(-1);
                _inputCooldown = Time.fixedTime;
                Debug.Log(_currentTarget.name);
            }
        }

        //if currently targeted enemy moves outside of range, target nearest enemy
        if (_currentTarget != null && Vector3.Distance(_currentTarget.transform.position, transform.position) > _maxRange)
        {
            GetNearestEnemy();
            Debug.Log(_currentTarget.name);
        }

        //Orient player towards current target
        if (_currentTarget != null)
        {
            LookAtTarget();
        }
    }

    //sets target to nearest enemy
    void GetNearestEnemy()
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
            if (hit.collider == enemy)
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
        transform.RotateAround(transform.position, Vector3.up, angle);
    }
}
