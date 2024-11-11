using System;
using System.Collections;
using System.ComponentModel.Design;
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
    public ArrayList transactions;

    private static int last_transaction_id = 0;

    //constructor
    public User(int id, string name, string surname, DateTime birth_date)
    {
        this.id = id;
        this.name = name;
        this.surname = surname;
        this.birth_date = birth_date;

        accounts = new List<Account>
        {
            new Account("checking", 100.00),
            new Account("savings", 0.00),
            new Account("prepaid", 0.00)
        };

        transactions = new ArrayList();
        transactions.Add(new Transaction
        {
            id = 0,
            amount = 0.0,
            description = "Standardna transakcija",
            type = "",
            category = "",
            date = DateTime.Now,
        });
    }

    public void AddTransaction(double amount, string description, string type, string category)
    {
        int new_id = last_transaction_id++;

        transactions.Add(new Transaction
        {
            id = new_id,
            amount = amount,
            description = description,
            type = type,
            category = category,
            date = DateTime.Now
        });

    }
}

public struct Account
{
    public string name_acc;
    public double amount;

    public Account(string name_acc, double amount)
    {
        this.name_acc = name_acc;
        this.amount = amount;
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
        users.Add(new User(0, "Ana", "Anic", new DateTime(1990, 1, 1)));
        users[0].AddTransaction(100.0, "placa", "prihod", "placa");
        users[0].AddTransaction(50, "kupovina hrane", "rashod", "hrana");
        users[0].AddTransaction(20, "struja", "rashod", "rezije");

        users.Add(new User(1, "Marko", "Maric", new DateTime(2000, 5, 6))); 
        users[1].AddTransaction(200.0, "Bonus", "prihod", "stipendija");
        users[1].AddTransaction(70.0, "voda", "rashod", "rezije");
        users[1].AddTransaction(50.0, "nogomet", "rashod", "hobi");

        while (true)
        {
            Console.WriteLine("Izbornik");
            Console.WriteLine("1 - Korisnici");
            Console.WriteLine("2 - Racuni");
            Console.WriteLine("3 - Izlaz");

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
                    Console.WriteLine("neispravan odabir, pokusajte ponovno");
                    choice = Console.ReadLine();
                    break;
            }
        }
    }

    static void UserMenu()
    {
        Console.Clear();
        Console.WriteLine("1 - unos novog korisnika");
        Console.WriteLine("2 - brisanje korisnika");
        Console.WriteLine("3 - uredjivanje korisnika");
        Console.WriteLine("4 - pregled korisnika");
        Console.WriteLine("\n0 - povratak");

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
                Console.WriteLine("Nedopustena akcija, probajte ponovno\n");
                UserMenu();
                break;
        }
    }

    static void AddNewUser()
    {
        Console.Clear();

        Console.WriteLine("Unesite 'izlaz' za povratak: \n");
        Console.WriteLine("Unesite ime korisnika: ");
        string name = Console.ReadLine();

        if(name == "izlaz")
        {
            Console.Clear();
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
        return;
        Console.WriteLine("Korisnik uspjesno dodan\n\n");
    }

    static void DeleteUser()
    {
        Console.Clear();
        Console.WriteLine("1 - brisanje po id-u");
        Console.WriteLine("2 - brisanje po prezimenu");
        Console.WriteLine("\n0 - povratak");
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
                        return;
                        Console.WriteLine($"Korisnik {ToDelete.name} {ToDelete.surname} je izbrisan.\n\n");
                        validId = true;
                    }
                    else
                    {
                        Console.WriteLine("Korisnik s navedenim ID-em ne postoji.\n");
                    }
                }
                else
                {
                    Console.WriteLine("Unos nije validan. Unesite broj za ID.\n");
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
                    Console.WriteLine("Korisnik s navedenim prezimenom ne postoji.\n");
                }
            }
        }
        else if(choice == "0")
        {
            Console.Clear();
            return;
        }

        else
        {
            Console.WriteLine("Nepoznata operacija. Molimo pokušajte ponovo.\n");
        }

    }

    static void EditUser()
    {
        Console.Clear(); 
        
        Console.WriteLine("Unesite 'izlaz' za povratak, ili bilo sto drugo za nastavak: \n");
        string name = Console.ReadLine().ToLower();

        if (name == "izlaz")
        {
            Console.Clear();
            return;
        }
        Console.WriteLine("Uneiste ID korisnika");
        int id;
        bool valid_id = false;

        while (!valid_id)
        {
            if (int.TryParse(Console.ReadLine(), out id))
            {
                int userIndex = users.FindIndex(u => u.id == id);

                if (userIndex != -1)
                {
                    var userEdit = users[userIndex];

                    Console.WriteLine("Unesite novo ime korisnika: ");
                    userEdit.name = Console.ReadLine();

                    Console.WriteLine("Unesite novo prezime korisnika: ");
                    string lname = Console.ReadLine();

                    lname = Char.ToUpper(lname[0]) + lname.Substring(1).ToLower();

                    userEdit.surname = lname;

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

                    userEdit.birth_date = bdate;

                    users[userIndex] = userEdit;

                    Console.Clear();
                    Console.WriteLine("Korisnik uspješno uređjen.\n");
                    valid_id = true;
                }
                else
                {
                    Console.WriteLine("Korisnik s navedenim ID-em ne postoji. Pokušajte ponovo.");
                }
            }
            else
            {
                Console.WriteLine("Unos nije validan. Unesite broj za ID.");
            }
        }
    }

    static void ViewUsers()
    {
        Console.Clear();

        Console.WriteLine("1 - ispis svih korisnika po prezimenu");
        Console.WriteLine("2 - ispis svih korisnika starijih od 30 god");
        Console.WriteLine("3 - ispis svih korisnika s barem jednim racunom u minusu");
        Console.WriteLine("\n0 - povratak");

        string choice = Console.ReadLine().ToLower();

        if (choice == "1")
        {
            Console.WriteLine("\nPopis korisnika abeceno:");

            var sortedUsers = users.OrderBy(u => u.surname).ToList();

            Console.WriteLine("\nID\tIME\tPREZIME\t\tDATUM RODJENJA\n");
            foreach (var user in sortedUsers)
            {
                Console.WriteLine($"{user.id}\t{user.name}\t{user.surname}\t\t{user.birth_date.ToString("dd.MM.yyyy")}");
            }
            Console.WriteLine("\n\n");
        }

        else if (choice == "2")
        {
            Console.WriteLine("\nPopis korisnika starijih od 30:");

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
            Console.WriteLine("\n\n");
        }

        else if (choice == "3")
        {
            Console.WriteLine("\nPopis korisnika s barem jednim racunom u minusu:");

            var usersNegative = users.Where(u => u.accounts.Any(a => a.amount < 0)).ToList();

            Console.WriteLine("\nID\tIME\tPREZIME\t\tDATUM RODJENJA\n");
            foreach (var user in usersNegative)
            {
                Console.WriteLine($"{user.id}\t{user.name}\t{user.surname}\t\t{user.birth_date.ToString("dd.MM.yyyy")}");
            }
            Console.WriteLine("\n\n");
        }

        else if (choice == "0")
        {
            Console.Clear();
            return;
        }

    }


    //accounts
    static void AccountMenu()
    {
        Console.Clear();

        string fname = "";
        string lname = "";

        User curr_user = new User();

        while (true)
        {
            Console.WriteLine("Unesite ime korisnika");
            fname = Console.ReadLine();

            fname = Char.ToUpper(fname[0]) + fname.Substring(1).ToLower();

            Console.WriteLine("Unesite prezime korisnika");
            lname = Console.ReadLine();

            lname = Char.ToUpper(lname[0]) + lname.Substring(1).ToLower();

            curr_user = users.Find(u => u.name == fname && u.surname == lname);

            if (curr_user.id != -1)
            {
                break;
            }
            else
            {
                Console.WriteLine("Korisnik s tim imenom i prezimenom nije pronađen, pokušajte ponovo.\n");
            }
        }

        Console.Clear();

        Console.WriteLine("1 - unos nove transakcije");
        Console.WriteLine("2 - brisanje transakcije");
        Console.WriteLine("3 - uređivanje transakcije");
        Console.WriteLine("4 - pregled transakcija");
        Console.WriteLine("5 - financijsko izvješće");
        Console.WriteLine("\n0 - povratak na početni izbornik");

        string choice = Console.ReadLine();

        while (true)
        {
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
                    return;
                default:
                    Console.WriteLine("Nedopuštena radnja, pokušajte ponovo.");
                    break;
            }
        }
    }

    static void AddNewTransaction(User person)
    {
        Console.Clear();

        string choice;

        while (true)
        {
            Console.WriteLine("1 - Trenutno izvršena transakcija");
            Console.WriteLine("2 - Ranije izvršena transakcija");
            Console.WriteLine("\n0 - povratak");
            Console.WriteLine("\n\n");

            choice = Console.ReadLine().ToLower();

            if (choice == "1" || choice == "2")
            {
                break;
            }
            else if(choice == "0")
            {
                Console.Clear();
                AccountMenu();
            }
            else
            {
                Console.WriteLine("Pogresan unos, pokusajte ponovno.");
            }
        }

        // Declare and initialize
        double amount = 0;
        string description;
        string transaction_type = "";
        string catg = "";
        DateTime transaction_date = DateTime.Now;
        int id = person.transactions.Count + 1;

        Console.WriteLine("Odaberite tip transakcije:");
        Console.WriteLine("1 - Prihod");
        Console.WriteLine("2 - Rashod");
        Console.WriteLine("\n0 - povratak");
        Console.WriteLine("\n\n");

        string type_choice = Console.ReadLine().ToLower();

        string[] incomeCategories = { "placa", "honorar", "poklon", "stipendija" };
        string[] expenseCategories = { "hrana", "prijevoz", "rezije", "hobi" };

        // category input
        while (true)
        {
            if (type_choice == "1")
            {
                transaction_type = "prihod";
                while (true)
                {
                    Console.WriteLine("Odaberite kategoriju prihoda: placa, honorar, poklon, stipendija");
                    catg = Console.ReadLine().ToLower();

                    if (Array.Exists(incomeCategories, category => category == catg))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Nepoznata kategorija, pokušajte ponovno.\n");
                    }
                }
                break;
            }
            else if (type_choice == "2")
            {
                transaction_type = "rashod";
                while (true)
                {
                    Console.WriteLine("Odaberite kategoriju rashoda: hrana, prijevoz, režije, hobi");
                    catg = Console.ReadLine().ToLower();

                    if (Array.Exists(expenseCategories, category => category == catg))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Nepoznata kategorija, pokušajte ponovno.\n");
                    }
                }
                break;
            }
            else if(choice == "0")
            {
                Console.Clear();
                AccountMenu();
            }
            else
            {
                Console.WriteLine("Neispravan unos, pokusajte ponovno.\n");
                type_choice = Console.ReadLine().ToLower();
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
                Console.WriteLine("Pogrešan unos, pokušajte ponovno.\n");
            }
        }

        //transaction input
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
                    Console.WriteLine("Pogresan datum, pokusajte ponovno.\n");
                }
            }
        }

        // creating new transaction
        Transaction newTransaction = new Transaction()
        {
            id = id,
            amount = amount,
            description = description,
            category = catg,
            type = transaction_type,
            date = transaction_date,
        };

        person.transactions.Add(newTransaction);
        Console.Clear();
        Console.WriteLine("Transakcija uspjesno dodana.\n");
        return;
    }


    static void DeleteTransaction(User person)
    {
        Console.Clear();
        Console.WriteLine("Brisanje po:");
        Console.WriteLine("1 - ID-u");
        Console.WriteLine("2 - ispod unesenog iznosa");
        Console.WriteLine("3 - iznad unesenog iznosa");
        Console.WriteLine("4 - svih prihoda");
        Console.WriteLine("5 - svih rashoda");
        Console.WriteLine("6 - svih transakcija za odabranu kategoriju");
        Console.WriteLine("\n0 - povratak");

        string choice = Console.ReadLine();
        
        while (true)
        {
            Console.WriteLine("Jeste li sigurni da zelite obrisati transakciju? (yes/no)");
            string dilema = Console.ReadLine().ToLower();

            if (dilema == "no")
            {
                Console.WriteLine("Brisanje otkazano");
                AccountMenu();
            }
            if(dilema == "yes")
            {
                break;
            }
            else
            {
                Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
            }
        }
       

        while (true)
        {
            if (choice == "1")
            {
                Console.Clear();
                Console.WriteLine("Unesite ID transakcije");
                int id_tran;
                while (!int.TryParse(Console.ReadLine(), out id_tran) && id_tran < 0)
                {
                    Console.WriteLine("ID trazene transakcije ne postoji, pokusajte ponovno, id mora biti pozitivan");
                }

                var ToDelete = person.transactions.Cast<Transaction>().FirstOrDefault(t => t.id == id_tran);

                if (ToDelete.id != 0)
                {
                    person.transactions.Remove(ToDelete);

                    Console.Clear();
                    Console.WriteLine("Transakcija uspjesno obrisana!");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Transakcija s navedenim ID-em ne postoji");
                }
                break;
            }


            else if (choice == "2")
            {
                Console.Clear();
                Console.WriteLine("Unesite ispod kojeg iznosa se transakcije brisu: ");
                double amount_tran;
                if (!double.TryParse(Console.ReadLine(), out amount_tran) || amount_tran < 0)
                {
                    Console.WriteLine("iznos mora biti veci od nule! Unesite ponovno\n");
                }

                var ToDelete = person.transactions.Cast<Transaction>().Where(t => t.amount < amount_tran).ToList();

                if (ToDelete.Any())
                {
                    foreach (var transaction in ToDelete)
                    {
                        person.transactions.Remove(transaction);
                    }
                    Console.Clear();
                    Console.WriteLine("transakcije ispod unesenog iznosa su obrisane\n");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("nema transakcija ispod tog iznosa\n");
                }
                break;
            }


            else if (choice == "3")
            {
                Console.Clear();
                Console.WriteLine("Unesite iznad kojeg iznosa se transakcije brisu: ");
                double amount_tran;
                if (!double.TryParse(Console.ReadLine(), out amount_tran) || amount_tran < 0)
                {
                    Console.WriteLine("iznos mora biti veci od nule! Unesite ponovno\n");
                }

                var ToDelete = person.transactions.Cast<Transaction>().Where(t => t.amount > amount_tran).ToList();

                if (ToDelete.Any())
                {
                    foreach (var transaction in ToDelete)
                    {
                        person.transactions.Remove(transaction);
                    }
                    Console.Clear();
                    Console.WriteLine("Transakcije iznad unesenog iznosa su obrisane.\n");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Nema transakcija iznad tog iznosa\n");
                }
                break;
            }


            else if (choice == "4")
            {
                var income_tran = person.transactions.Cast<Transaction>().Where(t => t.type == "prihod").ToList();
                foreach (var transaction in income_tran)
                {
                    person.transactions.Remove(transaction);
                }
                Console.Clear();
                Console.WriteLine("Svi prihodi su obrisani\n");
                break;
            }

            else if (choice == "5")
            {
                var expense_tran = person.transactions.Cast<Transaction>().Where(t => t.type == "rashod").ToList();
                foreach (var transaction in expense_tran)
                {
                    person.transactions.Remove(transaction);
                }
                Console.Clear();
                Console.WriteLine("Svi rashodi su obrisani\n");
                break;
            }

            else if (choice == "6")
            {
                Console.WriteLine("Unesite kategoriju");
                Console.WriteLine("Kategorije:");
                Console.WriteLine("prihodi: placa, honorar, poklon, stipendija");
                Console.WriteLine("rashodi: hrana, prijevoz, rezije, hobi\n");
                string input = Console.ReadLine().ToLower();

                string[] validCategories = { "placa", "honorar", "poklon", "stipendija", "hrana", "prijevoz", "rezije", "hobi" };

                while (!validCategories.Contains(input))
                {
                    Console.WriteLine("Nepoznata kategorija, pokusajte ponovno\n");
                    input = Console.ReadLine();
                }

                var ToDelete = person.transactions.Cast<Transaction>().Where(t => t.category == input).ToList();

                if (ToDelete.Any())
                {
                    foreach (var transaction in ToDelete)
                    {
                        person.transactions.Remove(transaction);
                    }
                    Console.Clear();
                    Console.WriteLine($"Sve transakcije kategorije {input} su obrisane\n");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine($"Nema transakcija u kategoriji {input}\n");
                }

                break;

            }

            else if(choice == "0")
            {
                Console.Clear();
                AccountMenu();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("nedopustena radnja, pokusajte ponovno\n");
                choice = Console.ReadLine().ToLower();
            }
        }
    }

    static void EditTransaction(User person)
    {
        Console.Clear();

        Console.WriteLine("Unesite 'izlaz' za povratak");
        string input = Console.ReadLine();
        if(input == "izlaz")
        {
            AccountMenu();
        }

        int id;

        while (true)
        {
            Console.WriteLine("Unesite ID transakcije");
            if (int.TryParse(Console.ReadLine(), out id))
            {
                break;
            }
            else
            {
                Console.WriteLine("Neispravan unos, unesite cijeli broj za ID: \n");
            }
        }

        var edit_tran = person.transactions.Cast<Transaction>().FirstOrDefault(t => t.id == id);

        if (edit_tran.id <= 0)
        {
            Console.Clear();
            Console.WriteLine("Transakcija s navedenim ID-om ne postoji\n");
            AccountMenu();
        }


        Console.WriteLine("Jeste li sigurni da zelite urediti transakciju? (yes/no)");
        string confirmation = Console.ReadLine().ToLower();
        if (confirmation != "yes")
        {
            Console.Clear();
            Console.WriteLine("Uredjivanje otkazano\n");
            AccountMenu();
        }

        double new_amount;

        while (true)
        {
            Console.WriteLine("Unesite novi iznos transakcije: ");
            if (!double.TryParse(Console.ReadLine(), out new_amount) && new_amount > 0)
            {
                edit_tran.amount = new_amount;
                break;
            }
            else
            {
                Console.WriteLine("Neispravan unos, iznos mora biti pozitivan broj\n");
            }
        }

        //new descriptiom
        Console.WriteLine("Unesite novi opis transakcije");
        string new_description = Console.ReadLine();
        edit_tran.description = string.IsNullOrEmpty(new_description) ? "Standardna transakcija" : new_description;

        string[] validTypes = { "prihod", "rashod" };
        while (true)
        {
            Console.WriteLine("Unesite tip transakcije (prihod/rashod): ");
            string new_type = Console.ReadLine().ToLower();

            if (Array.Exists(validTypes, t => t == new_type))
            {
                edit_tran.type = new_type;
                break;
            }
            else
            {
                Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
            }
        }

        string[] incomeCategories = { "placa", "honorar", "poklon", "stipendija" };
        string[] expenseCategories = { "hrana", "prijevoz", "rezije", "hobi" };
        string[] available_categories;

        while (true)
        {
            Console.WriteLine("Unesite kategoriju transakcije:");
            if (edit_tran.type == "prihod")
            {
                available_categories = incomeCategories;
                Console.WriteLine("Dostupne kategorije za prihode");
            }
            else
            {
                available_categories = expenseCategories;
                Console.WriteLine("Dostupne kategorije za rashode");
            }

            foreach (string category in available_categories)
            {
                Console.WriteLine("- " + category);
            }

            Console.WriteLine("Unesite kategoriju");
            string new_category = Console.ReadLine().ToLower();

            if (Array.Exists(available_categories, category => category == new_category))
            {
                edit_tran.category = new_category;
                break;
            }
            else
            {
                Console.WriteLine("Neispravna kategorija, pokusajte ponovno.\n");
            }
        }

        edit_tran.date = DateTime.Now;

        Console.Clear();
        Console.WriteLine("Transakcija je uspješno uređena.\n");
        AccountMenu();
    }

    static void ViewTransactions(User person)
    {
        Console.Clear();
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

        string user_choice = Console.ReadLine();

        var transactions_for_display = person.transactions.Cast<Transaction>().ToList();

        string[] validCategories = { "placa", "honorar", "poklon", "stipendija", "hrana", "prijevoz", "rezije", "hobi" };

        switch (user_choice)
        {
            case "1":
                Console.Clear();
                Console.WriteLine("Sve transakcije:");
                break;

            case "2":
                transactions_for_display.Sort((t1, t2) =>
                {
                    return t1.amount.CompareTo(t2.amount);
                });
                Console.Clear();
                Console.WriteLine("Transakcije sortirane po iznosu (uzlazno):");
                break;

            case "3":
                transactions_for_display.Sort((t1, t2) =>
                {
                    return t2.amount.CompareTo(t1.amount);
                });
                Console.Clear();
                Console.WriteLine("Transakcije sortirane po iznosu (silazno):");
                break;

            case "4":
                transactions_for_display = transactions_for_display.OrderBy(t => t.description).ToList();
                Console.Clear();
                Console.WriteLine("Transakcije sortirane po opisu (abecedno):");
                break;

            case "5":
                transactions_for_display = transactions_for_display.OrderBy(t => t.date).ToList();
                Console.Clear();
                Console.WriteLine("Transakcije sortirane po datumu (uzlazno):");
                break;

            case "6":
                transactions_for_display = transactions_for_display.OrderByDescending(t => t.date).ToList();
                Console.Clear();
                Console.WriteLine("Transakcije sortirane po datumu (silazno):");
                break;

            case "7":
                transactions_for_display = transactions_for_display.Where(t => t.type.ToLower() == "prihod").ToList();
                Console.Clear();
                Console.WriteLine("Svi prihodi:");
                break;

            case "8":
                transactions_for_display = transactions_for_display.Where(t => t.type.ToLower() == "rashod").ToList();
                Console.Clear();
                Console.WriteLine("Svi rashodi:");
                break;

            case "9":
                string category_input;
                do
                {
                    Console.WriteLine("Unesite kategoriju (placa, honorar, poklon, stipendija, hrana, prijevoz, rezije, hobi):");
                    category_input = Console.ReadLine().ToLower();

                    if (!validCategories.Contains(category_input))
                    {
                        Console.WriteLine("neispravan unos, pokusajte ponovno\n");
                    }
                } while (!validCategories.Contains(category_input));

                
                transactions_for_display = transactions_for_display.Where(t => t.category.ToLower() == category_input).ToList();
                Console.Clear();
                Console.WriteLine($"Sve transakcije za kategoriju '{category_input}':");
                break;

            case "10":
                string type_input;
                do
                {
                    Console.WriteLine("Unesite tip transakcije (prihod/rashod):");
                    type_input = Console.ReadLine().ToLower();

                    if (type_input != "prihod" && type_input != "rashod")
                    {
                        Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
                    }

                } while (type_input != "prihod" && type_input != "rashod");

                do
                {
                    Console.WriteLine("Unesite kategoriju (placa, honorar, poklon, stipendija, hrana, prijevoz, rezije, hobi):");
                    category_input = Console.ReadLine().ToLower();

                    if (!validCategories.Contains(category_input))
                    {
                        Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
                    }

                } while (!validCategories.Contains(category_input));

                transactions_for_display = transactions_for_display.Where(t => t.type.ToLower() == type_input && t.category.ToLower() == category_input).ToList();
                Console.Clear();
                Console.WriteLine($"Sve transakcije za tip '{type_input}' i kategoriju '{category_input}':");
                break;

            case "0":
                Console.Clear();
                AccountMenu();
                break;
            default:
                Console.Clear();
                Console.WriteLine("Nedopustena akcija, probajte ponovno\n");
                ViewTransactions(person);
                break;
        }

        if (transactions_for_display.Count > 0)
        {
            Console.Clear();
            Console.WriteLine("\nTIP\tIZNOS\\tOPIS\t\tKATEGORIJA\t\tDATUM");
            foreach (var transaction in transactions_for_display)
            {
                Console.WriteLine($"{transaction.type}\t{transaction.amount}\t{transaction.description}\t{transaction.category}\t{transaction.date.ToString("dd.MM.yyyy")}");
            }
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Nema transakcija za prikaz\n");
            AccountMenu();
        }

    }

    static void FinancialStatement(User person)
    {
        Console.Clear();
        Console.WriteLine("Izbornik: Financijsko Izvjesce");
        Console.WriteLine("1 - trenutno stanje racuna:");
        Console.WriteLine("2 - broj ukupnih transakcija");
        Console.WriteLine("3 - ukupan iznos prihoda i rashoda za odabrani mjesec i godinu");
        Console.WriteLine("4 - postotak udjela rashoda za odabranu kategoriju");
        Console.WriteLine("5 - prosjecni iznos transakcije za odabrani mjesec i godinu");
        Console.WriteLine("6 - prosjecni iznos transakcije za odabranu kategoriju");
        Console.WriteLine("\n0 - povratak");

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
                return;
            default:
                Console.Clear();
                Console.WriteLine("Nedopustena akcija, probajte ponovno\n");
                FinancialStatement(person);
                return;
        }

    }

    static void CurrentBalance(User person)
    {
        double totalIncome = 0;
        double totalExpenses = 0;

        foreach (var item in person.transactions)
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
        Console.WriteLine($"Trenutno stanje računa: {balance:F2}");
    }

    static void NumberOfTransactions(User person)
    {
        int all_transactions = person.transactions.Count;
        Console.Clear();
        Console.WriteLine($"Ukupan broj transakcija: {all_transactions}");
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
                Console.WriteLine("neispravan unos, pokusajte ponovno\n");
            }
        }

        //year
        while (true)
        {
            Console.WriteLine("Unesite godinu: ");
            if (int.TryParse(Console.ReadLine(), out year) && year > 0)
            {
                break;
            }
            else
            {
                Console.WriteLine("neispravan unos, pokusajte ponovno\n");
            }
        }
        var month_transactions = new List<Transaction>();

        foreach (var item in person.transactions)
        {
            if (item is Transaction transaction)
            {
                if (transaction.date.Month == month && transaction.date.Year == year)
                {
                    month_transactions.Add(transaction);
                }
            }
        }

        double totalIncome = 0;
        double totalExpense = 0;

        foreach (var transaction in month_transactions)
        {
            if (transaction.type.ToLower() == "prihod")
            {
                totalIncome += transaction.amount;
            }
            else if (transaction.type.ToLower() == "rashod")
            {
                totalExpense += transaction.amount;
            }
        }

        Console.Clear();
        Console.WriteLine($"Ukupni prihodi: {totalIncome}");
        Console.WriteLine($"Ukupni rashodi: {totalExpense}");
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

        double total_expense = 0;
        foreach (Transaction t in person.transactions)
        {
            if (t.type.ToLower() == "rashod")
            {
                total_expense += t.amount;
            }
        }

        double category_expense = 0;
        foreach (Transaction t in person.transactions)
        {
            if (t.type.ToLower() == "rashod" && t.category.ToLower() == category_input)
            {
                category_expense += t.amount;
            }
        }

        double percentage = (total_expense > 0) ? (category_expense / total_expense) * 100 : 0;

        Console.Clear();
        Console.WriteLine($"Postotak udjela rashoda za kategoriju '{category_input}' iznosi: {percentage:F2}%");
        Console.WriteLine("\n\n");
        return;
    }



    static void AverageMonthYear(User person)
    {
        Console.Clear();

        Console.WriteLine("Unesite godinu za koju zelite izracunati prosjek transakcija: ");
        int year;
        while (!int.TryParse(Console.ReadLine(), out year) || year < 1)
        {
            Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
        }

        Console.WriteLine("Unesite mjesec za koji zelite izracunati prosjek transakcija (1-12):");
        int month;
        while (!int.TryParse(Console.ReadLine(), out month) || month < 1 || month > 12)
        {
            Console.WriteLine("Neispravan unos, pokusajte ponovno\n");
        }

        double totalAmount = 0;
        int count = 0;

        foreach (var item in person.transactions)
        {
            if (item is Transaction transaction && transaction.date.Year == year && transaction.date.Month == month)
            {
                totalAmount += transaction.amount;
                count++;
            }
        }

        if (count > 0)
        {
            double averageAmount = totalAmount / count;
            Console.Clear();
            Console.WriteLine($"Prosečan iznos transakcija za {month}/{year} iznosi: {averageAmount:F2}\n");
        }
        else
        {
            Console.Clear();
            Console.WriteLine($"Nema transakcija za navedeni mesec i godinu ({month}/{year}).\n");
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

        double totalAmount = 0;
        int count = 0;

        foreach (var item in person.transactions)
        {
            if (item is Transaction transaction && transaction.category.ToLower() == category_input)
            {
                totalAmount += transaction.amount;
                count++;
            }
        }

        if (count > 0)
        {
            double averageAmount = totalAmount / count;
            Console.Clear();
            Console.WriteLine($"Prosjecan iznos transakcije u kategoriji '{category_input}' je: {averageAmount:F2}\n");
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Nema transakcija u odabranoj kategoriji.\n");
        }
    }
}