using Interfaces;
using System.Globalization;
using BankAccountSpace;
using Application;

namespace Facades
{
    public class AnalyticsFacade
    {
        private ICategoryRepository _categoryRepository;
        private IAccountRepository _accountRepository;
        public AnalyticsFacade(ICategoryRepository categoryRepository, IAccountRepository accountRepository)
        {
            _categoryRepository = categoryRepository;
            _accountRepository = accountRepository;
        }
        public void GiveSummaryByCategoryForPeriod()
        {
            Console.Clear();
            System.Console.WriteLine("Давайте выберем временной промежуток для сводки операций по категориям!");

            ConsoleInputHelper periodReader = new ConsoleInputHelper();
            (bool success, DateTime data_start, DateTime data_end) = periodReader.ReadPeriod();
            if (!success)
            {
                return;
            }

            Console.Clear();
            ConsoleInputHelper selector = new ConsoleInputHelper();

            BankAccount? chosen_account = selector.SelectAccount(_accountRepository);
            if (chosen_account == null)
            {
                return;
            }
            var result = chosen_account.GetInfByCategoryForPeriod(data_start, data_end);

            if (result.Count == 0)
            {
                System.Console.WriteLine("За данный период не было ни одной операции! (Вероятно, вы выбрали не тот интервал)");
                return;
            }

            Console.Clear();
            System.Console.WriteLine($"Сводка операций по категориям за период с {data_start:dd.MM.yyyy} по {data_end:dd.MM.yyyy}:");

            bool flag = true;

            decimal sum_income = 0;
            decimal sum_expense = 0;
            int count = 0;
            if (result.Count == 1)
            {
                string name_cat = "";
                var cat = _categoryRepository.FindCategoryById(result[0].CategoryId);
                if (cat == null)
                {
                    return;
                }

                name_cat = cat.Name;
                if (string.IsNullOrEmpty(name_cat))
                {
                    System.Console.WriteLine("Категория не нашлась!");
                    return;
                }
                System.Console.WriteLine($"\n\nКатегория {name_cat}:");

                System.Console.WriteLine($"\nТип: {result[0].Type};");
                System.Console.WriteLine($"Дата: {result[0].Date:dd.MM.yyyy};");
                System.Console.WriteLine($"Сумма: {result[0].Amount:F2};");
                System.Console.WriteLine($"Описание: {result[0].Description ?? "Описания нет"};");
                if (result[0].Type == OperationType.Income)
                {
                    sum_income += result[0].Amount;
                }
                else
                {
                    sum_expense += result[0].Amount;
                }
                System.Console.WriteLine($"Итого по категории: сумма доходов = {sum_income}; сумма расходов = {sum_expense}; кол-во = {1}");
                return;
            }

            for (int i = 0; i < result.Count - 1; i++)
            {
                if (flag)
                {
                    flag = false;
                    string name_cat = "";
                    var cat = _categoryRepository.FindCategoryById(result[i].CategoryId);
                    if (cat == null)
                    {
                        return;
                    }

                    name_cat = cat.Name;
                    if (string.IsNullOrEmpty(name_cat))
                    {
                        System.Console.WriteLine("Категория не нашлась!");
                        return;
                    }
                    System.Console.WriteLine($"\n\nКатегория {name_cat}:");
                }

                if (result[i].CategoryId == result[i + 1].CategoryId)
                {
                    System.Console.WriteLine($"\nТип: {result[i].Type};");
                    System.Console.WriteLine($"Дата: {result[i].Date:dd.MM.yyyy};");
                    System.Console.WriteLine($"Сумма: {result[i].Amount:F2};");
                    System.Console.WriteLine($"Описание: {result[i].Description ?? "Описания нет"};");
                    ++count;
                    if (result[i].Type == OperationType.Income)
                    {
                        sum_income += result[i].Amount;
                    }
                    else
                    {
                        sum_expense += result[i].Amount;
                    }

                    if (i == result.Count - 2)
                    {
                        ++i;
                        System.Console.WriteLine($"\nТип: {result[i].Type};");
                        System.Console.WriteLine($"Дата: {result[i].Date:dd.MM.yyyy};");
                        System.Console.WriteLine($"Сумма: {result[i].Amount:F2};");
                        System.Console.WriteLine($"Описание: {result[i].Description ?? "Описания нет"};");
                        ++count;
                        if (result[i].Type == OperationType.Income)
                        {
                            sum_income += result[i].Amount;
                        }
                        else
                        {
                            sum_expense += result[i].Amount;
                        }
                        System.Console.WriteLine($"Итого по категории: сумма доходов = {sum_income}; сумма расходов = {sum_expense}; кол-во = {count}");
                        sum_expense = 0;
                        sum_income = 0;
                        count = 0;
                    }
                }
                else
                {
                    System.Console.WriteLine($"\nТип: {result[i].Type};");
                    System.Console.WriteLine($"Дата: {result[i].Date:dd.MM.yyyy};");
                    System.Console.WriteLine($"Сумма: {result[i].Amount:F2};");
                    System.Console.WriteLine($"Описание: {result[i].Description ?? "Описания нет"};");
                    ++count;
                    if (result[i].Type == OperationType.Income)
                    {
                        sum_income += result[i].Amount;
                    }
                    else
                    {
                        sum_expense += result[i].Amount;
                    }
                    System.Console.WriteLine($"Итого по категории: сумма доходов = {sum_income}; сумма расходов = {sum_expense}; кол-во = {count}");
                    sum_expense = 0;
                    sum_income = 0;
                    count = 0;

                    flag = true;
                    if (i == result.Count - 2)
                    {
                        ++i;
                        string name_cat = "";
                        var cat = _categoryRepository.FindCategoryById(result[i].CategoryId);
                        if (cat == null)
                        {
                            return;
                        }

                        name_cat = cat.Name;
                        if (string.IsNullOrEmpty(name_cat))
                        {
                            System.Console.WriteLine("Категория не нашлась!");
                            return;
                        }
                        System.Console.WriteLine($"\n\nКатегория {name_cat}:");

                        System.Console.WriteLine($"\nТип: {result[i].Type};");
                        System.Console.WriteLine($"Дата: {result[i].Date:dd.MM.yyyy};");
                        System.Console.WriteLine($"Сумма: {result[i].Amount:F2};");
                        System.Console.WriteLine($"Описание: {result[i].Description ?? "Описания нет"};");
                        ++count;

                        if (result[i].Type == OperationType.Income)
                        {
                            sum_income += result[i].Amount;
                        }
                        else
                        {
                            sum_expense += result[i].Amount;
                        }
                        System.Console.WriteLine($"Итого по категории: сумма доходов = {sum_income}; сумма расходов = {sum_expense}; кол-во = {count}");
                        sum_expense = 0;
                        sum_income = 0;
                        count = 0;
                    }
                }
            }
            return;
        }

