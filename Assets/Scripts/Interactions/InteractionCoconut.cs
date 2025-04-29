using UnityEngine;
using System.Collections;
using DG.Tweening;
using FMODUnity;
using FMOD.Studio;
using rhythmhero.audio;
namespace rhythmhero
{
    public class InteractionCoconut : MonoBehaviour
    {
        [Header("交互的信息UI")][Tooltip("将会显示在交互点的正上方,比如<点击以互动/对话/聆听>等等")]
        public GameObject InteractionUI;

        // [Header("记录是否点击过")] 
        // public bool needClik;
        //这里为了方便管理，统一需要点击E进行互动, 并且**一定会有对话**

        [Header("是否已经触发过")]
        public bool hasInteracted;
        
        [Header("对话内容,没有就不填")]
        public DialogueData dialogue;

        [Header("此处**固定**的音效，，没有可以不填")] 
        public EventReference Sound;
        private EventInstance soundInstance;
        
        [Header("此处是点击互动的点击音效")]
        public EventReference interactEvent;
        
        public Transform nextInteractionPoint;
        
        // public Collider coconutCollider;


        void Start()
        {
            InteractionUI.gameObject.SetActive(false);
            
            
            GameManager.instance.interactionPoints.Add(this.gameObject);
            
        }

        void Update()
        {
            
            InteractionUI.transform.forward = Camera.main.transform.forward; //UI始终朝向摄像机
            if (inArea && GameManager.instance.gameState == GameState.ThirdPerson && !hasInteracted) 
            {
                // Debug.Log("ready to interact");
                HandleInteraction();
            }
        }

        //这个方法是交互的逻辑，**这里每个交互点需要重写**
        void HandleInteraction()
        {
            if (Input.GetKeyDown(KeyCode.E) && !hasInteracted)
            {
                // Debug.Log("dfjkdjfdklf");
                //--------这些是点击瞬间发生的事情
                hasInteracted = true; //标记为已经交互过
                InteractionUI.gameObject.SetActive(false);
                DialogueManager.instance.currentData = dialogue; //将对话传入Dialogue Manager
                dialogue.OnDialogueLineChanged += Interaction; //**订阅方法
                
                
                GameManager.instance.currentInteractionPoint = this.gameObject;//标记正在交互
                AudioManager.instance.PlayOneShot(interactEvent,this.transform.position); //播放一次点击音效
                
                
                DialogueManager.instance.StartDialogue(); //开启对话
                GameManager.instance.gameState = GameState.InDialogue; //此时将游戏的状态切换为InDialogue
            }
        }
        
        //**请注意！！加入你需要在对话中的某一句的同时，插入一个方法逻辑的话，请在这里声明public方法
        //他们的格式是 void MethodName(int currentline你想调用的对话行数) 并且再方法中判断行数是否匹配
        //并且要在点击时，订阅这个方法
        void Interaction(int currentline)
        {
            if (currentline != 4)
            {
                return;
            }
            else
            {
                // coconutCollider.gameObject.SetActive(true);

                // StartCoroutine(NextStep());

            }
            
        }
        
        private IEnumerator NextStep()
        {
            Vector3 dir = (nextInteractionPoint.transform.position - Ghost.instance.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            yield return (Ghost.instance.transform.DORotateQuaternion(lookRotation, 0.5f));
            Ghost.instance.MoveToTargetAndLookAt(nextInteractionPoint);


        }

        [SerializeField]
        private bool inArea;
        void OnTriggerEnter(Collider other)
        {
            
            if (other.gameObject.GetComponent<PlayerState>() != null && !hasInteracted)
            {
                inArea = true;
                InteractionUI.SetActive(true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<PlayerState>() != null)
            {
                inArea = false;
                InteractionUI.SetActive(false);
            }
        }
    }
}