using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Xml.Linq;

Dictionary<int, Tuple<string, string, DateTime>> user_list = new();
Dictionary<(int, string), float> account_list = new();
Dictionary<Tuple<int, int>, Tuple<float, bool, string, string, DateTime>> transaction_list = new();
//         id_korisnika, "tekući"          id_transakcije|(iznos, tip, kategorija, opis, datum provedbe)
//bool je za provjeriti je li prihod
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

bool Users()
{
    Console.WriteLine("Odabrali ste: Korisnici. ");
    Console.WriteLine("1 - Unos novog korisnika\n2 - Brisanje korisnika\n3 - Uređivanje korsnika\n4 - Pregled korsnika\n");
    string user_choice = CheckUserInput("Akcija: ");

    if (user_choice == "1")
    {
        while (true)
        {
            string name = CheckUserInput("Unesite ime: ");
            string last_name = CheckUserInput("Unesite prezime: ");

            DateTime birth_date = GetDate();
            if (UserExists(name, last_name) == -1)
            {
                Tuple<string, string, DateTime> new_user = Tuple.Create(name.ToLower(), last_name.ToLower(), birth_date);
                user_list[next_user_id] = new_user;
                //napravi mu 3 računa: prepaid, tekući (ima 100 eura) i žiro, svi ostali imaju 0
                account_list[(next_user_id, "tekući")] = 100f;
                account_list[(next_user_id, "žiro")] = 0f;
                account_list[(next_user_id, "prepaid")] = 0f;
                next_user_id++;
                Console.WriteLine("Izrađen novi korisnik.\nVraćanje na početak\n");
                return true;
            }
            Console.WriteLine("Korisnik već posoji.Pokušajte neko drugo ime. \n");
        }
    }


    else if (user_choice == "2")
    {
        if (user_list.Count == 0)
        {
            Console.WriteLine("Trenutan broj korisnika je 0. Molim Vas unesite korisnike.\n");
            return true;
        }

        while (true)
        {
            Console.WriteLine("1 - Brisanje po id-u \n2 - Brisanje po imenu i prezimenu\n");
            string task_delete = CheckUserInput("Akcija: ");

            if (task_delete == "2")
            {
                string name = CheckUserInput("Unesite ime: ");
                string last_name = CheckUserInput("Unesite prezime: ");

                foreach (KeyValuePair<int, Tuple<string, string, DateTime>> user in user_list)
                {
                    if (string.Equals(user.Value.Item1, name) && string.Equals(user.Value.Item2, last_name))
                    {
                        user_list.Remove(user.Key);
                        Console.WriteLine("Izbrisan korisnik.\n");
                        return true;
                    }
                }
                Console.WriteLine("Nije pronađena osoba sa tim imenom i prezimenom. \n");
            }



            else if (task_delete == "1")
            {
                string id = CheckUserInput("Unesite ID: ");

                foreach (KeyValuePair<int, Tuple<string, string, DateTime>> user in user_list)
                {
                    if (user.Key.ToString() == id)
                    {
                        user_list.Remove(user.Key);
                        Console.WriteLine("Izbrisan korisnik.\n");
                        return true;
                    }
                }
                Console.WriteLine("Nije pronađena osoba sa tim ID-jem\n");
                return true;
            }
            else Console.WriteLine("Nepoznata akcija. Pokušajte ponovno.\n");
        }
    }


    else if (user_choice == "3")
    {
        while (true)
        {
            string user_input = CheckUserInput("Unesite ID: ");

            if (Int32.TryParse(user_input, out int id))
            {
                if (user_list.ContainsKey(id))
                {
                    string name = CheckUserInput("Unesite ime: ");
                    string last_name = CheckUserInput("Unesite prezime: ");
                    DateTime birth_date = GetDate();
                    if (UserExists(name, last_name)>-1)
                    {
                        Tuple<string, string, DateTime> edit_user = Tuple.Create(name, last_name, birth_date);
                        user_list[id] = edit_user;
                        return true;
                    }
                    Console.WriteLine("Korisnik već postoji, pokušajte ponovno");
                }
                else Console.WriteLine("Ne postoji korisnik sa tim ID-jem.\n");
            }
            else Console.WriteLine("Nepoznata vrijednost. Pokušajte ponovno.\n");
        }

    }


    else if (user_choice == "4")
    {
        while (true)
        {
            if (user_list.Count == 0)
            {
                Console.WriteLine("Trenutan broj korisnika je 0. Molim Vas unesite korisnike. \n");
                return true;
            }
            else
            {
                Console.WriteLine("1 - Ispis abecedno \n2 - Ispis starijih od 30 godina\n3- Ispis svih kojima je barem jedan račun u minusu\n");
                string task_print = CheckUserInput("Akcija: ");

                if (task_print == "1")
                {
                    Console.WriteLine("Korisnici poredani abecedno: ");
                    var dict_sorted = user_list.OrderByDescending(user => user.Value.Item2);
                    foreach (KeyValuePair<int, Tuple<string, string, DateTime>> user in user_list)
                    {
                        Console.WriteLine($"{user.Key} - {user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3.Date.ToString("dd-MM-yyyy")}");
                    }
                    Console.WriteLine("\n");
                    return true;

                }

                else if (task_print == "2")
                {
                    DateTime today = DateTime.Today;
                    Console.WriteLine("Korisnici stariji od 30 godina: ");
                    foreach (KeyValuePair<int, Tuple<string, string, DateTime>> user in user_list)
                    {
                        int age = today.Year - user.Value.Item3.Year;
                        if (age > 30) Console.WriteLine($"{user.Key} - {user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3.ToString("dd-MM-YYYY")}");
                    }
                    Console.WriteLine("\n");
                    return true;
                }
                else Console.WriteLine("Nepoznata naredba. Pokušajte ponovno.\n");
            }

        }
    }


    else
    {
        Console.WriteLine("Nepoznat odgovor.\n");
        return true;
    }
}

