﻿using LabFusion.Marrow.Integration;
using LabFusion.Player;
using LabFusion.Utilities;

namespace LabFusion.Entities;

public class OwnershipEventsExtender : EntityComponentArrayExtender<OwnershipEvents>
{
    protected override void OnRegister(NetworkEntity networkEntity, OwnershipEvents[] components)
    {
        networkEntity.OnEntityOwnershipTransfer += OnEntityOwnershipTransfer;

        // Invoke the event if the owner has already been set
        if (networkEntity.OwnerId != null)
        {
            OnEntityOwnershipTransfer(networkEntity, networkEntity.OwnerId);
        }
    }

    protected override void OnUnregister(NetworkEntity networkEntity, OwnershipEvents[] components)
    {
        networkEntity.OnEntityOwnershipTransfer -= OnEntityOwnershipTransfer;
    }

    private void OnEntityOwnershipTransfer(NetworkEntity entity, PlayerId playerId)
    {
        bool owner = playerId.IsMe;

        foreach (var component in Components)
        {
            try
            {
                component.OnOwnerChanged(owner);
            }
            catch (Exception e)
            {
                FusionLogger.LogException("running OwnershipEvents", e);
            }
        }
    }
}