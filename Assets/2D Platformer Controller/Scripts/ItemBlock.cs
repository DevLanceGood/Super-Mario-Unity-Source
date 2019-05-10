using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBlock : MonoBehaviour
{
    public Player marioPlayer;
    public AudioClip coinSound;
    public string reward;
    public RuntimeAnimatorController itemAnimationController;
    public Sprite itemSprite;
    public int itemAmount;
    public Sprite itemUsedSprite;
    private int amountLeft;
    private bool allowedToRedeem = true;
    private void HitQuestionBlock()
    {
        //redeemed = true;
        if (reward == "coin")
        {
            GameObject item = new GameObject("Coin");
            item.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            item.transform.SetParent(GetComponent<Transform>());
            item.transform.position = new Vector2(transform.position.x, transform.position.y + 0.2f);
            item.AddComponent<SpriteRenderer>();
            item.AddComponent<Animator>();
            item.GetComponent<SpriteRenderer>().sprite = itemSprite;
            item.GetComponent<Animator>().runtimeAnimatorController = itemAnimationController;
            StartCoroutine(startItemAnimation(item));
            GetComponentInChildren<Animator>().SetBool("spinCoin", true);
            marioPlayer.emitPlayerSound(coinSound);
            marioPlayer.addScore(100);
            amountLeft--;
        }
        if (amountLeft <= 0)
        {
            DisableQuestionBlock();
        }
    }
    private void DisableQuestionBlock()
    {
        GetComponent<SpriteRenderer>().sprite = itemUsedSprite;
    }
    // Start is called before the first frame update
    void Start()
    {
        amountLeft = itemAmount;
    }

    // Update is called once per frame
    void Update()
    {
        if (allowedToRedeem)
        {
            foreach (BoxCollider2D box in GetComponents<BoxCollider2D>())
            {
                if (Physics2D.IsTouching(marioPlayer.GetComponent<BoxCollider2D>(), box) && box.isTrigger && amountLeft > 0)
                {
                    HitQuestionBlock();
                    StartCoroutine(startRedeemCooldown());
                }
                else if (Physics2D.IsTouching(marioPlayer.GetComponent<BoxCollider2D>(), box) && box.isTrigger && amountLeft <= 0)
                {
                    DisableQuestionBlock();
                }
            }
        }
    }
    IEnumerator startRedeemCooldown()
    {
        allowedToRedeem = false;
        yield return new WaitForSeconds(0.05f);
        allowedToRedeem = true;
    }
    IEnumerator startItemAnimation(GameObject item)
    {
        float maximum_asc = 0.1f;
        float current_asc = 0.0f;
        while (current_asc <= maximum_asc)
        {
            item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y + current_asc, item.transform.position.z);
            yield return new WaitForSeconds(0.01f);
            current_asc += 0.01f;
        }
        current_asc = 0.0f;
        while (current_asc <= maximum_asc)
        {
            item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y - current_asc, item.transform.position.z);
            yield return new WaitForSeconds(0.01f);
            current_asc += 0.01f;
        }
        Destroy(item);
    }
}
