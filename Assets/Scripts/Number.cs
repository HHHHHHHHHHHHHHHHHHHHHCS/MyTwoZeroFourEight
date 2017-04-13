using UnityEngine;
using System.Collections;

public class Number : MonoBehaviour
{
    public Vector3 startPos = new Vector3(-150, -105, -1);//开始点的位置
    public Vector2 boxSize = new Vector2(100, 90);//数字的尺寸大小

    private const float gaiLv = 0.3f;//产生 4的概率

    private int numberPosX;//数值的X轴位置
    private int numberPosY;//数值的Y轴位置
    public int _numberValue;//数字的名字，用来改变数字
    private UISprite sprite;//图片
    private TweenPosition tweenPos;
    private bool isMove = false;//是否在移动
    private bool _needDestory = false;//判断移动完成是否删除
    public int NumberValue { get { return _numberValue; } set { _numberValue = value; UpdatePhoto(); } }

    public bool NeedDestory
    {
        get
        {
            return _needDestory;
        }
        set
        {
            _needDestory = value;
        }
    }

    public void _Awake()
    {
        Init();
        RandomPos();
        InitNumber();
    }

    void Update()
    {
        if (!isMove)
        {
            Vector3 toPos = new Vector3(startPos.x + boxSize.x * numberPosX, startPos.y + boxSize.y * numberPosY, startPos.z);
            if (transform.localPosition != toPos || NeedDestory)
            {
                isMove = true;
                tweenPos.from = transform.localPosition;
                tweenPos.to = toPos;
                tweenPos.ResetToBeginning();
                tweenPos.PlayForward();
                GameManager.Instance.MoveingCount++;
            }
        }

    }

    void MoveFinish()
    {
        if (sprite.spriteName != NumberValue.ToString())
        {
            UpdatePhoto();
        }
        if (NeedDestory)
        {
            GameManager.Instance.NumberCount--;
            Destroy(gameObject);
        }
        GameManager.Instance.MoveingCount--;
        isMove = false;
    }


    void Init()
    {
        sprite = transform.GetComponent<UISprite>();
        tweenPos = transform.GetComponent<TweenPosition>();
        tweenPos.AddOnFinished(MoveFinish);
    }

    void InitNumber()
    {
        NumberValue = Random.value > gaiLv ? 2 : 4;
    }

    void UpdatePhoto()
    {
        sprite.spriteName = NumberValue.ToString();
    }


    void RandomPos()
    {

        do
        {
            numberPosX = Random.Range(0, GameManager.Lie);
            numberPosY = Random.Range(0, GameManager.Hang);
        }
        while (GameManager.Instance.Numbers[numberPosX, numberPosY] != null);

        GameManager.Instance.Numbers[numberPosX, numberPosY] = this;
        transform.localPosition = new Vector3(startPos.x + boxSize.x * numberPosX, startPos.y + boxSize.y * numberPosY, startPos.z);
        return;
    }


    public void Move(MoveDirection md)
    {
        int moveDis = 1;
        int isForward = 0;//是那个方向  +1: Up、Right   -1：Down、Left
        bool isNoIndexOut = false;//是否 没有越界
        if (md == MoveDirection.Up || md == MoveDirection.Down)
        {
            isForward = md == MoveDirection.Up ? 1 : -1;

            while (GameManager.Instance.IsEmpty(numberPosX, numberPosY + isForward * moveDis))
            {
                moveDis++;
            }
            if (moveDis > 1)
            {
                GameManager.Instance.HasMove = true;
                GameManager.Instance.Numbers[numberPosX, numberPosY] = null;
                numberPosY = numberPosY + isForward * moveDis - isForward;
                GameManager.Instance.Numbers[numberPosX, numberPosY] = this;
            }
            if (isForward > 0)
            {
                isNoIndexOut = numberPosY < GameManager.Hang - 1;
            }
            else
            {
                isNoIndexOut = numberPosY > 0;
            }
            if (isNoIndexOut
                && NumberValue == GameManager.Instance.Numbers[numberPosX, numberPosY + isForward].NumberValue)
            {
                GameManager.Instance.Numbers[numberPosX, numberPosY + isForward].NeedDestory = true;
                GameManager.Instance.Numbers[numberPosX, numberPosY] = null;
                numberPosY = numberPosY + isForward;
                GameManager.Instance.Numbers[numberPosX, numberPosY] = this;
                NumberValue = NumberValue + NumberValue;
                if (NumberValue >= GameManager.MaxNumber + GameManager.MaxNumber)
                {
                    GameManager.Instance.EndScore += NumberValue;
                    GameManager.Instance.Numbers[numberPosX, numberPosY].NeedDestory = true;
                    GameManager.Instance.Numbers[numberPosX, numberPosY] = null;
                }
                GameManager.Instance.HasMove = true;
            }
        }
        else
        {
            isForward = md == MoveDirection.Right ? 1 : -1;

            while (GameManager.Instance.IsEmpty(numberPosX + isForward * moveDis, numberPosY))
            {
                moveDis++;
            }
            if (moveDis > 1)
            {
                GameManager.Instance.HasMove = true;
                GameManager.Instance.Numbers[numberPosX, numberPosY] = null;
                numberPosX = numberPosX + isForward * moveDis - isForward;
                GameManager.Instance.Numbers[numberPosX, numberPosY] = this;
            }

            if (isForward > 0)
            {
                isNoIndexOut = numberPosX < GameManager.Lie - 1;
            }
            else
            {
                isNoIndexOut = numberPosX > 0;
            }

            if (isNoIndexOut
                && NumberValue == GameManager.Instance.Numbers[numberPosX + isForward, numberPosY].NumberValue)
            {
                GameManager.Instance.Numbers[numberPosX + isForward, numberPosY].NeedDestory = true;
                GameManager.Instance.Numbers[numberPosX, numberPosY] = null;
                numberPosX = numberPosX + isForward;
                GameManager.Instance.Numbers[numberPosX, numberPosY] = this;
                NumberValue = NumberValue + NumberValue;
                if (NumberValue >= GameManager.MaxNumber + GameManager.MaxNumber)
                {
                    GameManager.Instance.EndScore += NumberValue;
                    GameManager.Instance.Numbers[numberPosX, numberPosY].NeedDestory = true;
                    GameManager.Instance.Numbers[numberPosX, numberPosY] = null;
                }
                GameManager.Instance.HasMove = true;
            }
        }
    }
}
