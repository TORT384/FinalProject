namespace FinalProject.ViewModels;

public class GameViewModel : BaseViewModel
{
    private string _title = "Ігровий екран";
    private string _description = "Тут буде Sudoku-дошка 9x9, таймер та керування грою.";

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }
}
