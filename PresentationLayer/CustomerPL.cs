
using System;
using BusinessLogicLayer;
using BusinessObjects;
using System.Globalization;

namespace PresentationLayer
{
    public class Enter
    {
        public static void login()
        {
            Console.WriteLine("Enter 1 if you are a customer. ");
            Console.WriteLine("Enter 2 if you are an administrator.");
            string choice = Console.ReadLine();
            while (choice != "1" && choice != "2")
            {
                Console.WriteLine("INVALID ENTRY. Please enter again.");
                choice = Console.ReadLine();
            }
            if (choice == "1")
            {
                //go to customer login
                CustomerPL.customerLogin();
            }
            else if (choice == "2")
            {
                //go to admin login
                AdministratorPL.AdministratorLogin();
            }
        }
    }
    public class CustomerPL
    {
        internal static void displayBalance(Customer customer)
        {

            customer = CustomerBLL.returnDataBLL(customer);
            //print account no
            Console.WriteLine("Account # " + customer.accountNo);
            Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy"));
            Console.WriteLine("Balance : " + customer.Balance);
        }
        internal static bool printReceipt(Customer customer)
        {
            Console.WriteLine("Do you wish to print the receipt? (Y/N)");
            string yesOrNo = Console.ReadLine();
            while (yesOrNo != "Y" && yesOrNo != "N" && yesOrNo != "y" && yesOrNo != "n")
            {
                Console.WriteLine("INVALID ENTRY. Please enter again.");
                yesOrNo = Console.ReadLine();
            }
            if (yesOrNo == "Y" || yesOrNo == "y")   // print the receipt
            {
                customer = CustomerBLL.returnDataBLL(customer);
                //print account no
                Console.WriteLine("Account # " + customer.accountNo);
                Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy"));
                Console.WriteLine("Balance : " + customer.Balance);
                //print balance
                return true;
            }
            else if (yesOrNo == "N" || yesOrNo == "n")
            {
                return false;
            }
            return false;
        }
        internal static void depositeCash(Customer customer)
        {
            Console.WriteLine("Enter the cash amount to deposit:");
            string amount = Console.ReadLine();
            int amountToDeposit = 0;
            bool correctAmount = int.TryParse(amount, out amountToDeposit);
            while(!correctAmount)
            {
                Console.WriteLine("INVALID ENTRY. Please enter again.");
                amount = Console.ReadLine();
                correctAmount = int.TryParse(amount, out amountToDeposit);
            }

            //we have got the amount user wants to deposite
            
            //we have deposited the amount, and updated the database.
            customer.movingMoney = amountToDeposit;
            customer = CustomerBLL.depositeAmount(customer);
            customer = CustomerBLL.saveDepositeRecords(customer);
            //now we have updated the records of deposites and withdrawals
            Console.WriteLine("Cash Deposited Successfully.");
            bool ifReceipt = printReceipt(customer);
            if (ifReceipt == true)
            {
                Console.WriteLine("Cash deposited : " + amountToDeposit);
            }
        }
        internal static void cashTransfer(Customer customer)
        {
            Console.WriteLine("Enter amount in multiples of 500:");
            string amount1 = Console.ReadLine();
            int amountToTransfer = 0;
            bool correctAmount= int.TryParse(amount1 , out amountToTransfer);
            while(!correctAmount)
            {
                Console.WriteLine("INVALID ENTRY. Please enter again.");
                amount1 = Console.ReadLine();
                correctAmount = int.TryParse(amount1, out amountToTransfer);
            }
            while ((amountToTransfer % 500) != 0)  //making sure user enters amount in multiple of 500
            {
                Console.WriteLine("INVALID ENTRY. The amount must be a multiple of 500. Please enter again.");
                amount1 = Console.ReadLine();
                correctAmount = int.TryParse(amount1, out amountToTransfer);
                while (!correctAmount)  //making sure user enters valid input
                {
                    Console.WriteLine("INVALID ENTRY. Please enter again.");
                    amount1 = Console.ReadLine();
                    correctAmount = int.TryParse(amount1, out amountToTransfer);
                }
            }
            //the user has entered the amount he/she wishes to transfer


            Console.WriteLine("Enter the account number to which you want to transfer: ");
            string acNo = Console.ReadLine();
            int acountNoToTransfer = 0;
            bool trueAcNo = int.TryParse (acNo , out acountNoToTransfer);
            while(!trueAcNo)
            {
                Console.WriteLine("INVALID ENTRY. Please enter again.");
                acNo = Console.ReadLine();
                trueAcNo = int.TryParse(acNo, out acountNoToTransfer);
            }
            //user has entered the acount number, now we need to check if that acount number exists in the database

            Customer customerToTransfer = new Customer();
            customerToTransfer.accountNo = acountNoToTransfer;
            //
            bool exists = CustomerBLL.ifAcountNumberExists(customerToTransfer);
            if (exists == false)
            {
                Console.WriteLine("The account you entered doesnot exists in the databse.");
                return;
            }
            else if (exists == true)
            {
                //the account number user entered is correct. now we have to take confirmation from the user
                customerToTransfer = CustomerBLL.getInfoFromAcountNoBLL(customerToTransfer);
                Console.WriteLine("You wish to deposit Rs." + amountToTransfer + " in account held by " + customerToTransfer.Name + "; If this information is correct please re-enter the account number: ");
                int reEnter = Convert.ToInt32(Console.ReadLine());
                if (reEnter != acountNoToTransfer)
                {
                    Console.WriteLine("The account number you just entered doesnot match with the previous one.");
                    Console.WriteLine("Transaction cancelled.");
                }
                else if(reEnter == acountNoToTransfer)
                {
                    //the user confirmed that he/she wants to make the deposite
                    //now we need to make the transfer
                    //first we check if user's account balance has enough money to make transaction
                    customer.movingMoney = amountToTransfer;
                    bool canWithdraw = CustomerBLL.canWithdraw(customer);
                    if (canWithdraw == false)
                    {
                        customer.movingMoney = 0;
                        Console.WriteLine("Cannot complete the proceeding. You donot have enough money in your account to make the transfer.");
                        return;
                    }
                    else if (canWithdraw == true)
                    {
                        //the user has enough money to make the transfer
                        //now we are gonna transfer the money
                        customer = CustomerBLL.makeTransfer(customer);
                        //TILL HERE, WE ARE GOOD
                        
                        //we have made the withdrawal from customer and saved its record

                        //now we need to add balance to customerToTransfer
                        if (customerToTransfer.Balance != null)
                        {
                            customer.movingMoney = amountToTransfer;
                            customerToTransfer = CustomerBLL.addBalance(customerToTransfer, customer);
                            Console.WriteLine("Transaction successful.");
                            Console.WriteLine("You have transferred Rs." + amountToTransfer + " to " + customerToTransfer.Name + " account no. " + customerToTransfer.accountNo);
                            Console.WriteLine(customerToTransfer.Name + " has account balance of  : " + customerToTransfer.Balance);
                            bool ifReceipt = printReceipt(customer);
                            Console.WriteLine("The amount transferred : " + amountToTransfer);
                        }
                    }
                }
            }
        }
        internal static void normalCashWithDrawal(Customer customer)
        {
            Console.WriteLine("Enter the withdrawal amount: ");
            string amount1 = Console.ReadLine();
            int amountToWithdraw = 0;
            bool trueInput = int.TryParse(amount1 , out amountToWithdraw);
            while(!trueInput)
            {
                Console.WriteLine("INVALID INPUT. Please enter again");
                amount1 = Console.ReadLine();
                trueInput = int.TryParse(amount1, out amountToWithdraw);
            }
            customer.movingMoney = amountToWithdraw;
            bool canWithdraw = CustomerBLL.canWithdraw(customer);   //BLL checks if user can withdraw the amount
            //i mean if the user has enough money.
            if (canWithdraw == false)
            {
                customer.movingMoney = 0;
                customer.Balance = CustomerBLL.checkBalance(customer);  //checking the balance of customer
                Console.WriteLine("Cannot complete the proceeding. You do not have enough amount in your account.");
                Console.WriteLine("Your account balance is : " + customer.Balance);
            }
            else if (canWithdraw == true)
            {
                //----
                //now we know that the customer has enough money to withdraw from his account.
                //but the customer can not withdraw more than 20000 per day
                bool canWithdrawToday = CustomerBLL.canWithdrawToday(customer);
                if (canWithdrawToday == true)
                {
                    customer = CustomerBLL.withdrawMoney(customer);
                    Console.WriteLine("Cash Successfully Withdrawn!");
                    //now we print the receipt if user wants
                    bool ifPrint = printReceipt(customer);
                    if (ifPrint == true)
                    {
                        Console.WriteLine("Witdraw : " + amountToWithdraw);
                    }
                }
                else if (canWithdrawToday == false)
                {
                    Console.WriteLine("Process unsuccessful. You are trying to withdraw more than the restricted amount. You cannot withdraw more than Rs.20000 per day.");
                }
            }
        }
        internal static void fastCashWithdrawal(Customer customer)
        {
            Console.WriteLine("1-----500");
            Console.WriteLine("2-----1000");
            Console.WriteLine("3-----2000");
            Console.WriteLine("4-----5000");
            Console.WriteLine("5-----10000");
            Console.WriteLine("6-----15000");
            Console.WriteLine("7-----20000");
            Console.WriteLine("Select one of the denominations of money.");
            string denominationNo = Console.ReadLine();
            while (denominationNo != "1" && denominationNo != "2" && denominationNo != "3" && denominationNo != "4" && denominationNo != "5" && denominationNo != "6" && denominationNo != "7")
            {
                Console.WriteLine("INVALID ENTRY. Please enter again.");
                denominationNo = Console.ReadLine();
            }
            int moneyToWithdraw = 0;
            if (denominationNo == "1") { moneyToWithdraw = 500; }
            else if (denominationNo == "2") { moneyToWithdraw = 1000; }
            else if (denominationNo == "3") { moneyToWithdraw = 2000; }
            else if (denominationNo == "4") { moneyToWithdraw = 5000; }
            else if (denominationNo == "5") { moneyToWithdraw = 10000; }
            else if (denominationNo == "6") { moneyToWithdraw = 15000; }
            else if (denominationNo == "7") { moneyToWithdraw = 20000; }
            Console.WriteLine("Are you sure you want to withdraw Rs." + moneyToWithdraw + "? (Y/N)");
            string yesOrNo = Console.ReadLine();
            while (yesOrNo != "Y" && yesOrNo != "N" && yesOrNo != "y" && yesOrNo != "n")
            {
                Console.WriteLine("INVALID ENTRY. Please enter again.");
                yesOrNo = Console.ReadLine();
            }
            if (yesOrNo == "Y" || yesOrNo == "y")
            {
                //the user has selected the amount he/she needs to withdraw 
                //now we need to check if the user has the required amount in his/her acount
                customer.movingMoney = moneyToWithdraw;  //storing the amount user wants to withdraw in BO
                bool canWithdraw = CustomerBLL.canWithdraw(customer);   //BLL checks if user can withdraw the amount
                //i mean if the user has enough money.

                if (canWithdraw == false)
                {
                    customer.movingMoney = 0;
                    customer.Balance = CustomerBLL.checkBalance(customer);  //checking the balance of customer
                    Console.WriteLine("Cannot complete the proceeding. You do not have enough amount in your account.");
                    Console.WriteLine("Your account balance is : " + customer.Balance);
                }
                else if (canWithdraw == true)  //customer withdraws money
                {
                    //----
                    //now we know that the customer has enough money to withdraw from his account.
                    //but the customer can not withdraw more than 20000 per day
                    bool canWithdrawToday = CustomerBLL.canWithdrawToday(customer);

                    if (canWithdrawToday == true)
                    {
                        customer = CustomerBLL.withdrawMoney(customer);
                        Console.WriteLine("Cash Successfully Withdrawn!");
                        //now we print the receipt if user wants
                        bool ifPrint = printReceipt(customer);
                        if (ifPrint == true)
                        {
                            Console.WriteLine("Witdraw : " + moneyToWithdraw);
                        }
                    }
                    else if (canWithdrawToday == false)
                    {
                        Console.WriteLine("Process unsuccessful. You are trying to withdraw more than the restricted amount. You cannot withdraw more than Rs.20000 per day.");
                    }
                }
            }
            else if (yesOrNo == "N" || yesOrNo == "n")
            {
                return;
            }
        }
        internal static void customerMenu(Customer customer)
        {
            Console.WriteLine("Please select one of the following options : ");
            Console.WriteLine("Press 1 to withdraw cash.");
            Console.WriteLine("Press 2 to Cash Transfer.");
            Console.WriteLine("Press 3 to Deposit Cash.");
            Console.WriteLine("Press 4 to Display Balance.");
            Console.WriteLine("Press 5 to Exit.");
            string choice = Console.ReadLine();
            while (choice != "1" && choice != "2" && choice != "3" && choice != "4" && choice != "5")
            {
                Console.WriteLine("INVALID ENTRY. Please enter again.");
                choice = Console.ReadLine();
            }
            if (choice == "5")  // user wants to exit
            {
                return;  //exit the function
            }
            else if (choice == "1")  //the user wants to withdraw cash
            {
                Console.WriteLine("Please select the mode of withdrawal. ");
                Console.WriteLine("Press 1 for Fast Cash");
                Console.WriteLine("Press 2 for Normal Cash");
                Console.WriteLine("Press 3 to Exit.");
                string withdrawalMode = Console.ReadLine();
                while (withdrawalMode != "1" && withdrawalMode != "2" && withdrawalMode != "3")
                {
                    Console.WriteLine("INVALID ENTRY. Please enter again.");
                    withdrawalMode = Console.ReadLine();
                }
                if (withdrawalMode == "3")
                {
                    return;   //user wants to exits, return the function
                }
                else if (withdrawalMode == "1")  //user wants fast cash
                {
                    //fast cash
                    fastCashWithdrawal(customer);
                }
                else if (withdrawalMode == "2")// user wants normal cash
                {
                    //normal cash
                    normalCashWithDrawal(customer);
                }
            }
            else if (choice == "2")  //cash transfer
            {

                cashTransfer(customer);
            }
            else if (choice == "3")  //user wants to deposite cash
            {
                depositeCash(customer);
            }
            else if (choice == "4")  //display balance
            {

                displayBalance(customer);
            }
        }
        internal static void customerLogin()
        {
            string username;
            string pin;
            Customer customer = new Customer();


            //the customer enters its user name
            Console.WriteLine("Enter the username(less than 11 characters) : ");
            username = Console.ReadLine();
            while (username.Length > 10)
            {
                Console.WriteLine("INVALID username. Enter again. ");
                username = Console.ReadLine();
            }
            while (username.Length < 10)  //making length of username equal to 10
            {
                username += " ";
            }
            username = username.ToLower();  //converting the user name to lower case, as we store 
                          // username in lower case in the database
            customer.userName = username;

           
            //now we need to verify the username
            //we send this username to the business logic layer
            bool verification = CustomerBLL.verfiyUserName(customer);
            if (verification == false)
            {
                Console.WriteLine("The username doesnot exists.");
            }
            else if (verification == true)
            {
                Console.WriteLine("The username exists in the database. ");

                //as the username exist in database, we will ask the pin

                //---------------------

                int chances = 3;
                while (chances > 0)
                {
                    Console.WriteLine("please enter the PIN (five digit code) :");  //the pin should be five digit
                    pin = Console.ReadLine();
                    
                 
                    while (pin.Length != 5 )
                    {
                        Console.WriteLine("INVALID ENTRY. The pin must be 5 digit.");
                        pin = Console.ReadLine();
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        pin += " ";  //now pin is in nchar(10)
                    }
                    customer.Pin = pin;

                    //we have asked the pin, now we need to verify the pin from the database.
                    bool verificationPin = CustomerBLL.verifyPin(customer);
                    if (verificationPin == false)
                    {
                        Console.WriteLine("Wrong pin!");
                        chances--;
                        if (chances == 0)  //user entered wrong pin three times.
                        {
                            Console.WriteLine("You have entered the PIN wrong three times!");
                            //now we need to disbale the status of customer.
                            customer.Status = "F"; // status disabled
                            bool change = CustomerBLL.changeCustomerStatus(customer);
                            if (change)
                            {
                                Console.WriteLine("Login Disabled.");
                            }
                            break;
                        }
                    }
                    else if (verificationPin == true)
                    {
                        Console.WriteLine("Pin correct.");
                        //the user has entered the correct username and pin. now we need to check if 
                        //the user status is not disabled. if disabled, the user cannot login the system
                        string Customer_Status = CustomerBLL.getCustomerStatus(customer);
                        if (Customer_Status == "F")
                        {
                            Console.WriteLine("Cannot login .The customer login is disabled.");
                        }
                        else
                        {
                            Console.WriteLine("Login successfull.");
                            customerMenu(customer);
                        }
                        break;

                    }
                }
            }
        }
    }
    public class AdministratorPL
    {
        internal static void getAccountInRangePL()
        {
            int maxAmount = 0;
            int minAmount = 0;
            //user enters min amount
            Console.WriteLine("Enter the minimum amount: ");
            string min = Console.ReadLine();
            while (!int.TryParse(min, out minAmount))
            {
                Console.WriteLine("INVALID INPUT. Enter a valid amount.");
                min = Console.ReadLine();
            }

            //user enters the maximum amount
            Console.WriteLine("Enter the maximum amount: ");
            string max = Console.ReadLine();
            while (!int.TryParse(max, out maxAmount))
            {
                Console.WriteLine("INVALID INPUT. Enter a valid amount.");
                max = Console.ReadLine();
            }

            //user has entered the amounts.
            List<Customer> customerList = new List<Customer>();
            customerList = CustomerBLL.getAccountInRangeBLL(maxAmount, minAmount);
            if (customerList.Count == 0)
            {
                Console.WriteLine("No customer has account balance in the given range. ");
                return;
            }
            else
            {
                foreach (Customer customer in customerList)
                {
                    Console.WriteLine("---------------------------------------------------");
                    Console.WriteLine("Account Id : " + customer.accountNo);
                    Console.WriteLine("Username : " + customer.userName);
                    Console.WriteLine("Holder's name : " + customer.Name);
                    Console.WriteLine("Type : " + customer.AccountType);
                    Console.WriteLine("Balance : " + customer.Balance);
                    Console.WriteLine("Status : " + customer.Status);
                    Console.WriteLine("---------------------------------------------------");
                }
            }
        }
        internal static void getAccountInDateRange()
        {
            Console.WriteLine("Enter the account number : ");
            string AcNo = Console.ReadLine();
            int accountNumber = 0;
            while ( !int.TryParse(AcNo, out accountNumber) )
            {
                Console.WriteLine("INVALID ENTRY. Please enter a valid account number.");
                AcNo = Console.ReadLine();
            }
            //user has entered the account number
            //now we need to check if the account exists in database
            Customer customer = new Customer();
            customer.accountNo = accountNumber;
            bool ifAccountExists = CustomerBLL.ifAcountNumberExists(customer);
            if (ifAccountExists == false)
            {
                Console.WriteLine("The account number you entered doesnot exists in the database.");
                Console.WriteLine("Program ends.");
                return;
            }
            else if (ifAccountExists == true)
            {
                customer = CustomerBLL.getInfoFromAcountNoBLL(customer);
                DateTime startingDate = new DateTime();
                DateTime endingDate = new DateTime();
                //the account number exists
                Console.WriteLine("NOTE : The format for the date must always be Day/Month/Year [DD/MM/YYYY]");
                Console.WriteLine("Enter the starting date:");
                string startDate = Console.ReadLine();
                while( !DateTime.TryParseExact(startDate, "dd/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out startingDate))
                {
                    Console.WriteLine("INVALID ENTRY. Please enter again.");
                    startDate = Console.ReadLine();
                }
                Console.WriteLine("Enter the ending date:");
                string endDate = Console.ReadLine();
                while (!DateTime.TryParseExact(endDate, "dd/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out endingDate))
                {
                    Console.WriteLine("INVALID ENTRY. Please enter again.");
                    endDate = Console.ReadLine();
                }
                //user has entered accout number as well as starting and ending dates
                List<CustomerRecords> customerRecords = CustomerBLL.getAccountInDateRAnge(customer.userName, startingDate, endingDate);
                if (customerRecords.Count == 0)
                {

                    Console.WriteLine("No record found.");
                    return;
                }
                else
                {
                    Console.WriteLine("---------------------------");
                    Console.WriteLine("Holder's name : " + customer.Name);
                    Console.WriteLine("Username : " + customer.userName);
                    foreach(CustomerRecords record in customerRecords)
                    {
                        Console.WriteLine("------");
                        Console.WriteLine("Amount : " + record.amountMoved);
                        Console.WriteLine("Date : " + record.Date);
                        Console.WriteLine("Transaction Type : " + record.Mode);
                    }
                    Console.WriteLine("---------------------------");
                }
            }
        }
        internal static void viewReports()
        {
            Console.WriteLine("1---Accounts By Amount");
            Console.WriteLine("2---Accounts By Date");
            string choice = Console.ReadLine();
            while (choice != "1" && choice != "2")
            {
                Console.WriteLine("INVALID ENTRY. Please enter again.");
                choice = Console.ReadLine();
            }
            if (choice == "1")
            {
                getAccountInRangePL();
            }
            else if (choice == "2")
            {
                //the system shows all
                //transactions made by a specified customer within a specified date range
                getAccountInDateRange();
            }
        }
        internal static void SearchForAccount()
        {
            string query = "select Id, UserName, Name, Type, Balance, status from CustomerTable where ";
            bool flag = false;
            bool addAnd = false;
            Console.WriteLine("SEARCH MENU: ");

            //account number-------------------------
            Console.WriteLine("Account number : ");
            string acNo = Console.ReadLine();
            int acountNo;
            if (acNo != "")
            {
                bool ifInt = int.TryParse(acNo, out acountNo);
                while (! ifInt)
                {
                    Console.WriteLine("INVALID ENTRY. Please enter again.");
                    acNo = Console.ReadLine();
                    if(acNo == "")
                    {
                        break;
                    }
                    ifInt = int.TryParse(acNo, out acountNo);
                }
                if(ifInt)
                {
                    query = query + $"Id = '{acountNo}' ";
                    addAnd = true;
                    flag = true;
                }
            }

            //username -----------------------------
            Console.WriteLine("Username : ");
            string username = Console.ReadLine();
            if(username != "")
            {
                if(addAnd == true)
                {
                    query = query + " and ";
                }
                username = CustomerBLL.encryptionDecription(username);
                query = query +  $"UserName = '{username}' ";
                addAnd = true;
                flag = true;
            }

            //holder's name-----------------
            Console.WriteLine("Holder's name :");
            string name = Console.ReadLine();
            if(name != "")
            {
                if (addAnd == true)
                {
                    query = query + " and ";
                }
                query = query + $"Name = '{name}' ";
                addAnd= true;
                flag = true;
            }

            //type -----------------
            Console.WriteLine("Type (savings, current) : ");
            string type = Console.ReadLine();
            if(type != "")
            {
                type = type.ToLower();
                while(type != "savings" && type != "current")
                {
                    Console.WriteLine("INVALID ENTRY. Please enter again.");
                    type = Console.ReadLine();
                    if( type == "")
                    {
                        break;
                    }
                    type = type.ToLower();
                }
                if(type == "savings" || type == "current")
                {
                    if(addAnd == true)
                    {
                        query = query + " and ";
                    }
                    query = query + $"Type = '{type}' ";
                    addAnd = true;
                    flag = true;
                }
            }
            //balance-----------
            //
            Console.WriteLine("Balance : ");
            string blnc = Console.ReadLine();
            int balance;
            if (blnc != "")
            {
                bool ifBalance = int.TryParse(blnc, out balance);
                while (! ifBalance)
                {
                    Console.WriteLine("INVALID ENTRY. Please enter again.");
                    blnc = Console.ReadLine();
                    if (blnc == "")
                    {
                        break;
                    }
                    ifBalance = int.TryParse(blnc, out balance);
                }
                if (ifBalance)
                {
                    if(addAnd == true)
                    {
                        query = query + " and ";
                    }
                    query = query + $"Balance = '{balance}' ";
                    addAnd = true;
                    flag = true;
                }
            }

            //-Status ---------------

            Console.WriteLine("Status : ");
            string status = Console.ReadLine();
            if (status != "")
            {
                status = status.ToLower();
                while (status != "active" && status != "disabled")
                {
                    Console.WriteLine("INVALID ENTRY. Please enter again. ");
                    status = Console.ReadLine();
                    if (status == "")
                    {
                        break;
                    }
                    status = status.ToLower();
                }
                if (status == "active" && status == "disabled")
                {
                    if(addAnd == true)
                    {
                        query = query + " and ";
                    }
                    query = query + $"status = '{status}' ";
                    addAnd = true;
                    flag = true;
                }
            }

            List<Customer> customerList = new List<Customer>();
            if (flag == false)
            {
                query = "select Id, UserName, Name, Type, Balance, status from CustomerTable";
            }
            customerList = CustomerBLL.searchAccounts(query);
            Console.WriteLine("-----------------------------");
            foreach(Customer customer in customerList)
            {
                Console.WriteLine("Account number : " + customer.accountNo);
                Console.WriteLine("username : " + customer.userName);
                Console.WriteLine("Name : " + customer.Name);
                Console.WriteLine("Type : " + customer.AccountType);
                Console.WriteLine("Balance : " + customer.Balance);
                Console.WriteLine("Status : " + customer.Status);
                Console.WriteLine("------------");
            }

        }
        internal static string checkLengthAndSpace(string username)
        {
            while (username.Length > 10 || username.Length == 0)
            {
                Console.WriteLine("INVALID ENTRY. The length of username cannot be greater than 10. Please enter again.");
                username = Console.ReadLine();
                while (username.Contains(" "))
                {
                    Console.WriteLine("INVALID ENTRY. The username cannot contain spaces. Please enter again.");
                    username = Console.ReadLine();
                }
            }
            while (username.Contains(" "))
            {
                Console.WriteLine("INVALID ENTRY. The username cannot contain spaces. Please enter again.");
                username = Console.ReadLine();
                while (username.Contains(" "))
                {
                    Console.WriteLine("INVALID ENTRY. The username cannot contain spaces. Please enter again.");
                    username = Console.ReadLine();
                }
            }
            return username;
        }
        internal static string inputChecksOnPin(string pin)
        {
            int pinInt = 0;
            //input checks on the pin
            while (pin.Length != 5 || pin.Length == 0)
            {
                Console.WriteLine("INVALID ENTRY. The PIN must be five digits. Please enter again.");
                pin = Console.ReadLine();
                while (!int.TryParse(pin, out pinInt))  //if the pin cannot be converted into int, i mean pin is not all digits
                {
                    Console.WriteLine("INVALID ENTRY. The PIN must be all digits. Please enter again.");
                    pin = Console.ReadLine();
                }
            }
            while (!int.TryParse(pin, out pinInt))
            {
                Console.WriteLine("INVALID ENTRY. The PIN must be all digits. Please enter again.");
                pin = Console.ReadLine();
                while (pin.Length != 5)
                {
                    Console.WriteLine("INVALID ENTRY. The PIN must be five digits. Please enter again.");
                    pin = Console.ReadLine();
                }
            }
            return pin;
        }
        internal static void updateAccountInfo()
        {
            int acccountNoToUpdate = 0;
            Console.WriteLine("Enter the Account Number: ");
            string acNo = Console.ReadLine();
            while (int.TryParse(acNo, out acccountNoToUpdate) == false)  //if value entered cannot be converted to int
            {
                Console.WriteLine("INVALID ENTRY. Please enter a valid account number.");
                acNo = Console.ReadLine();
            }
            //now we need to check if the account number entered by admin is held by any customer
            Customer customerToUpdate = new Customer();
            customerToUpdate.accountNo = acccountNoToUpdate;
            bool ifAccountExist = CustomerBLL.ifAcountNumberExists(customerToUpdate);
            if (ifAccountExist == false)
            {
                Console.WriteLine("The account number you entered doesnot exists");
                Console.WriteLine("Program ends.");
                return;
            }
            else if (ifAccountExist == true)
            {
                //the account number user entered exists in the database
                //first we are gonna show all the old information of the customer
                customerToUpdate = CustomerBLL.getInfoFromAcountNoBLL(customerToUpdate);
                Console.WriteLine("Account # " + customerToUpdate.accountNo);
                Console.WriteLine("Type : " + customerToUpdate.AccountType);
                Console.WriteLine("Holder : " + customerToUpdate.Name);
                Console.WriteLine("Balance : " + customerToUpdate.Balance);
                if (customerToUpdate.Status == "F")
                {
                    Console.WriteLine("Status : disabled");
                }
                else
                {
                    Console.WriteLine("Status : active");
                }

                //we have displayed all the information of customer
                //----------------------------
                //customerToUpdate has all the old info of the user

                Console.WriteLine("Please enter in the fields you wish to update (leave blank otherwise) : ");
                //--------------USER NAME-----------
                Console.WriteLine("Username :");
                string username1 = Console.ReadLine();
                username1 = username1.ToLower();
                if (username1 != "")  //if user entered a username, update it
                {
                    username1 = checkLengthAndSpace(username1);  //returns username with correct format
                                                                 //now we need to check if username already exists in the database as we want all the usernames to be unique
                    bool ifUsernameExists = CustomerBLL.ifUserNameExistsBLL(username1);
                    while (ifUsernameExists == true)
                    {
                        Console.WriteLine("This username already exists. Try another one.");
                        username1 = Console.ReadLine();
                        username1 = checkLengthAndSpace(username1);
                        ifUsernameExists = CustomerBLL.ifUserNameExistsBLL(username1);
                    }
                    customerToUpdate.userName=username1;
                }
                //---------------------PIN CODE-------------
                Console.WriteLine("Pin code : ");
                string pin1 = Console.ReadLine();
                if (pin1 != "")
                {
                    pin1 = inputChecksOnPin(pin1);  //correct format pin code is returned
                    customerToUpdate.Pin = pin1;
                }
                //---------------------HOLDER'S NAME-----------
                Console.WriteLine("Holders Name: ");
                string name1 = Console.ReadLine();
                if (name1 != "")
                {
                    name1 = name1.ToLower();
                    customerToUpdate.Name = name1; //if user entered the name, we update it
                }
                //----------------------Status-----------------
                Console.WriteLine("Status (active, disabled):");
                string status1 = Console.ReadLine();
                if (status1 != "")  //if user didnot press enter, we update the status
                {
                    status1 = status1.ToLower();
                    while (status1 != "active" && status1 != "disabled")
                    {
                        Console.WriteLine("INVALID ENTRY. Choose active or disabled.");
                        status1 = Console.ReadLine();
                        status1 = status1.ToLower();
                    }
                    if (status1 == "active")
                    {
                        status1 = "T";  //if customer status is active, store T
                    }
                    else if (status1 == "disabled")
                    {
                        status1 = "F";  //if customer status is disabled, store F
                    }
                    customerToUpdate.Status = status1; 
                }
                //the customerToUpdate object has all the updated information
                //now we need to update the information in the database
                customerToUpdate.Name = customerToUpdate.Name.ToLower();
                customerToUpdate.userName = customerToUpdate.userName.ToLower();
                //we are going to store username and name in small letters
                CustomerBLL.updateCustomerAccountBLL(customerToUpdate);
                Console.WriteLine("Your account has been successfully been updated.");
            }
        }
        internal static void deleteAccount(Administrator administrator)
        {
            int accountNoToDelete;
            Console.WriteLine("Enter the account number which you want to delete:");
            string acNo = Console.ReadLine();
            while (int.TryParse(acNo, out accountNoToDelete) == false)  //if value entered cannot be converted to int
            {
                Console.WriteLine("INVALID ENTRY. Please enter a valid account number.");
                acNo = Console.ReadLine();
            }
            //now we need to check if the account number entered by admin is held by any customer
            Customer customerToDelete = new Customer();
            customerToDelete.accountNo = accountNoToDelete;
            bool ifAccountExist = CustomerBLL.ifAcountNumberExists(customerToDelete);
            if (ifAccountExist == false)
            {
                Console.WriteLine("The account number you entered does not exist.");
                Console.WriteLine("Program ends.");
                return;
            }
            else if (ifAccountExist == true)  //the account number user entered exists in  the database
            {
                //now we need to get information of the account, the user wants to delete
                customerToDelete = CustomerBLL.getInfoFromAcountNoBLL(customerToDelete);
                Console.WriteLine("You wish to delete the account held by " + customerToDelete.Name + "; If this information is correct please re - enter the account number: ");
                string reEnter = Console.ReadLine();
                int reEnterNumber = 0;
                while(int.TryParse(reEnter , out reEnterNumber) == false)
                {
                    Console.WriteLine("INVALID ENTRY. Please enter a valid account number.");
                    reEnter = Console.ReadLine();
                }
                if (reEnterNumber != accountNoToDelete)
                {
                    Console.WriteLine("The account number you just entered doesnot matches with the account number you entered previously.");
                    Console.WriteLine("Program ends.");
                    return;
                }
                else if (reEnterNumber == accountNoToDelete)
                {
                    Console.WriteLine("The account number you entered matches.");
                    //the user has confirmed that he/she wants to delete the account
                    //now we need to delete the account from the database 
                    CustomerBLL.deleteCustomerAccountBLL(customerToDelete);
                    Console.WriteLine("The account deleted successfully.");

                }
            }
        }
        internal static void createAccount(Administrator administrator)
        {
            string username;
            string pin;
            string holdersName;
            string accountType;
            int startingBalance;
            string status;
            //taking username from admin
            Console.WriteLine("Enter the username(login) for the account :");
            username = Console.ReadLine();
            //input checks on the username, the username cannot have length greater than 10 or contain spaces
            username = checkLengthAndSpace(username);  //this function checks if the format of the username is correct
            //WE NEED TO CHECK IF THE USERNAME ENTERED ALREADY EXISTS IN THE DATABASE
            bool ifUsernameExists = CustomerBLL.ifUserNameExistsBLL(username);
            while(ifUsernameExists == true)
            {
                Console.WriteLine("This username already exists. Try another one.");
                username = Console.ReadLine();
                username = checkLengthAndSpace(username);
                ifUsernameExists = CustomerBLL.ifUserNameExistsBLL(username);
            }
            
            //---------------------------------------
            // now taking pin code from admin
            Console.WriteLine("Enter the PIN code (5 digits) :");
            pin = Console.ReadLine();
            pin = inputChecksOnPin(pin);  //this function performs input checks on pin and returns correct format pin
            //-----------------
            //taking account holder's name as input
            
            Console.WriteLine("Enter account holder's name : ");
            holdersName = Console.ReadLine();

            //------------------
            //admin enters type of account he/she wants to create
            Console.WriteLine("Type (Savings,Current):");
            accountType = Console.ReadLine();
            accountType = accountType.ToLower();
            while(accountType != "savings" && accountType != "current")
            {
                Console.WriteLine("INVALID ENTRY. Please enter again.");
                accountType = Console.ReadLine();
                accountType = accountType.ToLower();
            }

            //------------------
            //admin enters the starting balance of the account
            Console.WriteLine("Starting Balance: ");
            string balance = Console.ReadLine();   //startingBalance would have the amount 
            while(! int.TryParse(balance, out startingBalance))
            {
                Console.WriteLine("INVALID ENTRY. Please enter again.");
                balance = Console.ReadLine();
            }

            //---------------------
            //admin enters the status of the account

            Console.WriteLine("Status(active, disabled): ");
            status = Console.ReadLine();
            status = status.ToLower();
            while (status != "active" && status != "disabled")
            {
                Console.WriteLine("INVALID ENTRY. Choose active or disabled.");
                status = Console.ReadLine();
                status = status.ToLower();
            }
            if (status == "active")
            {
                status = "T";  //if customer status is active, store T
            }
            else if (status == "disabled")
            {
                status = "F";  //if customer status is disabled, store F
            }

            //now we are going to create a customer object

            Customer customer = new Customer();
            customer.Name = holdersName.ToLower();
            customer.userName = username.ToLower();
            customer.Pin = pin;
            customer.Balance = startingBalance;
            customer.Status = status;
            customer.AccountType = accountType;

            //the Admin has entered the account information and we have created a customer object
            //now we need to store the new customer in the database
            CustomerBLL.createCustomerBLL(customer);
            //the new customer has been added in the database
            customer = CustomerBLL.returnDataBLL(customer);
            Console.WriteLine("Account Successfully Created – the account number assigned is : " + customer.accountNo);

        }
        internal static void AdminMenu(Administrator administrator)
        {
            Console.WriteLine("1----Create New Account.");
            Console.WriteLine("2----Delete Existing Account.");
            Console.WriteLine("3----Update Account Information.");
            Console.WriteLine("4----Search for Account.");
            Console.WriteLine("5----View Reports");
            Console.WriteLine("6----Exit");
            string choice = Console.ReadLine();
            while(choice != "1" && choice != "2" && choice != "3" && choice != "4" && choice != "5" && choice != "6")
            {
                Console.WriteLine("INVALID ENTRY. Please enter again.");
                choice = Console.ReadLine();
            }
            //the user has entered his/her choice
            if(choice == "1")
            {
                //create new account
                createAccount(administrator);
            }
            else if(choice == "2")
            {
                //delete existing account
                deleteAccount(administrator);
            }
            else if (choice == "3")
            {
                //Update Account Information.
                updateAccountInfo();
            }
            else if (choice == "4")
            {
                //Search for Account
                SearchForAccount();
            }
            else if (choice == "5")
            {
                //View Reports
                viewReports();
            }
            else if (choice == "6")
            {
                //Exit
                return;
            }
        }
        internal static void AdministratorLogin()
        {
            string username;
            string pin;
            Administrator administrator = new Administrator();


            //the Administrator enters its user name
            Console.WriteLine("Enter the username(less than 11 characters) : ");
            username = Console.ReadLine();
            while (username.Contains(' '))
            {
                Console.WriteLine("INVALID ENTRY. The username cannot contain space. Enter again.");
                username = Console.ReadLine();
            }
            while (username.Length > 10)
            {
                Console.WriteLine("INVALID username. Enter again. ");
                username = Console.ReadLine();
                while (username.Contains(' '))
                {
                    Console.WriteLine("INVALID ENTRY. The username cannot contain space. Enter again.");
                    username = Console.ReadLine();
                }
            }
            while (username.Length < 10)  //making length of username equal to 10
            {
                username += " ";
            }
            username = username.ToLower(); //converting the username to lower case
                         //as we store
            administrator.userName = username;

            //now we need to verify the username
            //we send this username to the business logic layer
            bool verification = AdministratorBLL.verfiyUserName(administrator);
            if (verification == false)
            {
                Console.WriteLine("The username doesnot exists.");
            }
            else if (verification == true)
            {
                Console.WriteLine("The username exists in the database. ");

                //as the username exist in database, we will ask the pin

                //---------------------
                //as the username exist in database, we will ask the pin

                //---------------------
                int chances = 3;
                while (chances > 0)
                {
                    Console.WriteLine("please enter the PIN (five digit code) :");  //the pin should be five digit
                    pin = Console.ReadLine();


                    while (pin.Length != 5)
                    {
                        Console.WriteLine("INVALID ENTRY. The pin must be 5 digit.");
                        pin = Console.ReadLine();
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        pin += " ";  //now pin is in nchar(10)
                    }
                    administrator.Pin = pin;

                    //we have asked the pin, now we need to verify the pin from the database.
                    //we have asked the pin, now we need to verify the pin from the database.
                    bool verificationPin = AdministratorBLL.verifyPin(administrator);
                    if (verificationPin == false)
                    {
                        Console.WriteLine("Wrong pin!");
                        chances--;
                        if (chances == 0)  //user entered wrong pin three times.
                        {
                            Console.WriteLine("You have entered the PIN wrong three times!");
                            break;
                        }
                    }
                    else if (verificationPin == true)
                    {
                        Console.WriteLine("Pin correct.Login successfull.");
                        AdminMenu(administrator);
                        break;
                    }
                }
            }
        }
    }
}