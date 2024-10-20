using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_PlayerName : UI_Base
{
    enum Texts
    {
        NameText
    }

    Stat _stat;

    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));
        _stat = transform.parent.GetComponent<Stat>();
    }

    void Update()
    {
        Transform parent = transform.parent;
        transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y);
        transform.rotation = Camera.main.transform.rotation;

        SetPlayerName();
    }

    public void SetPlayerName()
    {
        GetText((int)Texts.NameText).text = $"Player {_stat.ID}";
    }
}
