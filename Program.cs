using System.Globalization;
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
        user_choice = CheckUserInput("Akcija: ");

        if (user_choice == "2")
        {
            string name = CheckUserInput("Unesite ime: ");
            string last_name = CheckUserInput("Unesite prezime: ");

            foreach (KeyValuePair<int, Tuple<string, string, DateTime>> user in users_list)
            {
                if (string.Equals(user.Value.Item1, name) && string.Equals(user.Value.Item2, last_name))
                {
                    RemoveAccounts(user.Key);
                    users_list.Remove(user.Key);
                    Console.WriteLine("Izbrisan korisnik.\n");
                    return;
                }
            }
            Console.WriteLine("Nije pronađena osoba sa tim imenom i prezimenom. \n");
        }
        else if (user_choice == "1")
        {
            int user_id = GetInt("Unesite ID: ");
            RemoveAccounts(user_id);
            if (users_list.Remove(user_id)) Console.WriteLine("Izbrisan korisnik.\n");
            else Console.WriteLine("Nije pronađena osoba sa tim ID-jem\n");
        }
        else Console.WriteLine("Nepoznata akcija.\n");
    }


    else if (user_choice == "3")
    {
        if (users_list.Count == 0)
        {
            Console.WriteLine("Trenutan broj korisnika je 0. Molim Vas unesite korisnike.\n");
            return;
        }
        int user_id = GetInt("Unesite ID: ");
        if (users_list.ContainsKey(user_id))
        {
            string name = CheckUserInput("Unesite ime: ");
            string last_name = CheckUserInput("Unesite prezime: ");
            DateTime birth_date = GetBirthDay();
            if (GetUserId(name, last_name) == -1)
            {
                Tuple<string, string, DateTime> edit_user = Tuple.Create(name, last_name, birth_date);
                users_list[user_id] = edit_user;
            }
            else Console.WriteLine("Ime i prezime se već koristi, pokušajte ponovno.\n");
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
            user_choice = CheckUserInput("Akcija: ");

            if (user_choice == "1")
            {
                var dict_sorted = users_list.OrderByDescending(user => user.Value.Item2);
                foreach (var user in users_list)
                {
                    Console.WriteLine($"{user.Key} - {user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3.Date.ToString("dd-MM-yyyy")}");
                }
                Console.WriteLine("\n");
            }

            else if (user_choice == "2")
            {
                DateTime today = DateTime.Today;
                foreach (var user in users_list)
                {
                    int age = today.Year - user.Value.Item3.Year;
                    if (age > 30) Console.WriteLine($"{user.Key} - {user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3.ToString("dd-MM-YYYY")}");
                }
                Console.WriteLine("\n");
            }
            else if (user_choice == "3")
            {
                foreach (var user in users_list)
                {
                    if (accounts_list[(user.Key, acc_names[0])] < 0 || accounts_list[(user.Key, acc_names[1])] < 0 || accounts_list[(user.Key, acc_names[2])] < 0) 
                        Console.WriteLine($"{user.Key} - {user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3.ToString("dd-MM-YYYY")}");
                }
                Console.WriteLine("\n");
            }
            else Console.WriteLine("Nepoznata naredba. Pokušajte ponovno.\n");
        }
    }

    else Console.WriteLine("Nepoznat odgovor.\n");
}

