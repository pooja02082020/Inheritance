using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
abstract class Mail
{
    protected double weight;
    protected bool isExpress;
    protected string destinationAddress;

    public Mail(double weight, bool isExpress, string destinationAddress)
    {
        this.weight = weight;
        this.isExpress = isExpress;
        this.destinationAddress = destinationAddress;
    }

    public abstract double CalculatePostage();
    public abstract bool IsValid();
    public abstract string Display();
}

class Letter : Mail
{
    private string format;

    public Letter(double weight, bool isExpress, string destinationAddress, string format) : base(weight, isExpress, destinationAddress)
    {
        this.format = format;
    }

    public override double CalculatePostage()
    {
        if (!IsValid()) return 0.0; // Don't stamp invalid mails
        double baseFare = (format == "A4") ? 2.50 : 3.50;
        double amount = baseFare + weight * 0.001;
        if (isExpress)
            amount *= 2; // double the amount for express delivery
        return amount;
    }

    public override bool IsValid()
    {
        return !string.IsNullOrEmpty(destinationAddress);
    }

    public override string Display()
    {
        string express = (isExpress) ? "yes" : "no";
        double price = IsValid() ? CalculatePostage() : 0.0;
        return $"\nLetter\n\nWeight: {weight} grams\nExpress: {express}\nDestination: {destinationAddress}\nPrice: $ {price} Format: {format}\n";
    }
}

class Parcel : Mail
{
    private double volume;

    public double Volume // Adding Volume property
    {
        get { return volume; }
        set { volume = value; }
    }

    public Parcel(double weight, bool isExpress, string destinationAddress, double volume) : base(weight, isExpress, destinationAddress)
    {
        this.volume = volume;
    }

    public override double CalculatePostage()
    {
        if (!IsValid()) return 0.0; // Don't stamp invalid mails
        if (volume > 50.0 || volume <= 0) return 0.0; // Don't stamp parcels with volume exceeding 50 liters or less than/equal to 0 liters
        double amount = 0.25 * volume + weight * 0.001;
        if (isExpress)
            amount *= 2; // double the amount for express delivery
        return amount;
    }

    public override bool IsValid()
    {
        return !string.IsNullOrEmpty(destinationAddress);
    }

    public override string Display()
    {
        string express = (isExpress) ? "yes" : "no";
        double price = IsValid() ? CalculatePostage() : 0.0;
        return $"\nParcel\n\nWeight: {weight} grams\nExpress: {express}\nDestination: {destinationAddress}\nPrice: $ {price}\nVolume: {volume} liters\n";
    }
}

class Advertisement : Mail
{
    public Advertisement(double weight, bool isExpress, string destinationAddress) : base(weight, isExpress, destinationAddress)
    {
    }

    public override double CalculatePostage()
    {
        if (!IsValid()) return 0.0; // Don't stamp invalid mails
        double amount = 5.0 * weight * 0.001; // Convert grams to kilograms and calculate postage for advertisement
        if (isExpress)
            amount *= 2; // double the amount for express delivery
        return amount;
    }

    public override bool IsValid()
    {
        return !string.IsNullOrEmpty(destinationAddress);
    }

    public override string Display()
    {
        string express = (isExpress) ? "yes" : "no";
        double price = IsValid() ? CalculatePostage() : 0.0;
        return $"\nAdvertisement\n\nWeight: {weight} grams\nExpress: {express}\nDestination: {destinationAddress}\nPrice: $ {price}\n";
    }
}

class Mailbox
{
    private List<Mail> mails;

    public Mailbox()
    {
        mails = new List<Mail>();
    }

    public double CalculatePostage()
    {
        double totalPostage = 0.0;
        foreach (Mail mail in mails)
        {
            totalPostage += mail.CalculatePostage();
        }
        return totalPostage;
    }

    public int InvalidMails()
    {
        int count = 0;
        foreach (Mail mail in mails)
        {
            if (!mail.IsValid() || (mail is Parcel && ((Parcel)mail).Volume > 50.0))
            {
                count++;
            }
        }
        return count;
    }

    public void Display()
    {
        double totalPostage = CalculatePostage();
        Console.WriteLine($"The total amount of postage is {totalPostage} .\n");

        foreach (Mail mail in mails)
        {
            Console.WriteLine(mail.Display());
            if (!mail.IsValid())
                Console.WriteLine("(Invalid courier)");
        }

        int invalidMailCount = InvalidMails();
        Console.WriteLine($"\nThe box contains {invalidMailCount} invalid mails");
    }

    public void AddMail(Mail mail)
    {
        mails.Add(mail);
    }
}

class Program
{
    static void Main(string[] args)
    {
        Mailbox mailbox = new Mailbox();

        mailbox.AddMail(new Letter(200.0, true, "Chemin des Acacias 28, 1009 Pully", "A3"));
        mailbox.AddMail(new Letter(800.0, false, "", "A4"));
        mailbox.AddMail(new Advertisement(1500.0, true, "Les Moilles 13A, 1913 Saillon"));
        mailbox.AddMail(new Advertisement(3000.0, false, ""));
        mailbox.AddMail(new Parcel(5000.0, true, "Grand rue 18, 1950 Sion", 30.0));
        mailbox.AddMail(new Parcel(3000.0, true, "Chemin des fleurs 48, 2800 Delemont", 70.0));

        mailbox.Display();
    }
}
