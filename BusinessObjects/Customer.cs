using System;


namespace BusinessObjects
{
    public class CustomerRecords
    {
        private string username;
        private string mode;
        private string date;
        private int remainingbalance;
        private int amountmoved;
        public int amountMoved
        {
            get { return amountmoved; }
            set { amountmoved = value; }
        }
        public string userName
        {
            get { return username; }    
            set { username = value; }
        }
        public string Mode
        {
            get { return mode; }
            set { mode = value; }
        }
        public string Date
        {
            get { return date; }
            set { date = value; }
        }
        public int remainingBalance
        {
            get { return remainingbalance; }
            set { remainingbalance = value; }
        }
    }
    public class Customer
    {
        private string account_type;
        private string name;
        private int accountno;
        private string username;
        private string pin;
        private string status;
        private int balance;
        private int movingmoney;  //temporarily stores the money that user is withdrawing or depositing etc.
        public string AccountType
        {
            get { return account_type;}
            set { account_type = value; }
        }
        public string Name
        {
            get { return name;}
            set { name = value;}
        }
        public int movingMoney
        {
            get { return movingmoney; }
            set { movingmoney = value; }
        }
        public int accountNo
        {
            get { return accountno; }
            set { accountno = value; }
        }
        public int Balance
        {
            get { return balance; }
            set { balance = value; }
        }
        public string userName
        {
            get { return username; }
            set { username = value; }
        }
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
        public string Pin
        {
            get { return pin; }
            set { pin = value; }
        }
    }
    public class Administrator
    {
        private string username;
        private string pin;
        public string userName
        {
            get { return username; }
            set { username = value; }
        }
        public string Pin
        {
            get { return pin; }
            set { pin = value; }
        }
    }
}