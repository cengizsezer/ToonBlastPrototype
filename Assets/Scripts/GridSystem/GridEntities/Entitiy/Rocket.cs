using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : GridEntitiyBase
{
    [SerializeField] RocketDirection direction;
    public void OnClickRocket()
    {
        Debug.Log("rocket click");
        if (!gridController.GridInterractable) return;
        GameManager.I.CurrentLevel.MovesController.ClickedPowerUp();
        DestroyBlocksGridEvent destroyEvent = new DestroyBlocksGridEvent(EntityDestroyTypes.DestroyedByMatch);
        destroyEvent.OnStart(gridController, new List<Rocket>() { this });
    }

    public override void DestoryEntity(EntityDestroyTypes destroyType)
    {
        StartExplosion();
        base.DestoryEntity(destroyType);
    }

    private void StartExplosion()
    {
        RocketEvent destroyEvent1 = new RocketEvent(EntityDestroyTypes.DestroyedByPowerUp);
        RocketEvent destroyEvent2 = new RocketEvent(EntityDestroyTypes.DestroyedByPowerUp);

        List<IGridEntity> entitiesInDirection1 = null;
        List<IGridEntity> entitiesInDirection2 = null;

        switch (direction)
        {
            case RocketDirection.Horizontal:
                entitiesInDirection1 = gridController.GetEntitiesTowardsLeft(GridCoordinates);
                entitiesInDirection2 = gridController.GetEntitiesTowardsRight(GridCoordinates);
                CreateHorizontalVisiualRockets();
                break;
            case RocketDirection.Vertical:
                entitiesInDirection1 = gridController.GetEntitiesTowardsUp(GridCoordinates);
                entitiesInDirection2 = gridController.GetEntitiesTowardsDown(GridCoordinates);
                CreateVerticalVisiualRockets();
                break;
        }

        destroyEvent1.OnStart(gridController, entitiesInDirection1);
        destroyEvent2.OnStart(gridController, entitiesInDirection2);
    }

    private void CreateHorizontalVisiualRockets()
    {
        Vector2 rocketSize = GetComponent<RectTransform>().sizeDelta;

        IGridEntity leftEntity = PoolManager.I.ResolveFunc<IGridEntity, Rocket>(Configs.ObjectName.Horizontal);
        GameObject effectLeft = (leftEntity as PoolObject).gameObject;
       
        IGridEntity rightEntity = PoolManager.I.ResolveFunc<IGridEntity, Rocket>(Configs.ObjectName.Horizontal);
        GameObject effectRight = (rightEntity as PoolObject).gameObject;

        RectTransform layerParent = gridController.GridOverlay;

        effectRight.transform.Rotate(0, 0, 180);

        effectRight.GetComponent<RectTransform>().sizeDelta = rocketSize;
        effectLeft.GetComponent<RectTransform>().sizeDelta = rocketSize;

        effectLeft.transform.SetParent(layerParent);
        effectRight.transform.SetParent(layerParent);
    }

    private void CreateVerticalVisiualRockets()
    {
        Vector2 rocketSize = GetComponent<RectTransform>().sizeDelta;

        
        IGridEntity upEntity = PoolManager.I.ResolveFunc<IGridEntity, Rocket>(Configs.ObjectName.Vertical);
        GameObject effectUp = (upEntity as PoolObject).gameObject;

       
        IGridEntity downEntity = PoolManager.I.ResolveFunc<IGridEntity, Rocket>(Configs.ObjectName.Vertical);
        GameObject effectDown = (downEntity as PoolObject).gameObject;
       

        RectTransform layerParent = gridController.GridOverlay;

        effectUp.transform.Rotate(0, 0, 90);
        effectDown.transform.Rotate(0, 0, 270);

        effectDown.GetComponent<RectTransform>().sizeDelta = rocketSize;
        effectUp.GetComponent<RectTransform>().sizeDelta = rocketSize;

        effectUp.transform.SetParent(layerParent);
        effectDown.transform.SetParent(layerParent);
    }

    private enum RocketDirection
    {
        Horizontal,
        Vertical
    }

}
