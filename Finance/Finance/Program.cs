using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions;
using System.Xml.Linq;

//structures for data
public struct User
{
    public int id;
    public string name;
    public string surname;
    public DateTime birth_date;
    public List<Account> accounts;
    public bool exists;

    //constructor
    public User(int id, string name, string surname, DateTime birth_date)
    {
        this.id = id;
        this.name = name;
        this.surname = surname;
        this.birth_date = birth_date;

        this.accounts = new List<Account>
        {
            new Account("checking", 100.0, new ArrayList()),
            new Account("savings", 0.0, new ArrayList()),
            new Account("prepaid", 0.0, new ArrayList())
        };

        this.exists = true;
    }
    public void AddAccount(string name_acc, double amount)
    {
        accounts.Add(new Account(name_acc, amount, new ArrayList()));
    }

    public void AddTransactionToAccount(string account_type, double amount, string description, string type, string category)
    {
        int account_index = accounts.FindIndex(acc => acc.name_acc == account_type);

        if(account_index != -1)
        {
            Account account = accounts[account_index];

            account.AddTransaction(amount, description, type, category);

            accounts[account_index] = account;
        }
    }
}

public struct Account
{
    public string name_acc;
    public double amount;
    private static int last_transaction_id = 0;
    public ArrayList transactions;

    public Account(string name_acc, double amount, ArrayList transactions)
    {
        this.name_acc = name_acc;
        this.amount = amount;
        this.transactions = transactions;
    }

    public void AddTransaction(double amount, string description, string type, string category)
    {
        int transaction_id = last_transaction_id++;

        if (type == "prihod")
        {
            this.amount += amount; 
        }
        else if (type == "rashod")
        {
            this.amount -= amount;
        }

        transactions.Add(new Transaction
        {
            id = transaction_id,
            amount = amount,
            description = description,
            type = type,
            category = category,
            date = DateTime.Now,
        });

    }
}

struct Transaction
{
    public int id;
    public double amount;
    public string description;
    public string type;
    public string category;
    public DateTime date;
}

class Program
{
    static List<User> users = new List<User>();

