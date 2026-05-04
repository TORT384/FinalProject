using System.Text.Json;
using FinalProject.Models;
using FinalProject.Services;

namespace FinalProject.Tests.Services;

public class SettingsServiceTests
{
    [Fact]
    public async Task GetSettingsAsync_ReturnsDefaults_WhenFileDoesNotExist()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}-settings.json");
        var sut = new SettingsService(filePath);

        var settings = await sut.GetSettingsAsync();

        Assert.Equal(GameDifficulty.Easy, settings.DefaultDifficulty);
        Assert.True(settings.ShowTimer);
        Assert.True(settings.HighlightConflicts);
    }

    [Fact]
    public async Task SaveSettingsAsync_PersistsValuesToJson()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}-settings.json");
        var sut = new SettingsService(filePath);
        var expected = AppSettings.Create(GameDifficulty.Hard, showTimer: false, highlightConflicts: false);

        await sut.SaveSettingsAsync(expected);
        var content = await File.ReadAllTextAsync(filePath);
        var json = JsonDocument.Parse(content);

        Assert.Equal("Hard", json.RootElement.GetProperty("difficulty").GetString());
        Assert.False(json.RootElement.GetProperty("showTimer").GetBoolean());
        Assert.False(json.RootElement.GetProperty("highlightConflicts").GetBoolean());
    }
}
