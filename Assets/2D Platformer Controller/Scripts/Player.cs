using System.Collections;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    private int score = 0;
    public float maxJumpHeight = 4f;
    public float minJumpHeight = 1f;
    public float timeToJumpApex = .4f;
    private float accelerationTimeAirborne = .2f;
    private float accelerationTimeGrounded = .1f;
    private float moveSpeed = 6f;

    public bool canDoubleJump;
    private bool isDoubleJumping = false;
    private bool isJumping = false;

    private float gravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private Vector3 velocity;
    private float velocityXSmoothing;
    private int killStreak = 0;
    private Controller2D controller;

    private Vector2 directionalInput;
    private int wallDirX;

    public Animator anim;
    public AudioClip jumpSound;
    public Text label_text;
    public Text score_text;

    private bool isDead = false;

    private string getScorePrefix()
    {
        string score_prefix = "";
        if (score <= 9)
        {
            score_prefix = "00000";
        }
        else if (score <= 99)
        {
            score_prefix = "0000";
        }
        else if (score <= 999)
        {
            score_prefix = "000";
        }
        else if (score <= 9999)
        {
            score_prefix = "00";
        }
        else if (score <= 99999)
        {
            score_prefix = "0";
        }
        return score_prefix;
    }
    private void Start()
    {
        controller = GetComponent<Controller2D>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    private void Update()
    {
        if (!isDead)
        {
            Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            SetDirectionalInput(directionalInput);
            anim.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
            if (Input.GetAxis("Horizontal") >= 0.001)
            {
                anim.SetBool("isLeft", false);
            }
            else if (Input.GetAxis("Horizontal") <= -0.001)
            {
                anim.SetBool("isLeft", true);
            }
            if (Input.GetButtonDown("Jump"))
            {
                OnJumpInputDown();
            }

            if (Input.GetButtonUp("Jump"))
            {
                OnJumpInputUp();
            }
            Debug.Log(controller.collisions.below);

            string score_prefix = getScorePrefix();
            score_text.text = score_prefix + score.ToString();
            CalculateVelocity();
            controller.Move(velocity * Time.deltaTime, directionalInput);
            if (controller.collisions.below)
            {
                isJumping = false;
                killStreak = 0;
                anim.SetBool("isJumping", false);
            }
            else
            {
                isJumping = true;
                anim.SetBool("isJumping", true);
            }
            if (controller.collisions.above || controller.collisions.below)
            {
                velocity.y = 0f;
            }
        }
    }

    public void emitPlayerSound(AudioClip sound)
    {
        AudioSource soundPlayer = gameObject.AddComponent<AudioSource>();
        soundPlayer.clip = sound;
        soundPlayer.Play();
        StartCoroutine(deletePlayerSound(soundPlayer));
    }

    IEnumerator deletePlayerSound(AudioSource soundPlayer)
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            if (!soundPlayer.isPlaying)
            {
                Debug.Log("Deleted.");
                Destroy(soundPlayer);
                break;
            }
        }
    }

    IEnumerator playDeathAnimation(bool direction)
    {
        isDead = true;
        anim.SetBool("isDead", true);
        GameObject.Find("Main Camera").GetComponent<CameraFollow>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        if (direction)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        float maximum_asc = 0.1f;
        float current_asc = 0.0f;
        yield return new WaitForSeconds(1.0f);
        while (current_asc <= maximum_asc * 2)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + current_asc, transform.position.z);
            yield return new WaitForSeconds(0.01f);
            current_asc += 0.01f;
        }
        current_asc = 0.0f;
        while (current_asc <= maximum_asc * 4)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - current_asc, transform.position.z);
            yield return new WaitForSeconds(0.01f);
            current_asc += 0.01f;
        }
    }

    public void CheckDeath(bool direction)
    {
        StartCoroutine(playDeathAnimation(direction));
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (controller.collisions.below)
        {
            emitPlayerSound(jumpSound);
            velocity.y = maxJumpVelocity;
            isDoubleJumping = false;
        }
        if (canDoubleJump && !controller.collisions.below && !isDoubleJumping)
        {
            velocity.y = maxJumpVelocity;
            isDoubleJumping = true;
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    private void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
        velocity.y += gravity * Time.deltaTime;
    }

    public void addScore(int incr)
    {
        score += incr;
    }
    public void addScore(int incr, bool streak)
    {
        if (streak)
        {
            if (killStreak == 0)
            {
                killStreak++;
                score += incr;
            }
            else
            {
                killStreak++;
                score += incr * killStreak;
            }
        }
    }
    public void bouncePlayer()
    {
        velocity.y = maxJumpVelocity / 2;
    }
}
