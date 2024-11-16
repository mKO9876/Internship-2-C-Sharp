using System.Globalization;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

Dictionary<int, Tuple<string, string, DateTime>> users_list = new();
Dictionary<(int, string), decimal> accounts_list = new();
Dictionary<(int, int, string), Tuple<decimal, bool, string, string, DateTime>> transactions_list = new();
//(USER ID, TRANSACTION_ID, ACCOUNT_NAME): (VALUE, IS_INCOME, CATEGORY, DESCRIPTION)

//data
users_list.Add(0, new Tuple<string, string, DateTime>("Ivan", "Horvat", new DateTime(2002, 1, 15)));
users_list.Add(1, new Tuple<string, string, DateTime>("Ana", "Kovačić", new DateTime(1992, 5, 10)));
users_list.Add(2, new Tuple<string, string, DateTime>("Marko", "Novak", new DateTime(1980, 2, 11)));

accounts_list.Add((0, "tekući"), 100.00m);
accounts_list.Add((0, "žiro"), 0.00m);
accounts_list.Add((0, "prepaid"), 0.00m);
accounts_list.Add((1, "tekući"), 100.00m);
accounts_list.Add((1, "žiro"), 0.00m);
accounts_list.Add((1, "prepaid"), 0.00m);
accounts_list.Add((2, "tekući"), 1100.50m);
accounts_list.Add((2, "žiro"), 200.00m);
accounts_list.Add((2, "prepaid"), 0.00m);

transactions_list.Add((2, 0, "tekući"), new Tuple<decimal, bool, string, string, DateTime>(1200.00m, true, "salary", "asdf", new DateTime(2022, 1, 1)));
transactions_list.Add((2, 1, "žiro"), new Tuple<decimal, bool, string, string, DateTime>(200.00m, true, "gift", "asdfghjklć", new DateTime(2002, 3, 10)));
transactions_list.Add((2, 2, "tekući"), new Tuple<decimal, bool, string, string, DateTime>(200.00m, false, "sport", "člkj", new DateTime(2002, 10, 2)));

