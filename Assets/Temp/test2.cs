using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class test2 : MonoBehaviour
{
    int a;
    
    public void add_money()
    {
        a++;
        MonoBehaviour.print(a);
    }





    // Start is called before the first frame update
    void Start()
    {
        a = 0;

        
    }

}
