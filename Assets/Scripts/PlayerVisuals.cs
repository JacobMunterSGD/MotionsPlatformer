using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer bodyRenderer;
    public PlayerController playerController;

    private readonly int walkingHash = Animator.StringToHash("IsWalking");
    private readonly int idleHash = Animator.StringToHash("Idle");
    private readonly int jumpingHash = Animator.StringToHash("Jumping");
    private readonly int deadHash = Animator.StringToHash("Dead");


    void Update()
    {
        UpdateVisuals();

        switch (playerController.GetFacingDirection())
        {
            case PlayerController.FacingDirection.left:
                bodyRenderer.flipX = true;
                break;
            case PlayerController.FacingDirection.right:
                bodyRenderer.flipX = false;
                break;
        }
    }

    private void UpdateVisuals()
    {
        if (playerController.previousState != playerController.currentState)
        {
            switch (playerController.currentState)
            {
                case PlayerState.idle:
                    animator.CrossFade(idleHash, 0);
                    break;
                case PlayerState.walking:
                    animator.CrossFade(walkingHash, 0);
                    break;
                case PlayerState.jumping:
                    animator.CrossFade(jumpingHash, 0);
                    break;
                case PlayerState.dead:
                    animator.CrossFade(deadHash, 0);
                    break;
            }
        }
    }
}
