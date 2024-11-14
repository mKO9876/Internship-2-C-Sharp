using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

Dictionary<int, Tuple<string, string, DateTime>> users_list = new();
Dictionary<(int, string), float> accounts_list = new();
Dictionary<(int, int, string), Tuple<float, bool, string, string, DateTime>> transactions_list = new();
//         (USER ID, TRANSACTION_ID, NAZIV RAČUNA): (VALUE, ISREVENUE, CATEGORY, DESCRIPTION)
int next_user_id = 0;
int next_transaction_id = 0;
string[] acc_names = ["tekući", "žiro", "prepaid"];

Console.WriteLine("DOBRODOŠLI");
while (true)
{
    Console.WriteLine("1 - Korisnici\n2 - Računi\n3 - Izraz iz aplikacije\n");
    string user_choice = CheckUserInput("Akcija: ");

    if (user_choice == "1") Users();
    else if (user_choice == "2") Accounts();
    else if (user_choice == "3") break;
    else Console.WriteLine("Nepoznat odgovor, pokušajte ponovno.\n");
}

void Users()
{
    Console.WriteLine("Odabrali ste: Korisnici. ");
    Console.WriteLine("1 - Unos novog korisnika\n2 - Brisanje korisnika\n3 - Uređivanje korsnika\n4 - Pregled korsnika\n");
    string user_choice = CheckUserInput("Akcija: ");

    if (user_choice == "1")
    {
        string name = CheckUserInput("Unesite ime: ");
        string last_name = CheckUserInput("Unesite prezime: ");

        DateTime birth_date = GetBirthDay();
        if (GetUserId(name, last_name) == -1)
        {
            Tuple<string, string, DateTime> new_user = Tuple.Create(name.ToLower(), last_name.ToLower(), birth_date);
            users_list[next_user_id] = new_user;
            accounts_list[(next_user_id, "tekući")] = 100.00f;
            accounts_list[(next_user_id, "žiro")] = 0.00f;
            accounts_list[(next_user_id, "prepaid")] = 0.00f;
            next_user_id++;
            Console.WriteLine("Izrađen novi korisnik.\nVraćanje na početak\n");
        }
        else Console.WriteLine("Korisnik već posoji. Vraćamo na početak \n");
    }


    else if (user_choice == "2")
    {
        if (users_list.Count == 0)
        {
            Console.WriteLine("Trenutan broj korisnika je 0. Molim Vas unesite korisnike.\n");
            return;
        }

        Console.WriteLine("1 - Brisanje po id-u \n2 - Brisanje po imenu i prezimenu\n");
        string task_delete = CheckUserInput("Akcija: ");

        if (task_delete == "2")
        {
            string name = CheckUserInput("Unesite ime: ");
            string last_name = CheckUserInput("Unesite prezime: ");

            foreach (KeyValuePair<int, Tuple<string, string, DateTime>> user in users_list)
            {
                if (string.Equals(user.Value.Item1, name) && string.Equals(user.Value.Item2, last_name))
                {
                    users_list.Remove(user.Key);
                    Console.WriteLine("Izbrisan korisnik.\n");
                    return;
                }
            }
            Console.WriteLine("Nije pronađena osoba sa tim imenom i prezimenom. \n");
        }



        else if (task_delete == "1")
        {
            int user_id = GetInt("Unesite ID: ");
            if (users_list.Remove(user_id)) Console.WriteLine("Izbrisan korisnik.\n");
            else Console.WriteLine("Nije pronađena osoba sa tim ID-jem\n");
        }
        else Console.WriteLine("Nepoznata akcija.\n");
    }


    else if (user_choice == "3")
    {
        int user_id = GetInt("Unesite ID: ");
        if (users_list.ContainsKey(user_id))
        {
            string name = CheckUserInput("Unesite ime: ");
            string last_name = CheckUserInput("Unesite prezime: ");
            DateTime birth_date = GetBirthDay();
            if (GetUserId(name, last_name) > -1)
            {
                Tuple<string, string, DateTime> edit_user = Tuple.Create(name, last_name, birth_date);
                users_list[user_id] = edit_user;
            }
            else Console.WriteLine("Korisnik već postoji, pokušajte ponovno.\n");
        }
        else Console.WriteLine("Ne postoji korisnik sa tim ID-jem.\n");

    }


    else if (user_choice == "4")
    {
        if (users_list.Count == 0)
        {
            Console.WriteLine("Trenutan broj korisnika je 0. Molim Vas unesite korisnike. \n");
            return;
        }
        else
        {
            Console.WriteLine("1 - Ispis abecedno \n2 - Ispis starijih od 30 godina\n3- Ispis svih kojima je barem jedan račun u minusu\n");
            string task_print = CheckUserInput("Akcija: ");

            if (task_print == "1")
            {
                Console.WriteLine("Korisnici poredani abecedno: ");
                var dict_sorted = users_list.OrderByDescending(user => user.Value.Item2);
                foreach (KeyValuePair<int, Tuple<string, string, DateTime>> user in users_list)
                {
                    Console.WriteLine($"{user.Key} - {user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3.Date.ToString("dd-MM-yyyy")}");
                }
                Console.WriteLine("\n");
            }

            else if (task_print == "2")
            {
                DateTime today = DateTime.Today;
                Console.WriteLine("Korisnici stariji od 30 godina: ");
                foreach (KeyValuePair<int, Tuple<string, string, DateTime>> user in users_list)
                {
                    int age = today.Year - user.Value.Item3.Year;
                    if (age > 30) Console.WriteLine($"{user.Key} - {user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3.ToString("dd-MM-YYYY")}");
                }
                Console.WriteLine("\n");
            }
            //fali 3
            else Console.WriteLine("Nepoznata naredba. Pokušajte ponovno.\n");
        }
    }

    else Console.WriteLine("Nepoznat odgovor.\n");
}

