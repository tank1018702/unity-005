using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Character
{

    protected override void Start()
    {
        base.Start();
    }
    protected virtual IEnumerator Action()
    {
        while (HP != 0 && isAlive)
        {
            if (onAttackState)
            {
                
                yield return null;

                while (HP != 0 && isAlive && ObstacleCheck(Direction2Vector2(targetDir), Enemylayer))
                {
                    yield return Attack();
                }

                onAttackState = false;
                yield return null;
            }
            else
            {
                RandomDir();

                if (Random.Range(0, 4) > 2)
                {
                    yield return IdleTime;
                }
                else
                {
                    yield return null;
                    Vector2 _dir = Direction2Vector2(CurDir);
                    if (ObstacleCheck(_dir, TempLayer))
                    {
                        yield return Behaviour(_dir);
                        continue;
                    }
                    yield return Move(Direction2Vector2(CurDir));
                }
            }
        }
        Die();
        yield return IdleTime;
        Destroy(gameObject);
    }

}
