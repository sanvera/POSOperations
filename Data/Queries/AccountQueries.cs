namespace Data.Queries
{
    public static class AccountQueries
    {
        public const string CreateAccountTableQuery =
            "CREATE TABLE tblAccounts (AccountId BIGINT NOT NULL PRIMARY KEY, Balance MONEY NOT NULL DEFAULT(0))";

        public const string SelectAccountBalanceQuery = "Select Amount From tblAccounts Where AccountId = @AccountId";

        public const string SelectAccountByAccountId = "Select * From tblAccounts Where AccountId = @AccountId";

        public const string InsertIntoAccountsTableQuery = "INSERT INTO tblAccounts VALUES (@AccountId, @Balance)";

        public const string UpdateBalanceQuery = "UPDATE tblAccounts SET Balance = @Amount Where AccountId = @AccountId";

        public const string AddBalanceQuery = "UPDATE tblAccounts SET Balance = Balance + @Amount Where AccountId = @AccountId";
    }
}
