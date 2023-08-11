using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] private string foodName;
    public string FoodName => foodName;
    
    private void Update()
    {
        gameObject.transform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime);
    }
}