        public void CheckYourBalanceFunc()
        {
            Console.Clear();
            Console.WriteLine("Давайте проверим баланс!\n");

            ConsoleInputHelper selector = new ConsoleInputHelper();
            var account = selector.SelectAccount(_accountRepository);
            if (account == null)
            {
                return;
            }

            var ops = account.ShowOperationsByDate(DateTime.MinValue, DateTime.MaxValue);

            if (ops.Count == 0)
            {
                System.Console.WriteLine("Операций за весь период нет!");
                return;
            }

            decimal income = 0, expense = 0;

            foreach (var op in ops)
            {
                if (op.Type == OperationType.Income)
                {
                    income += op.Amount;
                }
                else if (op.Type == OperationType.Expense)
                {
                    expense += op.Amount;
                }
            }

            decimal computed = income - expense;
            decimal stored = account.Balance;
            decimal diff = computed - stored;

            Console.WriteLine($"Счёт: {account.Name} ({account.AccountId})");
            Console.WriteLine($"Текущий баланс: {stored:F2}");
            Console.WriteLine($"Баланс по операциям: {computed:F2}");
            Console.WriteLine($"Доходы: {income:F2}; Расходы: {expense:F2}; Операций: {ops.Count}");

            if (diff == 0)
            {
                Console.WriteLine("Баланс сходится");
            }
            else
            {
                Console.WriteLine($"Баланс НЕ сходится. Разница: {diff:F2}");
            }
            return;
        }

