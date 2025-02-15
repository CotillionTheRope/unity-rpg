using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    private Player player;

    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void AnimationTrigger()
    {
        player.AttackOver();
    }
}
