using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Game : UI_Scene
{
    enum Buttons
    {
        ConnectButton,
        //SkillButton_1,
        //SkillButton_2,
        //SkillButton_3,
        //SkillButton_4,
    }

    enum Inputs
    {
        //ChatInput,
    }

    enum Texts
    {
        ConnectButtonText,
        //ChatText,
        DelayText,
        //HPText,
        //MPText,
        //SkillCoolText_1,
        //SkillCoolText_2,
        //SkillCoolText_3,
        //SkillCoolText_4,
    }

    enum GameObjects
    {
        //HPBar,
        //MPBar,
        //Content,
    }

    Stat _stat;

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        //Bind<TMP_InputField>(typeof(Inputs));
        Bind<TMP_Text>(typeof(Texts));
        //Bind<GameObject>(typeof(GameObjects));

        GetButton((int)Buttons.ConnectButton).gameObject.AddUIEvent(ConnectToServer);

        StartCoroutine(WaitforPlayer());
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    TMP_InputField chat = Get<TMP_InputField>((int)Inputs.ChatInput);
        //    if (chat.gameObject.activeSelf && chat.text != "")
        //    {
        //        EnterChat(Managers.Game.PlayerId ,chat.text);
        //        chat.gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        chat.gameObject.SetActive(true);
        //        chat.ActivateInputField();
        //    }
        //}

        SetDelay(Managers.Network.Delay);

        //if (_stat != null)
        //{
        //    float hpRatio = (float)_stat.HP / (float)_stat.MaxHP;
        //    float mpRatio = (float)_stat.MP / (float)_stat.MaxMP;
        //    SetHPRatio(hpRatio);
        //    SetMPRatio(mpRatio);
        //}
    }

    IEnumerator WaitforPlayer()
    {
        yield return new WaitUntil(() => Managers.Game.GetPlayer() != null);
        _stat = Managers.Game.GetPlayer().GetComponent<Stat>();
        yield return null;
    }

    public void ConnectToServer(PointerEventData data)
    {
        Managers.Network.Init();
    }

    //public void EnterChat(int playerId, string text)
    //{
    //    string chatText = GetText((int)Texts.ChatText).text;
    //    chatText += "[Player " + playerId + "]: " + text + "\n";
    //    GetText((int)Texts.ChatText).text = chatText;

    //    float textHeight = GetText((int)Texts.ChatText).GetComponent<RectTransform>().sizeDelta.y;
    //    Vector2 contentRect = GetObject((int)GameObjects.Content).GetComponent<RectTransform>().sizeDelta;
    //    if(contentRect.y < textHeight)
    //    {
    //        GetObject((int)GameObjects.Content).GetComponent<RectTransform>().sizeDelta = new Vector2(contentRect.x, contentRect.y + 18.5f);
    //    }

    //    Get<TMP_InputField>((int)Inputs.ChatInput).text = "";
    //    Managers.Network.SendChatPacket(text);
    //}

    public void SetDelay(float delay)
    {
        GetText((int)Texts.DelayText).text = "Delay: " + delay + "ms";
        //GetText((int)Texts.DelayText).text = "";
    }

    //public void SetHPRatio(float ratio)
    //{
    //    GetObject((int)GameObjects.HPBar).GetComponent<Slider>().value = ratio;
    //    GetText((int)Texts.HPText).text = _stat.HP + " / " + _stat.MaxHP;
    //}

    //public void SetMPRatio(float ratio)
    //{
    //    GetObject((int)GameObjects.MPBar).GetComponent<Slider>().value = ratio;
    //    GetText((int)Texts.MPText).text = _stat.MP + " / " + _stat.MaxMP;
    //}
}
