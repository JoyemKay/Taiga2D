using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerCanAttack : MonoBehaviour
{

    public void SetCanAttack(bool state){
        
        Player player =  FindObjectOfType<Player>();
        if (player){
            player.CanAttack(state);
        }
    }
}