    static void Main()
    {
        //some users
        //categories: placa, honorar, poklon, stipendija, hrana, rezije, prijevoz, hobi

        users.Add(new User(0, "Ana", "Anic", new DateTime(1990, 1, 1)));
        users[0].AddTransactionToAccount("checking", 1000.0, "placa", "prihod", "placa");
        users[0].AddTransactionToAccount("savings", 2000.0, "projekt", "prihod", "honorar");
        users[0].AddTransactionToAccount("savings", 50.0, "by grandparents", "prihod", "poklon");
        users[0].AddTransactionToAccount("prepaid", 200.0, "fesb", "prihod", "stipendija");
        users[0].AddTransactionToAccount("checking", 48.52, "kupovina hrane", "rashod", "hrana");
        users[0].AddTransactionToAccount("checking", 64.3, "struja", "rashod", "rezije");
        users[0].AddTransactionToAccount("checking", 78.4, "auto", "rashod", "prijevoz");
        users[0].AddTransactionToAccount("checking", 40.0, "tenis", "rashod", "hobi");

        users.Add(new User(1, "Marko", "Maric", new DateTime(2000, 5, 6)));
        users[1].AddTransactionToAccount("savings", 0.0, "umjetnina", "prihod", "honorar");
        users[1].AddTransactionToAccount("checking", 50.0, "placa", "prihod", "placa");
        users[1].AddTransactionToAccount("checking", 72.6, "voda", "rashod", "rezije");
        users[1].AddTransactionToAccount("checking", 40.0, "nogomet", "rashod", "hobi");
        users[1].AddTransactionToAccount("checking", 25.5, "autobus", "rashod", "prijevoz");
        users[1].AddTransactionToAccount("checking", 82.2, "yumyum", "rashod", "hrana");

        users.Add(new User(2, "Ante", "Antic", new DateTime(1976, 10, 12)));
        users[2].AddTransactionToAccount("prepaid", 2000.0, "za unuke", "prihod", "stipendija");
        users[2].AddTransactionToAccount("checking", 4563.5, "mirovina", "prihod", "placa");
        users[2].AddTransactionToAccount("savings", 550.0, "umjetnina", "prihod", "honorar");
        users[2].AddTransactionToAccount("checking", 100.0, "rodjendan", "prihod", "poklon");
        users[2].AddTransactionToAccount("checking", 90.6, "voda", "rashod", "rezije");
        users[2].AddTransactionToAccount("checking", 20.0, "turnir u sahu", "rashod", "hobi");
        users[2].AddTransactionToAccount("checking", 160.5, "auto", "rashod", "prijevoz");
        users[2].AddTransactionToAccount("checking", 150.2, "namirnice", "rashod", "hrana");

        Console.Clear();

        while (true)
        {
            Console.WriteLine("Izbornik\n");
            Console.WriteLine("1 - Korisnici");
            Console.WriteLine("2 - Racuni");
            Console.WriteLine("3 - Izlaz\n");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    UserMenu();
                    break;
                case "2":
                    AccountMenu();
                    break;
                case "3":
                    Console.Clear();
                    return;
                default:
                    Console.Clear();
                    Console.WriteLine("neispravan odabir, pokusajte ponovno\n");
                    break;
            }
        }
    }

    static void UserMenu()
    {
        Console.Clear();

        while (true)
        {
            Console.WriteLine("Izbornik: Korisnici\n");
            Console.WriteLine("1 - unos novog korisnika");
            Console.WriteLine("2 - brisanje korisnika");
            Console.WriteLine("3 - uredjivanje korisnika");
            Console.WriteLine("4 - pregled korisnika");
            Console.WriteLine("\n0 - povratak\n");

            string user_choice = Console.ReadLine();

            switch (user_choice)
            {
                case "1":
                    AddNewUser();
                    break;
                case "2":
                    DeleteUser();
                    break;
                case "3":
                    EditUser();
                    break;
                case "4":
                    ViewUsers();
                    break;
                case "0":
                    Console.Clear();
                    return;
                default:
                    Console.Clear();
                    Console.WriteLine("Nedopustena akcija, probajte ponovno\n");
                    break;
            }
        }
        
    }

    static void AddNewUser()
    {
        Console.Clear();
        Console.WriteLine("Izbornik: Novi Korisnik\n");

        Console.WriteLine("Unesite '0' za povratak, ili '00' za pocetni izbornik: \n");
        Console.WriteLine("Unesite ime korisnika: ");
        string name = Console.ReadLine();

        if(name == "0")
        {
            Console.Clear();
            UserMenu();
            return;
        }
        else if (name == "00")
        {
            Console.Clear();
            Main();
            return;
        }

        while (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("neispravan unos, pokusajte poonnvno.");
            name = Console.ReadLine();
        }

        Console.WriteLine("Unesite prezime korisnika: ");
        string surname = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(surname))
        {
            Console.WriteLine("neispravan unos, pokusajte ponovno.");
            surname = Console.ReadLine();
        }

        DateTime birthdate;
        Console.WriteLine("Unesite datum rodjenja korisnika (u formatu dd.MM.yyyy): ");
        while (true)
        {
            if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out birthdate))
            {
                break;
            }
            else
            {
                Console.WriteLine("Neispravan datum, pokusajte ponovno u formatu dd.MM.yyyy.");
            }
        }

        int newId = users.Count + 1;

        User newUser = new User(id: newId, name: name, surname: surname, birth_date: birthdate);

        users.Add(newUser);

        Console.Clear();
        Console.WriteLine("Korisnik uspjesno dodan\n");
        return;
    }

    static void DeleteUser()
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("Izbornik: Brisanje Korisnika\n");
            Console.WriteLine("1 - brisanje po id-u");
            Console.WriteLine("2 - brisanje po prezimenu");
            Console.WriteLine("\n0 - povratak");
            Console.WriteLine("00 - pocetni izbornik\n");
            string choice = Console.ReadLine();

            //removing from id
            if (choice == "1")
            {
                int id = 0;
                bool validId = false;

                while (!validId)
                {
                    Console.WriteLine("Unesite ID korisnika: ");
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out id))
                    {
                        var user_index = users.FindIndex(u => u.id == id);

                        if (user_index != -1)
                        {
                            var ToDelete = users[user_index];
                            users.Remove(ToDelete);

                            Console.Clear();
                            Console.WriteLine($"Korisnik {ToDelete.name} {ToDelete.surname} je izbrisan.\n");
                            validId = true;
                            return;
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Korisnik s navedenim ID-em ne postoji.\n");
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Neispravan unos, unesite ponovno\n");
                    }
                }
            }

            // deleting by surname
            else if (choice == "2")
            {
                string lname = "";
                bool validSurname = false;

                while (!validSurname)
                {
                    Console.WriteLine("Unesite prezime korisnika: ");
                    lname = Console.ReadLine().ToLower();

                    var usersToDelete = users.Where(u => u.surname.ToLower() == lname).ToList();

                    if (usersToDelete.Any())
                    {
                        foreach (var user in usersToDelete)
                        {
                            users.Remove(user);

                            Console.Clear();
                            lname = Char.ToUpper(lname[0]) + lname.Substring(1).ToLower();
                            Console.WriteLine($"Korisnik s prezimenom {lname} je izbrisan.\n");
                        }

                        validSurname = true;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Korisnik s navedenim prezimenom ne postoji.\n");
                        break;
                    }
                }
            }
            else if (choice == "00")
            {
                Console.Clear();
                return;
            }
            else if (choice == "0")
            {
                Console.Clear();
                UserMenu();
                return;
            }

            else
            {
                Console.Clear();
                Console.WriteLine("Neispravan unos, unesite ponovno\n");
            }
        }

    }

    static void EditUser()
    {
        Console.Clear();
        Console.WriteLine("Izbornik: Uredjivanje Korisnika\n");

        Console.WriteLine("Unesite '0' za povratak, '00' za pocetni izbornik, ili bilo sto drugo za pocetak uredjivanja korisnika: \n");
        string input = Console.ReadLine().ToLower();

        if (input == "0")
        {
            Console.Clear();
            UserMenu();
            return;
        }
        else if (input == "00")
        {
            Console.Clear();
            Main();
            return;
        }

        bool valid_id  = false;

        while (!valid_id)
        {
            Console.WriteLine("Uneiste ID korisnika");
            int id;
            
            if (int.TryParse(Console.ReadLine(), out id))
            {
                int user_index = users.FindIndex(u => u.id == id);

                if (user_index != -1)
                {
                    var user_edit = users[user_index];

                    string fname;
                    while (true)
                    { 
                        Console.WriteLine("Unesite novo ime korisnika: ");
                        fname = Console.ReadLine();

                        fname = Char.ToUpper(fname[0]) + fname.Substring(1).ToLower();
                        if (CheckIfDigit(fname))
                        {
                            Console.Clear();
                            Console.WriteLine("\nIme ne smije sadrzavati brojeve, pokusajte ponovno\n");
                            break;
                        }

                    }
                    user_edit.name = fname;

                    string lname;
                    while (true)
                    {
                        Console.WriteLine("Unesite novo prezime korisnika: ");
                        lname = Console.ReadLine();

                        lname = Char.ToUpper(lname[0]) + lname.Substring(1).ToLower();
                        if (CheckIfDigit(lname))
                        {
                            Console.Clear();
                            Console.WriteLine("\nPrezime ne smije sadrzavati brojeve, pokusajte ponovno\n");
                            break;
                        }
                    }
                    user_edit.surname = lname;

                    DateTime bdate;
                    Console.WriteLine("Unesite datum rodjenja korisnika (u formatu dd.MM.yyyy): ");
                    while (true)
                    {
                        if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out bdate))
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Neispravan datum, pokusajte ponovno u formatu dd.MM.yyyy");
                        }
                    }

                    user_edit.birth_date = bdate;

                    users[user_index] = user_edit;

                    Console.Clear();
                    Console.WriteLine("Korisnik uspjesno uredjen.\n");
                    valid_id = true;
                }
                else
                {
                    Console.WriteLine("Korisnik s navedenim ID-em ne postoji, pokusajte ponovo.\n");
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Neispravan unos, unesite ponovno\n");
            }
        }
    }

    static bool CheckIfDigit(string input)
    {
        bool contains = false;
        foreach (char c in input)
        {
            if (Char.IsDigit(c))
            {
                contains = true;
                break;
            }
        }
        if (contains)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    static void ViewUsers()
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("Izbornik: Pregled Korisnika\n");
            Console.WriteLine("1 - ispis svih korisnika po prezimenu");
            Console.WriteLine("2 - ispis svih korisnika starijih od 30 god");
            Console.WriteLine("3 - ispis svih korisnika s barem jednim racunom u minusu");
            Console.WriteLine("\n0 - povratak");
            Console.WriteLine("00 - pocetni izbornik\n");

            string choice = Console.ReadLine().ToLower();

            switch (choice)
            {
                case "1":
                    Console.Clear();
                    Console.WriteLine("Popis korisnika abeceno:");

                    var sortedUsers = users.OrderBy(u => u.surname).ToList();

                    Console.WriteLine("\nID\tIME\tPREZIME\t\tDATUM RODJENJA\n");
                    foreach (var user in sortedUsers)
                    {
                        Console.WriteLine($"{user.id}\t{user.name}\t{user.surname}\t\t{user.birth_date.ToString("dd.MM.yyyy")}");
                    }
                    Console.WriteLine("\nUnesite bilo sto za povratak ili '00' za pocetni izbornik\n");
                    string user_choice = Console.ReadLine();

                    if (user_choice == "00")
                    {
                        Console.Clear();
                        Main();
                        return;
                    }
                    Console.Clear();
                    break;

                case "2":
                    Console.Clear();
                    Console.WriteLine("Popis korisnika starijih od 30:");

                    var usersOver30 = users.Where(u =>
                    {
                        var age = DateTime.Now.Year - u.birth_date.Year;
                        if (DateTime.Now.Month < u.birth_date.Month || (DateTime.Now.Month == u.birth_date.Month && DateTime.Now.Day < u.birth_date.Day))
                        {
                            age--;
                        }
                        return age >= 30;
                    }).ToList();

                    Console.WriteLine("\nID\tIME\tPREZIME\t\tDATUM RODJENJA\n");
                    foreach (var user in usersOver30)
                    {
                        Console.WriteLine($"{user.id}\t{user.name}\t{user.surname}\t\t{user.birth_date.ToString("dd.MM.yyyy")}");
                    }
                    Console.WriteLine("\nUnesite bilo sto za povratak ili '00' za pocetni izbornik\n");
                    string input = Console.ReadLine();

                    if (input == "00")
                    {
                        Console.Clear();
                        Main();
                        return;
                    }
                    Console.Clear();
                    break;
                case "3":
                    Console.Clear();
                    Console.WriteLine("Popis korisnika s barem jednim racunom u minusu:\n");

                    bool found_negative = false;

                    foreach (var user in users)
                    {
                        var negative_accounts = user.accounts.Where(a => a.amount < 0).ToList();

                        if (negative_accounts.Any())
                        {
                            found_negative = true;
                            Console.WriteLine($"User: {user.name} {user.surname}");

                            foreach (var account in negative_accounts)
                            {
                                Console.WriteLine($"\tracun: {account.name_acc}, stanje: {account.amount}");
                            }

                            Console.WriteLine("\n");
                        }
                    }
                    if (!found_negative)
                    {
                        Console.WriteLine("Nema korisnika s negativnim stanjem racuna.\n");
                    }
                    Console.WriteLine("\nUnesite bilo sto za povratak ili '00' za pocetni izbornik\n");
                    input = Console.ReadLine();

                    if (input == "00")
                    {
                        Console.Clear();
                        Main();
                        return;
                    }
                    Console.Clear();
                    break;

                case "0":
                    Console.Clear();
                    UserMenu();
                    return;

                case "00":
                    Console.Clear();
                    Main();
                    return;

                default:
                    Console.Clear();
                    Console.WriteLine("neispravan unos, unesite ponovno\n");
                    break;
            }
        }
    }


    //accounts
    static void AccountMenu()
    {
        Console.Clear();
        Console.WriteLine("Izbornik: Racuni\n");

        string fname = "";
        string lname = "";

        User curr_user = new User();
        curr_user.exists = false;

        while (true)
        {
            Console.WriteLine("Unesite '00' za pocetni izbornik, ili bilo sto drugo za nastavak: \n");
            string input = Console.ReadLine();

            if (input == "00")
            {
                Console.Clear();
                return;
            }

            Console.WriteLine("Unesite ime korisnika");
            fname = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(fname) || fname.Length < 2)
            {
                Console.Clear();
                Console.WriteLine("Ime ne moze biti prazno, pokusajte ponovo.\n");
                continue;
            }

            Console.WriteLine("Unesite prezime korisnika");
            lname = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(lname) || lname.Length < 2)
            {
                Console.Clear();
                Console.WriteLine("Prezime ne moze biti prazno, pokusajte ponovo.\n");
                continue;
            }

            curr_user = users.FirstOrDefault(u => u.name.Equals(fname, StringComparison.OrdinalIgnoreCase) && u.surname.Equals(lname, StringComparison.OrdinalIgnoreCase));

            if (!curr_user.exists)
            {
                Console.Clear();
                Console.WriteLine("Korisnik s tim imenom i prezimenom nije pronadjen, pokusajte ponovo.\n");
            }
            else
            {
                break;
            }
        }

        Console.Clear();

        if (curr_user.exists)
        {
            while (true)
            {
                Console.WriteLine("1 - unos nove transakcije");
                Console.WriteLine("2 - brisanje transakcije");
                Console.WriteLine("3 - uređivanje transakcije");
                Console.WriteLine("4 - pregled transakcija");
                Console.WriteLine("5 - financijsko izvješće");
                Console.WriteLine("\n0 - povratak");
                Console.WriteLine("00 - povratak na pocetni izbornik\n");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddNewTransaction(curr_user);
                        break;
                    case "2":
                        DeleteTransaction(curr_user);
                        break;
                    case "3":
                        EditTransaction(curr_user);
                        break;
                    case "4":
                        ViewTransactions(curr_user);
                        break;
                    case "5":
                        FinancialStatement(curr_user);
                        break;
                    case "0":
                        Console.Clear();
                        AccountMenu();
                        return;
                    case "00":
                        Console.Clear();
                        Main();
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Nedopustena radnja, pokusajte ponovo.\n");
                        break;
                }
            }
        }

    }

    static void AddNewTransaction(User person)
    {
        Console.Clear();
        Console.WriteLine("Izbornik: Nova Transakcija\n");

        string choice;

        while (true)
        {
            Console.WriteLine("1 - Trenutno izvrsena transakcija");
            Console.WriteLine("2 - Ranije izvrsena transakcija");
            Console.WriteLine("\n0 - povratak");
            Console.WriteLine("00 - pocetni izbornik\n");

            choice = Console.ReadLine().ToLower();

            if (choice == "1" || choice == "2")
            {
                Console.Clear();
                break;
            }
            else if(choice == "0")
            {
                Console.Clear();
                return;
            }
            else if (choice == "00")
            {
                Console.Clear();
                Main();
                return;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Pogresan unos, pokusajte ponovno.");
            }
        }

        string account_type;
        while (true)
        {
            Console.WriteLine("0 - povratak");
            Console.WriteLine("00 - pocetni izbornik\n");

            Console.WriteLine("Odaberite racun (checking, savings, prepaid): ");
            account_type = Console.ReadLine().ToLower().Trim();

            if(account_type == "0")
            {
                Console.Clear();
                return;
            }
            else if (account_type == "00")
            {
                Console.Clear();
                Main();
                return;
            }

            if (string.IsNullOrWhiteSpace(account_type))
            {
                Console.Clear();
                Console.WriteLine("Unos je prazan, pokusajte ponovno.\n");
                continue;
            }

            if (account_type.Equals("checking", StringComparison.OrdinalIgnoreCase) ||
                account_type.Equals("savings", StringComparison.OrdinalIgnoreCase) ||
                account_type.Equals("prepaid", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Neispravan tip racuna, pokusajte ponovno.\n");
            }
        }


        int acc_index = person.accounts.FindIndex(acc => acc.name_acc == account_type);
        if(acc_index == -1)
        {
            Console.Clear();
            Console.WriteLine("Racun nije pronadjen\n");
            return;
        }

        Account selected_account = person.accounts[acc_index];

        // transaction ingredients
        double amount = 0;
        string description;
        string transaction_type = "";
        string catg = "";
        DateTime transaction_date = DateTime.Now;


        // category input
        while (true)
        {
            Console.WriteLine("\nOdaberite tip transakcije:");
            Console.WriteLine("1 - Prihod");
            Console.WriteLine("2 - Rashod");
            Console.WriteLine("\n0 - povratak\n");

            string type_choice = Console.ReadLine().ToLower();

            if (type_choice == "1")
            {
                transaction_type = "prihod";
                break;
            }
            else if (type_choice == "2")
            {
                transaction_type = "rashod";
                break;
            }
            else if (type_choice == "0")
            {
                Console.Clear();
                AccountMenu();
                return;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Neispravan unos, pokusajte ponovno.\n");
            }
        }

        string[] incomeCategories = { "placa", "honorar", "poklon", "stipendija" };
        string[] expenseCategories = { "hrana", "prijevoz", "rezije", "hobi" };

        while (true)
        {
            Console.WriteLine(transaction_type == "prihod" ?
                    "Odaberite kategoriju prihoda: placa, honorar, poklon, stipendija" :
                    "Odaberite kategoriju rashoda: hrana, prijevoz, režije, hobi");

            catg = Console.ReadLine().ToLower();

            if (transaction_type == "prihod" && Array.Exists(incomeCategories, category => category == catg) ||
                transaction_type == "rashod" && Array.Exists(expenseCategories, category => category == catg))
            {
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Nepoznata kategorija, pokusajte ponovno.\n");
            }
        }

        // amount input
        while (true)
        {
            Console.WriteLine("Unesite iznos transakcije: ");
            string amountInput = Console.ReadLine();

            if (double.TryParse(amountInput, out amount) && amount > 0)
            {
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Pogresan unos, pokusajte ponovno.\n");
            }
        }

        //description
        Console.WriteLine("Unesite opis transakcije: ");
        description = Console.ReadLine();
        if (string.IsNullOrEmpty(description))
        {
            description = "Standardna transakcija";
        }

        // date input for previous transaction
        if (choice == "2")
        {
            Console.WriteLine("Unesite datum transakcije (dd/mm/yyyy): ");
            while (true)
            {
                string dateInput = Console.ReadLine();
                if (DateTime.TryParse(dateInput, out transaction_date))
                {
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Pogresan datum, pokusajte ponovno.\n");
                }
            }
        }

        // adding new transaction
        selected_account.AddTransaction(amount, description, transaction_type, catg);
        person.accounts[acc_index] = selected_account;
        
        Console.Clear();
        Console.WriteLine("Transakcija uspjesno dodana.\n");
        return;
    }


    static void DeleteTransaction(User person)
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("Izbornik: Brisanje Transakcija\n");
            Console.WriteLine("Brisanje po:");
            Console.WriteLine("1 - ID-u");
            Console.WriteLine("2 - ispod unesenog iznosa");
            Console.WriteLine("3 - iznad unesenog iznosa");
            Console.WriteLine("4 - svih prihoda");
            Console.WriteLine("5 - svih rashoda");
            Console.WriteLine("6 - svih transakcija za odabranu kategoriju");
            Console.WriteLine("\n0 - povratak");
            Console.WriteLine("\n00 - pocetni izbornik\n");

            string choice = Console.ReadLine();

            if (choice == "0")
            {
                Console.Clear();
                return;
            }
            else if (choice == "00")
            {
                Console.Clear();
                Main();
                return;
            }

            if (choice != "1" && choice != "2" && choice != "3" && choice != "4" && choice != "5" && choice != "6")
            {
                Console.Clear();
                Console.WriteLine("Neispravan izbor, pokusajte ponovno.\n");
                continue;
            }

            while (true)
            {
                Console.WriteLine("Jeste li sigurni da zelite obrisati transakciju? (yes/no)\n");
                string confirmation = Console.ReadLine().ToLower().Trim();

                if (confirmation == "no")
                {
                    Console.Clear();
                    Console.WriteLine("Brisanje otkazano.\n");
                    return;
                }
                else if (confirmation == "yes")
                {
                    Console.Clear();
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Neispravan unos, molimo unesite 'yes' ili 'no'.\n");
                }
            }


            //choices
            if (choice == "1")
            {
                Console.Clear();

                Console.WriteLine("Unesite ID transakcije");
                int id_tran;
                while (!int.TryParse(Console.ReadLine(), out id_tran) || id_tran < 0)
                {
                    Console.Clear();
                    Console.WriteLine("ID trazene transakcije ne postoji, pokusajte ponovno, id mora biti pozitivan\n");
                }

                bool transaction_found = false;

                foreach (var account in person.accounts)
                {

                    var to_delete = account.transactions.Cast<Transaction>().FirstOrDefault(t => t.id == id_tran);

                    if (!to_delete.Equals(default(Transaction)))
                    {
                        account.transactions.Remove(to_delete);
                        transaction_found = true;
                        break;
                    }
                }

                if (transaction_found)
                {
                    Console.Clear();
                    Console.WriteLine("Transakcija uspjesno obrisana!\n");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Transakcija s navedenim ID-em ne postoji\n");
                }

                while (true)
                {
                    Console.WriteLine("\nZelite li ponovno brisati transakciju? (yes/no)\n");
                    string again = Console.ReadLine().ToLower();
                    if (again == "yes")
                    {
                        Console.Clear();
                        break; 
                    }
                    else if (again == "no")
                    {
                        Console.Clear();
                        return; 
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
                    }
                }
            }



            else if (choice == "2" || choice == "3")
            {
                Console.Clear();

                string amount_type = choice == "2" ? "ispod" : "iznad";

                Console.WriteLine($"Unesite iznos {amount_type} kojeg iznosa se transakcije brisu: \n");
                double amount_tran;

                while (!double.TryParse(Console.ReadLine(), out amount_tran) || amount_tran <= 0)
                {
                    Console.WriteLine("iznos mora biti veci od nule! Unesite ponovno\n");
                }

                bool transaction_found = false;

                foreach (var account in person.accounts)
                {
                    var to_delete = choice == "2"
                    ? account.transactions.Cast<Transaction>().Where(t => t.amount < amount_tran).ToList()
                    : account.transactions.Cast<Transaction>().Where(t => t.amount > amount_tran).ToList();

                    if (to_delete.Any())
                    {
                        foreach(var transaction in to_delete)
                        {
                            account.transactions.Remove(transaction);
                        }
                        transaction_found = true;
                    }
                }
                Console.Clear();
                Console.WriteLine(transaction_found
                    ? $"Transakcije {amount_type} {amount_tran} iznosa su obrisane.\n"
                    : $"Nema transakcija {amount_type} {amount_tran}.\n");
            }


            else if (choice == "4" || choice == "5")
            {
                string transaction_type = choice == "4" ? "prihod" : "rashod";

                bool transaction_found = false;

                foreach (var account in person.accounts)
                {
                    var transactions_to_delete = account.transactions.Cast<Transaction>().Where(t => t.type == transaction_type).ToList();

                    foreach (var transaction in transactions_to_delete)
                    {
                        account.transactions.Remove(transaction);
                    }
                    transaction_found = true;
                }
                Console.Clear();
                Console.WriteLine(transaction_found
                    ? $"Svi {transaction_type} su obrisani.\n"
                    : $"Nema {transaction_type} za brisanje.\n");
            }

            else if (choice == "6")
            {
                Console.Clear();

                Console.WriteLine("Unesite kategoriju (placa, honorar, poklon, stipendija, hrana, prijevoz, rezije, hobi):\n");
                string input = Console.ReadLine().ToLower();

                string[] validCategories = { "placa", "honorar", "poklon", "stipendija", "hrana", "prijevoz", "rezije", "hobi" };

                while (!validCategories.Contains(input))
                {
                    Console.WriteLine("Nepoznata kategorija, pokusajte ponovno\n");
                    input = Console.ReadLine().ToLower().Trim();
                }

                bool transaction_found = false;

                foreach (var account in person.accounts)
                {
                    var transactions_to_delete = account.transactions.Cast<Transaction>().Where(t => t.category == input).ToList();
                    if (transactions_to_delete.Any())
                    {
                        foreach (var transaction in transactions_to_delete)
                        {
                            account.transactions.Remove(transaction);
                        }
                        transaction_found = true;
                    }
                }

                Console.Clear();
                Console.WriteLine(transaction_found
                    ? $"Sve transakcije kategorije '{input}' su obrisane.\n"
                    : $"Nema transakcija u kategoriji '{input}'.\n");

            }

            while (true)
            {
                Console.WriteLine("\nZelite li ponovno brisati transakciju? (yes/no)\n");
                string again = Console.ReadLine().ToLower().Trim();
                if (again == "yes")
                {
                    break;
                }
                else if(again == "no")
                {
                    Console.Clear();
                    return;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("neispravan unos, pokusajte ponovno\n");
                }
            }
        }
    }

    static void EditTransaction(User person)
    {
        Console.Clear();
        Console.WriteLine("Izbornik: Uredjivanje transakcija\n");

        Console.WriteLine("Unesite '0' za povratak ili '00' za pocetni izbornik, za nastavak pritisnite bilo sto\n");
        string input = Console.ReadLine();

        if (input == "0")
        {
            Console.Clear();
            AccountMenu();
            return;
        }
        else if (input == "00")
        {
            Console.Clear();
            Main();
            return;
        }

        while (true)
        {
            Console.WriteLine("Jeste li sigurni da zelite urediti transakciju? (yes/no)");
            string confirmation = Console.ReadLine().ToLower().Trim();

            if (confirmation == "no")
            {
                Console.Clear();
                Console.WriteLine("Uredjivanje otkazano.\n");
                return;
            }
            else if (confirmation == "yes")
            {
                Console.Clear();
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Neispravan unos, molimo unesite 'yes' ili 'no'.\n");
            }
        }

        int id;
        bool transaction_found = false;

        while (true)
        {
            Console.WriteLine("Unesite ID transakcije:");
            if (!int.TryParse(Console.ReadLine(), out id) || id < 0)
            {
                Console.Clear();
                Console.WriteLine("Neispravan unos. ID mora biti pozitivan cijeli broj.");
                continue;
            }

            foreach (var account in person.accounts)
            {
                var transaction = account.transactions.Cast<Transaction>().FirstOrDefault(t => t.id == id);
                if (transaction.id != 0)
                {
                    transaction_found = true;
                    break;
                }
            }

            if (transaction_found)
            {
                break;  
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Transakcija s navedenim ID-om ne postoji, pokusajte ponovo.\n");
            }
        }

        transaction_found = false;
        Transaction edit_tran = new Transaction();

        foreach(var account in person.accounts)
        {
            var transaction = account.transactions.Cast<Transaction>().FirstOrDefault(t => t.id == id);
            if(transaction.id != 0)
            {
                edit_tran = transaction;
                transaction_found = true;
                break;
            }
        }

        if (!transaction_found)
        {
            Console.Clear();
            Console.WriteLine("Transakcija s navedenim ID-om ne postoji\n");
            AccountMenu();
            return;
        }


        //new amount
        double new_amount;
        while (true)
        {
            Console.WriteLine("Unesite novi iznos transakcije: \n");
            if (!double.TryParse(Console.ReadLine(), out new_amount) || new_amount <= 0)
            {
                Console.Clear();
                Console.WriteLine("Neispravan unos, iznos mora biti pozitivan broj\n");
            }
            else
            {
                edit_tran.amount = new_amount;
                break;
            }
        }

        //new descriptiom
        Console.WriteLine("Unesite novi opis transakcije:\n");
        string new_description = Console.ReadLine();
        edit_tran.description = string.IsNullOrEmpty(new_description) ? "Standardna transakcija" : new_description;

        string[] validTypes = { "prihod", "rashod" };
        while (true)
        {
            Console.WriteLine("Unesite tip transakcije (prihod/rashod): \n");
            string new_type = Console.ReadLine().ToLower();

            if (Array.Exists(validTypes, t => t == new_type))
            {
                edit_tran.type = new_type;
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
            }
        }

        string[] income_categories = { "placa", "honorar", "poklon", "stipendija" };
        string[] expense_categories = { "hrana", "prijevoz", "rezije", "hobi" };
        string[] available_categories;

        //type and category
        while (true)
        {
            Console.WriteLine("Unesite kategoriju transakcije:\n");
            if (edit_tran.type == "prihod")
            {
                available_categories = income_categories;
                Console.WriteLine("\nDostupne kategorije za prihode:");
            }
            else
            {
                available_categories = expense_categories;
                Console.WriteLine("\nDostupne kategorije za rashode:");
            }

            foreach (string category in available_categories)
            {
                Console.WriteLine("- " + category);
            }

            string new_category;

            while (true)
            {
                Console.WriteLine("Unesite kategoriju");
                Console.WriteLine($"Kategorije: {string.Join(", ", available_categories)}");
                new_category = Console.ReadLine().ToLower();

                if (Array.Exists(available_categories, category => category == new_category))
                {
                    edit_tran.category = new_category;
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Neispravna kategorija, pokusajte ponovno.\n");
                }
            }
            break;
        }

        edit_tran.date = DateTime.Now;

        foreach(var account in person.accounts)
        {
            var transaction = account.transactions.Cast<Transaction>().FirstOrDefault(t => t.id == id);
            if (transaction.id != 0)
            {
                account.transactions.Remove(transaction);
                account.transactions.Add(edit_tran);
                break;
            }
        }
        Console.Clear();
        Console.WriteLine("Transakcija je uspjesno uredjena.\n");
        Console.WriteLine("\nUnesite bilo sto za povratak ili '00' za pocetni izbornik\n");
        string user_choice = Console.ReadLine();

        if (user_choice == "00")
        {
            Console.Clear();
            Main();
            return;
        }

        Console.Clear();
        return;
    }

    static void ViewTransactions(User person)
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("Izbornik: Pregled transakcija\n");
            Console.WriteLine("1 - sve transakcije:");
            Console.WriteLine("2 - sortirane po iznosu uzlazno");
            Console.WriteLine("3 - sortirane po iznosu uzlazno");
            Console.WriteLine("4 - sortirane po opisu abecedno");
            Console.WriteLine("5 - sortirane po datumu uzlazno");
            Console.WriteLine("6 - sortirane po datumu silazno");
            Console.WriteLine("7 - svi prihodi");
            Console.WriteLine("8 - svi rashodi");
            Console.WriteLine("9 - sve transakcije za odredjenu kategoriju");
            Console.WriteLine("10 - sve transakcije za odredjeni tip i kategoriju");
            Console.WriteLine("\n0 - povratak");
            Console.WriteLine("00 - pocetni izbornik\n");

            string user_choice = Console.ReadLine();

            var transactions_for_display = person.accounts.SelectMany(acc => acc.transactions.Cast<Transaction>()).ToList();

            string[] validCategories = { "placa", "honorar", "poklon", "stipendija", "hrana", "prijevoz", "rezije", "hobi" };
            string sort_name = "";

            switch (user_choice)
            {
                case "1":
                    Console.Clear();
                    Console.WriteLine("Sve transakcije:");
                    sort_name = "Sve transakcije";
                    break;

                case "2":
                    transactions_for_display.Sort((t1, t2) => t1.amount.CompareTo(t2.amount));
                    Console.Clear();
                    Console.WriteLine("Transakcije sortirane po iznosu (uzlazno):");
                    sort_name = "Transakcije po iznosu uzlazno";
                    break;

                case "3":
                    transactions_for_display.Sort((t1, t2) => t2.amount.CompareTo(t1.amount));
                    Console.Clear();
                    Console.WriteLine("Transakcije sortirane po iznosu (silazno):");
                    break;

                case "4":
                    transactions_for_display = transactions_for_display.OrderBy(t => t.description).ToList();
                    Console.Clear();
                    Console.WriteLine("Transakcije sortirane po opisu (abecedno):");
                    sort_name = "Transakcije po iznosu silazno";
                    break;

                case "5":
                    transactions_for_display = transactions_for_display.OrderBy(t => t.date).ToList();
                    Console.Clear();
                    Console.WriteLine("Transakcije sortirane po datumu (uzlazno):");
                    sort_name = "Transakcije po datumu uzlazno";
                    break;

                case "6":
                    transactions_for_display = transactions_for_display.OrderByDescending(t => t.date).ToList();
                    Console.Clear();
                    Console.WriteLine("Transakcije sortirane po datumu (silazno):");
                    sort_name = "Transakcije po iznosu silazno";
                    break;

                case "7":
                    transactions_for_display = transactions_for_display.Where(t => t.type.ToLower() == "prihod").ToList();
                    Console.Clear();
                    Console.WriteLine("Svi prihodi:");
                    sort_name = "Svi prihodi";
                    break;

                case "8":
                    transactions_for_display = transactions_for_display.Where(t => t.type.ToLower() == "rashod").ToList();
                    Console.Clear();
                    Console.WriteLine("Svi rashodi:");
                    sort_name = "Svi rashodi";
                    break;

                case "9":
                    string category_input;
                    do
                    {
                        Console.WriteLine("Unesite kategoriju (placa, honorar, poklon, stipendija, hrana, prijevoz, rezije, hobi):");
                        category_input = Console.ReadLine().ToLower();

                        if (!validCategories.Contains(category_input))
                        {
                            Console.Clear();
                            Console.WriteLine("neispravan unos, pokusajte ponovno\n");
                        }
                    } while (!validCategories.Contains(category_input));


                    transactions_for_display = transactions_for_display.Where(t => t.category.ToLower() == category_input).ToList();
                    Console.Clear();
                    Console.WriteLine($"Sve transakcije za kategoriju '{category_input}':");
                    sort_name = $"Sve transakcije za kategoriju '{category_input}'";
                    break;

                case "10":
                    Console.Clear();
                    string type_input;
                    string[] available_categories;
                    do
                    {
                        Console.WriteLine("Unesite tip transakcije (prihod/rashod):");
                        type_input = Console.ReadLine().ToLower();

                        if (type_input != "prihod" && type_input != "rashod")
                        {
                            Console.Clear();
                            Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
                        }

                    } while (type_input != "prihod" && type_input != "rashod");

                    string[] income_categories = { "placa", "honorar", "poklon", "stipendija" };
                    string[] expense_categories = { "hrana", "prijevoz", "rezije", "hobi" };

                    if (type_input == "prihod")
                    {
                        available_categories = income_categories;
                        Console.WriteLine("\nDostupne kategorije za prihod:");
                    }
                    else
                    {
                        available_categories = expense_categories;
                        Console.WriteLine("\nDostupne kategorije za rashod:");
                    }


                    string category_input_;

                    do
                    {
                        Console.WriteLine($"Kategorije: {string.Join(", ", available_categories)}");
                        category_input_ = Console.ReadLine().ToLower();

                        if (!validCategories.Contains(category_input_))
                        {
                            Console.Clear();
                            Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
                            Console.WriteLine(string.Join(", ", available_categories));
                            continue;
                        }
                        else
                        {
                            break; 
                        }

                    } while (true);

                    transactions_for_display = transactions_for_display.Where(t => t.type.ToLower() == type_input && t.category.ToLower() == category_input_).ToList();
                    Console.Clear();
                    Console.WriteLine($"Sve transakcije za tip '{type_input}' i kategoriju '{category_input_}':");
                    sort_name = $"Sve transakcije za tip '{type_input}' i kategoriju '{category_input_}'";
                    break;

                case "0":
                    Console.Clear();
                    AccountMenu();
                    return;

                case "00":
                    Console.Clear();
                    return;

                default:
                    Console.Clear();
                    Console.WriteLine("Nedopusten odabir, probajte ponovno\n");
                    continue;
            }

            if (transactions_for_display.Count > 0)
            {
                Console.WriteLine("\n{0,-10} {1,10} {2,-30} {3,-15} {4,10}", "TIP", "IZNOS", "OPIS", "KATEGORIJA", "DATUM");

                foreach (var transaction in transactions_for_display)
                {
                    Console.WriteLine(string.Format("{0,-10} {1,10:F2} {2,-30} {3,-15} {4,10:dd.MM.yyyy}",
                        transaction.type,
                        transaction.amount,
                        transaction.description.Length > 30 ? transaction.description.Substring(0, 30) : transaction.description,
                        transaction.category,
                        transaction.date));
                }
            }

            else
            {
                Console.Clear();
                Console.WriteLine("Nema transakcija za prikaz\n");
                return;
            }

            Console.WriteLine("\nUnesite bilo sto za povratak ili '00' za pocetni izbornik\n");
            user_choice = Console.ReadLine();
            Console.Clear();

            if (user_choice == "00")
            {
                Console.Clear();
                Main();
                return;
            }
        }
        
    }

    static void FinancialStatement(User person)
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("Izbornik: Financijsko Izvjesce\n");
            Console.WriteLine("1 - trenutno stanje racuna");
            Console.WriteLine("2 - broj ukupnih transakcija");
            Console.WriteLine("3 - ukupan iznos prihoda i rashoda za odabrani mjesec i godinu");
            Console.WriteLine("4 - postotak udjela rashoda za odabranu kategoriju");
            Console.WriteLine("5 - prosjecni iznos transakcije za odabrani mjesec i godinu");
            Console.WriteLine("6 - prosjecni iznos transakcije za odabranu kategoriju");
            Console.WriteLine("\n0 - povratak");
            Console.WriteLine("00 - pocetni izbornik\n");

            string user_choice = Console.ReadLine().ToLower();

            switch (user_choice)
            {
                case "1":
                    CurrentBalance(person);
                    break;

                case "2":
                    NumberOfTransactions(person);
                    break;

                case "3":
                    IncomeAndExpensesDate(person);
                    break;

                case "4":
                    PercentageCategory(person);
                    break;

                case "5":
                    AverageMonthYear(person);
                    break;

                case "6":
                    AverageCategory(person);
                    break;
                case "0":
                    Console.Clear();
                    AccountMenu();
                    return;
                case "00":
                    Console.Clear();
                    Main();
                    return;
                default:
                    Console.Clear();
                    Console.WriteLine("Nedopustena akcija, probajte ponovno\n");
                    break;
            }
        }
    }

    static void CurrentBalance(User person)
    {
        double totalIncome = 0;
        double totalExpenses = 0;

        var all_transactions = person.accounts.SelectMany(acc => acc.transactions.Cast<Transaction>()).ToList();

        foreach (var item in all_transactions)
        {
            if (item is Transaction transaction)
            {
                if (transaction.type.ToLower() == "prihod")
                {
                    totalIncome += transaction.amount;
                }
                else if (transaction.type.ToLower() == "rashod")
                {
                    totalExpenses += transaction.amount;
                }
            }
        }

        double balance = totalIncome - totalExpenses;

        Console.Clear();
        Console.WriteLine($"Trenutno stanje racuna: {balance:F2}\n");

        Console.WriteLine("\nUnesite bilo sto za povratak ili '00' za pocetni izbornik\n");
        string user_choice = Console.ReadLine();
        Console.Clear();
        if (user_choice == "00")
        {
            Console.Clear();
            Main();
            return;
        }
    }

    static void NumberOfTransactions(User person)
    {
        int total = 0;

        foreach (var account in person.accounts)
        {
            foreach (Transaction transaction in account.transactions)
            {
                total++;
            }
        }

        Console.Clear();
        Console.WriteLine($"Ukupan broj transakcija: {total}\n");

        Console.WriteLine("\nUnesite bilo sto za povratak ili '00' za pocetni izbornik\n");
        string user_choice = Console.ReadLine();
        Console.Clear();
        if (user_choice == "00")
        {
            Console.Clear();
            Main();
            return;
        }
    }

    static void IncomeAndExpensesDate(User person)
    {
        int month, year;

        //month
        while (true)
        {
            Console.WriteLine("Unesite mjesec (1-12). ");
            if (int.TryParse(Console.ReadLine(), out month) && month >= 1 && month <= 12)
            {
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("neispravan unos, pokusajte ponovno\n");
            }
        }

        //year
        while (true)
        {
            Console.WriteLine("Unesite godinu: ");
            if (int.TryParse(Console.ReadLine(), out year) && year > 0)
            {
                if (year >= 1000 && year <= 9999)
                {
                    break; 
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Godina mora imati 4 znamenke, pokusajte ponovno.\n");
                }
            }
            else
            {
                Console.WriteLine("neispravan unos, pokusajte ponovno\n");
            }
        }

        var all_transactions = person.accounts.SelectMany(acc => acc.transactions.Cast<Transaction>()).ToList();
        var month_transactions = all_transactions.Where(t => t.date.Month == month && t.date.Year == year).ToList();

        double total_income = 0;
        double total_expenses = 0;

        foreach (var transaction in month_transactions)
        {
            if (transaction.type.ToLower() == "prihod")
            {
                total_income += transaction.amount;
            }
            else if (transaction.type.ToLower() == "rashod")
            {
                total_expenses += transaction.amount;
            }
        }

        Console.Clear();
        Console.WriteLine($"Ukupni prihodi: {total_income}");
        Console.WriteLine($"Ukupni rashodi: {total_expenses}\n");

        Console.WriteLine("\nUnesite bilo sto za povratak ili '00' za pocetni izbornik\n");
        string user_choice = Console.ReadLine();
        Console.Clear();
        if (user_choice == "00")
        {
            Console.Clear();
            Main();
            return;
        }
    }

    static void PercentageCategory(User person)
    {
        Console.Clear();

        Console.WriteLine("Unesite kategoriju za koju želite izračunati postotak udjela:");
        Console.WriteLine("Dostupne kategorije: hrana, prijevoz, rezije, hobi");

        string category_input = Console.ReadLine().ToLower();
        string[] validCategories = { "hrana", "prijevoz", "rezije", "hobi" };

        while (!Array.Exists(validCategories, category => category == category_input))
        {
            Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
            category_input = Console.ReadLine().ToLower();
        }

        var allTransactions = person.accounts.SelectMany(acc => acc.transactions.Cast<Transaction>()).ToList();

        double total_expense = allTransactions.Where(t => t.type.ToLower() == "rashod").Sum(t => t.amount);

        double category_expense = allTransactions.Where(t => t.type.ToLower() == "rashod" && t.category.ToLower() == category_input).Sum(t => t.amount);

        double percentage = (total_expense > 0) ? (category_expense / total_expense) * 100 : 0;

        Console.Clear();
        Console.WriteLine($"Postotak udjela rashoda za kategoriju '{category_input}' iznosi: {percentage:F2}%");
        
        Console.WriteLine("\nUnesite bilo sto za povratak ili '00' za pocetni izbornik\n");
        string user_choice = Console.ReadLine();
        Console.Clear();
        if (user_choice == "00")
        {
            Console.Clear();
            Main();
            return;
        }
        return;
    }



    static void AverageMonthYear(User person)
    {
        Console.Clear();

        int month, year;
        while (true)
        {
            Console.WriteLine("Unesite godinu za koju zelite izracunati prosjek transakcija: ");
            if (int.TryParse(Console.ReadLine(), out year) && year >= 1)
            {
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
            }
        }

        while (true)
        {
            Console.WriteLine("Unesite mjesec za koji zelite izracunati prosjek transakcija (1-12):");
            if (int.TryParse(Console.ReadLine(), out month) && month >= 1 && month <= 12)
            {
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
            }
        }
        

        var all_transactions = person.accounts.SelectMany(acc => acc.transactions.Cast<Transaction>()).ToList();
        var month_transactions = all_transactions.Where(t => t.date.Year == year && t.date.Month == month).ToList();

        if (month_transactions.Any())
        {
            double total_amount = month_transactions.Sum(t => t.amount);
            double average_amount = total_amount / month_transactions.Count;
            Console.Clear();
            Console.WriteLine($"Prosjecan iznos transakcija za {month}/{year}. iznosi: {average_amount:F2}\n");
        }
        else
        {
            Console.Clear();
            Console.WriteLine($"Nema transakcija za {month}/{year}.\n");
        }

        Console.WriteLine("\nUnesite bilo sto za povratak ili '00' za pocetni izbornik\n");
        string user_choice = Console.ReadLine();
        Console.Clear();
        if (user_choice == "00")
        {
            Console.Clear();
            Main();
            return;
        }
    }


    static void AverageCategory(User person)
    {
        Console.Clear();

        Console.WriteLine("Dostupne kategorije: placa, honorar, poklon, stipendija, hrana, prijevoz, rezije, hobi");
        Console.Write("Unesite kategoriju: ");
        string category_input = Console.ReadLine().ToLower();

        string[] validCategories = { "placa", "honorar", "poklon", "stipendija", "hrana", "prijevoz", "rezije", "hobi" };
        while (!validCategories.Contains(category_input))
        {
            Console.WriteLine("Neispravan unos, pokusajte ponovno.\n");
            category_input = Console.ReadLine().ToLower();
        }

        var all_transactions = person.accounts.SelectMany(acc => acc.transactions.Cast<Transaction>()).ToList();

        var category_transactions = all_transactions.Where(t => t.category.ToLower() == category_input).ToList();

        if (category_transactions.Any())
        {
            double total_amount = category_transactions.Sum(t => t.amount);
            double average_amount = total_amount / category_transactions.Count;
            Console.Clear();
            Console.WriteLine($"prosjecan iznos transakcije u kategoriji '{category_input}' je: {average_amount:F2}\n");
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Nema transakcija u odabranoj kategoriji.\n");
        }

        Console.WriteLine("\nUnesite bilo sto za povratak ili '00' za pocetni izbornik\n");
        string user_choice = Console.ReadLine();
        Console.Clear();
        if (user_choice == "00")
        {
            Console.Clear();
            Main();
            return;
        }
    }
}