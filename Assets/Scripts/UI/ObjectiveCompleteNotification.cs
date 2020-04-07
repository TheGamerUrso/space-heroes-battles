using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveCompleteNotification : MonoBehaviour
{
    public static ObjectiveCompleteNotification Instance;
    private Animator animator;
    public TextMeshProUGUI Title;
    public Image Completed;


    [SerializeField] private float delay;
    [SerializeField] private float timer;
    [SerializeField] private float showNext = 2;

    [SerializeField] private GameObject content;
    private List<ObjectiveData> ListOfObjectiveData;

    private bool showing;
    private float delayBetweenNotificaitons;
    private Queue<ObjectiveData> ObjectiveFinished = new Queue<ObjectiveData>();
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        timer = delay;
        animator = GetComponent<Animator>();
        PlayerData playerData =  DataController.Instance.GetPlayerData();
        if (playerData.ListOfOnGoingObjectives.Count > 0)
            ListOfObjectiveData = playerData.ListOfOnGoingObjectives;
        content.SetActive(false);

    }
    public void AddToQue(ObjectiveData objectiveData)
    {
        ObjectiveFinished.Enqueue(objectiveData);
    }
    private void Update()
    {
        if (Time.frameCount % 1 == 0)
        {
            if (timer > 0 && showing)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    showing = false;
                    animator.SetBool("Show", showing);
                    timer = delay;
                }
            }

            if (ObjectiveFinished.Count > 0 && !showing)
            {
                ObjectiveData objectiveData = ObjectiveFinished.Dequeue();
                ShowNotification(objectiveData);
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                int num = Random.Range(0, 100);
                AddToQue(new ObjectiveData(0, "test" + num, 0, num, num, "Test Notification"));
            }
        }
    }

    public void ShowNotification(ObjectiveData objectiveData)
    {
        showing = true;
        content.SetActive(true);
        Title.text = string.Format("{0} Quest Completed \n Ready to Claim", (ObjectiveType)objectiveData.objectiveType);
        animator.SetBool("Show", showing);

    }
}
