using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MovesController
{
    public static readonly int MinGroupSizeForExplosion = 2;
    public int MovesLeft { get; private set; }

    private GridController gridController;
    private GridEntitySpawnController gridEntitySpawner;
    private TMPro.TMP_Text movesLeftText;

    public MovesController(GridController gridController, GridEntitySpawnController entitySpawner, MovesControllerSettings settings, MovesControllerSceneReferences references)
    {
        this.gridController = gridController;
        this.gridEntitySpawner = entitySpawner;
        this.movesLeftText = references.MovesLeftText;

        this.gridController.OnGridInterractable.AddListener(this.OnGridReadyForNextMove);
        MovesLeft = settings.MoveCount;
        TryLoadMovesLeftSaveData();
        UpdateMovesLeftUiText();
    }

    public bool TryMakeMatchMove(Block blockEntity)
    {
        if (MovesLeft == 0) return false;
        if (blockEntity.CurrentMatchGroup.Count < MinGroupSizeForExplosion) return false;

        Vector2Int matchClickCoordinates = blockEntity.GridCoordinates;

        BlockMatchCondition? condition = blockEntity.ActiveBlockCondition();
        using MatchGridEvent matchEvent = new MatchGridEvent(blockEntity.EntityTransform.position, matchClickCoordinates, condition);
        matchEvent.OnStart(gridController, blockEntity.CurrentMatchGroup);

        MovesLeft--;
        UpdateMovesLeftUiText();
        return true;
    }

    public void ClickedPowerUp()
    {
        MovesLeft--;
        UpdateMovesLeftUiText();
    }

    private void TryLoadMovesLeftSaveData()
    {
        if (!LevelSaveData.Data.HasLevelSaved) return;
        //MovesLeft = LevelSaveData.Data.MovesLeft;
    }

    private void OnGridReadyForNextMove()
    {
        if (MovesLeft != 0) return;
        GameManager.I.CurrentLevel.LevelFailed();
    }
    private void UpdateMovesLeftUiText()
    {
        movesLeftText.text = MovesLeft.ToString();
    }
}