//data
int next_user_id = 3;
int next_transaction_id = 3;
string[] acc_names = ["tekući", "žiro", "prepaid"];
List<string> income_categories = new List<string> { "food", "transport", "sport" };
List<string> expense_categories = new List<string> { "salary", "gift", "fee" };


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
    while (true)
    {
        Console.WriteLine("1 - Unos novog korisnika\n2 - Brisanje korisnika\n3 - Uređivanje korsnika\n4 - Pregled korsnika\n5 - Povratak");
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
                accounts_list[(next_user_id, "tekući")] = 100.00M;
                accounts_list[(next_user_id, "žiro")] = 0.00M;
                accounts_list[(next_user_id, "prepaid")] = 0.00M;
                next_user_id++;
                Console.WriteLine("Izrađen novi korisnik.\n");
            }
            else Console.WriteLine("Korisnik već posoji. \n");

        }


        else if (user_choice == "2")
        {
            while (true)
            {
                Console.WriteLine("1 - Brisanje po id-u \n2 - Brisanje po imenu i prezimenu\n3 - Povratak");
                user_choice = CheckUserInput("Akcija: ");

                if (user_choice == "2")
                {

                    string name = CheckUserInput("Unesite ime: ");
                    string last_name = CheckUserInput("Unesite prezime: ");
                    bool check_if_printed = false;

                    foreach (KeyValuePair<int, Tuple<string, string, DateTime>> user in users_list)
                    {
                        if (string.Equals(user.Value.Item1, name) && string.Equals(user.Value.Item2, last_name))
                        {
                            if (CheckUserInput("Želite li obrisati korisnika (y/n): ").ToLower() == "y")
                            {
                                RemoveAccounts(user.Key);
                                users_list.Remove(user.Key);
                                Console.WriteLine("Izbrisan korisnik.\n");
                                check_if_printed = true;
                                break;
                            }

                        }
                    }
                    if (!check_if_printed) Console.WriteLine("Nije pronađena osoba sa tim imenom i prezimenom. \n");

                }
                else if (user_choice == "1")
                {
                    while (true)
                    {
                        int user_id;
                        do
                        {
                            user_id = GetInt("Unesite ID: ");
                            if (!users_list.ContainsKey(user_id)) Console.WriteLine("Korisnik ne postoji, pokušajte ponovno.\n");
                        } while (!users_list.ContainsKey(user_id));

                        if (CheckUserInput("Želite li izbrisati korisnika (y/n): ").ToLower() == "y")
                        {
                            users_list.Remove(user_id);
                            Console.WriteLine("Izbrisan korisnik.\n");
                            break;
                        }
                    }
                }
                else if (user_choice == "3") break;
                else Console.WriteLine("Nepoznata naredba.\n");
            }
        }


        else if (user_choice == "3")
        {

            int user_id;
            do
            {
                user_id = GetInt("Unesite ID: ");
                if (!users_list.ContainsKey(user_id)) Console.WriteLine("Korisnik ne postoji, pokušajte ponovno.\n");
            } while (!users_list.ContainsKey(user_id));
            while (true)
            {
                string name = CheckUserInput("Unesite ime: ");
                string last_name = CheckUserInput("Unesite prezime: ");
                DateTime birth_date = GetBirthDay();
                if (GetUserId(name, last_name) == -1)
                {
                    Tuple<string, string, DateTime> edit_user = Tuple.Create(name, last_name, birth_date);
                    if (CheckUserInput("Želite li izmijeniti ovog korisnika (y/n): ").ToLower() == "y")
                    {
                        users_list[user_id] = edit_user;
                        Console.WriteLine("Uspješno izmijenjeni korsisnički podaci");
                        break;
                    }
                    else Console.WriteLine("Odbijeno.");
                }
                else Console.WriteLine("Ime i prezime se već koristi.\n");
            }


        }


        else if (user_choice == "4")
        {
            while (true)
            {
                if (users_list.Count == 0)
                {
                    Console.WriteLine("Trenutan broj korisnika je 0. Molim Vas unesite korisnike. \n");
                    return;
                }
                else
                {
                    Console.WriteLine("1 - Ispis abecedno \n2 - Ispis starijih od 30 godina\n3- Ispis svih kojima je barem jedan račun u minusu\n4- Povratak");
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
                            if (age > 30) Console.WriteLine($"{user.Key} - {user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3.ToString("dd-MM-yyy")}");
                        }
                        Console.WriteLine("\n");
                    }
                    else if (user_choice == "3")
                    {
                        bool check_if_printed = false;
                        foreach (var user in users_list)
                        {
                            if (accounts_list[(user.Key, acc_names[0])] < 0 || accounts_list[(user.Key, acc_names[1])] < 0 || accounts_list[(user.Key, acc_names[2])] < 0)
                            {
                                Console.WriteLine($"{user.Key} - {user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3.ToString("dd-MM-yyyy")}");
                                check_if_printed = true;
                            }
                        }
                        if (!check_if_printed) Console.WriteLine("Nema korisnika sa tim kriterijem.\n");
                        Console.WriteLine("\n");
                    }
                    else if (user_choice == "4") break;
                    else Console.WriteLine("Nepoznata naredba.\n");
                }
            }
        }

        else if (user_choice == "5") return;

        else Console.WriteLine("Nepoznat odgovor.\n");
    }
}


