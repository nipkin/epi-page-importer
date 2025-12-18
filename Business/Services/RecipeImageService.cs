using EpiPageImporter.Models.Media;
using EPiServer.DataAccess;
using EPiServer.Framework.Blobs;
using EPiServer.Security;
using EPiServer.Web;
using System.Security.Cryptography;

namespace EpiPageImporter.Business.Services;

public class RecipeImageService(
    IContentRepository contentRepository,
    IBlobFactory blobFactory)
{
    private readonly IContentRepository _contentRepository = contentRepository;
    private readonly IBlobFactory _blobFactory = blobFactory;

    public ContentReference? Import(string? url, ContentReference current)
    {
        if (string.IsNullOrEmpty(url)) return current;

        using var http = new HttpClient();
        var data = http.GetByteArrayAsync(url).Result;

        if (IsSameAsExisting(current, data))
            return current;

        var folder = GetOrCreateFolder();
        var img = _contentRepository.GetDefault<ImageFile>(folder);
        img.Name = Path.GetFileName(url);

        var blob = _blobFactory.CreateBlob(img.BinaryDataContainer, ".jpg");
        using var s = blob.OpenWrite();
        s.Write(data);

        img.BinaryData = blob;
        return _contentRepository.Save(img, SaveAction.Publish, AccessLevel.NoAccess);
    }

    private bool IsSameAsExisting(ContentReference image, byte[] data)
    {
        if (!_contentRepository.TryGet(image, out ImageData img))
            return false;

        using var s = img.BinaryData.OpenRead();
        using var ms = new MemoryStream();
        s.CopyTo(ms);

        return SHA256.HashData(ms.ToArray())
            .SequenceEqual(SHA256.HashData(data));
    }

    private ContentReference GetOrCreateFolder()
    {
        var root = SiteDefinition.Current.GlobalAssetsRoot;

        var existing = _contentRepository
            .GetChildren<ContentFolder>(root)
            .FirstOrDefault(x => x.Name == "Recipe Images");

        if (existing != null)
            return existing.ContentLink;

        var folder = _contentRepository.GetDefault<ContentFolder>(root);
        folder.Name = "Recipe Images";
        return _contentRepository.Save(folder, SaveAction.Publish, AccessLevel.NoAccess);
    }
}