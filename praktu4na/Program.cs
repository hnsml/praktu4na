using Microsoft.Data.Sqlite;
using System;
using System.Text;

internal class Program
{
    private static string ConnectionString = "Data Source=students.sqlite;";

    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        CreateTable(connection);
        CreateItem(connection);

        ReadAndDisplayAll(connection, "SELECT * FROM student_marks", "Вся інформація про оцінки студентів");
        ReadAndDisplayAll(connection, "SELECT firstname, lastname FROM student_marks", "ПІБ студентів");
        ReadAndDisplayAll(connection, "SELECT midlemark FROM student_marks GROUP BY midlemark", "Середні оцінки студентів");
        ReadAndDisplayAll(connection, "SELECT firstname, lastname, midlemark FROM student_marks WHERE midlemark > 9", "Студенти, в яких середній бал більше 9");
        ReadAndDisplayAll(connection, "SELECT subjectmin FROM student_marks GROUP BY subjectmin", "Назви усіх предметів із мінімальними середніми оцінками");
        ReadAndDisplayAll(connection, "SELECT MIN(midlemark) FROM student_marks", "Мінімальна середня оцінка");
        ReadAndDisplayAll(connection, "SELECT MAX(midlemark) FROM student_marks", "Максимальна середня оцінка");
        ReadAndDisplayAll(connection, "SELECT COUNT(*) FROM student_marks WHERE subjectmin=\"Математика\"", "Кількість студентів з мінімальною середньою оцінкою з математики");
        ReadAndDisplayAll(connection, "SELECT COUNT(*) FROM student_marks WHERE subjectmax=\"Математика\"", "Кількість студентів з максимальною середньою оцінкою з математики");
        ReadAndDisplayAll(connection, "SELECT number,  COUNT(*) FROM student_marks GROUP BY number", "Кількість студентів у кожній групі");
        ReadAndDisplayAll(connection, "SELECT number, AVG(midlemark)  FROM student_marks GROUP BY number", "Cередня оцінка групи");

        Console.ReadKey();
    }

    private static void CreateTable(SqliteConnection connection)
    {
        string sql = "CREATE TABLE IF NOT EXISTS student_marks (firstname VARCHAR(255), lastname VARCHAR(255), number VARCHAR(255), midlemark INT, subjectmin VARCHAR(255), subjectmax VARCHAR(255))";
        ExecuteNonQuery(connection, sql);
    }

    private static void CreateItem(SqliteConnection connection)
    {
        using var transaction = connection.BeginTransaction();

        string[,] items = {
    {"Сидоренко", "Анна", "120 Г","7", "Розробка Web-додатків", "3-D моделювання"},
    {"Литвиненко", "Максим", "344 А","9", "Програмування", "Інформатика"},
    {"Шевчук", "Катерина", "267 В","10", "Англійська мова", "Математика"},
    {"Коваленко", "Олександр", "244 А","11", "Хімія", "Фізика"},
    {"Мельник", "Юлія", "313 В","10", "Історія", "Географія"},
    {"Васильєв", "Віктор", "112 Б", "3","Економіка", "Право"},
    {"Іванова", "Анастасія", "108 Б","7", "Мистецтво", "Музика"},
    {"Петренко", "Ольга", "431 В", "5","Психологія", "Соціологія"},
    {"Павленко", "Тетяна", "212 Г","9", "Українська мові", "Біологія"},
    {"Петренко", "Дмитро", "150 В","12", "Фізична культура", "Захист Вітчизни"}
        };

        for (int i = 0; i < items.GetLength(0); i++)
        {
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO student_marks (firstname, lastname, number, midlemark, subjectmin, subjectmax) VALUES ($firstname, $lastname, $number, $midlemark, $subjectmin, $subjectmax)";

            string firstname = items[i, 0];
            string lastname = items[i, 1];
            string number = items[i, 2];
            int midlemark = int.Parse(items[i, 3]);
            string subjectmin = items[i, 4];
            string subjectmax = items[i, 5];

            insertCommand.Parameters.AddWithValue("$firstname", firstname);
            insertCommand.Parameters.AddWithValue("$lastname", lastname);
            insertCommand.Parameters.AddWithValue("$number", number);
            insertCommand.Parameters.AddWithValue("$midlemark", midlemark);
            insertCommand.Parameters.AddWithValue("$subjectmin", subjectmin);
            insertCommand.Parameters.AddWithValue("$subjectmax", subjectmax);

            insertCommand.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    private static void ReadAndDisplayAll(SqliteConnection connection, string query, string displayMessage)
    {
        using var command = connection.CreateCommand();
        command.CommandText = query;

        try
        {
            using var reader = command.ExecuteReader();
            Console.WriteLine();
            Console.WriteLine(displayMessage);
            Console.WriteLine();

            while (reader.Read())
            {
                string output = "";
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    output += $"{reader.GetString(i),-15}\t";
                }
                Console.WriteLine(output);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }

    private static void ExecuteNonQuery(SqliteConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }
}