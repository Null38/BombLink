using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bomb : MonoBehaviour
{
    public int thisBombType;
    public Vector2Int bombPos;

    [HideInInspector] public Game game;

    [SerializeField] Sprite[] bombSprite;

    [SerializeField] Sprite[] trOPo;

    public List<GameObject> ifBigBomb;
    public bool isBombFall = false;

    public int trOPoRo = 0;

    public GameObject trainOfPowder;
    private SpriteRenderer trainOfPowderSpR;

    [SerializeField] GameObject bigExp;
    [SerializeField] GameObject normalExp;
    
    private int saveCombo = 0;

    // Use this for initialization
    void Start()
    {
        if (thisBombType >= 2)
        {
            ifBigBomb.Remove(this.gameObject);
        }

        this.gameObject.GetComponent<SpriteRenderer>().sprite = bombSprite[thisBombType];
        trainOfPowderSpR = trainOfPowder.GetComponent<SpriteRenderer>();

        TrainOfPowderSet();
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void BombUp()
    {
        bombPos.y++;
        TrainOfPowderSet();
    }

    public void TrainOfPowderSet()
    {
        if (bombPos.y > 1 && trainOfPowder.activeSelf)//심지 설정
        {
            trainOfPowderSpR.sprite = trOPo[trOPoRo];

            Vector2 check = Vector2.zero;

            if (trOPoRo == 0)
                check = Vector2.up;
            else if (trOPoRo == 1)
                check = Vector2.right;
            else if (trOPoRo == 2)
                check = Vector2.down;
            else if (trOPoRo == 3)
                check = Vector2.left;

            if ((trOPoRo == 0 && bombPos.y < 9) || (trOPoRo == 1 && bombPos.x != 4) || (trOPoRo == 2 && bombPos.y != 2) || (trOPoRo == 3 && bombPos.x != 0))
            {
                if (game.bombsPos[bombPos.y + (int)check.y, bombPos.x + (int)check.x] != null)
                    if (game.bombsPos[bombPos.y + (int)check.y, bombPos.x + (int)check.x].trOPoRo == trOPoRo - 2 || game.bombsPos[bombPos.y + (int)check.y, bombPos.x + (int)check.x].trOPoRo == trOPoRo + 2)
                    {
                        trainOfPowderSpR.sprite = trOPo[trOPoRo + 8];
                    }
            }
        }
    }

    public void TrainOfPowderRotation(int right)
    {
        if (thisBombType != 0 || !this.gameObject.activeSelf)
            return;

        StartCoroutine(TraRot(right));
    }

    IEnumerator TraRot(int right)
    {
        Vector2 check = Vector2.zero;

        if (trOPoRo == 0)
            check = Vector2.up;
        else if (trOPoRo == 1)
            check = Vector2.right;
        else if (trOPoRo == 2)
            check = Vector2.down;
        else if (trOPoRo == 3)
            check = Vector2.left;

        trOPoRo += right;

        if (trOPoRo < 0 || trOPoRo > 3)
        {
            trOPoRo -= right * 4;
        }

        if (bombPos.y + check.y <= 9 && bombPos.x + check.x <= 4 && bombPos.y + check.y >= 2 && bombPos.x + check.x >= 0)
        {
            if (game.bombsPos[bombPos.y + (int)check.y, bombPos.x + (int)check.x] != null)
                game.bombsPos[bombPos.y + (int)check.y, bombPos.x + (int)check.x].TrainOfPowderSet();
        }

        yield return new WaitForSeconds(0.03f);

        if (right < 0)
            trainOfPowderSpR.sprite = trOPo[trOPoRo + 4];
        else
        {
            if (trOPoRo == 0)
                trainOfPowderSpR.sprite = trOPo[trOPoRo + 7];
            else
                trainOfPowderSpR.sprite = trOPo[trOPoRo + 3];
        }

        yield return new WaitForSeconds(0.04f);

        TrainOfPowderSet();

        if (trOPoRo == 0)
            check = Vector2.up;
        else if (trOPoRo == 1)
            check = Vector2.right;
        else if (trOPoRo == 2)
            check = Vector2.down;
        else if (trOPoRo == 3)
            check = Vector2.left;

        if ((trOPoRo == 0 && bombPos.y != 9) || (trOPoRo == 1 && bombPos.x != 4) || (trOPoRo == 2 && bombPos.y != 2) || (trOPoRo == 3 && bombPos.x != 0))
        {
            if (game.bombsPos[bombPos.y + (int)check.y, bombPos.x + (int)check.x] != null)
                game.bombsPos[bombPos.y + (int)check.y, bombPos.x + (int)check.x].TrainOfPowderSet();
        }

        yield break;
    }

    private GameObject exp;

    public void Exposion(int ExX, int ExY, int combo, bool isClear)
    {
        if (!isClear)
        {
            Vector2 check = Vector2.zero;

            if (trOPoRo == 0)
                check = new Vector2(0, 1);
            else if (trOPoRo == 1)
                check = new Vector2(1, 0);
            else if (trOPoRo == 2)
                check = new Vector2(0, -1);
            else if (trOPoRo == 3)
                check = new Vector2(-1, 0);

            if (bombPos.y < 2 || (ExX != bombPos.x + check.x || ExY != bombPos.y + check.y))
                return;

            saveCombo = combo;


            if (game.combo == combo)
            {
                game.combo++;
                game.expSound.Play();
                if (game.combo > 1)
                    game.textCombo.text = "\n" + game.combo + " Combo!";
            }
            game.countExpBomb++;

        }
        game.bombs_List.Remove(this.gameObject);
        if ((game.dummy_List.Count == 8 || game.dummy_List.Count == 17 || game.dummy_List.Count == 26) && !isClear)
        {
            exp = Instantiate(bigExp);
            game.bigCount++;
        }
        else
        {
            exp = Instantiate(normalExp);
        }
        exp.transform.position = this.gameObject.transform.position;
        game.dummy_List.Add(exp);
        
        if (!isClear)
        {
            this.gameObject.SetActive(false);

            if (ifBigBomb.Count == 1)
                game.bigScore += 10000;
            else if (ifBigBomb.Count == 2)
                game.bigScore += 30000;
            else if (ifBigBomb.Count == 3)
                game.bigScore += 50000;

            Invoke("CheckBombs", 0.4f);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void CheckBombs()
    {
        Vector2 check = Vector2.zero;

        if (!game.endCheck)
        {
            game.endCheck = true;
            game.StartendChecker();
        }

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
                check = new Vector2(0, 1);
            else if (i == 1)
                check = new Vector2(1, 0);
            else if (i == 2)
                check = new Vector2(0, -1);
            else if (i == 3)
                check = new Vector2(-1, 0);

            if (bombPos.x + check.x >= 0 && bombPos.x + check.x <= 4 && bombPos.y + check.y >= 2 && bombPos.y + check.y <= 9)
            {
                if (game.bombsPos[bombPos.y + (int)check.y, bombPos.x + (int)check.x] != null)
                    game.bombsPos[bombPos.y + (int)check.y, bombPos.x + (int)check.x].Exposion(bombPos.x, bombPos.y, saveCombo + 1,false);
            }
        }

        game.countExpBomb1++;
        Destroy(this.gameObject);
    }

    public void AirCheck()
    {
        if (bombPos.y <= 2 || !trainOfPowder.activeSelf)
        {
            game.fallBomb++;
            return;
        }

        if (thisBombType <= 1)
        {
            if (game.bombsPos[bombPos.y - 1, bombPos.x] != null)
            {
                game.fallBomb++;
                return;
            }
        }
        else
        {
            isBombFall = false;
            for (int i = 0; i < ifBigBomb.Count; i++)
            {
                ifBigBomb[i].GetComponent<Bomb>().isBombFall = false;
            }

            if (game.bombsPos[bombPos.y - 1, bombPos.x] != null)
            {
                game.fallBomb++;
                return;
            }

            for (int i = 0; i < ifBigBomb.Count; i++)
            {
                if (game.bombsPos[bombPos.y - 1, (int)ifBigBomb[i].transform.position.x + 2] != null)
                {
                    game.fallBomb++;
                    return;
                }
                else
                    ifBigBomb[i].GetComponent<Bomb>().isBombFall = true;
            }
        }
        
        game.airList.Add(this.gameObject);
        game.fallBomb++;
        game.dontDown = true;
    }

    public void BombDown(bool isEnd)
    {
        if (!isEnd)
        {
            if (thisBombType >= 2 && !isBombFall)
            {
                isBombFall = true;
                for (int i = 0; i < ifBigBomb.Count; i++)
                {
                    if (!ifBigBomb[i].GetComponent<Bomb>().isBombFall)
                    {
                        if (game.bombsPos[bombPos.y - 1, ifBigBomb[i].GetComponent<Bomb>().bombPos.x])
                        {
                            game.fallBomb++;
                            return;
                        }
                        else
                            ifBigBomb[i].GetComponent<Bomb>().isBombFall = true;
                    }
                }

                for (int i = 0; i < ifBigBomb.Count; i++)
                {
                    ifBigBomb[i].GetComponent<Bomb>().BombDown(false);
                    if (game.bombsPos[bombPos.y - 1, ifBigBomb[i].GetComponent<Bomb>().bombPos.x] == null && !game.airList.Contains(ifBigBomb[i]))
                    {
                        game.fallBomb--;
                    }
                }
            }
            isBombFall = false;
            if (bombPos.y < 11) if (game.bombsPos[bombPos.y + 1, bombPos.x] != null)
                {
                    game.bombsPos[bombPos.y + 1, bombPos.x].BombDown(false);
                    game.dontDown = true;
                    game.fallBomb--;
                }
        }

        StartCoroutine(MoveBombDAnim(0.25f, isEnd));
    }

    private IEnumerator MoveBombDAnim(float fallTime, bool isEnd)
    {
        game.bombsPos[bombPos.y, bombPos.x] = null;
        Vector3 dStartPos = transform.position;
        Vector3 dEndPos = transform.position + Vector3.down;
        float StartTime = Time.time;
        float fracComplete;


        while (transform.position != dEndPos)
        {
            fracComplete = (Time.time - StartTime) / fallTime;
            transform.position = Vector3.Lerp(dStartPos, dEndPos, fracComplete);

            if (trOPoRo == 0)
            {
                if ((Time.time - StartTime) / fallTime >= 0.8f)
                {
                    trainOfPowderSpR.sprite = trOPo[2];
                }
                else if ((Time.time - StartTime) / fallTime > 0.2f)
                {
                    trainOfPowderSpR.sprite = trOPo[12];
                }
            }
            if (trOPoRo == 2)
            {
                if ((Time.time - StartTime) / fallTime >= 0.8f)
                {
                    trainOfPowderSpR.sprite = trOPo[0];
                }
                else if ((Time.time - StartTime) / fallTime > 0.2f)
                {
                    trainOfPowderSpR.sprite = null;
                }
            }

            yield return null;
        }
        if (trOPoRo == 0)
            trOPoRo = 2;
        else if (trOPoRo == 2)
            trOPoRo = 0;
        bombPos.y--;
        game.bombsPos[bombPos.y, bombPos.x] = this.gameObject.GetComponent<Bomb>();
        game.fallBomb++;

        if (isEnd)
        {
            if (bombPos.y <= 0)
                Destroy(this.gameObject);
            BombDown(true);
        }
        yield break;
    }
}
