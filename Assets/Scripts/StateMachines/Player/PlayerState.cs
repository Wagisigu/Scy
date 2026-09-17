using UnityEngine;

public abstract class PlayerState
{
    protected PlayerController _playerController;

    public PlayerState(PlayerController playerController)
    {
        _playerController = playerController;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void OnEnter()
    {

    }

    public virtual void OnExit()
    {

    }

}
