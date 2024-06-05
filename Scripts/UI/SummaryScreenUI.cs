using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using GDE.GenericSelectionUI;
using System.Linq;

public class SummaryScreenUI : SelectionUI<TextSlot>{
    [Header("Pages")]
    [SerializeField] Text pageName;
    [SerializeField] GameObject skillPage;
    [SerializeField] GameObject movesPage;

    [Header("Basic Details")]
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Image image;

    [Header("Pokemon Skills")]
    [SerializeField] Text hpText;
    [SerializeField] Text attackText;
    [SerializeField] Text defenseText;
    [SerializeField] Text spAttackText;
    [SerializeField] Text spDefenseText;
    [SerializeField] Text speedText;
    [SerializeField] Text currExpText;
    [SerializeField] Text nextLevelText;
    [SerializeField] Transform expBar;

    [Header("Pokemon Moves")]
    [SerializeField] List<Text> moveTypes;
    [SerializeField] List<Text> moveNames;
    [SerializeField] List<Text> movePPs;
    [SerializeField] GameObject moveEffectsUI;
    [SerializeField] Text moveDescription;
    [SerializeField] Text movePower;
    [SerializeField] Text moveAccuracy;

    List<TextSlot> moveSlots;
    

    Pokemon pokemon;
    bool inMoveSelection;

    public bool InMoveSelection{
        get => inMoveSelection;

        set{
            inMoveSelection = value;

            if(inMoveSelection){
                moveEffectsUI.SetActive(true);
                SetItems(moveSlots.Take(pokemon.Moves.Count).ToList());
            } else {
                moveEffectsUI.SetActive(false);
                moveDescription.text = "";
                ClaerItems();
            }
        }
    }

    private void Start(){
        moveSlots = moveNames.Select( m => m.GetComponent<TextSlot>()).ToList();
        moveEffectsUI.SetActive(false);
        moveDescription.text = "";
    }

    public void SetBasicDetails(Pokemon pokemon){
        this.pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        image.sprite = pokemon.Base.FrontSprite;
    }

    public void SetSkills(){
        hpText.text = $"{pokemon.HP} / {pokemon.MaxHp}";
        attackText.text = "" + pokemon.Attack;
        defenseText.text = "" + pokemon.Defense;
        spAttackText.text = "" + pokemon.SpAttack;
        spDefenseText.text = "" + pokemon.SpDefense;
        speedText.text = "" + pokemon.Speed;

        currExpText.text = "" + pokemon.Exp;
        nextLevelText.text = "" + (pokemon.Base.GetExpForLevel(pokemon.Level + 1) - pokemon.Exp);
        expBar.localScale = new Vector2(pokemon.GetNormalizedExp(), 1);
    }

    public void SetMoves(){
        for(int i = 0; i < moveNames.Count ; i++){
            if(i < pokemon.Moves.Count){
                var move = pokemon.Moves[i];

                moveTypes[i].text = move.Base.Type.ToString().ToUpper();
                moveNames[i].text = move.Base.Name.ToUpper();
                movePPs[i].text = $"PP {move.PP} / {move.Base.PP}";
            } else {
                moveTypes[i].text = " - ";
                moveNames[i].text = " - ";
                movePPs[i].text = " - / - ";
            }
        }
    }

    public void ShowPage(int pageNumber){
        if(pageNumber == 0){
            //Skill Page
            pageName.text = "Pokemon Skills";
            skillPage.SetActive(true);
            movesPage.SetActive(false);

            SetSkills();
        } else if(pageNumber == 1){
            //Moves Page
            pageName.text = "Pokemon Moves";
            skillPage.SetActive(false);
            movesPage.SetActive(true);

            SetMoves();
        }
    }

    public override void HandleUpdate(){
        if(InMoveSelection){
            base.HandleUpdate();
        }
    }

    public override void UpdateSelectionInUI(){
        base.UpdateSelectionInUI();
        
        var move = pokemon.Moves[selectedItem];

        moveDescription.text = move.Base.Description;
        movePower.text = "" + move.Base.Power;
        moveAccuracy.text = "" + move.Base.Accuracy;
    }
}
