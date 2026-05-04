using FinalProject.Models;

namespace FinalProject.ViewModels;

public class GameCellViewModel : BaseViewModel
{
    private readonly Cell _cell;
    private readonly Action<GameCellViewModel, string?> _onInputChanged;
    private string? _inputValue;
    private bool _isConflict;
    private bool _suppressInputCallback;

    public GameCellViewModel(Cell cell, Action<GameCellViewModel, string?> onInputChanged)
    {
        _cell = cell ?? throw new ArgumentNullException(nameof(cell));
        _onInputChanged = onInputChanged ?? throw new ArgumentNullException(nameof(onInputChanged));
        _inputValue = cell.Value == 0 ? string.Empty : cell.Value.ToString();
    }

    public int Row => _cell.Row;

    public int Column => _cell.Column;

    public bool IsFixed => _cell.IsFixed;

    public bool IsEditable => !IsFixed;

    public string? InputValue
    {
        get => _inputValue;
        set
        {
            if (!SetProperty(ref _inputValue, value))
            {
                return;
            }

            if (_suppressInputCallback)
            {
                return;
            }

            _onInputChanged(this, value);
        }
    }

    public bool IsConflict
    {
        get => _isConflict;
        set => SetProperty(ref _isConflict, value);
    }

    public Cell Cell => _cell;

    public void RefreshFromModel()
    {
        SetInputSilently(_cell.Value == 0 ? string.Empty : _cell.Value.ToString());
    }

    public void SetInputSilently(string value)
    {
        _suppressInputCallback = true;
        InputValue = value;
        _suppressInputCallback = false;
    }
}
