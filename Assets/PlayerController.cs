using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public GameObject A1,A2,A3;
    public Transform SkillFirePoint1,SkillFirePoint2,SkillFirePoint3;
    Transform SP1,SP2,SP3;
    float attackCounter;
    public Transform cam;
    public float gravityScale = 1f;
    public float globalGravity = -9.81f;
    Rigidbody rb;

    public FP FirePointController;
    public bool grounded;

    public float speed = 6f;
    public float turnSmoothTime = 0.3f;
    public float turnSmoothVelocity = 0.4f;
    public float jumpForce;
    Animator anim;
    public GameObject Skill2Effect;

    bool canSkill1,canSkill2,canAttack,canMove,canAction;

    // Update is called once per frame
    private void Start() {
        canAction=false;
        canMove=true;
        canSkill1=true;
        canSkill2=true;
        canAttack=true;
        attackCounter=0;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {   
        if(canAction&&Input.GetKeyDown(KeyCode.E)){
            //anim.Play("Talking");
            anim.SetBool("isAction",true);
        }

        //input movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f ,vertical).normalized;

        if(Input.anyKey==false){
            anim.SetBool("isRunning",false);
        }

        //movement
        if(direction.magnitude >= 0.1f){
            anim.SetBool("isAction",false);
            //rotation
            float targetAngle = Mathf.Atan2(direction.x,direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); 
            
            if(canMove){
            transform.rotation=Quaternion.Euler(0f,angle,0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            //movement
            transform.Translate (moveDir*speed*Time.deltaTime, Space.World);
            anim.SetBool("isRunning",true);
            }
        }

        

        if(grounded&&(Input.GetKeyDown(KeyCode.Space))){
            anim.Play("Jump");
            
            rb.velocity=new Vector3(0,jumpForce,0);
            
            grounded=false;
        }

        if(Input.GetKeyDown(KeyCode.Q)){
            anim.Play("Spin Attack");
        }

        //skill state

        if(Input.GetKeyDown(KeyCode.Alpha1) && canSkill1){
            anim.Play("Skill 1");
            SP1=SkillFirePoint1;
            SP2=SkillFirePoint2;
            SP3=SkillFirePoint3;
            
            //spawn earthquake
            Invoke("EarthQuake",1f);
            //cooldown 8 sec
            canSkill1=false;
            canMove=false;
            Invoke("MoveDisabler",2f);
            Invoke("Skill1Cooldown",8f);
        }

        if(Input.GetKeyDown(KeyCode.Alpha2) && canSkill2){
            Skill2Effect.SetActive(true);
            anim.Play("Skill 2");
            canMove=false;
            canSkill2=false;
            Invoke("MoveDisabler",1.5f);
            Invoke("Skill2Cooldown",10f);
        }
        //attack state
        if(Input.GetMouseButtonDown(0) && canAttack){
            FirePointController.Attack();

            if(attackCounter%3==0){
                anim.Play("Attack1");
            }
            if(attackCounter%3==1){
                anim.Play("Attack2");
            }
            if(attackCounter%3==2){
                anim.Play("Attack3");
            }

            attackCounter++;
            Debug.Log(attackCounter);
            canAttack=false;
            speed=2;
            Invoke("AttackCD",0.8f);
        }

        if(Input.GetKeyDown(KeyCode.LeftControl)){
            anim.SetBool("isCrouch",true);
            speed=0;
        }

         if(Input.GetKeyUp(KeyCode.LeftControl)){
            anim.SetBool("isCrouch",false);
            speed=6f;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift)){
            anim.Play("Dive");
        }

    }
    void EarthQuake(){
            GameObject a = Instantiate(A1,new Vector3(SP1.transform.position.x,SP1.transform.position.y,SP1.transform.position.z),Quaternion.identity);
            //delay 0.5sec
            Invoke("SecondEQ",0.5f);
            
    }
    void SecondEQ(){
        GameObject b = Instantiate(A2,new Vector3(SP2.transform.position.x,SP2.transform.position.y,SP2.transform.position.z),Quaternion.identity);
            //delay 0.5sec
            Invoke("ThirdEQ",0.5f);
    }

    void ThirdEQ(){
          GameObject c = Instantiate(A3,new Vector3(SP3.transform.position.x,SP3.transform.position.y,SP3.transform.position.z),Quaternion.identity);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag=="ground"){
            grounded=true;
        }

        if(other.gameObject.tag=="Enemy"){
            Debug.Log("get hurt");
            PlayerDead();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag=="ActionSpot"){
            canAction=true;
        }
    }

    void PlayerDead(){
        anim.Play("Death");
        speed=0;
    }

    private void FixedUpdate() {
        //fall gravity
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);

    }

    //BOOLEANS

    public void AttackCD(){
        canAttack=true;
        speed=6f;
    }
    public void Skill1Cooldown(){
        canSkill1=true;
    }

    public void Skill2Cooldown(){
        canSkill2=true;
    }

    public void MoveDisabler(){
        canMove=true;
    }

}
