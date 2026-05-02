# Sudoku Game

## Description

Sudoku Game — десктопний застосунок, створений на C# та WPF, який дозволяє грати в Sudoku з різними рівнями складності, збереженням прогресу та статистикою користувача.

---

## Features

- Генерація нової гри Sudoku
- Рівні складності: Easy / Medium / Hard
- Перевірка правильності введених значень
- Таймер гри
- Збереження незавершеної гри
- Завантаження останньої гри
- Статистика користувача
- Налаштування застосунку
- Зручний графічний інтерфейс

---

## Technologies

- C#
- WPF
- .NET
- MVVM
- JSON Serialization

---

## Installation / Run

1. Клонувати репозиторій:

```bash
git clone <repo-url>

---

## Project Overview

Десктопний застосунок **Sudoku Game**, створений на **C# + WPF**.

## Chosen Architecture: MVVM + Layered Structure

Для WPF це один із найкращих варіантів.

### Чому саме так:

- ідеально підходить під WPF Data Binding
- UI відокремлений від логіки
- простіше тестувати
- код стає чистішим
- легко масштабувати проєкт

---

# Project Structure

```text
SudokuApp/
│
├── Models/
│   ├── Cell.cs
│   ├── SudokuBoard.cs
│   ├── GameSession.cs
│   └── Statistics.cs
│
├── ViewModels/
│   ├── MainMenuViewModel.cs
│   ├── GameViewModel.cs
│   ├── SettingsViewModel.cs
│   └── StatisticsViewModel.cs
│
├── Views/
│   ├── MainWindow.xaml
│   ├── GameView.xaml
│   ├── SettingsView.xaml
│   └── StatisticsView.xaml
│
├── Services/
│   ├── SudokuGenerator.cs
│   ├── SudokuSolver.cs
│   ├── SaveLoadService.cs
│   ├── ValidationService.cs
│   └── StatisticsService.cs
│
├── Commands/
│   └── RelayCommand.cs
│
├── Factories/
│   └── SudokuFactory.cs
│
├── Strategies/
│   ├── EasyGenerationStrategy.cs
│   ├── MediumGenerationStrategy.cs
│   └── HardGenerationStrategy.cs
│
├── Persistence/
│   ├── save.json
│   ├── settings.json
│   └── statistics.json
│
└── App.xaml

---

## CodeQuality

## Programming Principles

### SRP — Single Responsibility Principle
Кожен клас виконує лише одну задачу, що спрощує підтримку та зміну коду.

### OCP — Open/Closed Principle
Новий функціонал додається через розширення, а не зміну вже існуючого коду.

### DIP — Dependency Inversion Principle
Залежності будуються через інтерфейси, що робить систему гнучкішою.

### DRY — Don't Repeat Yourself
Повторюваний код виноситься у спільні методи або сервіси.

### KISS — Keep It Simple Stupid
Використовуються прості рішення без зайвого ускладнення архітектури.

---

## Design Patterns

### Strategy Pattern
Дозволяє змінювати алгоритм генерації Sudoku залежно від складності гри.

### Factory Pattern
Централізує створення нових ігрових сесій та об'єктів Sudoku.

### Observer Pattern
Автоматично оновлює UI при зміні даних через `INotifyPropertyChanged`.

---

## Refactoring Techniques

### Extract Method
Великі методи розбиваються на менші та зрозуміліші частини.

### Rename Method / Variable
Зрозумілі назви покращують читабельність коду.

### Move Method
Методи переносяться в ті класи, де вони логічно повинні знаходитись.

### Remove Duplicate Code
Однакова логіка об'єднується в одному місці замість копіювання.

### Extract Class
Великі класи розділяються на менші з окремою відповідальністю.