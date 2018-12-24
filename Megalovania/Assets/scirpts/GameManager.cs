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

    Pos HeroStartPos = new Pos(-22, 1);
    Pos KingStartPos = new Pos(1, -2);
    public GameObject Hero;
    public GameObject King;
    public GameObject KingCursor;

    public GameObject UI;


    bool OnSelect = false;
    Vector2 MousePos;

    GameObject k;
    GameObject H;
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
        StartCoroutine(Level_1(30f));
        k= Instantiate(King, Pos.Pos2Vector2(KingStartPos), Quaternion.identity);
        KingCursor.SetActive(false);
    }



    void Update()
    {
        MousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        if (!OnSelect)
        {
            InputProcess();
        }
        else
        {
            SelectPos();
        }


    }
    

    IEnumerator Level_1(float time)
    {
        yield return new WaitForSeconds(time);
        OnSelect = true;
        Time.timeScale = 0;
        H= Instantiate(Hero, Pos.Pos2Vector2(HeroStartPos), Quaternion.identity);
        
        KingCursor.SetActive(true);
        k.SetActive(false);
    }

     public  void GameOver()
    {

        Time.timeScale = 0;
        UI.transform.GetChild(0).gameObject.SetActive(true);
    }

 

    void SelectPos()
    {

        
        if(Input.GetMouseButtonDown(1))
        {
            Time.timeScale = 1;
            OnSelect = false;
            k.SetActive(true);
            k.transform.position = KingCursor.transform.position;
            KingCursor.SetActive(false);
            H.GetComponent<Hero>().Init(Pos.Float2IntPos(k.transform.position));

        }
        if(!BlockMap.ContainsKey(Pos.Float2IntPos(MousePos)))
        {
            
            KingCursor.transform.position = Pos.Pos2Vector2(Pos.Float2IntPos(MousePos));


        }
        

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

    bool CheckBlockInMap()
    {
        Pos p = Pos.Float2IntPos(MousePos);
        if (BlockMap.ContainsKey(p))
        {
            
            if(BlockMap[p].OnClick())
            {
                OnMouseClick(Pos.Pos2Vector2(p));

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
            
            
            //Debug.Log(Pos.Float2IntPos(MousePos));
            CheckBlockInMap();

        }
    }


   void PlayParticles(Vector2 pos)
    {
        ParticleSystem p = Instantiate(MouseClick, new Vector3(pos.x,pos.y,-1), Quaternion.identity);
        p.Play();
       
     
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
