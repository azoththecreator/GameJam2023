using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] float timer = 0;
    WaitForSeconds second;
    WaitForSeconds half;
    [SerializeField] bool pause = false;
    [SerializeField] int day = 1;
    [SerializeField] int cooldown = 0;

    [SerializeField] int happiness = 0;

    int full = 3;
    int fun = 3;
    int clean = 3;
    int stress = 0;

    [SerializeField] GameObject mainMenu;
    [SerializeField] Transform stats;
    [SerializeField] TextMeshProUGUI fullText;
    [SerializeField] TextMeshProUGUI funText;
    [SerializeField] TextMeshProUGUI cleanText;
    [SerializeField] TextMeshProUGUI stressText;
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] GameObject speakerButton;

    [SerializeField] GameObject textBox;
    [SerializeField] TextMeshProUGUI textBoxText;
    [SerializeField] GameObject acceptButton;
    [SerializeField] GameObject declineButton;

    string fullActString = "THEY WANT TO\nGIVE YOU FOOD!";
    string funActString = "THEY WANT TO\nPLAY WITH YOU!";
    string cleanActString = "THEY WANT TO\nMAKE YOU CLEAN!";
    [SerializeField] string suggestion = "";

    string sadString = "THEY SMILE AT YOU.";
    string normalString = "THEY ARE PROUD OF YOU!";
    string happyString = "THEY LOOK HAPPY!";
    bool accepted = false;

    string deadEnding = "YOU DIED OUT OF\n";
    string sadEnding = "YOU COULD DO BETTER...";
    string normalEnding = "YOU MADE THEIR DAY!";
    string happyEnding = "THEY ARE THE\nHAPPIEST PERSON!";

    [SerializeField] Transform focus;
    [SerializeField] Image characterImage;
    [SerializeField] Image foodImage;
    [SerializeField] Image showerImage;
    [SerializeField] Image waterImage;
    [SerializeField] Image ballImage;

    bool eating = false;
    bool dead = false;
    bool happy = false;

    [SerializeField] GameObject replayButton;
    [SerializeField] Image replayFade;

    [SerializeField] Sprite egg0;
    [SerializeField] Sprite egg1;
    [SerializeField] Sprite characterIdle0;
    [SerializeField] Sprite characterIdle1;
    [SerializeField] Sprite characterEat;
    [SerializeField] Sprite characterHappy;
    [SerializeField] Sprite characterDie;

    [SerializeField] Sprite food0;
    [SerializeField] Sprite food1;
    [SerializeField] Sprite food2;

    [SerializeField] Sprite water0;
    [SerializeField] Sprite water1;

    [SerializeField] Sprite ball0;
    [SerializeField] Sprite ball1;

    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource fx;
    [SerializeField] AudioSource act;

    [SerializeField] AudioClip bgmDie;
    [SerializeField] AudioClip bgmSad;
    [SerializeField] AudioClip bgmNormal;
    [SerializeField] AudioClip bgmHappy;

    private void Start()
    {
        second = new WaitForSeconds(1f);
        half = new WaitForSeconds(.5f);
    }

    public void GameStart()
    {
        StartCoroutine(GameStartCoroutine());
    }

    IEnumerator GameStartCoroutine()
    {
        mainMenu.SetActive(false);
        characterImage.sprite = egg1;
        for (int i = 0; i < 5; i++)
        {
            stats.GetChild(i).gameObject.SetActive(true);
            FXPlay();
            yield return second;
        }
        speakerButton.SetActive(true);
        FXPlay();

        characterImage.sprite = characterIdle0;
        bgm.Play();

        StartCoroutine(TimerCount());
        StartCoroutine(CharacterAnimation());
    }

    IEnumerator TimerCount()
    {
        yield return second;

        if (!pause)
        {
            timer++;
            if (cooldown > 0)
            {
                cooldown--;
                if (cooldown == 0)
                    speakerButton.GetComponent<Button>().interactable = true;
            }

            if (timer % 30 == 0)
            {
                day++;
                dayText.text = "DAY " + day.ToString();
            }
            if (day != 4)
            {
                if (timer % 10 == 0)
                {
                    Act(false);
                }
                if (timer % 5 == 0)
                {
                    int act = 0;
                    if (full <= 5 && fun <= 5 && clean <= 5)
                    {
                        act = Random.Range(0, 3);
                    }
                    else if (full > 5)
                        act = 0;
                    else if (fun > 5)
                        act = 1;
                    else if (clean > 5)
                        act = 2;

                    switch (act)
                    {
                        case 0:
                            StatUpdate("full", false);
                            break;
                        case 1:
                            StatUpdate("fun", false);
                            break;
                        case 2:
                            StatUpdate("clean", false);
                            break;
                    }
                }
            }
            else
            {
                Ending();
            }
        }
        StartCoroutine(TimerCount());
    }

    IEnumerator CharacterAnimation()
    {
        yield return half;

        if (eating)
        {
            characterImage.sprite = characterEat;
            yield return half;
        }
        else if (happy)
        {
            characterImage.sprite = characterHappy;
            yield return half;
        }
        else if (dead)
        {
            characterImage.sprite = characterDie;
            yield return half;
        }
        else
        {
            characterImage.sprite = characterIdle1;
            yield return half;
        }
        if (dead)
            characterImage.sprite = characterDie;
        else
            characterImage.sprite = characterIdle0;

        StartCoroutine(CharacterAnimation());
    }

    void TextBoxUpdate(string sentence)
    {
        textBox.SetActive(true);
        textBoxText.text = sentence;
        Typing(textBoxText);
    }

    void Typing(TextMeshProUGUI text)
    {
        text.maxVisibleCharacters = 0;
        DOTween.To(x => text.maxVisibleCharacters = (int)x, 0f, text.text.Length, 1).SetEase(Ease.Linear);
    }

    void StatUpdate(string stat, bool increase)
    {
        bool over = false;
        bool barely = false;

        switch (stat)
        {
            case "full":
                if (increase)
                {
                    full += 3;
                    if (full > 5)
                        stress += full - 5;
                }
                else
                {
                    if (full > 6)
                        over = true;
                    else if (full == 6)
                        barely = true;

                    full--;
                    if (full == 0)
                        Die("STARVATION");
                }
                break;

            case "fun":
                if (increase)
                {
                    fun += 3;
                    if (fun > 5)
                        stress += fun - 5;
                }
                else
                {
                    if (fun > 6)
                        over = true;
                    else if (fun == 6)
                        barely = true;

                    fun--;
                    if (fun == 0)
                        Die("BOREDOM");
                }
                break;

            case "clean":
                if (increase)
                {
                    clean += 3;
                    if (clean > 5)
                        stress += clean - 5;
                }
                else
                {
                    if (clean > 6)
                        over = true;
                    else if (clean == 6)
                        barely = true;

                    clean--;
                    if (clean == 0)
                        Die("DISEASE");
                }
                break;
        }

        if (!increase)
        {
            if (!over && !barely)
            {
                stress--;
                if (stress < 0)
                    stress = 0;
            }
            else if (over)
            {
                stress++;
                if (stress >= 5)
                    Die("STRESS");
            }
        }

        StatTextUpdate();
    }

    void StatTextUpdate()
    {
        fullText.text = full.ToString() + "/5";
        funText.text = fun.ToString() + "/5";
        cleanText.text = clean.ToString() + "/5";
        stressText.text = stress.ToString() + "/5";
    }

    void Act(bool call)
    {
        pause = true;
        int act = 0;

        if (!call)
            act = Random.Range(0, 3);
        else
        {
            if (full <= fun && full <= clean)
                act = 0;
            else if (fun <= full && fun <= clean)
                act = 1;
            else if (clean <= full && clean <= fun)
                act = 2;
        }
        switch (act)
        {
            case 0:
                TextBoxUpdate(fullActString);
                suggestion = "full";
                break;
            case 1:
                TextBoxUpdate(funActString);
                suggestion = "fun";
                break;
            case 2:
                TextBoxUpdate(cleanActString);
                suggestion = "clean";
                break;
        }

        StartCoroutine(ButtonActivateCoroutine());
    }

    IEnumerator ButtonActivateCoroutine()
    {
        yield return second;
        yield return half;

        if (!dead)
            ButtonActivate(true);
    }

    void ButtonActivate(bool activation)
    {
        acceptButton.SetActive(activation);
        declineButton.SetActive(activation);
    }

    public void Accept()
    {
        foodImage.sprite = food0;
        ballImage.sprite = ball0;
        waterImage.sprite = water0;

        accepted = true;

        StartCoroutine(ActCoroutine(suggestion));

        ButtonActivate(false);
    }

    public void Decline()
    {
        IsSatisfied(false);
        ButtonActivate(false);
    }

    IEnumerator ActCoroutine(string act)
    {
        switch (act)
        {
            case "full":
                focus.DOLocalMoveX(30, 1).SetEase(Ease.Linear);
                foodImage.transform.DOLocalMoveX(-80, 1).SetEase(Ease.Linear);
                yield return second;
                eating = true;
                yield return second;
                yield return half;
                foodImage.sprite = food1;
                yield return second;
                yield return half;
                foodImage.sprite = food2;
                yield return second;
                eating = false;
                focus.DOLocalMoveX(0, 1).SetEase(Ease.Linear);
                foodImage.transform.DOLocalMoveX(-120, 1).SetEase(Ease.Linear);
                break;
            case "fun":
                focus.DOLocalMoveY(-45, 1).SetEase(Ease.Linear);
                yield return second;
                ballImage.transform.DOLocalMoveY(22, 1).SetEase(Ease.InCubic);
                yield return second;
                ballImage.sprite = ball1;
                ballImage.transform.DOLocalMoveY(70, 1);
                yield return second;
                ballImage.sprite = ball0;
                ballImage.transform.DOLocalMoveY(22, 1).SetEase(Ease.InCubic);
                yield return second;
                ballImage.sprite = ball1;
                ballImage.transform.DOLocalMoveY(110, 1);
                yield return second;
                focus.DOLocalMoveY(0, 1).SetEase(Ease.Linear);
                break;
            case "clean":
                focus.DOLocalMoveY(-45, 1).SetEase(Ease.Linear);
                showerImage.transform.DOLocalMoveY(55, 1).SetEase(Ease.Linear);
                yield return second;
                yield return half;
                waterImage.gameObject.SetActive(true);
                yield return half;
                waterImage.sprite = water1;
                yield return half;
                waterImage.sprite = water0;
                yield return half;
                waterImage.sprite = water1;
                yield return half;
                waterImage.sprite = water0;
                yield return half;
                waterImage.sprite = water1;
                yield return half;
                waterImage.gameObject.SetActive(false);
                yield return half;
                focus.DOLocalMoveY(0, 1).SetEase(Ease.Linear);
                showerImage.transform.DOLocalMoveY(100, 1).SetEase(Ease.Linear);
                break;
        }

        StatUpdate(suggestion, true);
        IsSatisfied(true);
    }

    void IsSatisfied(bool satisfied)
    {
        if (satisfied)
        {
            happiness++;
            if (happiness > 4)
                happiness = 4;
        }
        else if (!satisfied)
        {
            happiness--;
            if (happiness < 1)
                happiness = 1;
        }

        if (stress < 5)
        {
            switch (happiness)
            {
                case 1:
                    TextBoxUpdate(sadString);
                    break;
                case 2:
                case 3:
                    TextBoxUpdate(normalString);
                    break;
                case 4:
                    TextBoxUpdate(happyString);
                    break;
            }
            StartCoroutine(TextBoxDeactivate());
        }
        else
            Die("STRESS");
    }

    IEnumerator TextBoxDeactivate()
    {
        if (accepted)
            happy = true;
        yield return second;
        yield return second;
        if (accepted)
        {
            happy = false;
            accepted = false;
        }    
        textBox.SetActive(false);
        pause = false;
    }

    public void Call()
    {
        Act(true);
        cooldown = 10;
    }

    void Ending()
    {
        pause = true;
        StartCoroutine(ReplayButtonCoroutine());

        switch (happiness)
        {
            case 1:
                TextBoxUpdate(sadEnding);
                bgm.clip = bgmSad;
                bgm.Play();
                break;
            case 2:
            case 3:
                TextBoxUpdate(normalEnding);
                bgm.clip = bgmNormal;
                bgm.Play();
                break;
            case 4:
                TextBoxUpdate(happyEnding);
                bgm.clip = bgmHappy;
                bgm.Play();

                happy = true;
                break;
        }
    }

    void Die(string reason)
    {
        dead = true;
        pause = true;
        StartCoroutine(ReplayButtonCoroutine());

        deadEnding = deadEnding + reason + "...";

        characterImage.sprite = characterDie;
        TextBoxUpdate(deadEnding);

        bgm.clip = bgmDie;
        bgm.Play();
    }

    public void FXPlay()
    {
        fx.Play();
    }

    public void ActPlay()
    {
        act.Play();
    }

    IEnumerator ReplayButtonCoroutine()
    {
        yield return second;
        replayButton.SetActive(true);
    }

    public void Replay()
    {
        StartCoroutine(ReplayCoroutine());
    }

    IEnumerator ReplayCoroutine()
    {
        replayFade.gameObject.SetActive(true);
        replayFade.DOFade(1, 2.5f);
        yield return second;
        yield return second;
        yield return second;
        SceneManager.LoadScene(0);
    }
}
