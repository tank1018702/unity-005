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

    public int  CarryNutrients;
    public bool isLowMonster;

    [Header("times")]
    

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
    }

    //private void Update()
    //{
    //    Debug.Log(dir.ToString());
    //}

    protected Direction RandomDir()
    {
        
        CurDir = (Direction)Random.Range((int)Direction.Up, (int)Direction.Right + 1);
        _animator.SetInteger("Dir", (int)CurDir);


        return CurDir;
    }

  

    protected virtual IEnumerator TransportNutrients()
    {
        RaycastHit2D[] hit;
        if (ObstacleCheck(CurDir, WallLayer, out hit))
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


    protected IEnumerator CheckAndAttackEnemyAround()
    {
        Collider2D[] arounds = Physics2D.OverlapCircleAll(transform.position, 0.09f, Enemylayer);

        if(arounds.Length>0)
        {
            
            targetDir = GetDirection(transform.position, arounds[0].transform.position);
            yield return Attack(targetDir);
          
        }
       
    }

    protected IEnumerator Attack(Direction dir)
    {
        RaycastHit2D[] hit;

        
        while (ObstacleCheck(dir, Enemylayer, out hit))
        {
            _animator.SetBool("MoveState", false);

            _animator.SetInteger("Dir", (int)dir);
            yield return null;

            _animator.SetTrigger("Attack");

            for (int i = 0; i < hit.Length; i++)
            {
                Character c = hit[i].transform.GetComponent<Character>();
                if (c)
                {
                    c.OnBeHit(damage);
                }
            }
            yield return AttackInterval;
        }
    }

    /// <summary>
    /// direction=>Vector2
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    Vector2 Direction2Vector2(Direction dir)
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
    /// <summary>
    /// check if anything on this direction by layer
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    protected bool ObstacleCheck(Direction dir, LayerMask layer)
    {
        RaycastHit2D[] hit;
        hit = Physics2D.RaycastAll(transform.position, Direction2Vector2(dir), 0.18f, layer);
        return hit.Length > 0;
    }

    protected bool ObstacleCheck(Direction dir, LayerMask layer, out RaycastHit2D[] _hit)
    {
        RaycastHit2D[] hit;
        hit = Physics2D.RaycastAll(transform.position, Direction2Vector2(dir), 0.18f, layer);
        _hit = hit;
        return hit.Length > 0;
    }



    public void OnBeHit(int damage)
    {
        HP -= damage;
      
        if (HP <= 0)
        {
            HP = 0;
            StopAllCoroutines();
            Invoke("Die", 0.5f);
        }
    }
    protected virtual void Die()
    {
        Destroy(gameObject);
    }


    /// <summary>
    /// Move by direction
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    protected IEnumerator Move(Direction dir)
    {
        Vector2 correctionPos = Pos.Pos2Vector2(Pos.Float2IntPos(transform.position));
        //transform.position = correctionPos;
        Vector2 endPos = correctionPos + (Direction2Vector2(dir) * 0.18f);
        if (!isMoving)
            yield return SmoothMovement(endPos);
    }
    /// <summary>
    /// move to a vector2 position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    protected IEnumerator MoveTo(Vector2 pos)
    {
        if (!isMoving)
            yield return SmoothMovement(pos);
    }

    
    /// <summary>
    /// get direction by two Vector2
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
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

        float Distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), endPos);

        Direction _dir = GetDirection(transform.position, endPos);
        _animator.SetInteger("Dir", (int)_dir);
        yield return null;
        while (Distance > float.Epsilon)
        {
            if (_animator.GetInteger("Dir") != (int)_dir)
            {
                _animator.SetInteger("Dir", (int)_dir);
                yield return null;
            }
            _animator.SetBool("MoveState", true);
            Vector2 newPos = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), endPos, MoveSpeed * Time.deltaTime);

            //Debug.DrawRay(new Vector2(transform.position.x, transform.position.y), Vector3.back, Color.red, 10f);
            //Debug.DrawRay(endPos, Vector3.back, Color.blue, 10f);
            //Debug.DrawLine(newPos, endPos, Color.yellow, 10f);


            transform.position = newPos;

            Distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), endPos);

            //check if enemys around
            yield return CheckAndAttackEnemyAround();
        }
        _animator.SetBool("MoveState", false);
        isMoving = false;
        yield return null;

    }

    protected virtual IEnumerable Eat()
    {
        RaycastHit2D[] hit;
        if(ObstacleCheck(CurDir, FoodLayer, out hit ))
        {
            _animator.SetTrigger("Attack");
            var script = hit[0].transform.GetComponent<Character>();
            CarryNutrients += script.CarryNutrients;
            Destroy(hit[0].transform.gameObject);
            yield return IdleTime;
        }

        
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

