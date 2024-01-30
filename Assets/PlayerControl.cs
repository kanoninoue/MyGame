using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerControl : MonoBehaviour
{
    
    
    
    
    
    
    
    
    #region//�C���X�y�N�^�[�Őݒ肷��
    [Header("�ړ����x")] public float speed;
    [Header("�d��")] public float gravity;
    [Header("�W�����v���x")] public float jumpSpeed;
    [Header("�W�����v���鍂��")] public float jumpHeight;
    [Header("�W�����v��������")] public float jumpLimitTime;
    [Header("�ڒn����")] public GroundCheck ground;
    [Header("�����Ԃ�������")] public GroundCheck head;
    [Header("�_�b�V���̑����\��")] public AnimationCurve dashCurve;
    [Header("�W�����v�̑����\��")] public AnimationCurve jumpCurve;
    #endregion
    //�v���C�x�[�g�ϐ�
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
        //�R���|�[�l���g�̃C���X�^���X��߂܂���
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        dash = 0;
    }

    void FixedUpdate()
    {
        //�ڒn����𓾂�
        isGround = ground.IsGround();
        //isHead = head.IsGround();

        //�L�[���͂��ꂽ��s������
        float horizontalKey = Input.GetAxisRaw("Horizontal");
        float xSpeed = 0.0f;
        float ySpeed = -gravity;
        float verticalKey = Input.GetAxis("Vertical");

        if (isGround)
        {
            if (verticalKey > 0)
            {
                ySpeed = jumpSpeed;
                jumpPos = transform.position.y; //�W�����v�����ʒu���L�^����
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
            //������L�[�������Ă��邩
            bool pushUpKey = verticalKey > 0;
            //���݂̍�������ׂ鍂����艺��
            bool canHeight = jumpPos + jumpHeight > transform.position.y;
            //�W�����v���Ԃ������Ȃ肷���ĂȂ���
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

        //�O��̓��͂���_�b�V���̔��]�𔻒f���đ��x��ς��� New
        //if (horizontalKey > 0 && beforeKey < 0)
        //{
        //    dashTime = 0.0f;
        //}
        //else if (horizontalKey < 0 && beforeKey > 0)
        //{
        //    dashTime = 0.0f;
        //}
        //beforeKey = horizontalKey;

        //�A�j���[�V�����J�[�u�𑬓x�ɓK�p New
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
