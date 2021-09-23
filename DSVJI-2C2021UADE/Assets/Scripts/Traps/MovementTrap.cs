﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTrap : MonoBehaviour
{
    #region SerializedFields
#pragma warning disable 649
    [Header("Movement Trap Settings")] [Space(5)]
    [SerializeField] private bool canMoveBack = false;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private float moveBackInterval = 1f;
    [SerializeField] private float moveStartInterval = 1f;
    [SerializeField] private Transform movePivot;
#pragma warning restore 649
    #endregion
    
    private bool isMovementActive = false;

    private void Update()
    {
        StartCoroutine(ObjectMovement());
    }
    
    private IEnumerator ObjectMovement()
    {
        if (isMovementActive)
            yield break;
        isMovementActive = true;
        float counter = 0;
        Vector3 defaultPosition = movePivot.position;
        Vector3 maxPosition = targetPosition.position; // transform.position + targetPosition

        while (counter < moveDuration)
        {
            counter += Time.deltaTime;
            LerpPos(defaultPosition, maxPosition, counter);
            yield return null;
        }

        if (canMoveBack)
        {
            yield return new WaitForSeconds(moveBackInterval);

            while (counter > 0)
            {
                counter -= Time.deltaTime;
                LerpPos(defaultPosition, maxPosition, counter);
                yield return null;
            }
            
            yield return new WaitForSeconds(moveStartInterval);
        }

        isMovementActive = false;
    }
    
    private void LerpPos(Vector3 defaultValue, Vector3 targetValue, float counter)
    {
        movePivot.position = Vector3.Lerp(defaultValue, targetValue, counter / moveDuration);
    }
}
