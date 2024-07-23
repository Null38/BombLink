using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    public AudioSource expSound;
    public AudioSource bombUpSound;

    public AudioSource normalSong;
    public AudioSource dangerSong;

    [SerializeField] GameObject justBomb;
    [SerializeField] GameObject player;
    [SerializeField] GameObject fire;

    public Text levelText;
    int level = 0;
    int levelUpCheck = 0;

    public List<GameObject> bombs_List;

    public List<GameObject> dummy_List;

    [HideInInspector] public int combo = 0;
    public Text textCombo;
    [HideInInspector] public bool endCheck = false;
    [HideInInspector] public int countExpBomb;
    [HideInInspector] public int countExpBomb1;

    [HideInInspector] public int bigCount = 0;

    [HideInInspector] public int fallBomb;
    [HideInInspector] public bool dontDown = true;
    public List<GameObject> airList;
    private bool bombFallCorutine;
    public float changeString = 0.75f; 

    [SerializeField] float bombUpSpeed = 41;//41
    int totalBomb = 0;

    [SerializeField] Text textScore;
    [SerializeField] Text totalScore;
    private int score;
    [HideInInspector] public int bigScore;

    [SerializeField] Transform maskTransform;
    [SerializeField] Transform bombTransform;

    bool isbombUp = false;

    Vector3 startPos;
    Vector3 endPos;

    bool fireRight;
    [SerializeField] int fireCh;

    [HideInInspector] public bool bombExp;

    [HideInInspector] public float bombStopwatch = 0f;
    private float fireStopwatch = 0f;

    [SerializeField] int mode;
    [SerializeField] int looptime = 100;
    int difficulty;
    private int[,] difficultyRate = new int[3,5] {
        {1,4,1,4,50000},
        {1,3,1,3,100000},
        {1,2,1,2,200000}
    };

    public Bomb[,] bombsPos = new Bomb[12, 5]{
        {null,null,null,null,null},//0
        {null,null,null,null,null},//1 -----
        {null,null,null,null,null},//2
        {null,null,null,null,null},//3
        {null,null,null,null,null},//4
        {null,null,null,null,null},//5
        {null,null,null,null,null},//6
        {null,null,null,null,null},//7
        {null,null,null,null,null},//8 
        {null,null,null,null,null},//9 
        {null,null,null,null,null},//10
        {null,null,null,null,null}//11
    };

    [HideInInspector] public int firePos = 9;

    Coroutine loopCo;
    Coroutine endChCo;

    bool pauseChack;
    bool gameEnd = false;

    int countNumber = 0;
    [SerializeField] Text countText;

    [SerializeField] GameObject gameEndSheet;
    [SerializeField] Text highScoreText;
    [SerializeField] Text endScoreText;

    public Animator dangerAnim;

    private void Awake()
    {
        difficulty = PlayerPrefs.GetInt("diff");
        mode = PlayerPrefs.GetInt("mode");

        if (mode == 1)
        {
            countNumber = 100;
            countText.text = "100";
        }
        else
        {
            countText.text = "0";
        }

#if UNITY_ANDROID
        gameEndSheet.SetActive(false);
#endif
    }

    // Use this for initialization
    void Start()
    {
        PlayerPrefs.DeleteKey("diff");
        PlayerPrefs.DeleteKey("mode");

        for (int i = 1; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                GameObject tempObj;
                tempObj = Instantiate(justBomb, bombTransform, true);
                Bomb temBomb = tempObj.GetComponent<Bomb>();
                temBomb.bombPos = new Vector2Int(j,i);
                tempObj.transform.localPosition = new Vector3(j - 2, i, 0);
                tempObj.transform.localScale = Vector3.one;
                temBomb.game = gameObject.GetComponent<Game>();
                temBomb.trOPoRo = Random.Range(0, 4);
                temBomb.thisBombType = 0;
                bombsPos[i, j] = temBomb;

                bombs_List.Add(tempObj);
            }
        }
        NewBomb();

        loopCo = StartCoroutine(Loop());

        Invoke("AutoBombUp", bombUpSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isbombUp && (!bombFallCorutine && combo == 0))
        {
            isbombUp = true;
            BombUp();
        }
        
        if (!bombExp && !gameEnd)
        {
            fireStopwatch += Time.deltaTime;
            bombStopwatch += Time.deltaTime;
        }
    }

    public void ButtenBombUp()
    {
        if (!isbombUp && (!bombFallCorutine && combo == 0))
        {
            isbombUp = true;
            BombUp();
        }
    }

    void NewBomb()
    {
        int? squarePos0 = null;
        int? squarePos1 = null;
        int? emptyPos = null;

        if(Random.Range(0, difficultyRate[difficulty, 0]) >= Random.Range(0,difficultyRate[difficulty, 1]))
            squarePos0 = Random.Range(0,5);
        else
            squarePos0 = null;

        if (Random.Range(0, difficultyRate[difficulty, 2]) >= Random.Range(0, difficultyRate[difficulty, 3]))
        {
            if(difficulty > 1 && Random.Range(0,1) == 1)
                squarePos0 = Random.Range(0, 5);
            else
                emptyPos = Random.Range(0, 5);
        }
        else
        {
            squarePos1 = null;
            emptyPos = null;
        }

        for (int i = 0; i < 5; i++)
        {
            if (emptyPos != i)
            {
                GameObject tempObj;
                tempObj = Instantiate(justBomb, bombTransform, true);
                Bomb temBombManager = tempObj.GetComponent<Bomb>();
                temBombManager.bombPos = new Vector2Int(i, 0);
                tempObj.transform.localPosition = new Vector3(i - 2, 0, 0);
                tempObj.transform.localScale = Vector3.one;
                temBombManager.game = gameObject.GetComponent<Game>();
                temBombManager.trOPoRo = Random.Range(0, 4);
                if (squarePos0 == i || squarePos1 == i)
                    tempObj.GetComponent<Bomb>().thisBombType = 1;
                else
                    tempObj.GetComponent<Bomb>().thisBombType = 0;
                bombsPos[0, i] = tempObj.GetComponent<Bomb>();

                bombs_List.Add(tempObj);
            }
            else
                bombsPos[0, i] = null;
        }

        GmaeDangerChecker();
    }

    private GameObject FireCop;

    IEnumerator Loop()
    {
        yield return new WaitForSeconds(3f);
        fireCh = Random.Range(0, 4);
        while (true)
        {
            if (mode == 1)
            {
               looptime--;
                countText.text = looptime.ToString();
            }

            FireCop = Instantiate(fire, maskTransform, true);

            if (fireCh <= 1)
            {
                FireCop.transform.position = new Vector3(3, 9.5f, 0);
                fireRight = true;
            }
            else
            {
                FireCop.transform.position = new Vector3(-3, 9.5f, 0);
                fireRight = false;
            }

            yield return new WaitForSeconds(1f);

            startPos = FireCop.transform.position;
            endPos = FireCop.transform.position - (Vector3.up * 9);

            fireStopwatch = 0f;

            while (endPos != FireCop.transform.position)
            {
                float fracComplete = fireStopwatch / 3f;
                FireCop.transform.position = Vector3.Lerp(startPos, endPos, fracComplete);

                firePos = (int)Math.Ceiling(FireCop.transform.position.y - 0.5f);

                if (bombsPos[firePos, Convert.ToInt32(fireRight) * 4] != null)
                {
                    if (bombsPos[firePos, Convert.ToInt32(fireRight) * 4].trOPoRo == (Convert.ToInt32(fireRight) * -2) + 3 && firePos >= 2)
                    {
                        bombExp = true;
                        bombsPos[firePos, Convert.ToInt32(fireRight) * 4].Exposion(Convert.ToInt32(fireRight) * 6 - 1, firePos, 0,false);
                    }
                }
                
                yield return new WaitUntil(() => !bombExp);
                
                yield return null;
            }
            
            Destroy(FireCop);

            if (mode == 1 && looptime <= 0)
            {
                StartCoroutine(GameClear());
            }

            yield return new WaitForSeconds(3f);
            fireCh += (Random.Range(0,10) < 2 ? 1 : 0);

            if (fireCh >= 3)
                fireCh -= 3;
            else
                fireCh++;
        }
    }

    void AutoBombUp()
    {
        if (!isbombUp && (!bombFallCorutine && combo == 0))
        {
            isbombUp = true;
            BombUp();
        }
    }

    void BombUp()
    {
        bombStopwatch = 0f;

        bombUpSpeed -= 1.5f;
        if (bombUpSpeed < 0.5f)
            bombUpSpeed = 0.5f;

        bombUpSound.Play();
        StartCoroutine(MoveBombAnim(0.75f));

        player.GetComponent<Player>().GPlayerAutoUpSet();
    }

    public IEnumerator MoveBombAnim(float time)
    {
        bool addCheck = false;
        while (bombTransform.position != Vector3.up)
        {
            float fracComplete = bombStopwatch / time;
            bombTransform.position = Vector3.Lerp(Vector3.zero, Vector3.up, fracComplete);

            yield return new WaitUntil(() => !bombExp);

            if (!addCheck && fracComplete >= changeString)
            {
                addCheck = true;
                BombStringUp();
            }

            yield return null;
        }

        StartCoroutine(BombsFall());
        isbombUp = false;

        CancelInvoke("AutoBombUp");
        Invoke("AutoBombUp", bombUpSpeed);

        foreach (GameObject Object in bombs_List)
        {
            Object.transform.position += Vector3.up;
        }
        bombTransform.position = Vector3.zero;
        NewBomb();

        yield break;
    }

    void BombStringUp()
    {
        bombsPos = new Bomb[12, 5]{
        {null,null,null,null,null},//0
        {bombsPos[0,0],bombsPos[0,1],bombsPos[0,2],bombsPos[0,3],bombsPos[0,4]},//1 -----
        {bombsPos[1,0],bombsPos[1,1],bombsPos[1,2],bombsPos[1,3],bombsPos[1,4]},//2
        {bombsPos[2,0],bombsPos[2,1],bombsPos[2,2],bombsPos[2,3],bombsPos[2,4]},//3
        {bombsPos[3,0],bombsPos[3,1],bombsPos[3,2],bombsPos[3,3],bombsPos[3,4]},//4
        {bombsPos[4,0],bombsPos[4,1],bombsPos[4,2],bombsPos[4,3],bombsPos[4,4]},//5
        {bombsPos[5,0],bombsPos[5,1],bombsPos[5,2],bombsPos[5,3],bombsPos[5,4]},//6
        {bombsPos[6,0],bombsPos[6,1],bombsPos[6,2],bombsPos[6,3],bombsPos[6,4]},//7
        {bombsPos[7,0],bombsPos[7,1],bombsPos[7,2],bombsPos[7,3],bombsPos[7,4]},//8 
        {bombsPos[8,0],bombsPos[8,1],bombsPos[8,2],bombsPos[8,3],bombsPos[8,4]},//9 
        {bombsPos[9,0],bombsPos[9,1],bombsPos[9,2],bombsPos[9,3],bombsPos[9,4]},//10
        {bombsPos[10,0],bombsPos[10,1],bombsPos[10,2],bombsPos[10,3],bombsPos[10,4]}//11
    };

        foreach (GameObject Object in bombs_List)
        {
            Object.GetComponent<Bomb>().BombUp();
        }

        StartCoroutine(GameOver());
    }

    IEnumerator GameOver()
    {
        for (int i = 0; i < 5; i++)
        {
            if (bombsPos[9, i] != null)
                break;

            if (i == 4)
                yield break;
        }

        Debug.Log("GameOver");
        dangerAnim.SetInteger("dangerLv", 0);

        gameEnd = true;
        StopCoroutine(loopCo);

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (bombsPos[i, j] != null)
                    bombsPos[i, j].BombDown(true);
            }
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(2f);
        LoadGameEndSheet();

        yield break;
    }

    IEnumerator GameClear()
    {
        Debug.Log("GameClear");
        dangerAnim.SetInteger("dangerLv",0);

        StopCoroutine(loopCo);

        gameEnd = true;
        int?[] Floor = {null,null,null,null,null};
        int i = 0;
        int j = 9;
        while (Floor[4] == null)
        {
            if (bombsPos[j, i] != null || j == 1)
            {
                Floor[i] = j;
                i++;
                j = 10;
            }
            j--;
        }

        while (Floor[0] < 8 || Floor[1] < 8 || Floor[2] < 8 || Floor[3] < 8 || Floor[4] < 8)
        {
            for (i = 0; i < 5; i++)
            {
                if (Floor[i] < 8)
                {
                    GameObject tempObj;
                    tempObj = Instantiate(justBomb, bombTransform, true);
                    Bomb temBomb = tempObj.GetComponent<Bomb>();
                    temBomb.bombPos = new Vector2Int(i, 10);
                    tempObj.transform.localPosition = new Vector3(i - 2, 10, 0);
                    tempObj.transform.localScale = Vector3.one;
                    temBomb.game = gameObject.GetComponent<Game>();
                    temBomb.trOPoRo = Random.Range(0, 4);
                    temBomb.thisBombType = 0;
                    bombsPos[10, i] = temBomb;

                    bombs_List.Add(tempObj);

                    Floor[i] += 1;
                }
            }
            if (!bombFallCorutine)
            {
                StartCoroutine(BombsFall());
            }
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitUntil(() => !bombFallCorutine);
        yield return new WaitForSeconds(0.25f);

        for (i = 8; i > 0; i--)
        {
            for (j = 0; j < 5; j++)
            {
                if (bombsPos[i,j] != null)
                    bombsPos[i,j].Exposion(0,0,0,true);
                expSound.Play();
            }
            yield return new WaitForSeconds(0.1f);
        }

        LoadGameEndSheet();
        yield break;
    }

    void LoadGameEndSheet()
    {
        gameEndSheet.SetActive(true);

        if (mode == 0)
        {
            if (!PlayerPrefs.HasKey("HighScore"))
                PlayerPrefs.SetInt("HighScore", score);
            
            if(score >= PlayerPrefs.GetInt("HighScore"))
            {
                PlayerPrefs.SetInt("HighScore", score);
            }
            Social.ReportScore(PlayerPrefs.GetInt("HighScore"), GPGSIds.leaderboard_endless_high_score, (bool success) => { });

            highScoreText.text = "\nHighScore : " + PlayerPrefs.GetInt("HighScore").ToString();
        }
        else
        {
            if (!PlayerPrefs.HasKey("100AttackHighScore"))
                PlayerPrefs.SetInt("100AttackHighScore", score);

            if (score >= PlayerPrefs.GetInt("100AttackHighScore"))
            {
                PlayerPrefs.SetInt("100AttackHighScore", score);
            }
            Social.ReportScore(PlayerPrefs.GetInt("100AttackHighScore"), GPGSIds.leaderboard_100_attack_high_score, (bool success) => { });

            highScoreText.text = "\nHighScore : " + PlayerPrefs.GetInt("100AttackHighScore").ToString();
        }
        endScoreText.text = "\nScore : " + score.ToString();
    }

    public void StartendChecker()
    {
        if (endChCo != null)
            StopCoroutine(endChCo);
        endChCo = StartCoroutine(EndChecker());
    }

    IEnumerator EndChecker()
    {
        int save = combo;

        yield return new WaitUntil(() => countExpBomb1 != countExpBomb);

        if (save == combo)
        {
            dontDown = true;
            if (level < 99)
                levelUpCheck += dummy_List.Count;

            StartCoroutine(BombsFall());

            textCombo.text = "";
            score += (int)(100 * Math.Pow(Math.Max(0, (combo - 2)), 2)) + (100 * dummy_List.Count) ;
            score += bigScore;
            if(bigScore == 0)
                textScore.text = "\n" + ((100 * Math.Pow(Math.Max(0, (combo - 2)), 2)) + (100 * dummy_List.Count)).ToString();
            else
                textScore.text = "\n" + bigScore.ToString() +  "\n" + ((100 * Math.Pow(Math.Max(0, (combo - 2)), 2)) + (100 * dummy_List.Count)).ToString();
            totalScore.text = $"{score,0:D8}";
            if (mode == 0)
            {
                countNumber += dummy_List.Count;
                countText.text = countNumber.ToString();
            }

            Vector3 textStartPos = new Vector3(0,0.75f,0);
            Vector3 textEndPos = new Vector3(0,2.75f,0);
            float StartTime = Time.time;
            textScore.rectTransform.localPosition = textStartPos;

            while (textEndPos != textScore.rectTransform.localPosition)
            {
                float fracComplete = (Time.time - StartTime) / 0.5f;
                textScore.rectTransform.localPosition = Vector3.Lerp(textStartPos, textEndPos, fracComplete);
                
                yield return null;
            }

            textScore.text = "";

            if (levelUpCheck >= 16 && level < 99)
            {
                levelUpCheck = 0;
                level++;
                levelText.text = "\n" + "Lv." + level;
                score += difficultyRate[difficulty, 4];

                textScore.text = "\n" + "Level Up\n" + difficultyRate[difficulty,4];
                totalScore.text = $"{score,0:D8}";

                StartTime = Time.time;
                textScore.rectTransform.localPosition = textStartPos;

                while (textEndPos != textScore.rectTransform.localPosition)
                {
                    float fracComplete = (Time.time - StartTime) / 0.5f;
                    textScore.rectTransform.localPosition = Vector3.Lerp(textStartPos, textEndPos, fracComplete);

                    yield return null;
                }
                textScore.text = "";
            }

            yield return new WaitUntil(() => !bombFallCorutine);

            if (bigCount != 0)
                BigBomb();

            yield return new WaitUntil(() => bigCount == 0);

            StartCoroutine(BombsFall());
            yield return new WaitUntil(() => !bombFallCorutine);

            ScoreCalculate();
        }

        endCheck = false;
    }

    IEnumerator BombsFall()
    {
        bombFallCorutine = true;
        dontDown = true;
        while (dontDown)
        {
            dontDown = false;
            foreach (GameObject bomb in bombs_List)
            {
                bomb.GetComponent<Bomb>().AirCheck();
            }
            yield return new WaitUntil(() => bombs_List.Count == fallBomb);

            fallBomb = 0;
            foreach (GameObject bomb in airList)
            {
                bomb.GetComponent<Bomb>().BombDown(false);
            }
            yield return new WaitUntil(() => airList.Count == fallBomb);
            
            airList.Clear();
            foreach (GameObject bomb in bombs_List)
            {
                bomb.GetComponent<Bomb>().TrainOfPowderSet();
            }

            fallBomb = 0;
        }
        bombFallCorutine = false;
    }

    private void BigBomb()
    {
        int y = 9;

        List<int> bombCount = new List<int>();

        List<int> bigPos = new List<int>();

        for (int i = 0; i < 5 - bigCount; i++)
        {
            for (int j = 0; j <= bigCount; j++)
            {
                if (bombsPos[8, i + j] != null)
                    break;
                if (j == bigCount)
                    bigPos.Add(i);
            }
        }

        int randPos = bigPos[Random.Range(0, bigPos.Count)];

        while (bombCount.Count == 0)
        {
            y--;
            for (int i = randPos; i <= randPos + bigCount; i++)
            {
                if (bombsPos[y, i] != null)
                    bombCount.Add(i);
            }
        }

        int bigBombPos;

        if (bombCount.Count != 0)
        {
            bigBombPos = bombCount[Random.Range(0, bombCount.Count)];
        }
        else
        {
            bigBombPos = Random.Range(0, 5 - bigCount);
        }
        List<GameObject> bigBombs = new List<GameObject>();

        for (int i = randPos; i <= randPos + bigCount; i++)
        {
            GameObject tempObj;
            tempObj = Instantiate(justBomb, bombTransform, true);
            Bomb temBomb = tempObj.GetComponent<Bomb>();
            tempObj.transform.localPosition = new Vector3(i - 2, 10, 0);
            tempObj.transform.localScale = Vector3.one;
            temBomb.game = gameObject.GetComponent<Game>();
            bigBombs.Add(tempObj);
            tempObj.GetComponent<Bomb>().thisBombType = 3;
            if (bombStopwatch / 0.75 >= changeString && isbombUp)
            {
                temBomb.bombPos = new Vector2Int(i, 11);
                bombsPos[11, i] = temBomb;
            }
            else
            {
                temBomb.bombPos = new Vector2Int(i, 10);
                bombsPos[10, i] = temBomb;
            }

            if (i == randPos)
                temBomb.thisBombType = 2;
            else if(i == randPos + bigCount)
                temBomb.thisBombType = 4;

            temBomb.trainOfPowder.SetActive(false);

            if (i < bigBombPos)
            {
                temBomb.trOPoRo = 1;
            }
            else if (i == bigBombPos)
            {
                if (bombStopwatch / 0.75 >= changeString && isbombUp)
                {
                    if (y % 2 == 0)
                        temBomb.trOPoRo = 2;
                    else
                        temBomb.trOPoRo = 0;
                }
                else
                {
                    if (y % 2 == 0)
                        temBomb.trOPoRo = 0;
                    else
                        temBomb.trOPoRo = 2;
                }

                temBomb.trainOfPowder.SetActive(true);
            }
            else
            {
                temBomb.trOPoRo = 3;
            }
            
            bombs_List.Add(tempObj);
        }

        for (int i = 0; i < bigBombs.Count; i++)
        {
            bigBombs[i].GetComponent<Bomb>().ifBigBomb = new List<GameObject>(bigBombs);
        }
        bigCount = 0;
    }
    
    private void ScoreCalculate()
    {
        totalBomb += dummy_List.Count;
        countExpBomb1 = 0;
        countExpBomb = 0;
        combo = 0;
        bigScore = 0;
        if (bombUpSpeed < dummy_List.Count)
            bombUpSpeed = dummy_List.Count;

        foreach (GameObject dumm in dummy_List)
        {
            Destroy(dumm);
        }

        dummy_List.Clear();

        bombExp = false;

        GmaeDangerChecker();

        CancelInvoke("AutoBombUp");
        Invoke("AutoBombUp", bombUpSpeed);
    }


    bool Danger = false;
    void GmaeDangerChecker()
    {
        bool DangerChack = false;
        for (int i = 0; i < 5; i++)
        {
            if (bombsPos[7, i] != null)
            {
                DangerChack = true;
                break;
            }
        }

        if (Danger != DangerChack)
        {
            Danger = DangerChack;
            dangerAnim.SetInteger("dangerLv", Danger ? 1 : 0);

            StopCoroutine(ChangeBackgroundsong());
            StartCoroutine(ChangeBackgroundsong());
        }

        if (!Danger) return;

        for (int i = 0; i < 5; i++)
        {
            if (bombsPos[8, i] != null)
            {
                dangerAnim.SetInteger("dangerLv", 2);

                return;
            }
        }
        dangerAnim.SetInteger("dangerLv", 1);
    }

    float songChange = 0f;
    const float songChangeTime = 0.75f;
    IEnumerator ChangeBackgroundsong()
    {
        float saveTime = Time.time;
        while ((Danger == true && songChange < 1) || (Danger == false && songChange > 0))
        {
            songChange += (Time.time - saveTime) * (Danger ? 1 : -1) / songChangeTime;
            if (songChange < 0f || songChange > 1f) songChange = songChange < 0f ? 0 : 1;

            normalSong.volume = 1 - songChange;
            dangerSong.volume = songChange;

            saveTime = Time.time;
            yield return 0;
        }
    }
}