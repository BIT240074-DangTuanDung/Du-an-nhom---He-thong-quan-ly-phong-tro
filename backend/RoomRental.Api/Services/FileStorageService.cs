namespace RoomRental.Api.Services;

public interface IFileStorageService { Task<string?> SaveTicketImageAsync(IFormFile? file); }

public class FileStorageService(IWebHostEnvironment environment) : IFileStorageService
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    public async Task<string?> SaveTicketImageAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0) return null;
        if (file.Length > 5 * 1024 * 1024) throw new ArgumentException("Ảnh tối đa 5 MB.");
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension)) throw new ArgumentException("Chỉ chấp nhận JPG, PNG hoặc WEBP.");
        var folder = Path.Combine(environment.WebRootPath, "uploads", "tickets");
        Directory.CreateDirectory(folder);
        var filename = $"{Guid.NewGuid()}{extension}";
        await using var stream = File.Create(Path.Combine(folder, filename));
        await file.CopyToAsync(stream);
        return $"/uploads/tickets/{filename}";
    }
}
