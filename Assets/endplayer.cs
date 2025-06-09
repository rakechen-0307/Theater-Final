using UnityEngine;

public class endplayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(OSCSender.Instance != null){
            OSCSender.Instance.PlaySound("game2", 0);
            OSCSender.Instance.PlaySound("end", 1);
        }else
        {
            Debug.Log("No OSCSender Instance");
        }
    }

    
}
