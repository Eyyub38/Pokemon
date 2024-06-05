using UnityEngine;
using System.Linq;
using System.Collections;
using GDEUtills.StateMachine;
using System.Collections.Generic;

public class RunTurnState : State<BattleSystem>{
    public static RunTurnState i{get; private set;}
    BattleSystem battleSystem;

    BattleUnit playerUnit;
    BattleUnit enemyUnit;
    PartyScreen partyScreen;
    BattleDialogBox dialogBox;
    PokemonParty playerParty;
    PokemonParty trainerParty;
    Pokemon wildPokemon;
    bool IsBattleOver;
    bool IsTrainerBattle;

    private void Awake(){
        i = this;
    }

    public override void Enter(BattleSystem owner){
        battleSystem = owner;

        playerUnit = battleSystem.PlayerUnit;
        enemyUnit = battleSystem.EnemyUnit;
        dialogBox = battleSystem.DialogBox;
        partyScreen = battleSystem.PartyScreen;
        playerParty = battleSystem.PlayerParty;
        trainerParty = battleSystem.TrainerParty;
        wildPokemon = battleSystem.WildPokemon;
        IsBattleOver = battleSystem.IsBattleOver;
        IsTrainerBattle = battleSystem.IsTrainerBattle;

        StartCoroutine(RunTurns(battleSystem.SelectedAction));
    }

