using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum MoveDirection
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 3,
    Right = 4
}

public enum GameState
{
    NoStart = 0,//开始
    Gameing = 1,//正在游戏
    Winend = 2,//赢了游戏
    Loseend = -1//输了游戏
}

public class GameManager : MonoBehaviour
{
    public GameObject numberPrefab;//数字的prefab
    public Texture musicOn;//声音开启图片
    public Texture musicOff;//声音关闭图片



    private const int _lie = 4, _hang = 4;//确定行列的个数
    private const int _maxNumerb = 4096;//确定行列的个数
    private const int startNumber = 4;//初始游戏的图片个数
    private const string musicString = "music";//XML music 
    private const string highScoreString = "highScore";//XML music 


    private static GameManager _instance;
    private static GameState _gameState = GameState.NoStart;//游戏状态
    private GameObject welcomeBg;//欢迎界面
    private GameObject gameBg;//游戏界面
    private Number[,] numbers = new Number[Lie, Hang];//保存方格中数字的情况
    private int _moveingCount = 0;//正在移动的方格数量
    private bool _hasMove = false;//是否正在移动过
    private int _numberCount = 0;//数字的总数
    private int _maxNumberCount = 0;//数字最多的数量
    private bool isPlayMusic;//是否播放背景音乐
    private AudioSource _as;//背景音乐
    private TweenAlpha tweenAlpha;//Alpha动画
    private bool isTweenAlphaShow = true;//Alpha动画是否Show
    private UITexture musicTexture;//音乐图片
    private UIButton reStartBtn;//重新开始按钮
    private Transform winGameBg;//刷新记录Bg
    private Transform loseGameBg;//死亡Bg
    private UILabel winHigScoreText;//新纪录Label
    private UILabel nowScoreText;//当前Label
    private UILabel loseHighScoreText;//纪录Label
    private bool check = false;//是否检查了
    private int _endScore = 0;//最后分数

    

    public static GameManager Instance { get { return _instance; } }
    public Number[,] Numbers { get { return numbers; } }
    public static int Lie { get { return _lie; } }
    public static int Hang { get { return _hang; } }
    public static int MaxNumber { get { return _maxNumerb; } }
    public int MoveingCount { get { return _moveingCount; } set { _moveingCount = value; } }
    public bool HasMove { get { return _hasMove; } set { _hasMove = value; } }
    public int NumberCount { get { return _numberCount; } set { _numberCount = value; } }
    public int MaxNumberCount { get { return _maxNumberCount; } }
    public GameState GAMESTATE { get { return _gameState; } set { _gameState = value; } }
    public int EndScore { get { return _endScore; } set { _endScore = value; } }
    void Awake()
    {
        _instance = this;
        Init();
    }

    void Update()
    {
        if (GAMESTATE == GameState.Gameing)
        {
            InputMove();
        }

    }

