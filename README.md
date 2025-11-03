# hw-hse-bank
Домашнее задание номер два по КПО ПИ ВШЭ 2025


# HSE Bank — консольное приложение учёта финансов

> Учебный проект (КПО ДЗ) с упором на архитектуру, SOLID/GRASP и шаблоны проектирования (GoF)
> Язык: C# (.NET)



## Кратко о функциональности

- **Счета**: создание, просмотр, удаление, самопроверка баланса.
- **Категории**: доход/расход, добавление, поиск по имени/ID, удаление с защитой от удаления используемой категории.
- **Операции**: добавление доходов/расходов, фильтрация по периоду, вывод списков.
- **Аналитика**: итоговые суммы за период, сводка по категориям.
- **Экспорт/Импорт**:
  - **JSON** — полный экспорт/импорт снимка данных (`BankDataSnapshot`).
  - **CSV (импорт)** — поддержка импорта **категорий** из `CategoryId;Name;Type`.
- **Надёжность**: тихий перехват исключений и дружелюбные сообщения + замер времени выполнения команды.
- **DI/Композиция**;
- **Валидация**: строгие инварианты на уровне домена и при импорте снапшота.



## Архитектура (слои и роли)

```
Program (меню) 
  Commands (ICommand) [обёрнуты декораторами Timed + Safe]
    Facades (Controller-уровень: Accounts/Categories/Analytics/ImportExport)
      Domain (BankAccount, Operation, Category)
      Repositories (IAccountRepository, ICategoryRepository + реализации)
      Application services (Import/Export, DataRestoreService)
```

- **Program** печатает меню и вызывает команды через `ICommand` (никакой бизнес‑логики внутри).
- **Команды** инкапсулируют действия меню. Все обёрнуты декораторами:
  - `SafeCommand` — ловит исключения и печатает понятное сообщение;
  - `TimedCommand` — измеряет длительность выполнения команды.
- **Фасады** (GRASP Controller): тонкий слой «оркестровки» сценариев консоли.
- **Домен**: агрегат `BankAccount` хранит операции и баланс, соблюдает инварианты.
- **Репозитории**: интерфейсы + реализации (InMemory) и **Proxy** (логирующий).
- **Сервисы приложения**: импортёры/экспортёры, валидаторы, восстановление данных.



## Доменные сущности и инварианты

### `BankAccount`
- `AccountId`, `Name`, `Balance` (`private set` в идеале), коллекция операций.
- **Factory Method**: операции создаются **только через** `AddOperation(...)` агрегата:
  - `amount > 0` (строго положительная сумма);
  - `date <= DateTime.Now` (операции из будущего запрещены);
  - `category != null`, `AccountId/CategoryId` не пустые;
  - при `Expense` баланс не может стать отрицательным (иначе исключение);
  - ссылка на `AccountId` операции проставляется агрегатом;
  - операция добавляется в приватную коллекцию, баланс пересчитывается атомарно.
- Удаление операции корректно «откатывает» баланс.

### `Category`
- `CategoryId`, `Name` (уникален без учёта регистра), `Type: Income|Expense`.

### `Operation`
- `OperationId` (уникальный), `Type`, `Amount`, `Date`, `AccountId`, `CategoryId`, `Description?`.



## Экспорт/Импорт данных

### Снимок данных — `BankDataSnapshot`
- `Accounts: List<AccountWithOperations>`
- `Categories: List<Category>`

### JSON (полный формат)
- `JsonDataExporter` / `JsonDataImporter`.
- Импорт реализован по **Template Method**:
  - `BaseFileImporter.Import(path)` → `ReadFile()` → `Parse(content)` → `Validate(snapshot)`.
  - Конкретные импортёры переопределяют **только** `Parse`.
- `Validate(...)` проверяет:
  - `snapshot != null`, разделы существуют;
  - уникальность `AccountId`/`CategoryId`/`OperationId`;
  - уникальность имён категорий (без учёта регистра);
  - наличие всех ссылок (`Operation.CategoryId` ∈ `Categories`, `Operation.AccountId` согласован с размещением);
  - `Amount > 0`, `Date <= Now`;
  - отсутствие `null`‑элементов.

### CSV (импорт категорий, MVP)
- Формат: `CategoryId;Name;Type`
- Заголовок необязателен; `Type ∈ {Income, Expense}` (без учета регистра).
- Импортирует **только категории**, `Accounts/Operations` остаются пустыми — достаточно для демонстрации расширяемости алгоритма (Template Method + Strategy).



### Восстановление данных
- `DataRestoreService.RestoreFromSnapshot(snapshot)` очищает репозитории и последовательно восстанавливает категории/счета/операции.
- **Важно:** доменные инварианты при импорте контролируются валидатором `Validate(...)`; в `Restore` допускается установка баланса из снапшота (после добавления операций).



## Паттерны (GoF) и где они применены

- **Facade** — `AccountsFacade`, `CategoriesFacade`, `AnalyticsFacade`, `ImportExportFacade`.  
  Роль: координация сценариев консоли (GRASP Controller), тонкий вход в домен.

- **Command** — отдельная команда на пункт меню: `ICommand { string Name; void Execute(); }`.  
  Роль: меню формируется из команд; легко добавлять/выключать пункты.

