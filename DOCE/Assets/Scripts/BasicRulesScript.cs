////////////BasicRulesScript////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicRulesScript : MonoBehaviour
{
    public Text ruleText;
    public Text ruleText2;
    public Text subRuleText;
    public Image ruleImage;
    public GameObject blockerImage;
    public Sprite rule1Image;
    public Sprite rule2Image;
    public Button next;
    public Button previous;
    public Image pageNumberObject;
    public List<Sprite> pageNumberSpriteList;

    public void FirstRule()
    {
        pageNumberObject.sprite = pageNumberSpriteList[0];
        ruleImage.sprite = rule1Image;
        ruleImage.enabled = true;
        ruleText.enabled = true;
        ruleText2.enabled = false;
        blockerImage.SetActive(false);
        subRuleText.enabled = true;
        subRuleText.text = "<b>Basics</b>:";
        string rule = "Take turns placing a die on any available square on the grid with your number of choice." +
            "\n" +
            "\nAfter playing a die, <b> the marker moves on to of it</b>, so you know which one you played last." +
            "\n" +
            "\nOn your next turn, <b> you can't place the new die on any of the squares adjacent to your own marked die</b> (no-play zone)." +
            "\n" +
            "\n<b>Each player has their own no-play zone</b>, and are independent of each other.";
        ruleText.text = rule;
        previous.interactable = false;
        next.interactable = true;
        next.onClick.AddListener(delegate () { SecondRule(); });
    }
    public void SecondRule()
    {
        pageNumberObject.sprite = pageNumberSpriteList[1];
        ruleImage.sprite = rule2Image;
        ruleImage.enabled = true;
        ruleText.enabled = true;
        ruleText2.enabled = false;
        blockerImage.SetActive(false);
        subRuleText.enabled = true;
        subRuleText.text = "<b>Winning</b>:";
        string rule = "The first person to make a line on 4 of their own dice <b>where the points add up to 12</b> wins the round" +
            "\n"+
            "\nYou can also win with 3 of your own in a row and 1 of your opponent's die at either <b>end</b>." +
            "\n"+
            "\nIn single game mode, the player with the most points wins the game." +
            "\n"+
            "\nIf you're playing a 4-round match, the person with the most points <b>at the end of the 4th round</b> wins the game.";
        ruleText.text = rule;
        previous.interactable = true;
        next.interactable = true;
        previous.onClick.AddListener(delegate () { FirstRule(); });
        next.onClick.AddListener(delegate () { ThirdRule(); });
    }
    public void ThirdRule()
    {
        pageNumberObject.sprite = pageNumberSpriteList[2];
        ruleImage.enabled = false;
        blockerImage.SetActive(true);
        ruleText.enabled = true;
        ruleText2.enabled = false;
        subRuleText.enabled = true;
        subRuleText.text = "<b>The Blocker</b>:";
        string rule =
            "\nThe blocker eliminates the square it is played on, <b>interrupting any sequence of dice</b>" +
            "\n" +
            "\nBlockers can be played anywhere on the grid, even in your no-play zone. The marker does not move onto the blocker" +
            "\n" +
            "\n<b>1.</b> Use it <b>at the beginning of your round</b>." +
            "\n<b>2.</b> Play your numbered die as normal (You get 2 plays in one \tturn)." +
            "\n<b>3.</b> The marker moves onto <b>the new numbered die</b>.";
        ruleText.text = rule;
        previous.interactable = true;
        next.interactable = true;
        previous.onClick.AddListener(delegate () { SecondRule(); });
        next.onClick.AddListener(delegate () { FourthRule(); });
    }

    public void FourthRule()
    {
        pageNumberObject.sprite = pageNumberSpriteList[3];
        ruleImage.enabled = false;
        blockerImage.SetActive(false);
        ruleText.enabled = false;
        ruleText2.enabled = true;
        subRuleText.enabled = true;
        subRuleText.text = "<b>Scoring</b>";
        string rule = "<b>Winner:</b>" +
            "\n+ 12 points for winning the round." +
            "\n+ 2 points if all dice are yours." +
            "\n+ 5 points if you did not use your blocker die." +
            "\n+ 1 point for each empty square on yhe board." +
            "\n- 2 points for every 3 of your dice with the same number." +
            "\n<b>NOTE</b>: The loser's score is unaffected." +
            "\n" +
            "\n<b>Draw</b>:" +
            "\nBoth players get 2 points." +
            "\n+ 5 points for anyone who did not use the blocker die.";
        ruleText2.text = rule;
        previous.interactable = true;
        next.interactable = true;
        previous.onClick.AddListener(delegate () { ThirdRule(); });
        next.onClick.AddListener(delegate () { FifthRule(); });
    }

    public void FifthRule()
    {
        pageNumberObject.sprite = pageNumberSpriteList[4];
        ruleImage.enabled = false;
        blockerImage.SetActive(false);
        ruleText.enabled = false;
        ruleText2.enabled = true;
        subRuleText.enabled = true;
        subRuleText.text = "<b>Quick rules</b>";
        string rule = "- First player is alternated between rounds." +
            "\n" +
            "\n- Left click to select a die of your choice, right click to deselect it." +
            "\n" +
            "\n- Left click on the empty square you want to play your die. Once it has been played <b>it cannot be moved or changed</b>." +
            "\n (undo options will be available in the full version)." +
            "\n" +
            "\n- Playing your die ends your turn immediately.";
        ruleText2.text = rule;
        previous.interactable = true;
        next.interactable = false;
        previous.onClick.AddListener(delegate () { FourthRule(); });
    }
}
