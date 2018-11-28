using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    Animator _animator;

    //base 

   protected Direction CurDir;

   protected Direction targetDir;

    [Header("base")]
    public float MoveSpeed = 5f;

    public int HP;

    public int damage;

    [Header("times")]
    public float lifeTime;


    public float IntervalTime;
    public float idleTime;
    public int nutrient;

    [Header("Layers")]
    public LayerMask Enemylayer;

    public LayerMask FoodLayer;

    public LayerMask WallLayer;

    protected LayerMask TempLayer;


    //states
    protected bool isMoving = false;

    protected bool isAlive = true;

    protected bool onAttackState = false;




   protected WaitForSeconds IdleTime;

   protected WaitForSeconds AttackInterval;

    protected virtual void Start()
    {
        IdleTime = new WaitForSeconds(idleTime);

        AttackInterval = new WaitForSeconds(IntervalTime);

        GameManager.instance.AddCharacter(this);

        _animator = GetComponent<Animator>();

        TempLayer = Enemylayer | FoodLayer | WallLayer;

        if (transform.CompareTag("Hero"))
        {
            _animator.SetFloat("PlaySpeed", 4f);
        }
        else
        {
            _animator.SetFloat("PlaySpeed", 2f);
        }
        //StartCoroutine(Action());




        //InvokeRepeating("RandomDir", 0, 1f);
    }

    //private void Update()
    //{
    //    Debug.Log(dir.ToString());
    //}

    protected Direction RandomDir()
    {
        CurDir = (Direction)Random.Range((int)Direction.Up, (int)Direction.Right + 1);

        return CurDir;
    }

    

    protected virtual IEnumerator Behaviour(Vector2 _dir)
    {
        if (ObstacleCheck(_dir, WallLayer))
        {
            yield return TransportNutrients();
        }
        else if (ObstacleCheck(_dir, Enemylayer))
        {
            onAttackState = true;
        }
        else if (ObstacleCheck(_dir, FoodLayer))
        {
            yield return Eat();
        }
    }

    protected virtual IEnumerator TransportNutrients()
    {
        RaycastHit2D[] hit;
        if (ObstacleCheck(Direction2Vector2(CurDir), WallLayer, out hit))
        {
            _animator.SetTrigger("Attack");

            Block script = hit[0].transform.GetComponent<Block>();
            if (script)
            {
                script.ChangeNutrient(nutrient);
            }

        }
        yield return IdleTime;
    }

    IEnumerator Eat()
    {
        yield return null;
    }
    protected  IEnumerator Attack()
    {
        RaycastHit2D[] hit;
        if (ObstacleCheck(Direction2Vector2(targetDir), Enemylayer, out hit))
        {
            if(_animator.GetInteger("Dir")!=(int)targetDir)
            {
                _animator.SetInteger("Dir", (int)targetDir);
                yield return null;
            }
      
            _animator.SetTrigger("Attack");

            for (int i = 0; i < hit.Length; i++)
            {
                Character c = hit[i].transform.GetComponent<Character>();
                if (c)
                {
                    c.OnBeHit(damage, CurDir);
                }
            }
            yield return AttackInterval;
        }
        yield return null;
    }

    protected Vector2 Direction2Vector2(Direction dir)
    {
        Vector2 direction = Vector2.zero;
        switch (dir)
        {
            case Direction.Up:
                direction = Vector2.up;
                break;
            case Direction.Down:
                direction = Vector2.down;
                break;
            case Direction.Left:
                direction = Vector2.left;
                break;
            case Direction.Right:
                direction = Vector2.right;
                break;
        }
        return direction;
    }

    protected bool ObstacleCheck(Vector2 dir, LayerMask layer)
    {
        RaycastHit2D[] hit;
        hit = Physics2D.RaycastAll(transform.position, dir, 0.18f, layer);
        return hit.Length > 0;
    }

    protected bool ObstacleCheck(Vector2 dir, LayerMask layer, out RaycastHit2D[] _hit)
    {
        RaycastHit2D[] hit;
        hit = Physics2D.RaycastAll(transform.position, dir, 0.18f, layer);
        _hit = hit;
        return hit.Length > 0;
    }



    public bool OnBeHit(int damage, Direction dir)
    {
        onAttackState = true;
        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            return true;
        }
        switch (dir)
        {
            case Direction.Up:
                targetDir = Direction.Down;
                break;
            case Direction.Down:
                targetDir = Direction.Up;
                break;
            case Direction.Left:
                targetDir = Direction.Right;
                break;
            case Direction.Right:
                targetDir = Direction.Left;
                break;

        }
        return false;
    }

    protected IEnumerator Move(Vector2 dir)
    {
        Vector2 correctionPos = Pos.Pos2Vector2(Pos.Float2IntPos(transform.position));
        //transform.position = correctionPos;
        Vector2 endPos = correctionPos + (dir * 0.18f);
        if (!isMoving)
            yield return SmoothMovement(endPos);
    }

    protected IEnumerator MoveTo(Vector2 pos)
    {
        if (!isMoving)
            yield return SmoothMovement(pos);
    }

    

    protected Direction GetDirection(Vector2 from,Vector2 to)
    {
        Direction dir;
        float x1 = from.x;
        float y1 = from.y;
        float x2 = to.x;
        float y2 = to.y;
        if (Mathf.Abs(x1 - x2) > Mathf.Abs(y1 - y2))
        {
            dir = x1 - x2 > 0 ? Direction.Left : Direction.Right;
        }
        else
        {
            dir = y1 - y2 > 0 ? Direction.Down : Direction.Up;
        }
        return dir;
    }

    IEnumerator SmoothMovement(Vector2 endPos)
    {
        isMoving = true;
        Direction _dir = GetDirection(transform.position, endPos);
        if(_animator.GetInteger("Dir")!=(int)_dir)
        {
            _animator.SetInteger("Dir", (int)_dir);
            yield return null;
        }
      
        _animator.SetBool("MoveState", true);

        

        float Distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), endPos);


        while (Distance > float.Epsilon)
        {

            Vector2 newPos = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), endPos, MoveSpeed * Time.deltaTime);

            //Debug.DrawRay(new Vector2(transform.position.x, transform.position.y), Vector3.back, Color.red, 10f);
            //Debug.DrawRay(endPos, Vector3.back, Color.blue, 10f);
            //Debug.DrawLine(newPos, endPos, Color.yellow, 10f);


            transform.position = newPos;

            Distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), endPos);

            CheckAroundEnemy();

            if (onAttackState)
            {
                _animator.SetBool("MoveState", false);
                yield return null;
                _animator.SetInteger("Dir", (int)targetDir);

                while (HP != 0 && isAlive && ObstacleCheck(Direction2Vector2(targetDir), Enemylayer))
                {
                    yield return Attack();
                }
                onAttackState = false;
                _animator.SetInteger("Dir", (int)CurDir);
                _animator.SetBool("MoveState", true);
            }
            yield return null;
        }
        //check if enemys around


        _animator.SetBool("MoveState", false);
        isMoving = false;
        yield return null;

    }

    void CheckAroundEnemy()
    {
        Collider2D[] arounds = Physics2D.OverlapCircleAll(transform.position, 0.18f, Enemylayer);
        if (arounds.Length > 0)
        {
            targetDir = GetDirection(transform.position, arounds[0].transform.position);
            onAttackState = true;
        }
    }



    //bool CheckAroundEnemy(out Direction dir)
    //{

    //}

    protected void Die()
    {

    }

}


public enum Direction
{
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3,
    UnKnow=99

}

