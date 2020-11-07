using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;

public class DragDrop : NetworkBehaviour
{
    public GameManager GameManager;
    public GameObject Canvas;
    public PlayerManager PlayerManager;

    private bool isDragging = false;
    private bool isOverBattleZone = false;
    private bool isOverManaZone = false;
    private bool isDraggable = true;
    private GameObject battleZone;
    private GameObject manaZone;
    private GameObject startParent;
    private Vector2 startPosition;

    private void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Canvas = GameObject.Find("Main Canvas");
        NetworkIdentity netWorkIdentity = NetworkClient.connection.identity;
        PlayerManager = netWorkIdentity.GetComponent<PlayerManager>();

        if (!hasAuthority)
        {
            isDraggable = false;
        }
    }
    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(Canvas.transform, true);
        }        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //TODO ENTER BATTLEGROUND (Now it checks collision with shields)
        if (startParent == PlayerManager.PlayerArea)
        {
            //Modify 
            if (collision.gameObject == PlayerManager.PlayerBattleZone)
            {
                isOverBattleZone = true;
                battleZone = collision.gameObject;

            }

            if (collision.gameObject == PlayerManager.PlayerManaZone)
            {
                isOverManaZone = true;
                manaZone = collision.gameObject;

            }
        }
        //TODO ATTACK
        /*if (startParent == PlayerManager.PlayerBattleArea) 
        {
            if (collision.gameObject == PlayerManager.PlayerSockets[PlayerManager.CardsPlayed])
            {
                isOverBattleZone = true;
                battleZone = collision.gameObject;

            }
        }*/
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverBattleZone = false;
        isOverManaZone = false;
        battleZone = null;
        manaZone = null;
    }

    public void StartDrag()
    {
        if (!isDraggable) return;
        startParent = transform.parent.gameObject;
        startPosition = transform.position;
        isDragging = true;
    }

    public void EndDrag()
    {
        if (!isDraggable) return;
        isDragging = false;

        if (isOverBattleZone && PlayerManager.IsMyTurn)
        {
            transform.SetParent(battleZone.transform, false);
            isDraggable = false;
            PlayerManager.PlayCard(gameObject, "Battle");
        }
        else if (isOverManaZone && PlayerManager.IsMyTurn)
        {
            transform.SetParent(manaZone.transform, false);
            isDraggable = false;
            PlayerManager.PlayCard(gameObject, "Mana");
        } 
        else
        {
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
        }
    }
}
