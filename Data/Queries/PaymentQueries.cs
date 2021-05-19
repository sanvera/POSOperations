namespace Data.Queries
{
    public static class PaymentQueries
    {
        public const string CreatePaymentsTableQuery =
            "CREATE TABLE tblPayments (TransactionId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, AccountId BIGINT NOT NULL, Origin VARCHAR(20) NOT NULL, Amount MONEY NOT NULL DEFAULT(0))";

        public const string SelectTableQuery = "SELECT * From tblPayments";

        public const string UpdatePaymentQuery = "UPDATE tblPayments SET Amount = @amount Where AccountId = @AccountId and TransactionId = @TransactionId";

        public const string InsertIntoPaymentsTableQuery = "INSERT INTO tblPayments VALUES (@TransactionId, @AccountId, @Origin, @Amount)";

        public const string SelectIndividualPaymentQuery = "SELECT * FROM tblPayments WHERE TransactionId = @TransactionId AND AccountId = @AccountId AND Origin = @Origin";

        public const string SelectPaymentByTransactionIdQuery = "SELECT * FROM tblPayments WHERE TransactionId = @TransactionId";
    }
}
