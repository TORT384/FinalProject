namespace FinalProject.ViewModels;

public class StatisticsViewModel : BaseViewModel
{
    private string _title = "Екран статистики";
    private string _description = "Тут буде статистика перемог, середній час та інші метрики.";

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
