using EPiServer.Framework.DataAnnotations;

namespace EpiPageImporter.Models.Media
{
    [ContentType(DisplayName = "Image File",
        GUID = "D9739C0F-5E9C-4AA8-9CAE-7A2C787C1D32",
        Description = "Generic image file")]
    [MediaDescriptor(ExtensionString = "jpg,jpeg,png,gif,webp")]
    public class ImageFile : ImageData
    {
    }
}
