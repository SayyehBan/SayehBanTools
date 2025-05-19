using Microsoft.AspNetCore.Http;
using Moq;
using SayehBanTools.ManageFile;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
namespace SayehBanToolsTest.ManageFile;
public class ManageFilesTests
{
    private readonly MockFileSystem _fileSystem;
    private readonly Mock<IFormFile> _formFileMock;
    private readonly ManageFiles _manageFiles;

    public ManageFilesTests()
    {
        _fileSystem = new MockFileSystem();
        _manageFiles = new ManageFiles(_fileSystem);

        _formFileMock = new Mock<IFormFile>();
        var content = "Test content";
        var fileName = "test.jpg";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        _formFileMock.Setup(f => f.FileName).Returns(fileName);
        _formFileMock.Setup(f => f.Length).Returns(stream.Length);
        _formFileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        _formFileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns((Stream target, CancellationToken token) => stream.CopyToAsync(target, token));
    }

    // نرمال‌سازی مسیر برای حذف درایو (مثل C:\\) و یکسان‌سازی جداکننده‌ها به فوروارد اسلش (/)
    private string NormalizePath(string path)
    {
        if (path.StartsWith("C:\\", StringComparison.OrdinalIgnoreCase))
        {
            path = path.Substring(3); // حذف "C:\\"
        }
        return path.Replace("\\", "/"); // تبدیل بک‌اسلش به فوروارد اسلش
    }

    [Fact]
    public void DeleteFileServer_FileExists_DeletesFile()
    {
        var filePath = "wwwroot/Uploads/test.jpg";
        _fileSystem.AddFile(filePath, new MockFileData(""));
        _manageFiles.DeleteFileServer(filePath);
        Assert.False(_fileSystem.File.Exists(filePath));
    }

    [Fact]
    public void DeleteFileServer_FileDoesNotExist_DoesNothing()
    {
        var filePath = "wwwroot/Uploads/test.jpg";
        _manageFiles.DeleteFileServer(filePath);
        Assert.False(_fileSystem.File.Exists(filePath));
    }

    [Fact]
    public async Task UploadFileAsync_ValidFile_UploadsAndReturnsNewPath()
    {
        var basePath = "wwwroot/Uploads/Avatars/";
        var result = await _manageFiles.UploadFileAsync(basePath, _formFileMock.Object);
        Assert.True(_fileSystem.Directory.Exists(basePath));
        var uploadedFiles = _fileSystem.Directory.GetFiles(basePath);
        Assert.Single(uploadedFiles);
        Assert.StartsWith("Uploads/Avatars/", result);
        Assert.EndsWith(".jpg", result);
        Assert.DoesNotContain("wwwroot", result);
    }

    [Fact]
    public async Task UploadFileAsync_NullFile_ThrowsArgumentNullException()
    {
        var basePath = "wwwroot/Uploads/Avatars/";
        await Assert.ThrowsAsync<ArgumentNullException>(() => _manageFiles.UploadFileAsync(basePath, null!));
    }

    [Fact]
    public async Task UploadFileAsync_EmptyBasePath_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _manageFiles.UploadFileAsync("", _formFileMock.Object));
    }

    [Fact]
    public async Task UploadFileChunkedAsync_ValidFile_UploadsAndReturnsNewPath()
    {
        var basePath = "wwwroot/Uploads/Avatars/";
        var result = await _manageFiles.UploadFileChunkedAsync(basePath, _formFileMock.Object, 5);
        Assert.True(_fileSystem.Directory.Exists(basePath));
        var uploadedFiles = _fileSystem.Directory.GetFiles(basePath);
        Assert.Single(uploadedFiles);
        Assert.StartsWith("Uploads/Avatars/", result);
        Assert.EndsWith(".jpg", result);
        Assert.DoesNotContain("wwwroot", result);
        Assert.False(_fileSystem.File.Exists(uploadedFiles[0] + ".metadata"));
    }

    [Fact]
    public async Task UploadFileChunkedAsync_ResumesFromMetadata_ContinuesUpload()
    {
        var basePath = "wwwroot/Uploads/Avatars/";
        var newFileName = Guid.NewGuid().ToString() + ".jpg";
        var newFullPath = _fileSystem.Path.Combine(basePath, newFileName);
        var metadataPath = newFullPath + ".metadata";
        _fileSystem.AddFile(newFullPath, new MockFileData("Test"));
        _fileSystem.AddFile(metadataPath, new MockFileData($"4|{newFileName}"));

        var content = "Test content";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        _formFileMock.Setup(f => f.OpenReadStream()).Returns(stream);

        var result = await _manageFiles.UploadFileChunkedAsync(basePath, _formFileMock.Object, 5);

        var uploadedFiles = _fileSystem.Directory.GetFiles(basePath);
        Assert.Single(uploadedFiles);
        Assert.Equal(NormalizePath(newFullPath), NormalizePath(uploadedFiles[0]));
        Assert.False(_fileSystem.File.Exists(metadataPath));
        Assert.Contains("content", _fileSystem.File.ReadAllText(uploadedFiles[0]));
    }

    [Fact]
    public async Task UploadFileChunkedAsync_InvalidMetadata_StartsNewUpload()
    {
        var basePath = "wwwroot/Uploads/Avatars/";
        var oldFileName = Guid.NewGuid().ToString() + ".jpg";
        var oldFullPath = _fileSystem.Path.Combine(basePath, oldFileName);
        var metadataPath = oldFullPath + ".metadata";
        _fileSystem.AddFile(oldFullPath, new MockFileData("Test"));
        _fileSystem.AddFile(metadataPath, new MockFileData("invalid"));

        var content = "Test content";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        _formFileMock.Setup(f => f.OpenReadStream()).Returns(stream);

        var result = await _manageFiles.UploadFileChunkedAsync(basePath, _formFileMock.Object, 5);

        var uploadedFiles = _fileSystem.Directory.GetFiles(basePath);
        Assert.Single(uploadedFiles);
        Assert.NotEqual(NormalizePath(oldFullPath), NormalizePath(uploadedFiles[0]));
        Assert.False(_fileSystem.File.Exists(metadataPath));
        Assert.False(_fileSystem.File.Exists(oldFullPath));
        Assert.Contains("content", _fileSystem.File.ReadAllText(uploadedFiles[0]));
    }

    [Fact]
    public async Task UploadFileChunkedAsync_NullFile_ThrowsArgumentNullException()
    {
        var basePath = "wwwroot/Uploads/Avatars/";
        await Assert.ThrowsAsync<ArgumentNullException>(() => _manageFiles.UploadFileChunkedAsync(basePath, null!));
    }

    [Fact]
    public async Task UploadFileChunkedAsync_EmptyBasePath_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _manageFiles.UploadFileChunkedAsync("", _formFileMock.Object));
    }

    [Fact]
    public async Task UploadFileChunkedAsync_InvalidChunkSize_ThrowsArgumentException()
    {
        var basePath = "wwwroot/Uploads/Avatars/";
        await Assert.ThrowsAsync<ArgumentException>(() => _manageFiles.UploadFileChunkedAsync(basePath, _formFileMock.Object, 0));
    }
}