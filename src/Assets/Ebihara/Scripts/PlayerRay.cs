using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerRay : MonoBehaviour
{
    [SerializeField] Change change;
    [SerializeField] float distance = 50.0f;//���o�\�ȋ���
    [SerializeField] LayerMask gazeHitMask;
    Transform transforms;
    GameObject game;
    PlayerMove playerMove;
    bool shoot;
    Vector3 rayHitPosition;
    
    public bool Shoot
    {
        get { return shoot; }
    }
    public Vector3 RayHitPosition
    {
        get { return rayHitPosition; }
    }

    // Start is called before the first frame update
    void Start() 
    {
        playerMove = FindObjectOfType<PlayerMove>();
        shoot = false;
    }

    // Update is called once per frame
    void Update()
    {
        //rayの始まり
        var rayStartPosition = this.transform.position;
        //rayの方向
        var rayDirection = this.transform.forward.normalized;
        Debug.DrawRay(rayStartPosition, rayDirection * distance, Color.red);
        playerMove.Gun.transform.forward = rayDirection;

        Ray playerGaze = new Ray(rayStartPosition, rayDirection);
        if(Physics.Raycast(playerGaze, out RaycastHit hit, distance, gazeHitMask))
        {
            rayHitPosition = hit.transform.position;
        }
        else
        {
            rayHitPosition = transform.position + rayDirection * distance;
        }
    }

    public GameObject GetObj(){ return game; }

    public void Change(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //var center = transform.position;

            //// CapsuleCast�ɂ�铖���蔻��
            //var isHit = Physics.CapsuleCast(
            //    center + new Vector3(0, 0.5f, 0), // �n�_
            //    center + new Vector3(0, -0.5f, 0), // �I�_
            //    0.5f, // �L���X�g���镝
            //    Vector3.forward, // �L���X�g����
            //    out var hit // �q�b�g���
            //);

            //if (isHit == true)
            //{
            //    game = hit.collider.GameObject();
            //    change.ChangeEnemy(game);
            //}
            //rayの始まり
            var rayStartPosition = this.transform.position;

            //rayの方向
            var rayDirection = this.transform.forward.normalized;

            //Hitしたオブジェクト格納
            RaycastHit raycastHit;

            Debug.DrawRay(rayStartPosition, rayDirection * distance, Color.red);

            if (Physics.Raycast(rayStartPosition, rayDirection, out raycastHit, distance) && raycastHit.collider.tag == "Enemy")
            {
                game = raycastHit.collider.gameObject;
                change.ChangeEnemy(game);//FPS状態の時でもOK(playerMove.IsChangeGame == true)
            }

        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            // PlayerMoveに飛ばして弾を出す
            playerMove.Gun.Shoot(transform.position, playerMove.Gun.transform.forward, "Player", false);
            if(!shoot)
            {
                StartCoroutine(SetShootTrueForSeconds(0.2f));
            }
        }
    }

    public void CheckCanPossess(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //rayの始まり
            var rayStartPosition = this.transform.position;

            //rayの方向
            var rayDirection = this.transform.forward.normalized;

            //Hitしたオブジェクト格納
            RaycastHit raycastHit;

            Debug.DrawRay(rayStartPosition, rayDirection * distance, Color.red);

            if (Physics.Raycast(rayStartPosition, rayDirection, out raycastHit, distance))
            {
                //Debug.Log(context.phase);
                Debug.Log("HitObject : " + raycastHit.collider.gameObject.name);

                if (raycastHit.collider.tag == "Enemy")
                {
                    if (raycastHit.collider.TryGetComponent<CharacterStatus>(out CharacterStatus character) && character.CanPossess)
                    {
                        Debug.Log("EnemyHit");
                        transforms = raycastHit.transform;
                        game = raycastHit.collider.gameObject;
                    }
                }
            }
        }
        if(context.phase == InputActionPhase.Canceled)
        {
            transforms = null;
            game = null;
        }
    }

    IEnumerator SetShootTrueForSeconds(float second)
    {
        shoot = true;
        yield return new WaitForSeconds(second);
        shoot = false;
    }
}