void Accounts()
{
    Console.WriteLine("Odabrali ste: Računi. ");

    string name = CheckUserInput("Unesite ime: ");
    string last_name = CheckUserInput("Unesite prezime: ");
    int user_id = GetUserId(name, last_name);

    if (user_id == -1)
    {
        Console.WriteLine("Korisnik ne postoji, vraćamo vas na poketak.\n");
        return;

    }
    string account_name = GetValidAccountName();
    Console.WriteLine("Račun pronađen.\n1 - Unos nove transakcije\n2 - Brisanje transakcije\n3 - Uređivanje transakcije\n4 - Pregled transakcija\n5 - Financijsko izvješće\n");
    string user_choice = CheckUserInput("Akcija: ");

    if (user_choice == "1")
    {
        Console.WriteLine("1 - Trenutna transakcija\n2 - Zakazana transakcija");
        string next_user_choice = CheckUserInput("Akcija: ");
        if (next_user_choice == "1") MakeTransaction(true, user_id, account_name);
        else MakeTransaction(false, user_id, account_name);
    }

    else if (user_choice == "2")
    {
        Console.WriteLine("Brisanje:\n1 - po ID-u\n2 - ispod unesenog iznosa\n3 - iznad unesenog iznosa\n4 - svih prihoda\n5 - svih rashoda\n6 - svih transakcija za odabranu kategoriju\n");
        string next_user_choice = CheckUserInput("Akcija: ");
        List<(int, int, string)> delete_transactions = new();
        if (next_user_choice == "1")
        {
            int transaction_id = GetInt("Unesite ID transakcije: ");
            if (transactions_list.Remove((user_id, transaction_id, account_name))) Console.WriteLine("Uspješno obrisana transakcija");
            else Console.WriteLine("Transakcija ne postoji.\n");
        }
        else if (next_user_choice == "2")
        {
            float value = GetFloat("Unesite vrijednost: ");
            foreach (var transaction in transactions_list)
            {
                if (transaction.Key.Item3 == account_name && transaction.Value.Item1 < value) delete_transactions.Add(transaction.Key);
            }
            DeleteTransactions(delete_transactions);
        }
        else if (next_user_choice == "3")
        {
            float value = GetFloat("Unesite vrijednost: ");
            foreach (var transaction in transactions_list)
            {
                if (transaction.Key.Item3 == account_name && transaction.Value.Item1 > value) delete_transactions.Add(transaction.Key);
            }
            DeleteTransactions(delete_transactions);
        }
        else if (next_user_choice == "4")
        {
            float value = GetFloat("Unesite vrijednost: ");
            foreach (var transaction in transactions_list)
            {
                if (transaction.Key.Item3 == account_name && transaction.Value.Item2) delete_transactions.Add(transaction.Key);
            }
            DeleteTransactions(delete_transactions);
        }
        else if (next_user_choice == "5")
        {
            float value = GetFloat("Unesite vrijednost: ");
            foreach (var transaction in transactions_list)
            {
                if (transaction.Key.Item3 == account_name && !transaction.Value.Item2) delete_transactions.Add(transaction.Key);
            }
            DeleteTransactions(delete_transactions);
        }
        else if (next_user_choice == "6")
        {
            string category = CheckUserInput("Unesite kategoriju: ");
            foreach (var transaction in transactions_list)
            {
                if (transaction.Key.Item3 == account_name && transaction.Value.Item3 == category) delete_transactions.Add(transaction.Key);
            }
            DeleteTransactions(delete_transactions);
        }
        else Console.WriteLine("Nepoznata akcija.\n");
    }

    else if (user_choice == "3")
    {
        int transaction_id = GetInt("Unesite ID transakcije: ");
        if (transactions_list[(user_id, transaction_id, account_name)] != null)
        {
            float value = transactions_list[(user_id, transaction_id, account_name)].Item1;
            string category = CheckUserInput("Unesite kategoriju: ");
            string description = CheckUserInput("Unesite opis: ");

            DateTime transaction_date = GetValidTransactionDateTime(false);
            var transaction_detail = Tuple.Create(value, true, category, description, transaction_date);
            transactions_list[(user_id, next_transaction_id, account_name)] = transaction_detail;
            next_transaction_id++;
            Console.WriteLine("Uspješno izmijenjena transakcija");
        }
    }

    else if (user_choice == "4") { return; }
    else if (user_choice == "5") { return; }
    else Console.WriteLine("Nepoznata akcija. Pokušajte ponovno.\n");
}

