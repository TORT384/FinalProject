namespace FinalProject.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private string _title = "Екран налаштувань";
    private string _description = "Тут будуть параметри гри, тема та керування збереженням налаштувань.";

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
