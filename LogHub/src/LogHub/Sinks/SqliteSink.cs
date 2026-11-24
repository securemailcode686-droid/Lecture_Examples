using System;
using System.Globalization;
using LogHub.Composition;
using LogHub.Configuration;
using LogHub.Model;
using Microsoft.Data.Sqlite;

namespace LogHub.Sinks
{
    [ConfigName("sqlite")]
    public sealed class SqliteSink : ILogSink, IDisposable
    {
        private readonly SqliteConnection _connection;

        public SqliteSink(IConfigService config)
        {
            var path = config.GetValueOrDefault(
                ConfigKeys.Connection, "loghub.db");

            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = path
            };

            _connection = new SqliteConnection(
                builder.ToString());

            try
            {
                _connection.Open();

                using var command = _connection.CreateCommand();

                command.CommandText = """
                    CREATE TABLE IF NOT EXISTS logs (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        date TEXT NOT NULL,
                        severity TEXT NOT NULL,
                        message TEXT NOT NULL,
                        status_code INTEGER,
                        formatted_text TEXT NOT NULL
                    )
                    """;

                command.ExecuteNonQuery();
            }
            catch
            {
                _connection.Dispose();
                throw;
            }
        }

        public void Write(LogRecord record, string text)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = """
                INSERT INTO logs
                    (date, severity, message,
                     status_code, formatted_text)
                VALUES
                    (@date, @severity, @message,
                     @status, @text)
                """;

            command.Parameters.AddWithValue(
                "@date", record.Date.ToString(
                    "O", CultureInfo.InvariantCulture));

            command.Parameters.AddWithValue(
                "@severity", record.Severity);

            command.Parameters.AddWithValue(
                "@message", record.Message);

            command.Parameters.AddWithValue(
                "@status",
                (object?)record.StatusCode ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@text", text);

            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