- **Decorator** — `SafeCommand`, `TimedCommand`.  
  Роль: поперечные функции (устойчивость + метрики) без изменения команд.

- **Template Method** — `BaseFileImporter.Import(...)` → `ReadFile/Parse/Validate`.  
  Роль: фиксированный алгоритм импорта, новые форматы добавляются через переопределение `Parse` (JSON/CSV).

- **Strategy** — выбор формата импорта/экспорта по интерфейсам `IDataImporter`/`IDataExporter`.  
  Роль: подмена алгоритма в рантайме (в фасаде выбор «1 — JSON / 2 — CSV (импорт)»).

- **Factory Method** — `BankAccount.AddOperation(...)` создаёт и регистрирует операции, соблюдая инварианты агрегата.  
  Роль: «все операции только через агрегат»; единая точка правил суммы/даты/баланса.

- **Proxy (Smart Proxy)** — `LoggingAccountRepository`, `LoggingCategoryRepository`.  
  Роль: логирование и измерение времени работы репозиториев без изменения семантики.





## DI/Композиция

```csharp
IServiceCollection services = new ServiceCollection();


services.AddSingleton<IAccountRepository>(sp =>
    new LoggingAccountRepository(new InMemoryAccountRepository()));
services.AddSingleton<ICategoryRepository>(sp =>
    new LoggingCategoryRepository(new InMemoryCategoryRepository()));


services.AddSingleton<IDataImporter, JsonDataImporter>();
services.AddSingleton<IDataExporter, JsonDataExporter>();


services.AddSingleton<DataRestoreService>();

var sp = services.BuildServiceProvider();


var importer = sp.GetRequiredService<IDataImporter>();
var exporter = sp.GetRequiredService<IDataExporter>();
var restorer = sp.GetRequiredService<DataRestoreService>();
var accRepo = sp.GetRequiredService<IAccountRepository>();
var catRepo = sp.GetRequiredService<ICategoryRepository>();
```

- В `ImportExportFacade` выбор конкретной стратегии импорта (JSON/CSV) происходит по вводу пользователя.
- Все команды регистрируются и **оборачиваются** декораторами: `new TimedCommand(new SafeCommand(cmd))`.



## Как запустить

```bash
# 1) Построить
dotnet build

# 2) Запустить
dotnet run

# 3) Следовать меню в консоли
```

**Форматы ввода** (по умолчанию):
- Дата: `dd.MM.yyyy` (например, `20.10.2025`), даты из будущего запрещены.
- Сумма: строго положительное число. Если локаль мешает, используйте системный разделитель.

**Файлы**:
- Экспорт JSON — путь указывается **включая имя файла** (например, `/Users/.../data.json`).
- Импорт CSV (категории) — аналогично (`/Users/.../cats.csv`).



## Самопроверка баланса

Отдельная команда пересчитывает баланс каждого счёта из операций и сравнивает с хранимым значением. Полезно после импорта/ручных правок файла.

Инвариант: «баланс = сумма(доходов) − сумма(расходов)» для данного счёта.



## Навигация по коду (основные директории)

```
Application/
  BaseFileImporter.cs
  JsonDataImporter.cs
  CsvDataImporter.cs
  JsonDataExporter.cs
  DataRestoreService.cs
  BankDataSnapshot.cs
  AccountWithOperations.cs

BankAccount/
  bank_account.cs

Category/
  category.cs

Operation/
  operation.cs
  operation_type.cs

Facades/
  AccountsFacade.cs
  CategoriesFacade.cs
  AnalyticsFacade.cs
  ImportExportFacade.cs

Commands/
  (команды для пунктов меню)
  Decorator/SafeCommand.cs
  Decorator/TimedCommand.cs

Infrastructure/
  InMemoryAccountRepository.cs
  InMemoryCategoryRepository.cs
  LoggingAccountRepository.cs
  LoggingCategoryRepository.cs

Interfaces/
  IAccountRepository.cs
  ICategoryRepository.cs
  IDataImporter.cs
  IDataExporter.cs

Program.cs
```





## Чек‑лист на соответствие ТЗ

- [x] Базовые сценарии (счета/категории/операции) работают через фасады и команды.
- [x] Экспорт/импорт JSON; импорт CSV (категории, MVP).
- [x] Строгая валидация при импорте (`Validate`).
- [x] Самопроверка баланса.
- [x] Измерение времени сценариев (декоратор `TimedCommand`).
- [x] Перехват исключений и дружелюбные сообщения (`SafeCommand`).
- [x] DI для зависимостей.
- [x] GoF‑паттерны: Facade, Command, Decorator, Template Method, Strategy, Factory Method, Proxy.



## Сценарий демонстрации

1. **Создание счёта** => добавить **доход** и **расход** => показать длительность команды.  
2. **Экспорт JSON**, затем **очистка** (или перезапуск) => **импорт JSON** => запустить **самопроверку баланса**.  
3. **Импорт CSV** с категориями => показать, что алгоритм импорта не менялся (Template Method/Strategy), формат просто подставили.  
4. Пара действий, чтобы в консоли было видно логи **Proxy** (`[Репозиторий/...]: ...`).



Если будете читать код — начните с `Program.cs`, затем `Commands/`, `Facades/`, и дальше по слоям, как описано выше.
