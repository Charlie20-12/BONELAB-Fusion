﻿using LabFusion.Data;
using LabFusion.Exceptions;
using LabFusion.SDK.Gamemodes;
using LabFusion.Utilities;

namespace LabFusion.Network;

public class GamemodeMetadataSetData : IFusionSerializable
{
    public string gamemodeBarcode;
    public string key;
    public string value;

    public void Serialize(FusionWriter writer)
    {
        writer.Write(gamemodeBarcode);
        writer.Write(key);
        writer.Write(value);
    }

    public void Deserialize(FusionReader reader)
    {
        gamemodeBarcode = reader.ReadString();
        key = reader.ReadString();
        value = reader.ReadString();
    }

    public static GamemodeMetadataSetData Create(string gamemodeBarcode, string key, string value)
    {
        return new GamemodeMetadataSetData()
        {
            gamemodeBarcode = gamemodeBarcode,
            key = key,
            value = value,
        };
    }
}

public class GamemodeMetadataSetMessage : FusionMessageHandler
{
    public override byte Tag => NativeMessageTag.GamemodeMetadataSet;

    public override void HandleMessage(byte[] bytes, bool isServerHandled = false)
    {
        if (isServerHandled)
        {
            throw new ExpectedClientException();
        }

        using var reader = FusionReader.Create(bytes);
        var data = reader.ReadFusionSerializable<GamemodeMetadataSetData>();

        if (GamemodeManager.TryGetGamemode(data.gamemodeBarcode, out var gamemode))
        {
            gamemode.Metadata.ForceSetLocalMetadata(data.key, data.value);
        }
        else
        {
#if DEBUG
            FusionLogger.Warn($"Failed to find a Gamemode with barcode {data.gamemodeBarcode}!");
#endif
        }
    }
}