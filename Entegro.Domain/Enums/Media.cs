using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Domain.Enums
{
    [Flags]
    public enum MediaLoadFlags
    {
        None = 0,
        WithBlob = 1 << 0,
        WithTags = 1 << 1,
        WithTracks = 1 << 2,
        WithFolder = 1 << 3,
        AsNoTracking = 1 << 4,
        Full = WithBlob | WithTags | WithTracks | WithFolder,
        FullNoTracking = Full | AsNoTracking
    }

    public enum SpecialMediaFolder
    {
        AllFiles = -500,
        Trash = -400,
        Orphans = -300,
        TransientFiles = -200,
        UnassignedFiles = -100
    }

    public enum FileHandling
    {
        SoftDelete,
        MoveToRoot,
        Delete
    }

    public enum DuplicateFileHandling
    {
        ThrowError,
        Overwrite,
        Rename
    }

    public enum DuplicateEntryHandling
    {
        ThrowError,
        Overwrite,
        // Folder: Overwrite, File: Rename
        Rename,
        Skip
    }

    public enum MimeValidationType
    {
        NoValidation,
        MimeTypeMustMatch,
        MediaTypeMustMatch
    }
}
