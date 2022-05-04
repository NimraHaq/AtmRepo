using System;
using DataAccessLayer;
using BusinessObjects;

namespace BusinessLogicLayer
{
    public class CustomerBLL
    {
        public static List<Customer> searchAccounts(string query)
        {
            List<Customer> customerList = new List<Customer>();
            customerList = CustomerDAL.searchAccount(query);
            foreach (Customer customer in customerList)
            {
                customer.userName = encryptionDecription(customer.userName);
            }
            return customerList;
        }
        public static List<CustomerRecords> getAccountInDateRAnge(string username, DateTime start, DateTime end)
        {
            username = encryptionDecription(username);
            List<CustomerRecords> records = CustomerDAL.getAccountInDateRange(username, start, end);
           return records;
        }
        public static List<Customer> getAccountInRangeBLL(int maxAmount, int minAmount)
        {
            List<Customer> customerList = new List<Customer>();
            customerList = CustomerDAL.getAccountsInRange(maxAmount, minAmount);
            foreach (Customer customer in customerList)
            {
                customer.userName = encryptionDecription(customer.userName);  //decrypting the username
            }
            return customerList;
        }
        public static void updateCustomerAccountBLL(Customer customer)
        {
            customer.userName = encryptionDecription(customer.userName);
            customer.Pin = encryptionDecription(customer.Pin);
            //now we need to send to data to DAL so that it can be updated in the database.
            CustomerDAL.updateCustomerAccount(customer);
        }
        public static void deleteCustomerAccountBLL(Customer customer)
        {
            customer.userName = encryptionDecription(customer.userName);
            customer.Pin = encryptionDecription(customer.Pin);  //we have encrypted the info before sending it to the data access layer
            CustomerDAL.deleteCustomerAccountDAL(customer);  //the DAL deleted the account from the database
        }
        public static void createCustomerBLL(Customer customer)
        {
            customer.userName = encryptionDecription(customer.userName);
            customer.Pin = encryptionDecription(customer.Pin);
            //encrypting the username and pin before storing it in the database
            CustomerDAL.createCustomerInDB(customer);
        }
        public static bool ifUserNameExistsBLL(string username)
        {
            username = encryptionDecription(username); //encrypting the username before sending it to the data access layer
            return CustomerDAL.ifUserNameExists(username);
        }
        public static Customer depositeAmount(Customer customer)
        {
            customer.Balance = CustomerBLL.checkBalance(customer) + customer.movingMoney;
            CustomerBLL.updateBalanceBLL(customer);
            return customer;
        }
        public static Customer saveDepositeRecords(Customer customer)
        {
            CustomerRecords customerRecords = new CustomerRecords();
            customerRecords.userName = customer.userName;
            customerRecords.Mode = "D";
            customerRecords.Date = DateTime.Now.ToString("dd/MM/yyyy");
            customerRecords.remainingBalance = customer.Balance;
            customerRecords.amountMoved = customer.movingMoney;

            //now we need to save the record of this money withdrawal in database

            if (CustomerDAL.addRecordInRecordTable(customerRecords))
            {
                //as moneywithdrawal is done, and its record is also saved
                customer.movingMoney = 0;
            }

            return customer;
        }
        public static Customer addBalance(Customer customerToTransfer, Customer customer)
        {
            customerToTransfer.userName = encryptionDecription(customerToTransfer.userName);
            customerToTransfer.Pin = encryptionDecription(customerToTransfer.Pin);
            customerToTransfer.Balance = customerToTransfer.Balance + customer.movingMoney;
            customerToTransfer.movingMoney = customer.movingMoney;
            customer.movingMoney = 0;
            updateBalanceBLL(customerToTransfer);  //this will update the balance of the customer in the database
            //now we need to add record of this transaction in the records table
            customerToTransfer = saveDepositeRecords(customerToTransfer);
            return customerToTransfer;
        }
        public static Customer saveTransferRecords(Customer customer)
        {
            CustomerRecords customerRecords = new CustomerRecords();
            customerRecords.userName = customer.userName;
            customerRecords.Mode = "T";
            customerRecords.Date = DateTime.Now.ToString("dd/MM/yyyy");
            customerRecords.remainingBalance = customer.Balance;
            customerRecords.amountMoved = customer.movingMoney;

            //now we need to save the record of this money withdrawal in database

            if (CustomerDAL.addRecordInRecordTable(customerRecords))
            {
                //as moneywithdrawal is done, and its record is also saved
                customer.movingMoney = 0;
            }

            return customer;
        }
        public static Customer makeTransfer(Customer customer)
        {
            customer.Balance = customer.Balance - customer.movingMoney;
            updateBalanceBLL(customer);
            //we have updated the new balance of the customer after he/she made the transfer
            //now we need to save the record of transfer
            customer = saveTransferRecords(customer);
            //now we have saved the record of transfer
            return customer;
        }
        public static Customer getInfoFromAcountNoBLL(Customer customer)
        {
            customer = CustomerDAL.getInfoFromAccountNo(customer); 
            //we need to decrypt the username and pin before sending them back to the presentation layer
            customer.userName = encryptionDecription(customer.userName);
            customer.Pin = encryptionDecription(customer.Pin);
            return customer;
        }
        public static bool ifAcountNumberExists(Customer customer)
        {
            if(CustomerDAL.ifAcountnoExistDAL(customer) == true)
            {
                return true;  //account number exists in the database, so return true
            }
            return false;
        }
        public static bool canWithdrawToday(Customer customer)
        {
            int amountWithdrewToday = CustomerDAL.amountWithdrewToday(customer);
            if(customer.movingMoney + amountWithdrewToday <= 20000)
            {
                return true; // the user can withdraw the amount he/she entered
            }
            else if(customer.movingMoney + amountWithdrewToday > 20000)
            {
                customer.movingMoney = 0;
                return false; //the customer can not withdraw the amount
            }
            return false;
        }
        public static Customer saveWithdrawalInRecordsTable(Customer customer)
        {
            CustomerRecords customerRecords = new CustomerRecords();
            customerRecords.userName = customer.userName;
            customerRecords.Mode = "W";
            customerRecords.Date = DateTime.Now.ToString("dd/MM/yyyy");
            customerRecords.remainingBalance = customer.Balance;
            customerRecords.amountMoved = customer.movingMoney;

            //now we need to save the record of this money withdrawal in database

            if (CustomerDAL.addRecordInRecordTable(customerRecords))
            {
                //as moneywithdrawal is done, and its record is also saved
                customer.movingMoney = 0;
            }

            return customer;
        }
        public static Customer withdrawMoney(Customer customer)
        {
            customer.Balance = customer.Balance - customer.movingMoney;
            updateBalanceBLL(customer);
            customer = saveWithdrawalInRecordsTable(customer);
            return customer;
        }
        public static bool canWithdraw(Customer customer)
        {
            customer.Balance = checkBalance(customer);
            if(customer.Balance < customer.movingMoney)
            {
                customer.movingMoney = 0; //as money withdrawal is not possible
                return false;
            }
            else if (customer.Balance >= customer.movingMoney)
            {
                return true;
            }
            return false;
        }
        public static Customer returnDataBLL(Customer customer)
        {
            return CustomerDAL.returnAllData(customer);
        }
        public static void updateBalanceBLL(Customer customer)
        {
            //we need to update the balance in the database
            CustomerDAL.updateBalanceDAL(customer);
        }
        public static int checkBalance(Customer customer)
        {
            //we need to find customer's balance from the database
            return CustomerDAL.returnCustomerBalance(customer);
        }
        public static string encryptionDecription(string str)
        {
            //encrypting the string
            char[] arr = new char[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                arr[i] = ' ';
            }
            int x = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] > 64 && str[i] < 91)  //if the character is a capital letter
                {
                    x = (int)str[i] + ((90 - (int)str[i]) - ((int)str[i] % 65));
                    arr[i] = (char)x;
                }
                else if (str[i] > 96 && str[i] < 123)
                {
                    x = (int)str[i] + ((122 - (int)str[i]) - ((int)str[i] % 97));
                    arr[i] = (char)x;
                }
                else if (str[i] > 47 && str[i] < 58)
                {
                    x = (int)str[i] + ((57 - (int)str[i]) - ((int)str[i] % 48));
                    arr[i] = (char)x;
                }
            }
            string data = new string(arr);
            return data;  //encrypted string is returned
        }
        public static string getCustomerStatus(Customer customer)
        {
            //this function will get the customer status from data access layer and returns it to 
            // the presentation layers
            return CustomerDAL.returnCustomerStatus(customer);
        }
        public static bool changeCustomerStatus(Customer customer)
        {
            //we need to change the status of the customer in the database
            bool change = CustomerDAL.changeCustomerStatus(customer); // sending customer obejct to data access
                                                                      //layer where its status is changed
            return change;
        }
        public static bool verfiyUserName(Customer customer)
        {
            customer.userName = encryptionDecription(customer.userName);  //we encripted the username
            //we need to check if the username exists in the database
            if (CustomerDAL.checkUserNameDAL(customer) == true)
            {
                return true;
            }
            return false;
        }
        public static bool verifyPin(Customer customer)
        {
            customer.Pin = encryptionDecription(customer.Pin);  //encrypting the pin before sending to the database
            if (CustomerDAL.checkPinDAL(customer) == true)
            {
                return true;
            }
            return false;
        }
    }
    public class AdministratorBLL
    {
        public static bool verfiyUserName(Administrator administrator)
        {
            administrator.userName = CustomerBLL.encryptionDecription(administrator.userName);  //encrypting the name
            //we encripted the username
            //we need to check if the username exists in the database
            if(AdministratorDAL.checkUserNameDAL(administrator) == true)
            {
                return true; //the username exists in the database
            }
            return false;  //return false, if username doesnot exists in the database.
        }
        public static bool verifyPin(Administrator administrator)
        {
            administrator.Pin = CustomerBLL.encryptionDecription(administrator.Pin);  //encrypting the pin before sending to the database
           if(AdministratorDAL.checkPinDAL(administrator) == true)
            {
                return true;
            }
            return false;
        }
    }
}