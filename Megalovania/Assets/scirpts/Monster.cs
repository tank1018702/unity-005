using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Character
{
    

    WaitForSeconds WaitHpUpdate = new WaitForSeconds(1f);

    protected override void Start()
    {
        base.Start();
        StartCoroutine(HpUpdate());
        StartCoroutine(Action());

    }
    protected virtual IEnumerator Action()
    {
        while (true)
        {
            RandomDir();

            if (Random.Range(0, 4) > 2)
            {
                yield return Idle();
            }
            else
            {
                yield return null;
                if (ObstacleCheck(CurDir, TempLayer))
                {
                    yield return Behaviour();

                    continue;

                }
                yield return Move(CurDir);
            }
        }
    }


    IEnumerator HpUpdate()
    {
        while(HP>0)
        {
            OnBeHit(1);
            yield return WaitHpUpdate;
        }
       
    }


    IEnumerator Behaviour()
    {
        if(isLowMonster)
        {
            yield return  TransportNutrients();
        }
        else
        {
            yield return Eat();
        }
        
    }

    IEnumerator Idle()
    {
        float time = idleTime;
        while(time>float.Epsilon)
        {
            time -= Time.deltaTime;
            CheckAndAttackEnemyAround();
            yield return null;
        }
    }
}
