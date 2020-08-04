using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMovement))]
public class AnimatorController : MonoBehaviour
{
    private InputController controller;
    private PlayerMovement  player;
    private Animator animator;
   // private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        this.controller = this.GetComponent<InputController>();
        this.animator = this.GetComponent<Animator>();
        this.player = this.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {

        // Change direction of the animation Rigth or Left
        if(controller.x > 0f){
            transform.localScale = new Vector2 (1f,1f);
        }else if (controller.x < 0f){
            transform.localScale = new Vector2 (-1f,1f);
        }

        // Check if the player is walking
        if (controller.x != 0){
            this.animator.SetBool("isWalking", true);
        }else{
            this.animator.SetBool("isWalking", false);
        }
        // Check that the player is on the ground/platform or in the air  
        this.animator.SetBool("isGrounded", this.player.isGrounded);
        
        //Trigger when player jumps, to execute jumping animation 
        if(this.player.isGrounded && this.controller.jump){
            this.animator.SetTrigger("Jump");
        }else{
            this.animator.ResetTrigger("Jump");
        }

    }   

}
