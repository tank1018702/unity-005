using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public Camera _camera;

    Dictionary<Pos, Block> BlockMap;

    List<Character> characters_list;

    public Transform BlockMapRoot;


    public ParticleSystem MouseClick;

    public event  Action<Vector2> OnMouseClick;
    

    //float intervalTime=5f;

    //WaitForSeconds interval;

    private void Awake()
    {
        if(!instance)
        {
            instance = this;
        }
        else if(instance!=this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        characters_list = new List<Character>();

        BlockMap = new Dictionary<Pos, Block>();

        InitMap();

        OnMouseClick += PlayParticles;
       
        //interval = new WaitForSeconds(intervalTime);

        

    }

   

    void Start()
    {
        //StartCoroutine(CharacterUpdate());
    }



    void Update()
    {
        InputProcess();

    }


    void InitMap()
    {
        Block [] blocks = BlockMapRoot.GetComponentsInChildren<Block>();
        for (int i = 0; i < blocks.Length; i++)
        {
            if (BlockMap.ContainsKey(Pos.Float2IntPos(blocks[i].transform .position)))
            {
                Debug.Log(BlockMap[Pos.Float2IntPos(blocks[i].transform.position)].name+"");
                return;
            }
            //Debug.Log("I:" + i + "|" + blocks[i].transform.position + "|" + Pos.Float2IntPos(blocks[i].transform.position).ToString()+"---"+blocks[i].transform.name);
            BlockMap.Add(Pos.Float2IntPos(blocks[i].transform. position), blocks[i]);
        }
    }

    bool CheckBlockInMap(Vector2 MousePos)
    {
        Pos p = Pos.Float2IntPos(_camera.ScreenToWorldPoint(MousePos));
        if (BlockMap.ContainsKey(p))
        {
            OnMouseClick(Pos.Pos2Vector2(p));
            if(BlockMap[p].OnClick())
            {
                BlockMap.Remove(p);
            }
            return true;
        }
        
        return false;
    }

    void InputProcess()
    {
        if (Input.GetMouseButtonDown(0))
        {     
            
            Vector2 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log(Pos.Float2IntPos(pos));
            CheckBlockInMap(Input.mousePosition);

        }
    }


   void PlayParticles(Vector2 pos)
    {
      
       
        
     
    }

    //IEnumerator CharacterUpdate()
    //{
    //    while(true)
    //    {
    //        for (int i = 0; i < characters_list.Count; i++)
    //        {
    //            characters_list[i].Action();
    //        }
    //        yield return interval;
    //    }
      
    //}

    public void AddCharacter(Character c)
    {
        characters_list.Add(c);
    }


}

public struct Pos:IEquatable<Pos>
{
   public int x;
   public int y;

    
    public Pos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return "X:" + x + "|" + "Y:" + y;
    }

    public static Pos Float2IntPos(Vector2 pos)
    {
        int x = Mathf.FloorToInt((pos.x + 0.09f) / 0.18f);
        int y = Mathf.FloorToInt((pos.y + 0.09f )/ 0.18f);
        return new Pos(x, y);
    }

    public static Vector2 Pos2Vector2(Pos pos)
    {
        float x = pos.x * 0.18f ;
        float y = pos.y * 0.18f ;
        return new Vector2(x, y);
    }
    public bool Equals(Pos p)
    {
        if (this.x == p.x && this.y == p.y)
        {
            return true;
        }
        return false;
    }

    public static int GetManhattanDistance(Pos p1,Pos p2)
    {
        return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y);
    }

    public static Direction GetDirection(Pos from,Pos to)
    {
        if(Math.Abs( from.x-to.x)==Math.Abs(from.y-to.y))
        {
            Debug.Log("error");
            return Direction.UnKnow;
            
        }
        if(Mathf.Abs(from.x-to.x)>Mathf.Abs(from.y-to.y))
        {
            return from.x - to.x > 0 ? Direction.Left : Direction.Right;
        }
        return from.y - to.y > 0 ? Direction.Up : Direction.Down;
    }
   
    

}
