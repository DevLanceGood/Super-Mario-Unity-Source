using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    private Player player;
    public Animator anim;
    private bool isJumping;
    private bool isLeft = false;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {

    }
}