        public void ResultsByPeriod()
        {
            Console.Clear();
            System.Console.WriteLine("Давайте выберем временной промежуток для итогов!");

            ConsoleInputHelper periodReader = new ConsoleInputHelper();
            (bool success, DateTime data_start, DateTime data_end) = periodReader.ReadPeriod();
            if (!success)
            {
                return;
            }

            Console.Clear();
            ConsoleInputHelper selector = new ConsoleInputHelper();
            BankAccount? chosen_account = selector.SelectAccount(_accountRepository);
            if (chosen_account == null)
            {
                return;
            }
            var result = chosen_account.CalculateTotalsByDate(data_start, data_end);

            if (result.income == 0 && result.expense == 0 && result.total == 0)
            {
                System.Console.WriteLine("За данный период не было ни одной операции! (Вероятно, вы выбрали не тот интервал)");
                return;
            }

            System.Console.WriteLine($"Итоги операций за период с {data_start:dd.MM.yyyy} по {data_end:dd.MM.yyyy}:\n");

            System.Console.WriteLine($"Общий доход: {result.income:F2}");
            System.Console.WriteLine($"Общий расход: {result.expense:F2}");
            System.Console.WriteLine($"Итог: {result.total:F2}");
            return;
        }

        public void GiveExtractByPeriod()
        {
            Console.Clear();
            System.Console.WriteLine("Давайте выберем временной промежуток для выписки!");

            ConsoleInputHelper periodReader = new ConsoleInputHelper();
            (bool success, DateTime data_start, DateTime data_end) = periodReader.ReadPeriod();
            if (!success)
            {
                return;
            }

            Console.Clear();
            ConsoleInputHelper selector = new ConsoleInputHelper();
            BankAccount? chosen_account = selector.SelectAccount(_accountRepository);
            if (chosen_account == null)
            {
                return;
            }
            var result = chosen_account.ShowOperationsByDate(data_start, data_end);

            if (result.Count == 0)
            {
                System.Console.WriteLine("За данный период не было ни одной операции! (Вероятно, вы выбрали не тот интервал)");
                return;
            }

            Console.Clear();
            System.Console.WriteLine($"Выписка по операциям аккаунта с именем {chosen_account.Name}:");
            System.Console.WriteLine($"Период с {data_start:dd.MM.yyyy} по {data_end:dd.MM.yyyy}");

            int count = 0;

            for (int i = 0; i < result.Count; ++i)
            {
                System.Console.WriteLine($"\nОперация №{i + 1}:");
                System.Console.WriteLine($"ID операции: {result[i].OperationId};");
                System.Console.WriteLine($"Дата: {result[i].Date:dd.MM.yyyy};");
                System.Console.WriteLine($"Тип операции: {result[i].Type};");
                string categ_name = "Категория не найдена";

                var cat = _categoryRepository.FindCategoryById(result[i].CategoryId);
                if (cat != null)
                {
                    categ_name = cat.Name;
                }
                System.Console.WriteLine($"Категория операции: {categ_name};");
                System.Console.WriteLine($"Сумма операции: {result[i].Amount:F2};");
                System.Console.WriteLine($"Описание: {result[i].Description ?? "Пусто"}.");
                count++;
            }
            System.Console.WriteLine($"\nИтого операций: {count}");
            return;
        }
    }
}