bool Accounts()
{
    Console.WriteLine("Odabrali ste: Računi. ");
    string name = CheckUserInput("Unesite ime: ");
    string last_name = CheckUserInput("Unesite prezime: ");
    while (true)
    {
        int user_id = UserExists(name, last_name);
        if (user_id>-1)
        {
            Console.WriteLine("1 - Unos nove transakcije\n2 - Brisanje transakcije\n3 - Uređivanje transakcije\n4 - Pregled transakcija\n6 - Financijsko izvješće\n");
            string user_choice = CheckUserInput("Akcija: ");
            if (user_choice == "1")
            {
                string user_input_value = CheckUserInput("Unesite iznos: ");
                string category = CheckUserInput("Unesite kategoriju: ");
                string description = CheckUserInput("Unesite opis: ");

                string user_account = CheckUserInput("Račun na koji želite prenijeti novac (tekući, žiro, prepaid): ");

                if(float.TryParse(user_input_value, out float value) && acc_names.Contains(user_account))
                {
                    Tuple<float, bool, string, string, DateTime> transaction_detail = Tuple.Create(value, true, category, description, DateTime.Now);
                    Tuple<int, int> transation_key = Tuple.Create(user_id, next_transaction_id);
                    transaction_list[transation_key] = transaction_detail;
                    next_transaction_id++;
                    //uplati na račun
                    account_list[(user_id, user_account)] += value;
                    Console.WriteLine("Uspješno obacljena transakcija");
                }
                Console.WriteLine("Neuspješna transakcija, pokušajte ponovno.\n");
            }
            else if (user_choice == "2") { }
            else if (user_choice == "3") { }
            else if (user_choice == "4") { }
            else if (user_choice == "5") { }
            else Console.WriteLine("Nepoznata akcija. Pokušajte ponovno.\n");
        }
    }
}

//POMOĆNE FUNKCIJE

int UserExists(string name, string last_name)
{
    foreach (KeyValuePair<int, Tuple<string, string, DateTime>> user in user_list)
    {
        if (string.Equals(user.Value.Item1, name) && string.Equals(user.Value.Item2, last_name)) return user.Key;
    }
    return -1;
}


DateTime GetDate()
{
    DateTime today = DateTime.Today;
    while (true)
    {
        Console.Write("Unesite datum rođenja u obliku DD-MM-YYYY: ");
        string? date_string = Console.ReadLine();
        if (DateTime.TryParse(date_string, out DateTime birth_date))
        {
            if (birth_date.Year < today.Year && birth_date.Year > (today.Year - 100)) return birth_date;
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