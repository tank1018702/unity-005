using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Character
{

    Dictionary<Pos, bool> map;
    Pos target;

    bool IsFindTarget;

    protected override void Start()
    {
        base.Start();

        map = new Dictionary<Pos, bool>();

        target = new Pos(10,12);

        IsFindTarget = false;

        StartCoroutine(HeroSearchRoad(Pos.Float2IntPos(transform.position)));
    }


    IEnumerator HeroSearchRoad(Pos next)
    {
        Pos cur = Pos.Float2IntPos(transform.position);
        //移动到当前坐标
        //Debug.DrawLine(transform.position, Pos.Pos2Vector2(cur), Color.white, 100000f);

        if (next.Equals(target))
        {
            IsFindTarget = true;
            yield return new WaitForSeconds(2f);
            yield break;
        }

        yield return MoveTo(Pos.Pos2Vector2(next));

        map[next] = false;

        //获取当前位置能移动到的其他节点坐标
        List<Pos> curNode = GetCurrentNode();

        //根据与目标距离对节点目标进行排序
        if (curNode.Count > 1)
        {
            curNode.Sort((a, b) => Pos.GetManhattanDistance(a, target).CompareTo(Pos.GetManhattanDistance(b, target)));
        }

        for (int i = 0; i < curNode.Count; i++)
        {
            //找到目标就不继续其他节点的探索了
            if (IsFindTarget)
            {
                break;
            }
            yield return HeroSearchRoad(curNode[i]);

        }
        //所有能走的节点都走完了,只能回头
        //回溯
        yield return MoveTo(Pos.Pos2Vector2(cur));
    }

   
    List<Pos> GetCurrentNode()
    {
        List<Pos> list = new List<Pos>();

        List<Pos> resut = new List<Pos>();

        Pos cur = Pos.Float2IntPos(transform.position);

        if (!ObstacleCheck(Direction2Vector2(Direction.Up), WallLayer))
        {
            list.Add(new Pos(cur.x, cur.y + 1));

        }
        if (!ObstacleCheck(Direction2Vector2(Direction.Down), WallLayer))
        {
            list.Add(new Pos(cur.x, cur.y - 1));

        }
        if (!ObstacleCheck(Direction2Vector2(Direction.Right), WallLayer))
        {
            list.Add(new Pos(cur.x + 1, cur.y));

        }
        if (!ObstacleCheck(Direction2Vector2(Direction.Left), WallLayer))
        {
            list.Add(new Pos(cur.x - 1, cur.y));

        }
        for (int i = 0; i < list.Count; i++)
        {
            if (!map.ContainsKey(list[i]))
            {
                Debug.DrawLine(transform.position, Pos.Pos2Vector2(list[i]), Color.blue, 10000f);
                resut.Add(list[i]);
                map.Add(list[i], true);
            }
            else
            {
                Debug.DrawLine(transform.position, Pos.Pos2Vector2(list[i]), Color.blue, 10000f);
                if (map[list[i]] == true)
                {
                    resut.Add(list[i]);
                }
            }
        }
        return resut;
    }

}
