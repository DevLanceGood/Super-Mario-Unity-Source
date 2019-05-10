using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyNPC : MonoBehaviour
{
    public Player marioPlayer;
    public bool leftStartDirection = true;
    private bool isLeft = false;
    private bool isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        isLeft = leftStartDirection;
        if (isLeft)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            CheckOnTouch();
            if (isLeft)
            {
                transform.position = new Vector2(transform.position.x - 0.01f, transform.position.y);
            }
            else if (!isLeft)
            {
                transform.position = new Vector2(transform.position.x + 0.01f, transform.position.y);
            }
        }
    }

    private void CheckOnTouch()
    {
        BoxCollider2D boxOne = GetComponents<BoxCollider2D>()[0];
        BoxCollider2D boxTwo = GetComponents<BoxCollider2D>()[1];
        if (Physics2D.IsTouching(marioPlayer.GetComponent<BoxCollider2D>(), boxOne) && Physics2D.IsTouching(marioPlayer.GetComponent<BoxCollider2D>(), boxTwo))
        {
            StartCoroutine(startDeathProtocol());
        }
        else
        {
            foreach (BoxCollider2D bc2d in GetComponents<BoxCollider2D>())
            {
                if (Physics2D.IsTouching(marioPlayer.GetComponent<BoxCollider2D>(), bc2d) && bc2d.isTrigger)
                {
                    transform.position = new Vector2(transform.position.x + 1.01f, transform.position.y);
                    marioPlayer.CheckDeath(isLeft);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        isLeft = !isLeft;
        if (isLeft)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    IEnumerator startDeathProtocol()
    {
        isDead = true;
        marioPlayer.addScore(100, true);
        marioPlayer.bouncePlayer();
        GetComponent<Animator>().SetBool("isDead", true);
        transform.position = new Vector2(transform.position.x, transform.position.y - 0.125f);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
