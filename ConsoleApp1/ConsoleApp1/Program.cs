using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

class Program
{
    static Dictionary<int, (string name, string surname, DateTime birth_date,
Dictionary<string, float> accounts)> users = new Dictionary<int, (string, string, DateTime, Dictionary<string, float>)>();

    static Dictionary<int, Dictionary<string, List<(int transaction_id, float transaction_amount,
        string transaction_description, string transaction_type, string transaction_category, DateTime transaction_date)>>> transactions =
        new Dictionary<int, Dictionary<string, List<(int, float, string, string, string, DateTime)>>>();

    static int last_transaction_id = 0;

    static void AddTransaction(int id, string account_name, float amount, string description, string type, string category)
    {
        if (!transactions.ContainsKey(id))
        {
            transactions[id] = new Dictionary<string, List<(int, float, string, string, string, DateTime)>>();
        }

        if (!transactions[id].ContainsKey(account_name))
        {
            transactions[id][account_name] = new List<(int, float, string, string, string, DateTime)>();
        }

        last_transaction_id++;

        var newTransaction = (
            last_transaction_id : last_transaction_id,
            transaction_amount : amount,
            transaction_description : description,
            transaction_type : type,
            transaction_category : category,
            transaction_date : DateTime.Now
            );

        transactions[id][account_name].Add(newTransaction);
        

        if (users.ContainsKey(id) && users[id].accounts.ContainsKey(account_name))
        {
            if(type == "prihod")
            {
                users[id].accounts[account_name] += amount;
            }
            else if (type == "rashod")
            {
                users[id].accounts[account_name] -= amount;
            }
        }
    }
    


