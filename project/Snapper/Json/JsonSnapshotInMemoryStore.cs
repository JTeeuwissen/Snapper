﻿using System.Text.Json;
using Snapper.Core;

namespace Snapper.Json;

internal class JsonSnapshotInMemoryStore : ISnapshotStore
{
    private readonly JsonElement _snapshot;

    public JsonSnapshotInMemoryStore(JsonSnapshotSanitiser jsonSnapshotSanitiser, object snapshot)
    {
        _snapshot = jsonSnapshotSanitiser.SanitiseSnapshot(snapshot);
    }

    public JsonSnapshot GetSnapshot(SnapshotId snapshotId)
        => new JsonSnapshot(snapshotId, _snapshot);

    public void StoreSnapshot(JsonSnapshot snapshot)
    {}
}
