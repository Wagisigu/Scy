using UnityEngine;

public class PlayerGroundState : PlayerState
{

    public PlayerGroundState(PlayerController playerController) : base(playerController)
    {
        
    }

    public override void Update()
    {
        if (_playerController._moveInput.x != 0)
        {
            _playerController._animator.Play("Walk");
        }
        else
        {
            _playerController._animator.Play("Idle");
        }
    }

    public override void FixedUpdate()
    {
        _playerController._rb.linearVelocityX = _playerController._moveInput.x * _playerController._walkSpeed;
    }

    public override void OnEnter()
    {
        _playerController._animator.Play("Idle");
    }

    public override void OnExit()
    {

    }


}