    static void Main()
    {
        //some users
        //categories: placa, honorar, poklon, stipendija, hrana, rezije, prijevoz, hobi

        users[0] = (
            "Ana", "Anic", 
            new DateTime(1990, 5, 20), 
            new Dictionary<string, float>
            {
                {"checking", 100.0f },
                {"savings", 0.0f },
                {"prepaid", 0.0f }
            }
        );
        AddTransaction(0, "checking", 150.0f, "stigla placa", "prihod", "placa");
        AddTransaction(0, "savings", 200.0f, "umjetnina", "prihod", "honorar");
        AddTransaction(0, "prepaid", 150.0f, "fesb", "prihod", "stipendija");
        AddTransaction(0, "checking", 200.0f, "rodjendan", "prihod", "poklon");
        AddTransaction(0, "checking", 50.0f, "gorivo", "rashod", "prijevoz");
        AddTransaction(0, "checking", 50.0f, "kupovina hrane", "rashod", "hrana");
        AddTransaction(0, "checking", 50.0f, "voda", "rashod", "rezije");
        AddTransaction(0, "checking", 200.0f, "tenis", "rashod", "hobi");


        users[1] = (
            "Marko", "Maric",
            new DateTime(1990, 5, 20),
            new Dictionary<string, float>
            {
                {"checking", 100.0f },
                {"savings", 0.0f },
                {"prepaid", 0.0f }
            }
        );

        AddTransaction(1, "checking", 100.0f, "placa", "prihod", "placa");
        AddTransaction(1, "savings", 60.0f, "umjetnina", "prihod", "honorar");
        AddTransaction(1, "prepaid", 150.0f, "fesb", "prihod", "stipendija");
        AddTransaction(1, "checking", 50.0f, "rodjendan", "prihod", "poklon");
        AddTransaction(1, "checking", 150.0f, "gorivo", "rashod", "prijevoz");
        AddTransaction(1, "checking", 180.0f, "kupovina hrane", "rashod", "hrana");
        AddTransaction(1, "checking", 50.0f, "voda", "rashod", "rezije");
        AddTransaction(1, "checking", 250.0f, "tenis", "rashod", "hobi");



        Console.Clear();

        while (true)
        {
            Console.WriteLine("Izbornik\n");
            Console.WriteLine("1 - Korisnici");
            Console.WriteLine("2 - Racuni");
            Console.WriteLine("3 - Izlaz\n");

            string choice = Console.ReadLine()?.Trim();

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

        string name;
        while (true)
        {
            Console.WriteLine("Unesite ime korisnika: ");
            name = Console.ReadLine()?.Trim();

            if (name == "0")
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
            else if (name.All(char.IsDigit))
            {
                Console.Clear();
                Console.WriteLine("ime ne smije imati broj, pokusajate ponovno\n");
                continue; 
            }
            else
            {
                break;
            }
        }

        string surname;
        while (true)
        {
            Console.Write("Unesite prezime korisnika: ");
            surname = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(surname))
            {
                Console.WriteLine("neispravan unos, pokusajte ponovo.\n");
                continue;
            }
            break;
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

        int new_id = users.Count + 1;

        var newUser = (
            name: name,
            surname: surname,
            birth_date: birthdate,
            accounts : new Dictionary<string, float>
            {
                { "checking", 100.0f },  
                { "savings", 0.0f },    
                { "prepaid", 0.0f }
            }
        );

        users.Add(new_id, newUser);

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
                        if (users.ContainsKey(id))
                        {
                            var ToDelete = users[id];
                             users.Remove(id);

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

            // removing by surname
            else if (choice == "2")
            {
                string lname = "";
                bool validSurname = false;

                while (!validSurname)
                {
                    Console.WriteLine("Unesite prezime korisnika: ");
                    lname = Console.ReadLine().ToLower();

                    if (lname.Any(char.IsDigit))
                    {
                        Console.Clear();
                        Console.WriteLine("Neispravan unos! Prezime ne moze sadrzavati broj.\n");
                        continue;
                    }

                    var usersToDelete = users.Where(u => u.Value.surname.ToLower() == lname).ToList();

                    if (usersToDelete.Any())
                    {
                        foreach (var user in usersToDelete)
                        {
                            users.Remove(user.Key);

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

        bool valid_id = false;

        while (!valid_id)
        {
            Console.WriteLine("Uneiste ID korisnika");
            int id;

            if (int.TryParse(Console.ReadLine(), out id))
            {
                
                if (users.ContainsKey(id))
                {
                    var user_edit = users[id];

                    string fname;
                    while (true)
                    {
                        Console.WriteLine("Unesite novo ime korisnika: ");
                        fname = Console.ReadLine()?.Trim();

                        if (string.IsNullOrWhiteSpace(fname) || fname.Any(char.IsDigit))
                        {
                            Console.Clear();
                            Console.WriteLine("Neispravan unos! ime ne moze sadrzavati broj, niti biti prazno\n");
                            continue;
                        }

                        fname = Char.ToUpper(fname[0]) + fname.Substring(1).ToLower();
                        break;

                    }
                    user_edit.name = fname;

                    string lname;
                    while (true)
                    {
                        Console.WriteLine("Unesite novo prezime korisnika: ");
                        lname = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(lname) ||lname.Any(char.IsDigit))
                        {
                            Console.Clear();
                            Console.WriteLine("Neispravan unos! Prezime ne moze sadrzavati broj, niti biti prazno\n");
                            continue;
                        }
                        lname = Char.ToUpper(lname[0]) + lname.Substring(1).ToLower();
                        break;
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

                    users[id] = user_edit;

                    Console.Clear();
                    Console.WriteLine("Korisnik uspjesno uredjen.\n");
                    valid_id = true;
                }
                else
                {
                    Console.Clear();
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

                    var sortedUsers = users.OrderBy(u => u.Value.surname).ToList();

                    Console.WriteLine("\nID\tIME\tPREZIME\t\tDATUM RODJENJA\n");
                    foreach (var user in sortedUsers)
                    {
                        Console.WriteLine($"{user.Key}\t{user.Value.name}\t{user.Value.surname}\t\t{user.Value.birth_date.ToString("dd.MM.yyyy")}");
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
                        var age = DateTime.Now.Year - u.Value.birth_date.Year;
                        if (DateTime.Now.Month < u.Value.birth_date.Month || (DateTime.Now.Month == u.Value.birth_date.Month && DateTime.Now.Day < u.Value.birth_date.Day))
                        {
                            age--;
                        }
                        return age >= 30;
                    }).ToList();

                    Console.WriteLine("\nID\tIME\tPREZIME\t\tDATUM RODJENJA\n");
                    foreach (var user in usersOver30)
                    {
                        Console.WriteLine($"{user.Key}\t{user.Value.name}\t{user.Value.surname}\t\t{user.Value.birth_date.ToString("dd.MM.yyyy")}");
                    }
                    Console.WriteLine("\nUnesite bilo sto za povratak ili '00' za pocetni izbornik\n");
                    string input = Console.ReadLine();

                    if (input == "00")
                    {
                        Console.Clear();
                        //Main();
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
                        var negative_accounts = user.Value.accounts.Where(a => a.Value < 0).ToList();

                        if (negative_accounts.Any())
                        {
                            found_negative = true;
                            Console.WriteLine($"User: {user.Value.name} {user.Value.surname}");

                            foreach (var account in negative_accounts)
                            {
                                Console.WriteLine($"\tracun: {account.Key}, stanje: {account.Value}");
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

        bool user_found = false;
        int user_id = -1;

        while (true)
        {
            Console.WriteLine("Unesite '00' za pocetni izbornik, ili bilo sto drugo za nastavak: \n");
            string input = Console.ReadLine()?.Trim();

            if (input == "00")
            {
                Console.Clear();
                return;
            }

            Console.WriteLine("Unesite ime korisnika");
            fname = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(fname) || fname.Length < 2)
            {
                Console.Clear();
                Console.WriteLine("Ime ne moze biti prazno, pokusajte ponovo.\n");
                continue;
            }
            fname = Char.ToUpper(fname[0]) + fname.Substring(1).ToLower();

            Console.WriteLine("Unesite prezime korisnika");
            lname = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(lname) || lname.Length < 2)
            {
                Console.Clear();
                Console.WriteLine("Prezime ne moze biti prazno, pokusajte ponovo.\n");
                continue;
            }
            lname = Char.ToUpper(lname[0]) + lname.Substring(1).ToLower();

            foreach (var user in users)
            {
                if (user.Value.name.Equals(fname, StringComparison.OrdinalIgnoreCase) && user.Value.surname.Equals(lname, StringComparison.OrdinalIgnoreCase))
                {
                    user_id = user.Key; 
                    user_found = true;  
                    break;             
                }
            }

            if (!user_found) 
            {
                Console.Clear();
                Console.WriteLine("Korisnik s tim imenom i prezimenom nije pronadjen, pokusajte ponovo.\n");
            }
            else
            {
                var user_data = users[user_id];

                Console.Clear();
                while (true)
                {
                    Console.WriteLine("1 - Unos nove transakcije");
                    Console.WriteLine("2 - Brisanje transakcije");
                    Console.WriteLine("3 - Uredjivanje transakcije");
                    Console.WriteLine("4 - Pregled transakcija");
                    Console.WriteLine("5 - Financijsko izvjesce");
                    Console.WriteLine("\n0 - Povratak");
                    Console.WriteLine("00 - Povratak na pocetni izbornik\n");

                    string choice = Console.ReadLine()?.Trim();

                    switch (choice)
                    {
                        case "1":
                            AddNewTransaction(user_id);
                            break;
                        case "2":
                            DeleteTransaction(user_id);
                            break;
                        case "3":
                            EditTransaction(user_id);
                            break;
                        case "4":
                            ViewTransactions(user_id);
                            break;
                        case "5":
                            FinancialStatement(user_id);
                            break;
                        case "0":
                            Console.Clear();
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
    }


    static void AddNewTransaction(int id)
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
            else if (choice == "0")
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
                Console.WriteLine("Pogresan unos, pokusajte ponovno\n");
            }
        }

        string account_type;
        while (true)
        {
            Console.WriteLine("0 - povratak");
            Console.WriteLine("00 - pocetni izbornik\n");

            Console.WriteLine("Odaberite racun (checking, savings, prepaid): ");
            account_type = Console.ReadLine().ToLower().Trim();

            if (account_type == "0")
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

            if (string.IsNullOrWhiteSpace(account_type) || account_type.Any(char.IsDigit))
            {
                Console.Clear();
                Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
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


        if (!users[id].accounts.ContainsKey(account_type))
        {
            Console.Clear();
            Console.WriteLine("Račun nije pronađen\n");
            return;
        }

        double amount = 0;
        string description;
        string transaction_type = "";
        string catg = "";
        DateTime transaction_date = DateTime.Now;


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

        Console.WriteLine("Unesite opis transakcije (enter - standardna transakcija): ");
        description = Console.ReadLine();
        if (string.IsNullOrEmpty(description))
        {
            description = "Standardna transakcija";
        }

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

        AddTransaction(id, account_type, (float)amount, description, transaction_type, catg);

        Console.Clear();
        Console.WriteLine("Transakcija uspjesno dodana.\n");
        return;
    }


    static void DeleteTransaction(int id)
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

                foreach (var account in transactions[id])
                {

                    var to_delete = account.Value.FirstOrDefault(t => t.Item1 == id_tran);

                    if (to_delete != default)
                    {
                        account.Value.Remove(to_delete);
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

                foreach (var account in transactions[id])
                {
                    var to_delete = choice == "2"
                    ? account.Value.Where(t => t.transaction_amount < amount_tran).ToList()
                    : account.Value.Where(t => t.transaction_amount > amount_tran).ToList();

                    if (to_delete.Any())
                    {
                        foreach (var transaction in to_delete)
                        {
                            account.Value.Remove(transaction);
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

                foreach (var account in transactions[id])
                {
                    var transactions_to_delete = account.Value.Where(t => t.transaction_type == transaction_type).ToList();

                    foreach (var transaction in transactions_to_delete)
                    {
                        account.Value.Remove(transaction);
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

                foreach (var account in transactions[id])
                {
                    var transactions_to_delete = account.Value.Where(t => t.transaction_category == input).ToList();
                    if (transactions_to_delete.Any())
                    {
                        foreach (var transaction in transactions_to_delete)
                        {
                            account.Value.Remove(transaction);
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
                else if (again == "no")
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


    static void EditTransaction(int id)
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

        bool transaction_found = false;
        int transaction_id;

        while (true)
        {
            Console.WriteLine("Unesite ID transakcije:");
            if (!int.TryParse(Console.ReadLine(), out transaction_id) || transaction_id < 0)
            {
                Console.Clear();
                Console.WriteLine("Neispravan unos. ID mora biti pozitivan cijeli broj\n");
                continue;
            }

            foreach (var account in users[id].accounts)
            {
                var transaction = transactions[id].FirstOrDefault(accountTransaction => accountTransaction.Key == account.Key && accountTransaction.Value.Any(transactionDetail => transactionDetail.Item1 == transaction_id));
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

        var target_transaction = default((int, float, string, string, string, DateTime));
        string target_account = null;

        foreach (var account in users[id].accounts)
        {
            var transaction = transactions[id][account.Key].FirstOrDefault(t => t.Item1 == transaction_id);
            if (transaction.Item1 != 0)
            {
                target_transaction = transaction;
                target_account = account.Key;
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
        float new_amount;
        while (true)
        {
            Console.WriteLine("Unesite novi iznos transakcije: \n");
            if (!float.TryParse(Console.ReadLine(), out new_amount) || new_amount <= 0)
            {
                Console.Clear();
                Console.WriteLine("Neispravan unos, iznos mora biti pozitivan broj\n");
            }
            else
            {
                transactions[id][target_account].Remove(target_transaction);
                target_transaction = (target_transaction.Item1, new_amount, target_transaction.Item3, target_transaction.Item4, target_transaction.Item5, DateTime.Now);
                transactions[id][target_account].Add(target_transaction);

            }
            break;
        }

        Console.WriteLine("Unesite novi opis transakcije:\n");
        string new_description = Console.ReadLine();
        target_transaction = (target_transaction.Item1, target_transaction.Item2,
                      string.IsNullOrEmpty(new_description) ? target_transaction.Item3 : new_description,
                      target_transaction.Item4, target_transaction.Item5, DateTime.Now);


        string[] validTypes = { "prihod", "rashod" };
        while (true)
        {
            Console.WriteLine("Unesite tip transakcije (prihod/rashod): \n");
            string new_type = Console.ReadLine().ToLower();

            if (Array.Exists(validTypes, t => t == new_type))
            {
                target_transaction = (target_transaction.Item1, target_transaction.Item2, target_transaction.Item3, new_type, target_transaction.Item5, DateTime.Now);
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
            if (target_transaction.Item4 == "prihod")
            {
                available_categories = income_categories;
            }
            else
            {
                available_categories = expense_categories;
            }

            string new_category;

            while (true)
            {
                available_categories = (target_transaction.Item4 == "prihod") ? income_categories : expense_categories;
                Console.WriteLine("\nDostupne kategorije:");

                foreach (string category in available_categories)
                {
                    Console.WriteLine("- " + category);
                }

                Console.WriteLine("Unesite kategoriju:");
                new_category = Console.ReadLine().ToLower();

                if (Array.Exists(available_categories, category => category == new_category))
                {
                    target_transaction = (target_transaction.Item1, target_transaction.Item2,
                                          target_transaction.Item3, target_transaction.Item4,
                                          new_category, target_transaction.Item6);
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

        if (target_account != null)
        {
            var accountTransactions = transactions[id][target_account];
            for (int i = 0; i < accountTransactions.Count; i++)
            {
                if (accountTransactions[i].Item1 == transaction_id)
                {
                    accountTransactions[i] = target_transaction;
                    break;
                }
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


    static void ViewTransactions(int id)
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("Izbornik: Pregled transakcija\n");
            Console.WriteLine("1 - sve transakcije:");
            Console.WriteLine("2 - sortirane po iznosu uzlazno");
            Console.WriteLine("3 - sortirane po iznosu silazno");
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

            var transactions_for_display = transactions[id].SelectMany(account => account.Value).ToList();

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
                    transactions_for_display.Sort((t1, t2) => t1.Item2.CompareTo(t2.Item2));
                    Console.Clear();
                    Console.WriteLine("Transakcije sortirane po iznosu (uzlazno):");
                    sort_name = "Transakcije po iznosu uzlazno";
                    break;

                case "3":
                    transactions_for_display.Sort((t1, t2) => t2.Item2.CompareTo(t1.Item2));
                    Console.Clear();
                    Console.WriteLine("Transakcije sortirane po iznosu (silazno):");
                    break;

                case "4":
                    transactions_for_display = transactions_for_display.OrderBy(t => t.Item3).ToList();
                    Console.Clear();
                    Console.WriteLine("Transakcije sortirane po opisu (abecedno):");
                    sort_name = "Transakcije po iznosu silazno";
                    break;

                case "5":
                    transactions_for_display = transactions_for_display.OrderBy(t => t.Item5).ToList();
                    Console.Clear();
                    Console.WriteLine("Transakcije sortirane po datumu (uzlazno):");
                    sort_name = "Transakcije po datumu uzlazno";
                    break;

                case "6":
                    transactions_for_display = transactions_for_display.OrderByDescending(t => t.Item5).ToList();
                    Console.Clear();
                    Console.WriteLine("Transakcije sortirane po datumu (silazno):");
                    sort_name = "Transakcije po iznosu silazno";
                    break;

                case "7":
                    transactions_for_display = transactions_for_display.Where(t => t.Item4.ToLower() == "prihod").ToList();
                    Console.Clear();
                    Console.WriteLine("Svi prihodi:");
                    sort_name = "Svi prihodi";
                    break;

                case "8":
                    transactions_for_display = transactions_for_display.Where(t => t.Item4.ToLower() == "rashod").ToList();
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
                            Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
                        }
                    } while (!validCategories.Contains(category_input));

                    transactions_for_display = transactions_for_display.Where(t => t.Item4.ToLower() == category_input).ToList();
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

                    } while (!validCategories.Contains(category_input_));

                    transactions_for_display = transactions[id]
                        .SelectMany(account => account.Value)  // Flatten the list of transactions
                        .Where(t => t.Item1.ToString().ToLower() == type_input.ToLower() && t.Item4.ToString().ToLower() == category_input_.ToLower())
                        .ToList();
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
                    string description = transaction.Item3.Length > 30 ? transaction.Item3.Substring(0, 30) : transaction.Item3;
                    string dateFormatted = transaction.Item6.ToString("dd.MM.yyyy"); 

                    Console.WriteLine($"{transaction.Item4,-10} {transaction.Item2,10:F2} {description,-30} {transaction.Item5,-15} {dateFormatted,10}");
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


    static void FinancialStatement(int user_id)
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
                    CurrentBalance(user_id);
                    break;

                case "2":
                    NumberOfTransactions(user_id);
                    break;

                case "3":
                    IncomeAndExpensesDate(user_id);
                    break;

                case "4":
                    PercentageCategory(user_id);
                    break;

                case "5":
                    AverageMonthYear(user_id);
                    break;

                case "6":
                    AverageCategory(user_id);
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

    static void CurrentBalance(int user_id)
    {
        double totalIncome = 0;
        double totalExpenses = 0;

        foreach (var account in users[user_id].accounts)
        {
            string accountName = account.Key;
            var accountTransactions = transactions[user_id][accountName];

            foreach (var transaction in accountTransactions)
            {
                if (transaction.transaction_type.ToLower() == "prihod")
                {
                    totalIncome += transaction.transaction_amount;
                }
                else if (transaction.transaction_type.ToLower() == "rashod")
                {
                    totalExpenses += transaction.transaction_amount;
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

    static void NumberOfTransactions(int user_id)
    {
        int total = 0;

        foreach (var account in users[user_id].accounts)
        {
            string accountName = account.Key;
            total += transactions[user_id][accountName].Count;
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

    static void IncomeAndExpensesDate(int user_id)
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

        double total_income = 0;
        double total_expenses = 0;

        foreach (var account in users[user_id].accounts)
        {
            string accountName = account.Key;
            var accountTransactions = transactions[user_id][accountName];

            foreach (var transaction in accountTransactions)
            {
                if (transaction.transaction_date.Month == month && transaction.transaction_date.Year == year)
                {
                    if (transaction.transaction_type.ToLower() == "prihod")
                    {
                        total_income += transaction.transaction_amount;
                    }
                    else if (transaction.transaction_type.ToLower() == "rashod")
                    {
                        total_expenses += transaction.transaction_amount;
                    }
                }
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

    static void PercentageCategory(int user_id)
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

        double total_expense = 0;
        double category_expense = 0;

        foreach (var account in users[user_id].accounts)
        {
            string accountName = account.Key;
            var accountTransactions = transactions[user_id][accountName];

            foreach (var transaction in accountTransactions)
            {
                if (transaction.transaction_type.ToLower() == "rashod")
                {
                    total_expense += transaction.transaction_amount;

                    if (transaction.transaction_category.ToLower() == category_input)
                    {
                        category_expense += transaction.transaction_amount;
                    }
                }
            }
        }
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



    static void AverageMonthYear(int user_id)
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


        double total_amount = 0;
        int transaction_count = 0;

        foreach (var account in users[user_id].accounts)
        {
            string accountName = account.Key;
            var accountTransactions = transactions[user_id][accountName];

            foreach (var transaction in accountTransactions)
            {
                if (transaction.transaction_date.Year == year && transaction.transaction_date.Month == month)
                {
                    total_amount += transaction.transaction_amount;
                    transaction_count++;
                }
            }
        }

        if (transaction_count > 0)
        {
            double averageAmount = total_amount / transaction_count;
            Console.Clear();
            Console.WriteLine($"Prosjecan iznos transakcija za {month}/{year}. iznosi: {averageAmount:F2}\n");
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


    static void AverageCategory(int user_id)
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

        double total_amount = 0;
        int transaction_count = 0;

        foreach (var account in users[user_id].accounts)
        {
            string accountName = account.Key;
            var accountTransactions = transactions[user_id][accountName];

            foreach (var transaction in accountTransactions)
            {
                if (transaction.transaction_category.ToLower() == category_input)
                {
                    total_amount += transaction.transaction_amount;
                    transaction_count++;
                }
            }
        }

        if (transaction_count > 0)
        {
            double averageAmount = total_amount / transaction_count;
            Console.Clear();
            Console.WriteLine($"Prosjecan iznos transakcije u kategoriji '{category_input}' je: {averageAmount:F2}\n");
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
