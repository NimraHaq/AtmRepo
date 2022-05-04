using System;
using BusinessObjects;
using Microsoft.Data.SqlClient;
namespace DataAccessLayer
{
    public class CustomerDAL
    {
        public static List<Customer> searchAccount(string query)
        {

            List<Customer> customerList = new List<Customer>();  //we have created a list of Customer

            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Customer customer = new Customer();
                customer.accountNo = Convert.ToInt32(reader.GetValue(0));
                customer.userName = reader.GetValue(1).ToString();
                customer.Name = reader.GetValue(2).ToString();
                customer.AccountType = reader.GetValue(3).ToString();
                customer.Balance = Convert.ToInt32(reader.GetValue(4));
                customer.Status = reader.GetValue(5).ToString();
                customerList.Add(customer);
            }
            return customerList;
        }
        public static List<CustomerRecords> getAccountInDateRange(string username, DateTime startDate, DateTime endDate)
        {
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            List<CustomerRecords> customerRecords = new List<CustomerRecords>();  //we have created a list of Customer

            string query = "select CustomerName,AmountMoved,Date,Mode from CustomerRecords";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if(reader.GetValue(0).ToString() == username) //if we got the user
                {
                    string stringDate1 = reader.GetValue(2).ToString();
                    DateTime date1 = DateTime.ParseExact(stringDate1, "dd/MM/yyyy", null);  //converting string into Date
                    int result1 = DateTime.Compare(startDate, date1);
                    int result2 = DateTime.Compare(date1, endDate);
                    if ((result1 < 0 || result1 == 0) && (result2 < 0 || result2 == 0))
                    {
                        CustomerRecords customerRecord = new CustomerRecords();
                        customerRecord.userName = username;
                        customerRecord.amountMoved = Convert.ToInt32(reader.GetValue(1));
                        customerRecord.Date = reader.GetValue(2).ToString();
                        customerRecord.Mode = reader.GetValue(3).ToString();
                        if (customerRecord.Mode == "W")
                        {
                            customerRecord.Mode = "Withdrawal";
                        }
                        else if (customerRecord.Mode == "D")
                        {
                            customerRecord.Mode = "Deposite";
                        }
                        else if (customerRecord.Mode == "T")
                        {
                            customerRecord.Mode = "Transferred money to other account";
                        }
                        customerRecords.Add(customerRecord);
                    }
                }

            }
            return customerRecords;
        }
        public static List<Customer> getAccountsInRange(int maxAmount, int minAmount)
        {
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            List<Customer> customerList = new List<Customer>();  //we have created a list of Customer
            //connection with the database has been created
            string query = "select Id, UserName, Name, Balance, Type, status from CustomerTable where Balance BETWEEN @n and @x";
            SqlParameter p1 = new SqlParameter("n", minAmount);
            SqlParameter p2 = new SqlParameter("x", maxAmount);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Customer customer = new Customer();
                customer.accountNo = Convert.ToInt32(reader.GetValue(0));
                customer.userName = reader.GetValue(1).ToString();
                customer.Name = reader.GetValue(2).ToString();
                customer.Balance = Convert.ToInt32(reader.GetValue(3));
                customer.AccountType = reader.GetValue(4).ToString();
                customer.Status = reader.GetValue(5).ToString();
                if (customer.Status == "F")
                {
                    customer.Status = "disabled";
                }
                else
                {
                    customer.Status = "active";
                }
                customerList.Add(customer);  //adding the customer into the list
            }
            return customerList;
        }
        public static void updateCustomerAccount(Customer customer)
        {
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created
            string query = "update CustomerTable set UserName=@u, PIN = @p, Name =@n, status = @s  where Id =@i  ";
            SqlParameter p1 = new SqlParameter("i", customer.accountNo);
            SqlParameter p2 = new SqlParameter("u", customer.userName);
            SqlParameter p3 = new SqlParameter("p", customer.Pin);
            SqlParameter p4 = new SqlParameter("n", customer.Name);
            SqlParameter p5 = new SqlParameter("s", customer.Status);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.Parameters.Add(p5);
            int rowsAffected = cmd.ExecuteNonQuery();
        }
        public static void deleteCustomerAccountDAL(Customer customer)
        {
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created
            string query = "delete from CustomerTable where Id=@i";

            SqlParameter p1 = new SqlParameter("i", customer.accountNo);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);

            int rowsDeleted = cmd.ExecuteNonQuery();
            con.Close();
        }
        public static void createCustomerInDB(Customer customer)
        {
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created
            string query = "insert into CustomerTable(UserName, PIN, status, Balance, Name, Type) values(@u, @p, @s, @b, @n ,@t)";
            SqlParameter p1 = new SqlParameter("u", customer.userName);
            SqlParameter p2 = new SqlParameter("p", customer.Pin);
            SqlParameter p3 = new SqlParameter("s", customer.Status);
            SqlParameter p4 = new SqlParameter("b", customer.Balance);
            SqlParameter p5 = new SqlParameter("n", customer.Name);
            SqlParameter p6 = new SqlParameter("t", customer.AccountType);

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.Parameters.Add(p5);
            cmd.Parameters.Add(p6);

            int rowsInserted = cmd.ExecuteNonQuery();
            con.Close();
        }
        public static bool ifUserNameExists(string username)
        {
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created

            string query = " select UserName from CustomerTable where UserName = @u";
            SqlParameter p1 = new SqlParameter("u", username);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return true; //the username exists in the database
            }
            return false;   //the username doesnot exists in the database
        }
        public static Customer getInfoFromAccountNo(Customer customer)
        {
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created

            string query = " select Name, UserName, PIN, Balance ,status ,Type from CustomerTable where Id =@i";
            SqlParameter p1 = new SqlParameter("i", customer.accountNo);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                customer.Name = reader.GetValue(0).ToString();
                customer.userName = reader.GetValue(1).ToString();
                customer.Pin = reader.GetValue(2).ToString();
                customer.Balance = Convert.ToInt32(reader.GetValue(3));
                customer.Status = reader.GetValue(4).ToString();
                customer.AccountType = reader.GetValue(5).ToString();
            }
            return customer;
        }
        public static bool ifAcountnoExistDAL(Customer customer)
        {
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created

            string query = " select Id from CustomerTable where Id = @i";
            SqlParameter p1 = new SqlParameter("i", customer.accountNo);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return true; //the account number exists in the database
            }
            return false;   //the account number doesnot exists in the database
        }
        public static int amountWithdrewToday(Customer customer)
        {
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created
            string date = DateTime.Now.ToString("dd/MM/yyyy");
            string mode = "W";
            string query = "select AmountMoved from CustomerRecords where (CustomerName=@u and Date=@d and Mode=@m)";
            SqlParameter p1 = new SqlParameter("u", customer.userName);
            SqlParameter p2  = new SqlParameter("d", date);
            SqlParameter p3 = new SqlParameter("m", mode);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            SqlDataReader reader = cmd.ExecuteReader();
            int amountToday = 0;
            while(reader.Read())
            {
                amountToday += Convert.ToInt32(reader.GetValue(0));
            }

            return amountToday;   //the amount the customer had withdrew today.
        }
        public static bool addRecordInRecordTable(CustomerRecords customerRecords)
        {
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created

            string query = "insert into CustomerRecords(CustomerName,Mode,RemainingBalance,Date,AmountMoved) values(@u,@m,@b,@d,@a)";
            SqlParameter p1 = new SqlParameter("u", customerRecords.userName);
            SqlParameter p2 = new SqlParameter("m", customerRecords.Mode);
            SqlParameter p3 = new SqlParameter("b", customerRecords.remainingBalance);
            SqlParameter p4 = new SqlParameter("d", customerRecords.Date);
            SqlParameter p5 = new SqlParameter("a", customerRecords.amountMoved);

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.Parameters.Add(p5);
            int rowsInserted = cmd.ExecuteNonQuery();
            if (rowsInserted > 0)
            {
                con.Close();
                return true;
            }
            else
            {
                con.Close();
                return false;
            }

        }
        public static Customer returnAllData(Customer customer)
        {
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created

            string query = " select Id,Balance,status from CustomerTable where UserName = @u and PIN = @p";
            SqlParameter p1 = new SqlParameter("u", customer.userName);
            SqlParameter p2 = new SqlParameter("p", customer.Pin);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            SqlDataReader reader = cmd.ExecuteReader();
            if(reader.Read())
            {
                customer.accountNo = Convert.ToInt32(reader.GetValue(0));
                customer.Balance = Convert.ToInt32(reader.GetValue(1));
                if (Convert.ToString(reader.GetValue(2)) == "F")
                {
                    customer.Status = "F";
                }
                else
                {
                    customer.Status = "T";
                }
            }
            return customer;
        }
        public static void updateBalanceDAL(Customer customer)
        {
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created
            string query = "update CustomerTable set Balance= @b where UserName =@u and PIN =@p ";
            SqlParameter p1 = new SqlParameter("b", customer.Balance);
            SqlParameter p2 = new SqlParameter("u", customer.userName);
            SqlParameter p3 = new SqlParameter("p", customer.Pin);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            int rowsAffected = cmd.ExecuteNonQuery();
        }
        public static int returnCustomerBalance(Customer customer)  //this function return the balance of the customer
        {
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created

            string query = " select Balance from CustomerTable where UserName = @u and PIN = @p";
            SqlParameter p1 = new SqlParameter("u", customer.userName);
            SqlParameter p2 = new SqlParameter("p", customer.Pin);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                return Convert.ToInt32(reader.GetValue(0));
            }
            return 0;
        }
        public static string returnCustomerStatus(Customer customer)
        {  //this function returns the status of the customer
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created

            string query = " select status from CustomerTable where UserName = @u and PIN = @p";
            SqlParameter p1 = new SqlParameter("u", customer.userName);
            SqlParameter p2 = new SqlParameter("p", customer.Pin);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                return reader.GetValue(0).ToString();
            }
            return "";
        }
        public static bool changeCustomerStatus(Customer customer)
        {
            //this function changes the status of the customer in the table
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created
            string query = "update CustomerTable set status= @s where UserName = @u";
            SqlParameter p1 = new SqlParameter("s", customer.Status);
            SqlParameter p2 = new SqlParameter("u", customer.userName);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            int rowsAffected = cmd.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                con.Close();
                return true;
            }
            con.Close();
            return false;
        }
        public static bool checkUserNameDAL(Customer customer)  //this function verifies if the username 
                                                                //exists in the database
        {
            //checking if the Customer table in the database has the username
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created

            string query = " select UserName from CustomerTable";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if ((reader.GetValue(0)).ToString() == customer.userName)  //the userName exists in the database
                {
                    con.Close();
                    return true;   // return true
                }
            }
            con.Close();
            return false;   //return false if the username doesnot exists in the database
        }
        public static bool checkPinDAL(Customer customer)
        {
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created
            string query = " select PIN from CustomerTable where UserName = @u";
            SqlParameter p1 = new SqlParameter("u", customer.userName);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if ((reader.GetValue(0)).ToString() == customer.Pin)  //the PINexists in the database
                {
                    con.Close();
                    return true;   // return true
                }
            }
            con.Close();
            return false;  //pin doesnot exists in the database, return false.
        }
    }
    public class AdministratorDAL
    {
        public static bool checkUserNameDAL(Administrator administrator)  //this function verifies if the username 
                                                                //exists in the database
        {
            //checking if the Administrator table in the database has the username
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created

            string query = " select UserName from AdministratorTable";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if ((reader.GetValue(0)).ToString() == administrator.userName)  //the userName exists in the database
                {
                    con.Close();
                    return true;   // return true
                }
            }
            con.Close();
            return false;   //return false if the username doesnot exists in the database
        }
        public static bool checkPinDAL(Administrator administrator)
        {
            string conString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=ATM_Database;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            //connection with the database has been created
            string query = " select PIN from AdministratorTable where UserName = @u";
            SqlParameter p1 = new SqlParameter("u", administrator.userName);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if ((reader.GetValue(0)).ToString() == administrator.Pin)  //the PINexists in the database
                {
                    con.Close();
                    return true;   // return true
                }
            }
            con.Close();
            return false;  //pin doesnot exists in the database, return false.
        }
    }
}