# Sudoku Game

> WPF-проєкт на C# з архітектурою **MVVM + Layered Structure**  
> 🎯 Фокус: чистий код, патерни, підтримуваність, Git-дисципліна

---

## Опис

`Sudoku Game` — десктопний застосунок, який дозволяє:
- запускати нову гру 9x9;
- обирати складність (`Easy / Medium / Hard`);
- отримувати валідацію ходів;
- користуватися таймером і паузою;
- зберігати/завантажувати прогрес (JSON);
- відстежувати статистику гравця;
- керувати базовими налаштуваннями.

---

## Основні можливості

- ✅ Нова гра Sudoku
- ✅ Рівні складності (через Strategy)
- ✅ Валідація введення та конфліктів
- ✅ Таймер + пауза/продовження
- ✅ Save/Load поточної сесії
- ✅ Статистика результатів
- ✅ Налаштування гри
- ✅ Unit-тести core логіки

---

## Технології

- `C#`
- `WPF`
- `.NET 9`
- `MVVM`
- `System.Text.Json`
- `xUnit`

---

## Запуск проєкту

```bash
git clone <repo-url>
cd FinalProject
dotnet build FinalProject.sln
dotnet run --project FinalProject/FinalProject.csproj
```

---

## Архітектура

Обрана архітектура: **MVVM + Layered Structure**.

Чому це оптимально для WPF:
- Data Binding природно працює з `INotifyPropertyChanged`;
- UI відокремлений від бізнес-логіки;
- легше масштабувати й тестувати;
- простіше підтримувати код перед захистом/рев’ю.

---

## Структура проєкту

```text
FinalProject/
│
├── FinalProject/                  # Основний WPF застосунок
├── Models/
├── ViewModels/
├── Views/
├── Services/
│   └── Interfaces/
├── Commands/
├── Factories/
├── Strategies/
└── Persistence/
└── Contracts/
```

---

## Де в коді реалізовано патерни

### Strategy Pattern (генерація складності)
- Інтерфейс стратегії: [`ISudokuGenerationStrategy`](FinalProject/Strategies/ISudokuGenerationStrategy.cs)
- Конкретні стратегії:
  - [`EasyGenerationStrategy`](FinalProject/Strategies/EasyGenerationStrategy.cs)
  - [`MediumGenerationStrategy`](FinalProject/Strategies/MediumGenerationStrategy.cs)
  - [`HardGenerationStrategy`](FinalProject/Strategies/HardGenerationStrategy.cs)
- Використання в генераторі: [`SudokuGenerator`](FinalProject/Services/SudokuGenerator.cs)

### Factory Pattern (створення гри)
- Фабрика сесій: [`SudokuFactory`](FinalProject/Factories/SudokuFactory.cs)
- Точка створення нової гри у VM: [`GameViewModel`](FinalProject/ViewModels/GameViewModel.cs)

### Observer Pattern (оновлення UI)
- Базова реалізація: [`BaseViewModel`](FinalProject/ViewModels/BaseViewModel.cs)
- Команди та повідомлення в UI:
  - [`GameViewModel`](FinalProject/ViewModels/GameViewModel.cs)
  - [`SettingsViewModel`](FinalProject/ViewModels/SettingsViewModel.cs)
  - [`StatisticsViewModel`](FinalProject/ViewModels/StatisticsViewModel.cs)

---

## SOLID / Clean Code у проєкті

### SRP
- Кожен сервіс має одну відповідальність:
  - [`ValidationService`](FinalProject/Services/ValidationService.cs)
  - [`SaveLoadService`](FinalProject/Services/SaveLoadService.cs)
  - [`SettingsService`](FinalProject/Services/SettingsService.cs)
  - [`StatisticsService`](FinalProject/Services/StatisticsService.cs)

### OCP
- Додати нову складність можна через нову Strategy без змін базової логіки.

### DIP
- Контракти винесені в інтерфейси:
  - [`ISudokuGenerator`](FinalProject/Services/Interfaces/ISudokuGenerator.cs)
  - [`IValidationService`](FinalProject/Services/Interfaces/IValidationService.cs)
  - [`ISaveLoadService`](FinalProject/Services/Interfaces/ISaveLoadService.cs)
  - [`ISettingsService`](FinalProject/Services/Interfaces/ISettingsService.cs)
  - [`IStatisticsService`](FinalProject/Services/Interfaces/IStatisticsService.cs)

### DRY / KISS
- Загальна логіка згрупована в сервіси та невеликі класи;
- мінімум “розумного” UI-коду, основна логіка в ViewModel/Services.

---

## Persistence (тільки JSON, без БД)

Файлова модель збереження:
- [`save.json`](FinalProject/Persistence/save.json)
- [`settings.json`](FinalProject/Persistence/settings.json)
- [`statistics.json`](FinalProject/Persistence/statistics.json)

DTO-контракти:
- [`SaveGameDto`](FinalProject/Persistence/Contracts/SaveGameDto.cs)
- [`SaveCellDto`](FinalProject/Persistence/Contracts/SaveCellDto.cs)
- [`SettingsDto`](FinalProject/Persistence/Contracts/SettingsDto.cs)
- [`StatisticsDto`](FinalProject/Persistence/Contracts/StatisticsDto.cs)

---

## Ключові екрани

- Гра: [`GameView`](FinalProject/Views/GameView.xaml)
- Налаштування: [`SettingsView`](FinalProject/Views/SettingsView.xaml)
- Статистика: [`StatisticsView`](FinalProject/Views/StatisticsView.xaml)
- Навігація: [`MainMenuViewModel`](FinalProject/ViewModels/MainMenuViewModel.cs)

---

## Примітка щодо Git-воркфлоу

Проєкт розбивався на невеликі фічі для чистих PR:
- `feature/project-architecture`
- `feature/domain-models`
- `feature/game-creation-core`
- `feature/game-board-mvp`
- `feature/timer-pause`
- `feature/save-system`
- `feature/statistics`
- `feature/settings`
- `feature/validation-polish`
- `feature/ui-polish`

Такий підхід добре показує контроль версій, поступову еволюцію архітектури та якісну історію комітів.