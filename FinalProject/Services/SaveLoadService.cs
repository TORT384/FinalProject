using System.IO;
using System.Text.Json;
using FinalProject.Models;
using FinalProject.Persistence.Contracts;
using FinalProject.Services.Interfaces;

namespace FinalProject.Services;

public class SaveLoadService : ISaveLoadService
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _saveFilePath;

    public SaveLoadService(string? saveFilePath = null)
    {
        _saveFilePath = saveFilePath ?? Path.Combine(AppContext.BaseDirectory, "Persistence", "save.json");
    }

    public async Task SaveSessionAsync(GameSession session, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(session);

        var dto = ToDto(session);
        var directoryPath = Path.GetDirectoryName(_saveFilePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        await using var stream = File.Create(_saveFilePath);
        await JsonSerializer.SerializeAsync(stream, dto, _serializerOptions, cancellationToken);
    }

    public async Task<GameSession?> LoadSessionAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_saveFilePath))
        {
            return null;
        }

        await using var stream = File.OpenRead(_saveFilePath);
        var dto = await JsonSerializer.DeserializeAsync<SaveGameDto>(stream, _serializerOptions, cancellationToken);
        if (dto is null || dto.Cells.Count != SudokuBoard.Size * SudokuBoard.Size)
        {
            return null;
        }

        return FromDto(dto);
    }

    private static SaveGameDto ToDto(GameSession session)
    {
        return new SaveGameDto
        {
            SessionId = session.SessionId,
            Difficulty = session.Difficulty,
            StartedAtUtc = session.StartedAtUtc,
            Elapsed = session.Elapsed,
            IsPaused = session.IsPaused,
            Cells = session.Board.Cells
                .Select(cell => new SaveCellDto
                {
                    Row = cell.Row,
                    Column = cell.Column,
                    Value = cell.Value,
                    IsFixed = cell.IsFixed
                })
                .ToList()
        };
    }

    private static GameSession FromDto(SaveGameDto dto)
    {
        var cells = dto.Cells
            .OrderBy(cell => cell.Row)
            .ThenBy(cell => cell.Column)
            .Select(cell => new Cell(cell.Row, cell.Column, cell.Value, cell.IsFixed))
            .ToList();

        var board = new SudokuBoard(cells);
        return new GameSession(
            dto.SessionId,
            board,
            dto.Difficulty,
            dto.StartedAtUtc,
            dto.Elapsed,
            dto.IsPaused);
    }
}