     IEnumerator RunTurns(BattleAction playerAction){

        if(playerAction==BattleAction.Move){
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[battleSystem.SelectedMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            int playerMovePriority=playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority=enemyUnit.Pokemon.CurrentMove.Base.Priority;

            //Check who goes first
            bool playerGoesFirst = true;
            if(enemyMovePriority > playerMovePriority){
                playerGoesFirst = false;
            }else if(enemyMovePriority == playerMovePriority){
                playerGoesFirst= playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;
            }

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondPokemon =secondUnit.Pokemon;

            //First Turn
            yield return RunMove(firstUnit,secondUnit,firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if(IsBattleOver) yield break;

            if(secondPokemon.HP > 0){
                //Second Turn
                yield return RunMove(secondUnit,firstUnit,secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if(IsBattleOver) yield break;
            }
        } else {
            if(playerAction == BattleAction.SwitchPokemon){
                yield return battleSystem.SwitchPokemon(battleSystem.SelectedPokemon);

            } else if(playerAction==BattleAction.UseItem){
                if(battleSystem.SelectedItem is PokeballItem){
                    yield return battleSystem.ThrowPokeball(battleSystem.SelectedItem as PokeballItem);
                    if(battleSystem.IsBattleOver) yield break;
                } else {
                    //This is handled from item screen, so do nothing and skip to enemy move
                }
            } else if(playerAction==BattleAction.Run){
                yield return TryToEscape();
            }

            //Enemy Turn
            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit,playerUnit,enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if(IsBattleOver) yield break;
        }

        if(!IsBattleOver){
            battleSystem.StateMachine.ChangeState(ActionSelectionState.i);
        }
    }

    IEnumerator RunMove(BattleUnit sourceUnit,BattleUnit targetUnit,Move move){
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        if(!canRunMove){
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.WaitForHPUpdate();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Pokemon);

        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}.");

        if(CheckIfMoveHits(move,sourceUnit.Pokemon,targetUnit.Pokemon)){
            sourceUnit.PlayAttackAnimation();
            AudioManager.i.PlaySFX(move.Base.Sound);

            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();

            AudioManager.i.PlaySFX(AudioID.Hit);

            if(move.Base.Category==MoveCategory.Status){
                yield return RunMoveEffects(move.Base.Effects,sourceUnit.Pokemon,targetUnit.Pokemon,move.Base.Target);
            } else {

                var damageDetails=targetUnit.Pokemon.TakeDamage(move,sourceUnit.Pokemon);
                yield return targetUnit.Hud.WaitForHPUpdate();
                yield return ShowDamageDetails(damageDetails);
            }

            if(move.Base.Secondaries!=null && move.Base.Secondaries.Count> 0 && targetUnit.Pokemon.HP>0){
                foreach (var secondary in move.Base.Secondaries)
                {
                    var rnd =UnityEngine.Random.Range(1,101);
                    if(rnd<=secondary.Chance){
                        yield return RunMoveEffects(secondary,sourceUnit.Pokemon,targetUnit.Pokemon,secondary.Target);
                    }
                }
            }

            if(targetUnit.Pokemon.HP<=0){
                yield return HandlePokemonFainted(targetUnit);
            }
        } else {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}'s attack missed.");
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects,Pokemon source,Pokemon target,MoveTarget moveTarget){
        //Stat Boosting
            if(effects.Boosts!=null){
                if(moveTarget==MoveTarget.Foe){
                    target.ApplyBoosts(effects.Boosts);
                } else {
                    source.ApplyBoosts(effects.Boosts);
                }
            }
            //Status Condition
            if(effects.Status!=ConditionID.none){
                target.SetStatus(effects.Status);
            }
            //Volatile Status Condition
            if(effects.VoltileStatus!=ConditionID.none){
                target.SetVolatileStatus(effects.VoltileStatus);
            }

            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(target);
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit){
        if(IsBattleOver) yield break;

        //Statues like burn or poison will hurt pokemon after turn
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.WaitForHPUpdate();
        
        if(sourceUnit.Pokemon.HP<=0){
            yield return HandlePokemonFainted(sourceUnit);
        }
    }
    bool CheckIfMoveHits(Move move,Pokemon source,Pokemon target){
        if(move.Base.AlwaysHit){ return true;}

        float moveAccuracy=move.Base.Accuracy;

        int accuracy =source.StatBoosts[Stat.Accuracy];
        int evasion =target.StatBoosts[Stat.Evasion];
        
        var boostValues=new float[]{1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f};
        
        if(accuracy>0){
            moveAccuracy*=boostValues[accuracy];
        } else {
            moveAccuracy/=boostValues[-accuracy];
        }
    
        if (evasion >0){
            moveAccuracy/=boostValues[evasion];
        } else {
            moveAccuracy*=boostValues[-evasion];
        }

        return UnityEngine.Random.Range(1,101) <= moveAccuracy;
    }

    IEnumerator ShowStatusChanges(Pokemon pokemon){
        while(pokemon.StatusChanges.Count>0){
            var message=pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit){
        yield return dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.Name} fainted.");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        if(!faintedUnit.IsPlayerUnit){
            bool battleWon = true;
            if(IsTrainerBattle){
                battleWon = trainerParty.GetHealthyPokemon() == null;
            }

            if(battleWon){
                if(IsTrainerBattle){
                    AudioManager.i.PlayMusic(battleSystem.TrainerBattleWonMusic);
                } else {
                    AudioManager.i.PlayMusic(battleSystem.WildBattleWonMusic);
                }
            }
            //Exp Gain
            int expYield = faintedUnit.Pokemon.Base.ExpYield;
            int enemyLevel = faintedUnit.Pokemon.Level;
            float trainerBonus = (IsTrainerBattle)? 1.5f : 1f;

            int expGain = Mathf.FloorToInt(expYield * enemyLevel * trainerBonus) / 7;
            playerUnit.Pokemon.Exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} gained {expGain} exp.");
            yield return playerUnit.Hud.SetExpSmooth();

            //Check Level Up
            while (playerUnit.Pokemon.CheckForLevelUp()){
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} grew {playerUnit.Pokemon.Level}");

                //Try to learn a new move
                var newMove = playerUnit.Pokemon.GetLearnableMoveCurrentLevel();
                if(newMove != null){
                    if(playerUnit.Pokemon.Moves.Count < PokemonBase.MaxNumberOfMoves){
                        
                        playerUnit.Pokemon.LearnMove(newMove.Base);
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} is learned {newMove.Base.Name}.");
                        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

                    } else {
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} trying to learn {newMove.Base.Name}.");
                        yield return dialogBox.TypeDialog($"But it cannot learn more than {PokemonBase.MaxNumberOfMoves} moves.");
                        yield return dialogBox.TypeDialog($"Choose a move to forget.");

                        MoveToForgetState.i.CurrentMoves = playerUnit.Pokemon.Moves.Select(x => x.Base).ToList();
                        MoveToForgetState.i.NewMove = newMove.Base;
                        yield return GameController.Instance.StateMachine.PushAndWait(MoveToForgetState.i);

                        int moveIndex = MoveToForgetState.i.Selection;

                        if(moveIndex == PokemonBase.MaxNumberOfMoves || moveIndex == -1){
                            //Don't learn any move
                            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} did not learn {newMove.Base.Name}.");
                        } else {
                            //Forget the selected move and learn the new move
                            var selectedMove = playerUnit.Pokemon.Moves[moveIndex].Base;
                            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} forgot {selectedMove.Name} and learn {newMove.Base.Name}");
                            playerUnit.Pokemon.Moves[moveIndex] = new Move(newMove.Base);
                        }
                    }
                }

                yield return playerUnit.Hud.SetExpSmooth(true);
            }

            yield return new WaitForSeconds(1f);
        }
        yield return CheckForBattleOver(faintedUnit);
    }

    IEnumerator CheckForBattleOver(BattleUnit faintedUnit){
        if(faintedUnit.IsPlayerUnit){
            var nextPokemon = playerParty.GetHealthyPokemon();
            if(nextPokemon!=null){
                yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
                yield return battleSystem.SwitchPokemon(PartyState.i.SelectedPokemon);
            } else {
                battleSystem.BattleOver(false);
            }
        } else{
            if(!IsTrainerBattle){
                battleSystem.BattleOver(true);
            } else {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if(nextPokemon!=null){
                    AboutToUseState.i.NewPokemon = nextPokemon;
                    yield return battleSystem.StateMachine.PushAndWait(AboutToUseState.i);
                } else {
                    battleSystem.BattleOver(true);
                }
            }
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails){
        if(damageDetails.Critical > 1f){
            yield return dialogBox.TypeDialog($"A critical hit!");
        }
        if(damageDetails.TypeEffectiveness > 1f){
            yield return dialogBox.TypeDialog($"It's super effective!");
        } else if(damageDetails.TypeEffectiveness < 1f){
            yield return dialogBox.TypeDialog($"It's not very effective.");
        }
    }

    IEnumerator TryToEscape(){
        if(IsTrainerBattle){
            yield return dialogBox.TypeDialog($"You can't run from trainer battle!");
            yield break;
        }

        ++battleSystem.EscapeAttemps;
        int playerSpeed = playerUnit.Pokemon.Speed;
        int enemySpeed = enemyUnit.Pokemon.Speed;

        if(enemySpeed < playerSpeed){
            yield return dialogBox.TypeDialog($"Run away safely!");
            battleSystem.BattleOver(true);
        } else {
            float escapeRate = ( playerSpeed * 128 ) / enemySpeed + 30 * battleSystem.EscapeAttemps;
            escapeRate = escapeRate % 256;

            if(UnityEngine.Random.Range(0,255) < escapeRate){
                yield return dialogBox.TypeDialog($"Run away safely!");
                battleSystem.BattleOver(true);
            } else {
                yield return dialogBox.TypeDialog($"Can't escape!");
            }
        }       
    }
}
