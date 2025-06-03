using System;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using UserService.BusinessLogic.Services.Interfaces;
using UserService.Shared.Contants;

namespace UserService.BusinessLogic.Services;

public class ImageService : IImageService
{
    private IFileStorageConfig _config;
    private List<string> _allowesExtensions = [".jpg", ".jpeg", ".png", ".gif"];
    private static int mb = 1048576;
    private static int Width = 300;
    private static int Height = 300;
    private static int MaxSizeBytes = 5 * mb;
    public ImageService(IFileStorageConfig config)
    {
        _config = config;
    }
    public async Task<string> SaveImage(IFormFile file, string newsId)
    {
        ValidateExtension(file);
        ValidateFileSize(file);

        var image = await Image.LoadAsync(file.OpenReadStream());

        ValidateImageSize(image);

        var folderPath = Path.Combine(_config.WebRootPath, "images");

        string filePath;
        string fileName;

        fileName = GenerateFileName(file, newsId);
        filePath = Path.Combine(folderPath, fileName);

        Resize(image);
        Crop(image);
        await image.SaveAsync(filePath, new JpegEncoder { Quality = 75 });

        return Path.Combine("images", fileName);
    }

    public bool DeleteImage(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return false;

        if (relativePath.StartsWith("/"))
            relativePath = relativePath[1..];

        var fullPath = Path.Combine(_config.WebRootPath, relativePath);

        if (!File.Exists(fullPath))
            return false;

        try
        {
            File.Delete(fullPath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении файла: {ex.Message}");
            return false;
        }
    }

    private void ValidateExtension(IFormFile file)
    {
        var fileExtension = Path.GetExtension(file.FileName);

        if (_allowesExtensions.Any(ext => ext == fileExtension.ToLower()))
            return;

        throw new ImageProcessingException(ErrorName.WrongImageFormat);
    }

    private void ValidateFileSize(IFormFile file)
    {
        if (file.Length > MaxSizeBytes)
            throw new ImageProcessingException(ErrorName.ImageTooLarge);
    }

    private void ValidateImageSize(Image image)
    {
        if (image.Width < Width || image.Height < Height)
            throw new ImageProcessingException(ErrorName.ImageTooSmall);
    }

    private string GenerateFileName(IFormFile file, string newsId)
    {
        var fileExtension = Path.GetExtension(file.FileName);
        var fileName = $"News_{newsId}_image";

        return $"{fileName}{fileExtension}";
    }

    private void Resize(Image image)
    {
        var resizeOptions = new ResizeOptions
        {
            Mode = ResizeMode.Min,
            Size = new Size(Width)
        };

        image.Mutate(action => action.Resize(resizeOptions));
    }

    private void Crop(Image image)
    {
        var rectangle = GetCropRectangle(image);
        image.Mutate(action => action.Crop(rectangle));
    }

    private Rectangle GetCropRectangle(Image image)
    {
        var widthDifference = image.Width - Width;
        var heightDifference = image.Height - Height;
        var x = widthDifference / 2;
        var y = heightDifference / 2;

        return new Rectangle(x, y, Width, Height);
    }
}
