
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System.Collections;
using UnityEngine.UI;
using VRC.Udon.Common.Interfaces;

public class NewNextButton : UdonSharpBehaviour
{
    public GameObject ownerSyncCounter;

    public GameObject[] generalObject;
    public GameObject[] wakObject;
    public GameObject currentCard;
    public Animator boxAnimator;
    public AudioSource boxSound;
    public Text objectName;
    public Text leftNumberOfCards;
    public BuyButton[] buyButtons;
    public QuestionButton[] questionButtons;
    public Timer timer;
    public BoxCollider bC;
    public BoxCollider[] questionBoxCollider;
    public NewScoreSystem scoreSystem;

    [UdonSynced] public int[] cardIntArray;
    [UdonSynced] public bool isGeneral = false;
    [UdonSynced] public bool isWak = false;
    [UdonSynced] public int count;
    [UdonSynced] public bool isCardExist;
    [UdonSynced] public string currentCardName;
    private int preCount = -1;
    [UdonSynced] public string playerTag;
    [UdonSynced] private int buyButtonCount;

    public void Start()
    {
        OnCountChanged();
    }
    public void Update()
    {
        //playerTag = Networking.GetOwner(gameObject).GetPlayerTag("Team");
    }
    public override void Interact()
    {
       if(count > 30)
        {
            // count가 30넘으면 그냥 진행 못하게
            return;
        }
       else
       {
            SpawnCycle();
       }
    }
    public void SpawnCycle()
    {
        //AddingMethod();
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        Debug.Log("Player tag is " + Networking.LocalPlayer.GetPlayerTag("Team"));
        //Networking.SetOwner(Networking.LocalPlayer, ownerSyncCounter);
        //AddingMethod();
        boxSound.Play();
        IncrementCount();
    }
    public void IncrementCount()
    {
        count += 1;
        SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(SetPlayerTag));
        Debug.Log("Player tag is " + playerTag);
        RequestSerialization();
        //playerTag = Networking.GetOwner(gameObject).GetPlayerTag("Team");
        OnCountChanged();
    }

    public override void OnDeserialization()
    {
        OnCountChanged();
    }
    public void OnCountChanged()
    {
        // 오너의 플레이어태그 playerTag에 저장

        if (preCount != count)
        {
            //애니메이션 코드
            bC.enabled = false;
            boxAnimator.SetTrigger("nextButton");
            //딜레이
            SendCustomEventDelayedSeconds(nameof(OnAnimationPlayed), 0.7f);
        }
        preCount = count;
    }
    public void OnAnimationPlayed()
    {
        currentCard.SetActive(false);

        // 일반모드인지 왁모드인지 확인하고 카드 active 코드
        if (isGeneral == true)
        {
            var card = generalObject[cardIntArray[count - 1]];

            currentCardName = card.name;

            currentCard = card;

            card.SetActive(true);
        }
        else if (isWak == true)
        {
            var card = wakObject[cardIntArray[count - 1]];

            currentCardName = card.name;

            currentCard = card;

            card.SetActive(true);
        }

        //이름이랑 남은 갯수 보여주는 코드
        objectName.text = currentCardName;
        leftNumberOfCards.text = count.ToString() + " / 30";

        //AddingMethod();

        //buy button 기능들 다 초기화
        for (int i = 0; i < buyButtons.Length; i++)
        {
            buyButtons[i].buyIsPressed = false;
            buyButtons[i].DisplayNone();
            buyButtons[i].buyCollider.enabled = true;

            //question button 기능들 다 초기화
            questionButtons[i].TextToNull();
            questionBoxCollider[i].enabled = true;
        }

        //타이머 초기화
        timer.ActivateTimerMethod();

        // 다시 interact가 가능하게 만들기
        bC.enabled = true;

        // question button들이 interact가 가능하게
    }
    public void AddingMethod()
    {
        buyButtonCount = 0;
        for (int i = 0; i < buyButtons.Length; i++)
        {
            if (buyButtons[i].buyIsPressed == true)
            {
                buyButtonCount++;
                RequestSerialization();
            }
        }
        Debug.Log("Number of buttonCount increased " + buyButtonCount);
        switch (playerTag)
        {
            case "1":
                scoreSystem.count = buyButtonCount;
                //scoreSystem.IncreaseTeamOne();
                scoreSystem.SendCustomNetworkEvent(NetworkEventTarget.All, "IncreaseTeamOne");
                //counterUB.SetProgramVariable("count", count);
                //counterUB.SendCustomNetworkEvent(NetworkEventTarget.Owner, "IncreaseTeamOne");
                break;
            case "2":
                scoreSystem.count = buyButtonCount;
                //scoreSystem.IncreaseTeamTwo();
                scoreSystem.SendCustomNetworkEvent(NetworkEventTarget.All, "IncreaseTeamTwo");
                //counterUB.SetProgramVariable("count", count);
                //counterUB.SendCustomNetworkEvent(NetworkEventTarget.Owner, "IncreaseTeamTwo");w
                break;
            case "3":
                scoreSystem.count = buyButtonCount;
                //scoreSystem.IncreaseTeamThree();
                scoreSystem.SendCustomNetworkEvent(NetworkEventTarget.All, "IncreaseTeamThree");
                //counterUB.SetProgramVariable("count", count);
                //counterUB.SendCustomNetworkEvent(NetworkEventTarget.Owner, "IncreaseTeamThree");
                break;
            case "4":
                scoreSystem.count = buyButtonCount;
                //scoreSystem.IncreaseTeamFour();
                scoreSystem.SendCustomNetworkEvent(NetworkEventTarget.All, "IncreaseTeamFour");
                //counterUB.SetProgramVariable("count", count);
                //counterUB.SendCustomNetworkEvent(NetworkEventTarget.Owner, "IncreaseTeamFour");
                break;
        }
    }

    public void SetPlayerTag()
    {
        playerTag = Networking.LocalPlayer.GetPlayerTag("Team");
    }
}
