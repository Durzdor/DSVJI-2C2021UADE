using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class CharacterAnimation : MonoBehaviour
{
    private Character character;

    private static readonly int JumpTrigger = Animator.StringToHash("JumpTrigger");
    private static readonly int LandTrigger = Animator.StringToHash("LandingTrigger");
    private static readonly int SwitchTrigger = Animator.StringToHash("SwitchTrigger");
    private static readonly int GroundFloat = Animator.StringToHash("GroundFloat");
    private static readonly int AirFloat = Animator.StringToHash("AirFloat");
    private static readonly int GoingForward = Animator.StringToHash("IsGoingForward");
    private static readonly int IdleBool = Animator.StringToHash("IsIdle");
    private static readonly int SprintBool = Animator.StringToHash("IsSprinting");
    
    public event Action OnSwitchComplete;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        character.ThirdPersonController.OnJump += JumpHandler;
        character.ThirdPersonController.OnSprint += SprintHandler;
        character.OnCharacterSwitch += SwitchHandler;
    }

    private void Update()
    {
        // character.Controller.velocity.magnitude < 0.001f
        if (character.Controller.isGrounded)
        {
            Landing();
            // moving
            if (character.ThirdPersonController.IsInputMoving)
            {
                character.Animator.SetBool(IdleBool, false);
                // Forward
                if (transform.InverseTransformDirection(character.ThirdPersonController.MoveDirection).z > 0)
                {
                    character.Animator.SetBool(GoingForward, true);
                }
                // Backward
                if (transform.InverseTransformDirection(character.ThirdPersonController.MoveDirection).z  < 0)
                {
                    character.Animator.SetBool(GoingForward, false);
                }
                // Walk or Sprint
                character.Animator.SetFloat(GroundFloat, character.Controller.velocity.magnitude);
                character.Animator.ResetTrigger(JumpTrigger);
            }

            // if not moving idle
            if (!character.ThirdPersonController.IsInputMoving)
            {
                character.Animator.SetBool(IdleBool, true);
                character.Animator.SetFloat(GroundFloat, 0f);
            }
        }

        if (!character.Controller.isGrounded)
        {
            NotGrounded();
            character.Animator.SetFloat(AirFloat, Mathf.Abs(character.Controller.velocity.y) + 0.1f);
        }
    }

    private void JumpHandler()
    {
        character.Animator.SetTrigger(JumpTrigger);
        character.Animator.ResetTrigger(LandTrigger);
    }

    private void SprintHandler()
    {
        character.Animator.SetBool(SprintBool, character.ThirdPersonController.IsSprinting);
    }

    private void NotGrounded()
    {
        character.Animator.SetFloat(GroundFloat, 0);
    }

    private void Landing()
    {
        if (character.Animator.GetFloat(AirFloat) > 0)
        {
            character.Animator.SetTrigger(LandTrigger);
        }

        character.Animator.SetFloat(AirFloat, 0);
    }

    private void SwitchHandler()
    {
        character.Animator.SetTrigger(SwitchTrigger);
        StartCoroutine(WaitForSwitch());
    }

    
    private IEnumerator WaitForSwitch()
    {
        yield return new WaitForSeconds(0.2f);
        var currAnim = character.Animator.GetCurrentAnimatorClipInfo(0);
        var clipLength = currAnim[0].clip.length;
        while (clipLength > 0)
        {
            clipLength -= Time.deltaTime;
            yield return null;
        }

        if (clipLength <= 0)
        {
            character.Animator.ResetTrigger(SwitchTrigger);
            OnSwitchComplete?.Invoke();
        }
    }
}