void Accounts()
{
    Console.WriteLine("Odabrali ste: Računi. ");
    if(users_list.Count == 0)
    {
        Console.WriteLine("Trenutan broj korisnika je 0.\n");
        return;
    }

    string name = CheckUserInput("Unesite ime: ");
    string last_name = CheckUserInput("Unesite prezime: ");
    int user_id = GetUserId(name, last_name);

    if (user_id == -1)
    {
        Console.WriteLine("Korisnik ne postoji.\n");
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

    else if (user_choice == "4")
    {
        Console.WriteLine("Sve transakcije: \n1 - Kako su spremljene\n2 - Sortiranje po cijeni uzlazno\n3 - Sortiranje po cijeni silazno");
        Console.WriteLine("4 - Sortiranje po opisu abecedno\n5 - Sortiranje po datumu uzlazno\n6 - Sortiranje po datumu silazno\n7 - Svi prihodi");
        Console.WriteLine("8 - Svi rashodi\n9 - Na temelju odabrane kategorije\n10 - Na temelju tipa i odabrane kategorije");
        string next_user_choice = CheckUserInput("Akcija: ");

        List<Tuple<float, bool, string, string, DateTime>> sorted_list = new();

        foreach (var transaction in transactions_list)
        {
            if (transaction.Key.Item1 == user_id && transaction.Key.Item3 == account_name) sorted_list.Add(transaction.Value);
        }

        if (next_user_choice == "2") sorted_list.Sort((x, y) => x.Item1.CompareTo(y.Item1));
        else if (next_user_choice == "3") sorted_list.Sort((x, y) => y.Item1.CompareTo(x.Item1));
        else if (next_user_choice == "4") sorted_list.Sort((x, y) => string.Compare(x.Item3, y.Item3, StringComparison.Ordinal));
        else if (next_user_choice == "5") sorted_list.Sort((x, y) => x.Item5.CompareTo(y.Item5));
        else if (next_user_choice == "6") sorted_list.Sort((x, y) => y.Item5.CompareTo(x.Item5));

        else if (next_user_choice == "7")
        {
            List<Tuple<float, bool, string, string, DateTime>> filtered_list = new();
            foreach (var transaction in sorted_list)
            {
                if (transaction.Item2) filtered_list.Add(transaction);
            }
            ShowTransactions(filtered_list);
            return;
        }
        else if (next_user_choice == "8")
        {
            List<Tuple<float, bool, string, string, DateTime>> filtered_list = new();
            foreach (var transaction in sorted_list)
            {
                if (!transaction.Item2) filtered_list.Add(transaction);
            }
        }
        else if (next_user_choice == "9")
        {
            List<Tuple<float, bool, string, string, DateTime>> filtered_list = new();
            string category = CheckUserInput("Unesite kategoriju koju želite pregledati: ");
            foreach (var transaction in sorted_list)
            {
                if (transaction.Item3 == category) filtered_list.Add(transaction);
            }
            ShowTransactions(filtered_list);
            return;
        }
        else if (next_user_choice == "10")
        {
            string category = CheckUserInput("Unesite kategoriju transakcija: ");
            string type;
            do
            {
                type = CheckUserInput("Unesite tip transakcije: ");
            } while (type != "rashod" || type != "prihod");


            foreach (var transaction in sorted_list)
            {
                List<Tuple<float, bool, string, string, DateTime>> filtered_list = new();
                if (type == "prihod")
                {
                    if (transaction.Item3 == category && transaction.Item2) filtered_list.Add(transaction);
                }
                else
                {
                    if (transaction.Item3 == category && !transaction.Item2) filtered_list.Add(transaction);
                }
                ShowTransactions(filtered_list);
                return;
            }
        }
        else
        {
            Console.WriteLine("Ne postoji unesena opcija.\n");
            return;
        }

        ShowTransactions(sorted_list);
    }
    else if (user_choice == "5")
    {
        Console.WriteLine("1 - Trenutno stanje računa\n2 - Broj ukupnih transakcija\n3 - Ukupan iznos prihoda i rashoda za odabrani mjesec i godinu\n4 - Postotak udjela rashoda za odabranu kategoriju\n5 - Prosječni iznos transakcije za odabrani mjesec i godinu\n6 - Prosječni iznos transakcije za odabranu kategoriju\n");
        user_choice = CheckUserInput("Akcija: ");
        if (user_choice == "1") Console.WriteLine("Trenutno stanje: " + accounts_list[(user_id, account_name)]);
        else if (user_choice == "2")
        {
            int transaction_number = 0;
            if (transactions_list.Count > 0)
            {
                foreach (var transaction in transactions_list)
                {
                    if (transaction.Key.Item1 == user_id && transaction.Key.Item3 == account_name) transaction_number++;
                }
                Console.WriteLine("Broj ukupnih transakcija: " + transaction_number + "\n");
            }
            else Console.WriteLine("Nema transakcija na tom računu");
        }
        else if (user_choice == "3")
        {
            string user_input_month_year;
            DateTime month_year_filter;
            do
            {
                user_input_month_year = CheckUserInput("Unesite mjesec i godinu u formatu MM-YYYY: ");
            }
            while (!DateTime.TryParseExact(user_input_month_year, "MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out month_year_filter));


            float income = 0;
            float expenses = 0;
            foreach (var transaction in transactions_list)
            {
                if (transaction.Key.Item1 == user_id && transaction.Key.Item3 == account_name)
                {
                    if (transaction.Value.Item2) income += transaction.Value.Item1;
                    else expenses += transaction.Value.Item1;
                }
            }
            Console.Write("Prihodi: " + income + ", Rashodi: " + expenses);
        }
        else Console.WriteLine("Nepoznata akcija. Pokušajte ponovno.\n");
    }

    else Console.WriteLine("Nepoznata naredba.\n");
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
    if (delete_transactions.Count == 0)
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

void RemoveAccounts(int user_id)
{
    List<(int, string)> accounts_id = new() { (user_id, acc_names[0]), (user_id, acc_names[1]), (user_id, acc_names[2]) };
    List<(int, int, string)> transactions_id = new();
    foreach (var transaction in transactions_list)
    {
        if (transaction.Key.Item1 == user_id) transactions_id.Add(transaction.Key);
    }

    foreach (var transaction in transactions_id) transactions_list.Remove(transaction);
}

void ShowTransactions(List<Tuple<float, bool, string, string, DateTime>> sortedList)
{
    if (sortedList.Count == 0)
    {
        foreach (var transaction in sortedList)
        {
            string type = "prihod";
            if (transaction.Item2 == false) type = "rashod";
            Console.WriteLine($"{type} - {transaction.Item1} - {transaction.Item3} - {transaction.Item5.Date.ToString("dd-MM-yyyy")}");
        }
    }

    else Console.WriteLine("Nisu pronađena podudaranja");
}