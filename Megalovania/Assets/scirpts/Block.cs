using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block : MonoBehaviour
{

    
    public Sprite[] sprites;
    public GameObject[] Monsters;

    int MonsterIndex=-1;

    SpriteRenderer Renderer;
    [SerializeField]
    int nutrient = 0;

    ParticleSystem block_p;

    
   
    public bool IsCanBroke
    {
        get
        {
            return !CheckNeighbor(Vector2.up) || !CheckNeighbor(Vector2.down) || !CheckNeighbor(Vector2.left) || !CheckNeighbor(Vector2.right);
        }
    }


    private void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
        nutrient = Random.Range(-9, 15);
        
    }

    void Start ()
    {
        SpriteUpdate();

	}
	
   
	 
	void Update ()
    {
	
	}

    bool CheckNeighbor(Vector2 dir)
    {
        RaycastHit2D [] hit;
        hit = Physics2D.RaycastAll(transform.position, dir, 0.18f,1<<LayerMask.NameToLayer("Block"));
        //Debug.Log(hit.Length);
        return hit.Length>1;
    }



    void SpriteUpdate()
    {
        //change by nutrient
        int n=0;
        if(nutrient<=-50)
        {
            MonsterIndex = 5;
            n = 6;
        }
        else if(nutrient<=-30)
        {
            MonsterIndex = 4;
            n = 5;
        }
        else if(nutrient<=-10)
        {
            MonsterIndex = 3;
            n = 4;
        }
        else if(nutrient<=10)
        {
            MonsterIndex = -1;
            n = 3;
        }
        else if(nutrient<=30)
        {
            MonsterIndex = 2;
            n = 2;
        }
        else if(nutrient<=50)
        {
            MonsterIndex = 1;
            n = 1;
        }
        else if(n>50)
        {
            MonsterIndex = 0;
            n = 0;
        }

        Renderer.sprite = sprites[2 * n + Random.Range(0, 2)];
    }

    public bool OnClick()
    {
        if(IsCanBroke)
        {
            Debug.Log("on click");
            if (MonsterIndex>=0)
            {
                Instantiate(Monsters[MonsterIndex],transform.position,Quaternion.identity);
                
            }
            Destroy(gameObject);
            return true;
        }
        else
        {
            Debug.Log("cant broke");
            return false;
        }
    }


    public void ChangeNutrient(int vaule)
    {
        nutrient += vaule;
        SpriteUpdate();

    }
   
}
