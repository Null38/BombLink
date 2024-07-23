using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    new SpriteRenderer renderer;
    public Sprite normal;
    public Sprite action;
    public Sprite MoveW;
    public Sprite MoveH;

    private float startTime;
    private Vector3 startPos;
    private Vector3 endPos;

    private Quaternion startRot;
    private Quaternion endRot;

    public Game game;

    [SerializeField] AudioSource moveSound;
    [SerializeField] AudioSource rotateSound;

    private bool move = false;
    private bool isUp = false;

    public int x = 3;
    public int y = 3;

    // Use this for initialization
    void Start ()
    {
		renderer = this.gameObject.GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (move == false)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                PlayerMoveSet(Vector3.up);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                PlayerMoveSet(Vector3.right);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                PlayerMoveSet(Vector3.down);
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                PlayerMoveSet(Vector3.left);

            if (Input.GetKeyDown(KeyCode.A))
                BombRotationSet(-1);
            else if (Input.GetKeyDown(KeyCode.D))
                BombRotationSet(1);
        }

        if (y > 8)
        {
            transform.localPosition += Vector3.down;
            y--;
        }
    }
    public void GPlayerAutoUpSet()
    {
        if (y == 8)
            return;
        
        StartCoroutine(GPlayerAutoUp(0.75f));
    }

    private IEnumerator GPlayerAutoUp(float time)
    {
        float fracComplete = 0;
        bool addCheck = false;

        while (fracComplete < 1)
        {
            fracComplete = game.bombStopwatch / time;
            
            yield return new WaitUntil(() => !game.bombExp);

            if (!addCheck && fracComplete >= game.changeString)
            {
                y++;
                isUp = true;
                addCheck = true;
            }

            yield return null;
        }
        isUp = false;
        this.transform.localPosition += Vector3.up; 
        
        yield break;
    }



    public void ButtenDown(int keyType)
    {
        if (move == false)
        {
            switch (keyType)
            {
                case 0:
                    BombRotationSet(-1);
                    break;
                case 1:
                    PlayerMoveSet(Vector3.up);
                    break;
                case 2:
                    BombRotationSet(1);
                    break;
                case 3:
                    PlayerMoveSet(Vector3.left);
                    break;
                case 5:
                    PlayerMoveSet(Vector3.right);
                    break;
                case 6:
                    PlayerMoveSet(Vector3.down);
                    break;
            }
        }
    }

    public void PlayerMoveSet(Vector3 keydir)
    {
        move = true;
        startTime = Time.time;
        Vector3 pluse = new Vector3(0.1f * keydir.x, 0.1f * keydir.y, 0);

        if (keydir == Vector3.up)
        {
            if (transform.position.y > 7)
            {
                move = false;
                return;
            }
            renderer.sprite = MoveH;
        }
        else if (keydir == Vector3.right)
        {
            if (transform.position.x == 2)
            {
                keydir = new Vector3(-4, 0, 0);
                pluse = new Vector3(-0.1f, 0, 0);
            }
            renderer.sprite = MoveW;
        }
        else if (keydir == Vector3.down)
        {
            if (transform.position.y <= 2.75)
            {
                move = false;
                return;
            }
            renderer.sprite = MoveH;
        }
        else if (keydir == Vector3.left)
        {
            if (transform.position.x == -2)
            {
                keydir = new Vector3(4, 0, 0);
                pluse = new Vector3(0.1f, 0, 0);
            }
            renderer.sprite = MoveW;
        }
        moveSound.Play();
        StartCoroutine(PlayerMoveAnim(0.035f, keydir, pluse));
    }

    private IEnumerator PlayerMoveAnim(float time, Vector3 dir, Vector3 pluse)
    {
        startPos = transform.localPosition;
        endPos = transform.localPosition + dir;
        while (transform.localPosition != endPos + pluse)
        {
            float fracComplete = (Time.time - startTime) / time;
            transform.localPosition = Vector3.Lerp(startPos, endPos + pluse, fracComplete);
            yield return null;
        }

        if (renderer.sprite == MoveH)
            renderer.sprite = MoveW;
        else
            renderer.sprite = MoveH;
        yield return new WaitForSeconds(0.015f);
        x += (int)dir.x;
        y += (int)dir.y;
        transform.localPosition = new Vector3(x - 2, y - Convert.ToInt32(isUp), 0);
        move = false;
        renderer.sprite = normal;
        yield break;
    }

    private void BombRotationSet(int right)
    {
        move = true;
        renderer.sprite = action;
        startTime = Time.time;
        Quaternion angle = Quaternion.identity;
        Quaternion pluse = Quaternion.identity;

        if (right > 0)
            angle = Quaternion.Euler(new Vector3(0, 0, -95));
        else
            angle = Quaternion.Euler(new Vector3(0, 0, 95));

        if(game.bombsPos[y, x] != null)
        game.bombsPos[y, x].GetComponent<Bomb>().TrainOfPowderRotation(right);

        rotateSound.Play();
        StartCoroutine(BombRotation(0.075f, angle));

    }

    private IEnumerator BombRotation(float time, Quaternion angle)
    {
        startRot = Quaternion.identity;
        endRot = angle;
        while (this.gameObject.transform.rotation != endRot)
        {
            float fracComplete = (Time.time - startTime) / time;
            this.gameObject.transform.rotation = Quaternion.Lerp(startRot, endRot, fracComplete);
            yield return null;
        }

        yield return new WaitForSeconds(0.025f);
        renderer.sprite = normal;
        this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        move = false;
        yield break;
    }
}
