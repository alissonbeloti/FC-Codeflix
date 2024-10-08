﻿using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.Domain.Entity;
public class Media : SeedWork.Entity
{
    public string FilePath { get; private set; }
    public string? EncondedPath { get; private set; }
    public MediaStatus Status { get; private set; }

    public Media(string filePath)
        : base ()
    {
        FilePath = filePath;
        Status = MediaStatus.Pending;
    }

    public void UpdateAsSentToEncode() =>
        Status = MediaStatus.Processing;

    public void UpdateAsEncoded(string encodedPath)
    {
        Status = MediaStatus.Completed;
        EncondedPath = encodedPath;
    }

    public void UpdateAsEncodingError()
    {
        Status = MediaStatus.Error;
        EncondedPath = null;
    }
}