void Accounts()
{
    Console.WriteLine("Odabrali ste: Računi. ");
    if (users_list.Count == 0)
    {
        Console.WriteLine("Trenutan broj korisnika je 0.\n");
        return;
    }
    string name, last_name;
    do
    {
        name = CheckUserInput("Unesite ime: ");
        last_name = CheckUserInput("Unesite prezime: ");
        if (GetUserId(name, last_name) == -1) Console.WriteLine("Korisnik ne postoji, pokušajte ponovno.");
    } while (GetUserId(name, last_name) == -1);
    int user_id = GetUserId(name, last_name);

    string account_name = GetValidAccountName();
    Console.WriteLine("Račun pronađen.");
    while (true)
    {

        Console.WriteLine("1 - Unos nove transakcije\n2 - Brisanje transakcije\n3 - Uređivanje transakcije\n4 - Pregled transakcija\n5 - Financijsko izvješće\n6 - Povratak");
        string user_choice = CheckUserInput("Akcija: ");

        if (user_choice == "1")
        {
            while (true)
            {
                Console.WriteLine("1 - Trenutna transakcija\n2 - Zakazana transakcija\n3 - Povratak");
                user_choice = CheckUserInput("Akcija: ");
                if (user_choice == "1")
                {
                    MakeTransaction(true, user_id, account_name);
                    break;
                }
                else if (user_choice == "2")
                {
                    MakeTransaction(false, user_id, account_name);
                    break;
                }
                else if (user_choice == "3") break;
                else Console.WriteLine("Nepoznata naredba.\n");
            }
        }

        else if (user_choice == "2")
        {
            while (true)
            {
                Console.WriteLine("Brisanje:\n1 - po ID-u\n2 - ispod unesenog iznosa\n3 - iznad unesenog iznosa\n4 - svih prihoda\n5 - svih rashoda\n6 - svih transakcija za odabranu kategoriju\n7 - Povratak\n");
                user_choice = CheckUserInput("Akcija: ");
                List<(int, int, string)> delete_transactions = new();
                while (true)
                {
                    if (user_choice == "1")
                    {
                        while (true)
                        {
                            int transaction_id;
                            do
                            {
                                transaction_id = GetInt("Unesite ID transakcije: ");
                                if (!transactions_list.ContainsKey((user_id, transaction_id, account_name))) Console.WriteLine("Ne postoji transakcija sa tim ID-jem.\n");
                            } while (!transactions_list.ContainsKey((user_id, transaction_id, account_name)));

                            if (CheckUserInput("Želite li izbrisati transakciju (y/n): ").ToLower() == "y")
                            {
                                transactions_list.Remove((user_id, transaction_id, account_name));

                                Console.WriteLine("Uspješno obrisana transakcija");
                                break;

                            }
                            else Console.WriteLine("Odbijeno");
                        }
                    }
                    else if (user_choice == "2")
                    {
                        decimal value = GetDecimal("Unesite novčanu vrijednost: ");
                        foreach (var transaction in transactions_list)
                        {
                            if (transaction.Key.Item1 == user_id && transaction.Key.Item3 == account_name && transaction.Value.Item1 < value) delete_transactions.Add(transaction.Key);
                        }
                        DeleteTransactions(delete_transactions);
                    }
                    else if (user_choice == "3")
                    {
                        decimal value = GetDecimal("Unesite novčanu vrijednost: ");
                        foreach (var transaction in transactions_list)
                        {
                            if (transaction.Key.Item1 == user_id && transaction.Key.Item3 == account_name && transaction.Value.Item1 > value) delete_transactions.Add(transaction.Key);
                        }
                        DeleteTransactions(delete_transactions);
                    }
                    else if (user_choice == "4")
                    {
                        foreach (var transaction in transactions_list)
                        {
                            if (transaction.Key.Item1 == user_id && transaction.Key.Item3 == account_name && transaction.Value.Item2) delete_transactions.Add(transaction.Key);
                        }
                        DeleteTransactions(delete_transactions);
                    }
                    else if (user_choice == "5")
                    {
                        foreach (var transaction in transactions_list)
                        {
                            if (transaction.Key.Item1 == user_id && transaction.Key.Item3 == account_name && !transaction.Value.Item2) delete_transactions.Add(transaction.Key);
                        }
                        DeleteTransactions(delete_transactions);
                    }
                    else if (user_choice == "6")
                    {
                        string category = CheckCategory();
                        foreach (var transaction in transactions_list)
                        {
                            if (transaction.Key.Item1 == user_id && transaction.Key.Item3 == account_name && transaction.Value.Item3 == category) delete_transactions.Add(transaction.Key);
                        }
                        DeleteTransactions(delete_transactions);
                    }
                    else if (user_choice == "7") break;
                    else Console.WriteLine("Nepoznata naredba.\n");
                }
            }
        }

        else if (user_choice == "3")
        {
            int transaction_id;
            do
            {
                transaction_id = GetInt("Unesite ID transakcije: ");
                if (!transactions_list.ContainsKey((user_id, transaction_id, account_name)))
                    Console.WriteLine("Ne postoji transakcija sa tim ID-jem.\n");
            } while (!transactions_list.ContainsKey((user_id, transaction_id, account_name)));

            decimal value = transactions_list[(user_id, transaction_id, account_name)].Item1;
            string category = CheckCategory();
            string description = CheckUserInput("Unesite opis: ");

            DateTime transaction_date = GetValidTransactionDateTime(false);
            var transaction_detail = Tuple.Create(value, true, category, description, transaction_date);
            if (CheckUserInput("Želite li izmijeniti ovu transakciju? (y/n): ").ToLower() == "y")
            {
                transactions_list[(user_id, next_transaction_id, account_name)] = transaction_detail;
                Console.WriteLine("Uspješno izmijenjena transakcija.\n");
                break;
            }
            else Console.WriteLine("Odbijeno.");

        }

        else if (user_choice == "4")
        {
            while (true)
            {
                Console.WriteLine("Sve transakcije: \n1 - Kako su spremljene\n2 - Sortiranje po cijeni uzlazno\n3 - Sortiranje po cijeni silazno");
                Console.WriteLine("4 - Sortiranje po opisu abecedno\n5 - Sortiranje po datumu uzlazno\n6 - Sortiranje po datumu silazno\n7 - Svi prihodi");
                Console.WriteLine("8 - Svi rashodi\n9 - Na temelju odabrane kategorije\n10 - Na temelju tipa i odabrane kategorije\n11 - Povratak\n");
                user_choice = CheckUserInput("Akcija: ");

                List<Tuple<decimal, bool, string, string, DateTime>> filtered_list = new();

                foreach (var transaction in transactions_list)
                {
                    if (transaction.Key.Item1 == user_id && transaction.Key.Item3 == account_name) filtered_list.Add(transaction.Value);
                }
                if (user_choice == "1")
                {
                    ShowTransactions(filtered_list);
                    break;
                }
                else if (user_choice == "2")
                {
                    filtered_list.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                    break;
                }
                else if (user_choice == "3")
                {
                    filtered_list.Sort((x, y) => y.Item1.CompareTo(x.Item1));
                    break;
                }
                else if (user_choice == "4")
                {
                    filtered_list.Sort((x, y) => string.Compare(x.Item3, y.Item3, StringComparison.Ordinal));
                    break;
                }
                else if (user_choice == "5")
                {
                    filtered_list.Sort((x, y) => x.Item5.CompareTo(y.Item5));
                    break;
                }
                else if (user_choice == "6")
                {
                    filtered_list.Sort((x, y) => y.Item5.CompareTo(x.Item5));
                    break;
                }
                else if (user_choice == "7")
                {
                    List<Tuple<decimal, bool, string, string, DateTime>> extra_filtered_list = new();
                    foreach (var transaction in filtered_list)
                    {
                        if (transaction.Item2) extra_filtered_list.Add(transaction);
                    }
                    ShowTransactions(extra_filtered_list);
                    break;
                }
                else if (user_choice == "8")
                {
                    List<Tuple<decimal, bool, string, string, DateTime>> extra_filtered_list = new();
                    foreach (var transaction in filtered_list)
                    {
                        if (!transaction.Item2) extra_filtered_list.Add(transaction);
                    }
                    break;
                }
                else if (user_choice == "9")
                {
                    List<Tuple<decimal, bool, string, string, DateTime>> extra_filtered_list = new();
                    string category = CheckCategory();
                    foreach (var transaction in filtered_list)
                    {
                        if (transaction.Item3 == category) extra_filtered_list.Add(transaction);
                    }
                    ShowTransactions(extra_filtered_list);
                    break;
                }
                else if (user_choice == "10")
                {
                    string category = CheckCategory();
                    string type;
                    do
                    {
                        type = CheckUserInput("Unesite tip transakcije: ");
                    } while (type != "rashod" || type != "prihod");


                    foreach (var transaction in filtered_list)
                    {
                        List<Tuple<decimal, bool, string, string, DateTime>> extra_filtered_list = new();
                        if (type == "prihod")
                        {
                            if (transaction.Item3 == category && transaction.Item2) extra_filtered_list.Add(transaction);
                        }
                        else
                        {
                            if (transaction.Item3 == category && !transaction.Item2) extra_filtered_list.Add(transaction);
                        }
                        ShowTransactions(extra_filtered_list);
                        break;
                    }
                }
                else if (user_choice == "11") return;
                else
                {
                    Console.WriteLine("Nepoznata naredba.\n");
                }

                    ShowTransactions(filtered_list);
                }
        }
        else if (user_choice == "5")
        {
            Console.WriteLine("1 - Trenutno stanje računa\n2 - Broj ukupnih transakcija\n3 - Ukupan iznos prihoda i rashoda za odabrani mjesec i godinu\n4 - Postotak udjela rashoda za odabranu kategoriju\n5 - Prosječni iznos transakcije za odabrani mjesec i godinu\n6 - Prosječni iznos transakcije za odabranu kategoriju\n7 - Povratak");
            user_choice = CheckUserInput("Akcija: ");
            if (user_choice == "1") Console.WriteLine("Trenutno stanje: " + accounts_list[(user_id, account_name)]);
            else if (user_choice == "2")
            {
                int transaction_number = 0;
                decimal income = 0;
                decimal expenses = 0;
                if (transactions_list.Count > 0)
                {
                    foreach (var transaction in transactions_list)
                    {
                        if (transaction.Key.Item1 == user_id && transaction.Key.Item3 == account_name) transaction_number++;

                        if (transaction.Key.Item1 == user_id && transaction.Key.Item3 == account_name)
                        {
                            if (transaction.Value.Item2) income += transaction.Value.Item1;
                            else expenses += transaction.Value.Item1;
                        }
                    }
                    Console.WriteLine("Stanje: " + accounts_list[(user_id, account_name)]);
                    if (accounts_list[(user_id, account_name)] < 0) Console.WriteLine("U minusu ste!");
                    Console.WriteLine("Broj ukupnih transakcija: " + transaction_number);
                    Console.Write("Prihodi: " + income + ", Rashodi: " + expenses);
                }
                else Console.WriteLine("Nema transakcija na tom računu.\n");
            }
            else if (user_choice == "3")
            {
                decimal income = 0;
                decimal expenses = 0;
                DateTime month_year_filter;
                string user_input_month_year;
                do
                {
                    user_input_month_year = CheckUserInput("Unesite mjesec i godinu u formatu MM-YYYY: ");
                }
                while (!DateTime.TryParseExact(user_input_month_year, "MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out month_year_filter));

                foreach (var transaction in transactions_list)
                {
                    if (transaction.Key.Item1 == user_id && transaction.Key.Item3 == account_name && transaction.Value.Item5.Month == month_year_filter.Month && transaction.Value.Item5.Year == month_year_filter.Year)
                    {
                        if (transaction.Value.Item2) income += transaction.Value.Item1;
                        else expenses += transaction.Value.Item1;
                    }
                }
                Console.Write("Prihodi: " + income + ", Rashodi: " + expenses);
            }
            else if (user_choice == "4")
            {
                string category = CheckCategory();
                decimal expense_sum = 0M;
                decimal category_expense_sum = 0;
                foreach (var transaction in transactions_list)
                {
                    if (transaction.Key.Item1 == user_id && transaction.Key.Item3 == account_name && !transaction.Value.Item2)
                    {
                        expense_sum += transaction.Value.Item1;
                        if (transaction.Value.Item3 == category) category_expense_sum += transaction.Value.Item1;
                    }
                }
                Console.WriteLine("Rezultat: " + (category_expense_sum / expense_sum) * 100);
            }
            else if (user_choice == "5")
            {
                decimal transation_sum = 0;
                int transation_count = 0;
                DateTime month_year_filter;
                string user_input_month_year;
                do
                {
                    user_input_month_year = CheckUserInput("Unesite mjesec i godinu u formatu MM-YYYY: ");
                }
                while (!DateTime.TryParseExact(user_input_month_year, "MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out month_year_filter));

                foreach (var transaction in transactions_list)
                {
                    if (transaction.Key.Item1 == user_id && transaction.Key.Item3 == account_name && transaction.Value.Item5.Month == month_year_filter.Month && transaction.Value.Item5.Year == month_year_filter.Year)
                    {
                        transation_sum += transaction.Value.Item1;
                        transation_count++;
                    }
                }
                Console.Write("Prosjek: " + decimal.Round(transation_sum / transation_count, 2, MidpointRounding.AwayFromZero));
            }
            else if (user_choice == "6")
            {
                decimal transation_sum = 0;
                int transation_count = 0;
                string category = CheckCategory();

                foreach (var transaction in transactions_list)
                {
                    if (transaction.Key.Item1 == user_id && transaction.Key.Item3 == account_name && transaction.Value.Item3 == category)
                    {
                        transation_sum += transaction.Value.Item1;
                        transation_count++;
                    }
                }
                Console.Write("Prosjek: " + decimal.Round(transation_sum / transation_count, 2, MidpointRounding.AwayFromZero));
            }
            else if (user_choice == "7") return;
            else Console.WriteLine("Nepoznata naredba.\n");
        }
        else if (user_choice == "6") break;

        else Console.WriteLine("Nepoznata naredba.\n");
    }
}

//POMOĆNE FUNKCIJE


string CheckCategory()
{
    string category;
    do
    {
        category = CheckUserInput("Odaberite jednu od 6 kategorija (hrana, transport, sport, plaća, poklon, honorar): ");
    } while (!expense_categories.Contains(category) || !income_categories.Contains(category));
    return category;
}

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

decimal GetDecimal(string text)
{
    string word;
    decimal result;
    do
    {
        word = CheckUserInput(text);
    } while (!decimal.TryParse(word, out result) || result < 0);
    return decimal.Round(result, 2, MidpointRounding.AwayFromZero);
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

        else Console.WriteLine("Neispravan unos, pokušajte ponovno.\n");
    }
}

DateTime GetValidTransactionDateTime(bool fixed_date)
{
    DateTime today = DateTime.Now;
    if (fixed_date) return today;
    while (true)
    {
        string date_string = CheckUserInput("Unesite datum i vrijeme za izvršavanje transakcije u obliku (DD-MM-YYYY HH:MM:SS): ");
        if (DateTime.TryParse(date_string, out DateTime transaction_date))
        {
            if (transaction_date > today && transaction_date.Year < (today.Year + 5)) return transaction_date;
        }

        else Console.WriteLine("Neispravan unos, pokušajte ponovno.\n");
    }
}

string CheckUserInput(string print_task)
{
    while (true)
    {
        Console.Write(print_task);
        var user_input = Console.ReadLine();
        if (!string.IsNullOrEmpty(user_input)) return user_input;
        else Console.WriteLine("Neispravan unos, pokušajte ponovno.\n");
    }
}

void MakeTransaction(bool fixed_date, int user_id, string account_name)
{
    decimal value = GetDecimal("Unesite iznos: ");

    string category = CheckCategory();
    string description = CheckUserInput("Unesite opis: ");

    DateTime transaction_date = GetValidTransactionDateTime(fixed_date); //fixed_date depends on user (1 or 2 option)
    Tuple<decimal, bool, string, string, DateTime> transaction_detail = Tuple.Create(value, true, category, description, transaction_date);
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
        if (CheckUserInput("Želite li obrisati transakciju (y/n)").ToLower() == "y") transactions_list.Remove(transaction_key);
    }
    Console.WriteLine("Uspješno obrisano.\n");
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

void ShowTransactions(List<Tuple<decimal, bool, string, string, DateTime>> sortedList)
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

    else Console.WriteLine("Nisu pronađena podudaranja.\n");
}