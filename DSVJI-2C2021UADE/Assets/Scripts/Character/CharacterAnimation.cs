using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class CharacterAnimation : MonoBehaviour
{
    private Character _character;

    private static readonly int JumpTrigger = Animator.StringToHash("JumpTrigger");
    private static readonly int LandTrigger = Animator.StringToHash("LandingTrigger");
    private static readonly int SwitchTrigger = Animator.StringToHash("SwitchTrigger");
    private static readonly int DeathTrigger = Animator.StringToHash("DeathTrigger");
    private static readonly int GroundFloat = Animator.StringToHash("GroundFloat");
    private static readonly int AirFloat = Animator.StringToHash("AirFloat");
    private static readonly int GoingForwardBool = Animator.StringToHash("IsGoingForward");
    private static readonly int IdleBool = Animator.StringToHash("IsIdle");
    private static readonly int SprintBool = Animator.StringToHash("IsSprinting");
    public event Action OnSwitchComplete;
    public event Action OnDeathComplete;

    private void Awake()
    {
        _character = GetComponent<Character>();
    }

    private void Start()
    {
        _character.ThirdPersonController.OnJump += JumpHandler;
        _character.ThirdPersonController.OnSprint += SprintHandler;
        _character.OnCharacterSwitch += SwitchHandler;
        _character.Health.OnDeath += DeathHandler;
    }

    private void Update()
    {
        //if (character.IsAnimationLocked) return; VOS ERAS EL QUE ROMPIA
        // character.Controller.velocity.magnitude < 0.001f
        if (_character.Controller.isGrounded) // character.Controller.isGrounded
        {
            Landing();
            // moving
            if (_character.ThirdPersonController.IsInputMoving)
            {
                _character.Animator.SetBool(IdleBool, false);
                // Forward
                if (transform.InverseTransformDirection(_character.ThirdPersonController.MoveDirection).z > 0 && _character.Input.VerticalAxis > 0)
                {
                    _character.Animator.SetBool(GoingForwardBool, true);
                }
                // Backward
                if (transform.InverseTransformDirection(_character.ThirdPersonController.MoveDirection).z  < 0 && _character.Input.VerticalAxis < 0)
                {
                    _character.Animator.SetBool(GoingForwardBool, false);
                }
                // Walk or Sprint
                _character.Animator.SetFloat(GroundFloat, _character.Controller.velocity.magnitude);
                _character.Animator.ResetTrigger(JumpTrigger);
            }

            // if not moving idle
            if (!_character.ThirdPersonController.IsInputMoving)
            {
                _character.Animator.SetBool(IdleBool, true);
                _character.Animator.SetFloat(GroundFloat, 0f);
            }
        }

        if (!_character.Controller.isGrounded) // !character.Controller.isGrounded / !character.ThirdPersonController.CoyoteGrounded
        {
            NotGrounded();
            //character.Animator.SetFloat(AirFloat, Mathf.Abs(character.Controller.velocity.y) + 0.1f);
            
            if (_character.Controller.velocity.y < 0.1f)
            {
                _character.Animator.SetFloat(AirFloat, Mathf.Abs(_character.Controller.velocity.y) + 0.1f);
            }
        }
    }
    
    private void DeathHandler()
    {
        _character.Animator.SetTrigger(DeathTrigger);
    }
    
    // RDeath Animation event uses this method
    public void DeathAnimationEndedEvent()
    {
        _character.Animator.ResetTrigger(DeathTrigger);
        OnDeathComplete?.Invoke();
    }
    
    private void JumpHandler()
    {
        _character.Animator.SetTrigger(JumpTrigger);
        _character.Animator.ResetTrigger(LandTrigger);
    }

    private void SprintHandler()
    {
        _character.Animator.SetBool(SprintBool, _character.ThirdPersonController.IsSprinting);
    }

    private void NotGrounded()
    {
        _character.Animator.SetFloat(GroundFloat, 0);
    }

    private void Landing()
    {
        if (_character.Animator.GetFloat(AirFloat) > -0.1)
        {
            _character.Animator.SetTrigger(LandTrigger);
        }

        _character.Animator.SetFloat(AirFloat, 0);
    }

    private void SwitchHandler()
    {
        _character.Animator.SetTrigger(SwitchTrigger);
        StartCoroutine(WaitForSwitch());
    }

    
    private IEnumerator WaitForSwitch()
    {
        yield return new WaitForSeconds(0.2f);
        var currAnim = _character.Animator.GetCurrentAnimatorClipInfo(0);
        var clipLength = currAnim[0].clip.length;
        while (clipLength > 0)
        {
            clipLength -= Time.deltaTime;
            yield return null;
        }

        if (clipLength <= 0)
        {
            _character.Animator.ResetTrigger(SwitchTrigger);
            OnSwitchComplete?.Invoke();
        }
    }
}