using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeterorUI : MonoBehaviour
{
    public Animator animator;
    public Image image;
    public List<Sprite> meteorSprites;
    public void ChangeState(int index){
        animator.Play("Meteor");
        image.sprite = meteorSprites[index];
    }
}
