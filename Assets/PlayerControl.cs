using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerControl : MonoBehaviour
{
    
    
    
    
    
    
    
    
    #region//インスペクターで設定する
    [Header("移動速度")] public float speed;
    [Header("重力")] public float gravity;
    [Header("ジャンプ速度")] public float jumpSpeed;
    [Header("ジャンプする高さ")] public float jumpHeight;
    [Header("ジャンプ制限時間")] public float jumpLimitTime;
    [Header("接地判定")] public GroundCheck ground;
    [Header("頭をぶつけた判定")] public GroundCheck head;
    [Header("ダッシュの速さ表現")] public AnimationCurve dashCurve;
    [Header("ジャンプの速さ表現")] public AnimationCurve jumpCurve;
    #endregion
    //プライベート変数
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private bool isGround = false;
    private bool isJump = false;
    private bool isHead = false;
    private float jumpPos = 0.0f;
    private float dashTime, jumpTime;  //New
    private float beforeKey;  //New
    private float dash;
    private float time;
    private float time2;



    void Start()
    {
        //コンポーネントのインスタンスを捕まえる
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        dash = 0;
    }

    void FixedUpdate()
    {
        //接地判定を得る
        isGround = ground.IsGround();
        //isHead = head.IsGround();

        //キー入力されたら行動する
        float horizontalKey = Input.GetAxisRaw("Horizontal");
        float xSpeed = 0.0f;
        float ySpeed = -gravity;
        float verticalKey = Input.GetAxis("Vertical");

        if (isGround)
        {
            if (verticalKey > 0)
            {
                ySpeed = jumpSpeed;
                jumpPos = transform.position.y; //ジャンプした位置を記録する
                isJump = true;
                jumpTime = 0.0f;
            }
            else
            {
                isJump = false;
            }
        }
        else if (isJump)
        {
            //上方向キーを押しているか
            bool pushUpKey = verticalKey > 0;
            //現在の高さが飛べる高さより下か
            bool canHeight = jumpPos + jumpHeight > transform.position.y;
            //ジャンプ時間が長くなりすぎてないか
            bool canTime = jumpLimitTime > jumpTime;

            if (pushUpKey && canHeight && canTime && !isHead)
            {
                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else
            {
                isJump = false;
                jumpTime = 0.0f;
            }
        }

        if (horizontalKey > 0)
        {
            if(dashTime > 0)
            {
                
                dashTime -= 0.1f;
            }
            transform.localScale = new Vector3(1, 1, 1);
            anim.SetBool("run", true);
            xSpeed -= 0.01f;
            //dashTime += Time.deltaTime;  //New
            dash += Time.deltaTime;
            if(Mathf.Floor(time)%10 == 0)
            {
                dash += 0.01f;
            }
            xSpeed = speed* dash - dashTime;

            if(xSpeed > 9)
            {
                xSpeed = 9;
            }

            time2 = 9.0f;
        }
        else if (horizontalKey < 0)
        {
            if (time2 > 0)
            {

                time2 -= 0.1f;
            }
            transform.localScale = new Vector3(-1, 1, 1);
            anim.SetBool("run", true);
            //time2 += Time.deltaTime;  //New
            dash += Time.deltaTime;
            if (Mathf.Floor(time) % 10 == 0)
            {
                dash += 0.01f;
            }
            xSpeed = speed * -dash +time2;

            if (xSpeed < -9)
            {
                xSpeed = -9;
            }

            dashTime = 9.0f;
        }
        else
        {
            anim.SetBool("run", false);
            //xSpeed = 0.0f;
            //dashTime = 7.0f;  //New
            dash = 0.0f;
        }

        //前回の入力からダッシュの反転を判断して速度を変える New
        //if (horizontalKey > 0 && beforeKey < 0)
        //{
        //    dashTime = 0.0f;
        //}
        //else if (horizontalKey < 0 && beforeKey > 0)
        //{
        //    dashTime = 0.0f;
        //}
        //beforeKey = horizontalKey;

        //アニメーションカーブを速度に適用 New
        //xSpeed *= dashCurve.Evaluate(dashTime);
        if (isJump)
        {
            ySpeed *= jumpCurve.Evaluate(jumpTime);
        }
        anim.SetBool("jump", isJump); //New
        anim.SetBool("ground", isGround); //New
        rb.velocity = new Vector2(xSpeed, ySpeed);
    }
}
