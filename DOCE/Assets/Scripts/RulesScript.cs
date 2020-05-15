using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulesScript : MonoBehaviour
{
    public List<GameObject> positions;
    public GameObject board;
    public Text ruleText;
    public Sprite nonPlayingMark;
    public Sprite black3;
    public Sprite white3;
    public Sprite black5;
    public Button next;
    public Button back;
    public List<Sprite> decals;
    public Image pageNumber;

    public GameObject blocker1;
    public GameObject blocker2;

    public void Start()
    {
        ClearPositions();
    }

    public void ClearPositions()
    {
        foreach (GameObject position in positions)
        {
            position.SetActive(false);
        }
        positions[9].GetComponent<SpriteRenderer>().sprite = black3;
        positions[17].GetComponent<SpriteRenderer>().sprite = black3;
        positions[8].GetComponent<SpriteRenderer>().sprite = black5;
        blocker1.SetActive(false);
        blocker2.SetActive(false);
    }
    public void RoutineRule0()
    {
        StopAllCoroutines();
        ClearPositions();
        ruleText.text = "Welcome to DOCE";
        pageNumber.enabled = false;
        next.interactable = true;
        back.interactable = false;
        next.onClick.AddListener(delegate () { RoutineRule1(); });
        StartCoroutine(Rule0Timer());
    }
    public IEnumerator Rule0Timer()
    {
        yield return new WaitForSeconds(2);
        RoutineRule1();
    }
    public void RoutineRule1()
    {
        StopAllCoroutines();
        ClearPositions();
        ruleText.text = "Players take turns by placing a die on any available square on the grid";
        pageNumber.enabled = true;
        pageNumber.sprite = decals[0];
       // StartCoroutine(Rule1());
        next.interactable = true;
        back.interactable = true;
        
        next.onClick.AddListener(delegate () { RoutineRule2(); });
        back.onClick.AddListener(delegate () { RoutineRule0(); });

        StartCoroutine(Rule1());
    }
    public void RoutineRule2()
    {
        StopAllCoroutines();
        ClearPositions();
        ruleText.text = "You cannot place a new die on any square surrounding  your last played die";
        pageNumber.sprite = decals[1];
       // StartCoroutine(Rule2());
        next.interactable = true;
        back.interactable = true;
        next.onClick.AddListener(delegate () { RoutineRule3(); });
        back.onClick.AddListener(delegate () { RoutineRule1(); });

        StartCoroutine(Rule2());

    }
    public void RoutineRule3()
    {
        StopAllCoroutines();
        ClearPositions();
        ruleText.text = "The objective is to add up twelve (12) by placing four (4) dice of your own in any horizontal, vertical, or diagonal row";
        pageNumber.sprite = decals[2];
       // StartCoroutine(Rule3());
        next.interactable = true;
        back.interactable = true;
        next.onClick.AddListener(delegate () { RoutineRule4(); });
        back.onClick.AddListener(delegate () { RoutineRule2(); });

        StartCoroutine(Rule3());
    }
    public void RoutineRule4()
    {
        StopAllCoroutines();
        ClearPositions();

        ruleText.text = "You may also win by adding one (1) of your opponent's die at either end to your own row";
        pageNumber.sprite = decals[3];
        positions[9].GetComponent<SpriteRenderer>().sprite = white3;
       // StartCoroutine(Rule4());
        next.interactable = true;
        back.interactable = true;
        next.onClick.AddListener(delegate () { RoutineRule5(); });
        back.onClick.AddListener(delegate () { RoutineRule3(); });

        StartCoroutine(Rule4());
    }
    public void RoutineRule5()
    {
        StopAllCoroutines();
        ClearPositions();
        ruleText.text = "Each player has one (1) blocker piece";
        pageNumber.sprite = decals[4];
        next.interactable = true;
        back.interactable = true;
        next.onClick.AddListener(delegate () { RoutineRule6(); });
        back.onClick.AddListener(delegate () { RoutineRule4(); });
        StartCoroutine(Rule5());


    }
    public void RoutineRule6()
    {
        StopAllCoroutines();
        ClearPositions();
        ruleText.text = "The blocker can be placed in any square surrounding your last played position";
        pageNumber.sprite = decals[5];
        next.interactable = false;
        back.interactable = true;
        // next.onClick.AddListener(delegate () { Rule5(); });
        back.onClick.AddListener(delegate () { RoutineRule5(); });
        StartCoroutine(Rule6());
    }
    public IEnumerator Rule1()
    {
        
        yield return new WaitForSeconds(1);
        positions[9].SetActive(true);
        yield return new WaitForSeconds(1);
        positions[16].SetActive(true);
        yield return new WaitForSeconds(1);
        positions[17].SetActive(true);
        yield return new WaitForSeconds(1);
        positions[2].SetActive(true);
        yield return new WaitForSeconds(1);
        positions[8].SetActive(true);
        yield return new WaitForSeconds(1);
        positions[11].SetActive(true);


    }
    public IEnumerator Rule2()
    {
        
        yield return new WaitForSeconds(0.5f);
        positions[13].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        positions[7].GetComponent<SpriteRenderer>().sprite = nonPlayingMark;
        positions[8].GetComponent<SpriteRenderer>().sprite = nonPlayingMark;
        positions[9].GetComponent<SpriteRenderer>().sprite = nonPlayingMark;
        positions[12].GetComponent<SpriteRenderer>().sprite = nonPlayingMark;
        positions[14].GetComponent<SpriteRenderer>().sprite = nonPlayingMark;
        positions[17].GetComponent<SpriteRenderer>().sprite = nonPlayingMark;
        positions[18].GetComponent<SpriteRenderer>().sprite = nonPlayingMark;
        positions[19].GetComponent<SpriteRenderer>().sprite = nonPlayingMark;

        positions[7].SetActive(true);
        positions[8].SetActive(true);
        positions[9].SetActive(true);
        positions[12].SetActive(true);
        positions[14].SetActive(true);
        positions[17].SetActive(true);
        positions[18].SetActive(true);
        positions[19].SetActive(true);

    }
    public IEnumerator Rule3()
    {
        

        yield return new WaitForSeconds(0.5f);
        positions[9].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        positions[13].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        positions[17].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        positions[21].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        positions[1].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        positions[2].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        positions[3].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        positions[4].SetActive(true);


    }
    public IEnumerator Rule4()
    {
        ClearPositions();
        yield return new WaitForSeconds(0.5f);
        positions[9].GetComponent<SpriteRenderer>().sprite = white3;
        positions[9].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        positions[13].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        positions[17].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        positions[21].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        positions[16].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        positions[11].SetActive(true);
        yield return new WaitForSeconds(0.25f);
        positions[6].SetActive(true);
        


    }
    public IEnumerator Rule5()
    {
        ClearPositions();
        blocker1.SetActive(true);
        blocker2.SetActive(true);

        yield return new WaitForSeconds(1);
        positions[9].SetActive(true);
        yield return new WaitForSeconds(1);
        positions[2].SetActive(true);
        yield return new WaitForSeconds(1);
        positions[21].SetActive(true);
        yield return new WaitForSeconds(1);
        blocker1.SetActive(false);
        positions[8].GetComponent<SpriteRenderer>().sprite = nonPlayingMark;
        positions[8].SetActive(true);
        ruleText.text = "It can be used at any time before placing a die and cannot be moved afterwards";
        yield return new WaitForSeconds(1);
        positions[16].SetActive(true);
        yield return new WaitForSeconds(1);
        positions[0].SetActive(true);

        

    }
    public IEnumerator Rule6()
    {
        ClearPositions();
        blocker1.SetActive(true);
        blocker2.SetActive(true);

        yield return new WaitForSeconds(1);
        positions[9].SetActive(true);
        yield return new WaitForSeconds(1);
        positions[16].SetActive(true);
        yield return new WaitForSeconds(1);
        positions[21].SetActive(true);
        yield return new WaitForSeconds(1);
        positions[1].SetActive(true);
        yield return new WaitForSeconds(1);
        blocker2.SetActive(false);
        positions[17].GetComponent<SpriteRenderer>().sprite = nonPlayingMark;
        positions[17].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        positions[8].SetActive(true);
        yield return new WaitForSeconds(1);
        positions[7].SetActive(true);
        positions[7].GetComponent<SpriteRenderer>().sprite = nonPlayingMark;
        blocker1.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        positions[11].SetActive(true);
        yield return new WaitForSeconds(3);
        ruleText.text = "If the blocker is used (-5) points will be substracted to your score";
    }
    public void ExitButton()
    {
        this.StopAllCoroutines();
        
    }

}