    void InputMove()
    {
        if (MoveingCount != 0)
        {
            return;
        }
        if (HasMove)
        {
            CreatePrefab();
            HasMove = false;
            check = false;
        }
        if (!check)
        {
            if (CheckGameEnd())
            {
                EndGame();
            }
        }
        MoveDirection md = MoveDirection.None;
        if (Input.GetKeyDown("up"))
        {
            md = MoveDirection.Up;
        }
        else if (Input.GetKeyDown("down"))
        {
            md = MoveDirection.Down;
        }
        else if (Input.GetKeyDown("left"))
        {
            md = MoveDirection.Left;
        }
        else if (Input.GetKeyDown("right"))
        {
            md = MoveDirection.Right;
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 moveVec2 = Input.GetTouch(0).deltaPosition;
            if (Mathf.Abs(moveVec2.y) > Mathf.Abs(moveVec2.x))
            {
                if (moveVec2.y > 10)
                {
                    md = MoveDirection.Up;
                }
                else if (moveVec2.y < -10)
                {
                    md = MoveDirection.Down;
                }
            }
            else
            {
                if (moveVec2.x > 10)
                {
                    md = MoveDirection.Right;
                }
                else if (moveVec2.x < -10)
                {
                    md = MoveDirection.Left;
                }

            }
        }


        if (md != MoveDirection.None)
        {
            if (md == MoveDirection.Up || md == MoveDirection.Right)
            {
                for (int i = Hang - 1; i >= 0; i--)
                {
                    for (int j = Lie - 1; j >= 0; j--)
                    {
                        if (Numbers[j, i] != null)
                        {
                            Numbers[j, i].Move(md);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Hang; i++)
                {
                    for (int j = 0; j < Lie; j++)
                    {
                        if (Numbers[j, i] != null)
                        {
                            Numbers[j, i].Move(md);
                        }
                    }
                }
            }
        }
    }



    void Init()
    {
        _maxNumberCount = Hang * Lie;
        _as = transform.GetComponent<AudioSource>();
        welcomeBg = transform.Find("WelcomeBg").gameObject;
        gameBg = transform.Find("GameBg").gameObject;
        tweenAlpha = welcomeBg.transform.GetChild(0).GetComponent<TweenAlpha>();
        musicTexture = gameBg.transform.Find("MusicTexture").GetComponent<UITexture>();
        reStartBtn = gameBg.transform.Find("ReStart").GetComponent<UIButton>();
        winGameBg = gameBg.transform.Find("WinGameBg");
        loseGameBg = gameBg.transform.Find("LoseGameBg");
        winHigScoreText = winGameBg.Find("HighScoreLabelBg/HigScoreText").GetComponent<UILabel>();
        nowScoreText = loseGameBg.Find("NowScoreLabelBg/NowScoreText").GetComponent<UILabel>();
        loseHighScoreText = loseGameBg.Find("HighScoreLabelBg/HighScoreText").GetComponent<UILabel>();

        EventDelegate ed1 = new EventDelegate(this, "StartGame");
        welcomeBg.GetComponent<UIButton>().onClick.Add(ed1);
        EventDelegate ed2 = new EventDelegate(this, "PlayMusic");
        musicTexture.GetComponent<UIButton>().onClick.Add(ed2);
        EventDelegate ed3 = new EventDelegate(this, "ReStartGame");
        reStartBtn.onClick.Add(ed3);
        EventDelegate ed4 = new EventDelegate(this, "PlayTweenAlpha");
        tweenAlpha.AddOnFinished(ed4);
        tweenAlpha.PlayForward();

        winGameBg.gameObject.SetActive(false);
        loseGameBg.gameObject.SetActive(false);
        gameBg.SetActive(false);
    }

    void PlayTweenAlpha()
    {
        if (isTweenAlphaShow)
        {
            tweenAlpha.PlayReverse();
        }
        else
        {
            tweenAlpha.PlayForward();
        }
        isTweenAlphaShow = !isTweenAlphaShow;
    }

    void StartMusic()
    {
        string play = XMLManager.GetString(musicString);
        if (play == "" || play == "True")
        {
            isPlayMusic = false;
            PlayMusic();
        }
        else
        {
            isPlayMusic = true;
            PlayMusic();
        }
    }

    void StartGame()
    {
        welcomeBg.SetActive(false);
        gameBg.SetActive(true);
        ReStartGame();
        StartMusic();
    }

    void CreatePrefab()
    {
        if (NumberCount < MaxNumberCount)
        {//如果有空的位置，防止溢出
            NumberCount++;//总数溢出判定  死亡减少
            GameObject newGo = NGUITools.AddChild(gameBg, numberPrefab);
            newGo.GetComponent<Number>()._Awake();
        }
    }

    public bool IsEmpty(int _x, int _y)
    {
        if (_x < 0 || _y < 0 || _x >= Lie || _y >= Hang)
        {
            return false;
        }
        else if (Numbers[_x, _y] != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void PlayMusic()
    {
        isPlayMusic = !isPlayMusic;
        if (isPlayMusic)
        {
            _as.Play();
            musicTexture.mainTexture = musicOff;
        }
        else
        {
            _as.Stop();
            musicTexture.mainTexture = musicOn;
        }
        XMLManager.SetString(musicString, isPlayMusic.ToString());
    }

    void ReStartGame()
    {
        GAMESTATE = GameState.Gameing;
        winGameBg.gameObject.SetActive(false);
        loseGameBg.gameObject.SetActive(false);
        NumberCount = 0;
        EndScore = 0;
        for (int i = 0; i < Hang; i++)
        {
            for (int j = 0; j < Lie; j++)
            {
                if (Numbers[j, i] != null)
                {
                    Destroy(Numbers[j, i].gameObject);
                    Numbers[j, i] = null;
                }
            }
        }

        for (int i = 0; i < startNumber; i++)
        {
            CreatePrefab();
        }
    }

    void EndGame()
    {
        string ans = XMLManager.GetString(highScoreString);
        if (ans == "")
        {
            ans = "0";
        }
        int highSocre = int.Parse(ans);
        for (int i = 0; i < Hang; i++)
        {
            for (int j = 0; j < Lie; j++)
            {
                if (Numbers[j, i] != null)
                {
                    EndScore += Numbers[j, i].NumberValue;
                }

            }
        }
        if (EndScore > highSocre)
        {
            WinGame(EndScore);
        }
        else
        {
            LoseGame(EndScore, highSocre);
        }
    }

    void WinGame(int endScore)
    {
        GAMESTATE = GameState.Winend;
        winHigScoreText.text = endScore.ToString();
        XMLManager.SetString(highScoreString, endScore.ToString());
        winGameBg.gameObject.SetActive(true);
    }

    void LoseGame(int endScore, int highSocre)
    {
        GAMESTATE = GameState.Loseend;
        nowScoreText.text = endScore.ToString();
        loseHighScoreText.text = highSocre.ToString();
        loseGameBg.gameObject.SetActive(true);
    }


    bool CheckGameEnd()
    {//true 死亡 false 还有机会
        check = true;
        if (NumberCount < MaxNumberCount)
        {
            return false;
        }
        for (int i = 0; i < Hang; i++)
        {
            for (int j = 0; j < Lie; j++)
            {
                if (Numbers[j, i] != null)
                {
                    if (Numbers[j, i].NumberValue < MaxNumber)
                    {

                        if (j + 1 < Lie && Numbers[j + 1, i] != null)
                        {
                            if (Numbers[j, i].NumberValue == Numbers[j + 1, i].NumberValue)
                            {
                                return false;
                            }
                        }
                        if (i + 1 < Hang && Numbers[j, i + 1] != null)
                        {
                            if (Numbers[j, i].NumberValue == Numbers[j, i + 1].NumberValue)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }
        return true;
    }

}