//POMOĆNE FUNKCIJE

int GetInt(string text)
{
    string word;
    int result;
    do
    {
        word = CheckUserInput(text);
    } while (!int.TryParse(word, out result));
    return result;
}

float GetFloat(string text)
{
    string word;
    float result;
    do
    {
        word = CheckUserInput(text);
    } while (!float.TryParse(word, out result));
    return result;
}

int GetUserId(string name, string last_name)
{
    foreach (KeyValuePair<int, Tuple<string, string, DateTime>> user in users_list)
    {
        if (string.Equals(user.Value.Item1, name) && string.Equals(user.Value.Item2, last_name)) return user.Key;
    }
    return -1;
}

string GetValidAccountName()
{
    string user_input;
    do
    {
        user_input = CheckUserInput("Unesite račun na koji želite vršiti izmjene: ");
    } while (!acc_names.Contains(user_input));

    return user_input;
}

DateTime GetBirthDay()
{
    DateTime today = DateTime.Today;
    while (true)
    {
        string date_string = CheckUserInput("Unesite datum rođenja u obliku DD-MM-YYYY: ");
        if (DateTime.TryParse(date_string, out DateTime birth_date))
        {
            if (birth_date.Year < today.Year && birth_date.Year > (today.Year - 100)) return birth_date;
        }

        else Console.WriteLine("Neispravan unos, pokušajte ponovno.");
    }
}

DateTime GetValidTransactionDateTime(bool fixed_date)
{
    DateTime today = DateTime.Now;
    if (fixed_date) return today;
    while (true)
    {
        string date_string = CheckUserInput("Unesite datum i vrijeme za izvršavanje transakcije u obliku DD-MM-YYYY HH:MM:SS : ");
        if (DateTime.TryParse(date_string, out DateTime transaction_date))
        {
            if (transaction_date > today && transaction_date.Year < (today.Year + 5)) return transaction_date;
        }

        else Console.WriteLine("Neispravan unos, pokušajte ponovno.");
    }
}

string CheckUserInput(string print_task)
{
    while (true)
    {
        Console.Write(print_task);
        var user_input = Console.ReadLine();
        if (!string.IsNullOrEmpty(user_input)) return user_input;
        else Console.WriteLine("Pogreška, pokušajte ponovno.\n");
    }
}

void MakeTransaction(bool fixed_date, int user_id, string account_name)
{
    float value = GetFloat("Unesite iznos: ");

    string category = CheckUserInput("Unesite kategoriju: ");
    string description = CheckUserInput("Unesite opis: ");

    DateTime transaction_date = GetValidTransactionDateTime(fixed_date); //fixed_date depends on user (1 or 2 option)
    Tuple<float, bool, string, string, DateTime> transaction_detail = Tuple.Create(value, true, category, description, transaction_date);
    transactions_list[(user_id, next_transaction_id, account_name)] = transaction_detail;
    next_transaction_id++;
    accounts_list[(user_id, account_name)] += value;
    Console.WriteLine("Uspješno obavljena transakcija");
}

void DeleteTransactions(List<(int, int, string)> delete_transactions)
{
    if(delete_transactions.Count == 0)
    {
        Console.WriteLine("Nije pronađena ni jedna transakcija sa tim parametrima.\n");
        return;
    }
    foreach (var transaction_key in delete_transactions)
    {
        transactions_list.Remove(transaction_key);
    }
    Console.WriteLine("Uspješno obrisano");
}