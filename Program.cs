using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MeatCafe
{
    class Program
    {
        static string[] menuItems = {
            "Cheeseburger", "French Fries", "Coke Soft Drink", "Water",
            "Steak", "Ribs", "Wings", "Beer"
        };
        static double[] menuPrices = { 13.95, 4.50, 3.50, 1.50, 25.50, 18.00, 12.00, 6.00 };

        static List<string> orderNames = new List<string>();
        static List<double> orderPrices = new List<double>();
        static List<int> orderQuantities = new List<int>();

        static string currentOrderNumber = "";

        static void Main(string[] args)
        {
            Console.WriteLine("Monk's Cafe");
            Console.WriteLine("---");
            currentOrderNumber = GetInput("Enter your order number (or 0 to exit): ", true);
            if (currentOrderNumber == "0") Environment.Exit(0);

            while (true)
            {
                Console.WriteLine("\n1. Add Item");
                Console.WriteLine("2. Remove Item");
                Console.WriteLine("3. Add Tip");
                Console.WriteLine("4. Display Bill");
                Console.WriteLine("5. Clear All");
                Console.WriteLine("6. Save to file");
                Console.WriteLine("7. Load from file");
                Console.WriteLine("8. Exit");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddItemMenu(); break;
                    case "2": RemoveItem(); break;
                    case "3": AddTip(); break;
                    case "4": DisplayBill(); break;
                    case "5": ClearOrder(); break;
                    case "6": SaveToFile(); break;
                    case "7": LoadFromFile(); break;
                    case "8": return;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
        }

        static void AddItemMenu()
        {
            while (true)
            {
                Console.WriteLine("\nDescription        Price");
                Console.WriteLine("---");
                for (int i = 0; i < menuItems.Length; i++)
                    Console.WriteLine($"{i + 1}. {menuItems[i],-15} ${menuPrices[i],6:F2}");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                if (choice == "0") break;

                if (int.TryParse(choice, out int idx) && idx > 0 && idx <= menuItems.Length)
                {
                    AddItem(menuItems[idx - 1], menuPrices[idx - 1]);
                }
                else Console.WriteLine("Error: Item does not exist. Please try again.");
            }
        }

        static void AddItem(string name, double price)
        {
            int index = orderNames.IndexOf(name);
            if (index != -1) orderQuantities[index]++;
            else { orderNames.Add(name); orderPrices.Add(price); orderQuantities.Add(1); }
            Console.WriteLine($"{name} added successfully.");
        }

        static void RemoveItem()
        {
            if (orderNames.Count == 0) { Console.WriteLine("Order is empty, nothing to remove."); return; }
            DisplayOrder();
            Console.Write("Enter item number to remove (or 0 to back): ");
            string choice = Console.ReadLine();
            if (choice == "0") return;
            if (int.TryParse(choice, out int idx) && idx > 0 && idx <= orderNames.Count)
            {
                if (orderQuantities[idx - 1] > 1) orderQuantities[idx - 1]--;
                else { orderNames.RemoveAt(idx - 1); orderPrices.RemoveAt(idx - 1); orderQuantities.RemoveAt(idx - 1); }
                Console.WriteLine("Item updated.");
            }
            else Console.WriteLine("Invalid index.");
        }

        static void AddTip()
        {
            if (orderNames.Count == 0) { Console.WriteLine("Error: Cannot add tip to empty order."); return; }

            double netTotal = 0;
            for (int i = 0; i < orderNames.Count; i++) netTotal += orderPrices[i] * orderQuantities[i];

            bool validTip = false;
            double tip = 0;

            while (!validTip)
            {
                Console.Write($"\nNet Total: ${netTotal:F2}. Leave tip? (1 - Yes, 0 - No): ");
                string tipResponse = Console.ReadLine();

                if (tipResponse == "1")
                {
                    Console.Write("1 - Percent, 2 - Fixed amount: ");
                    string tipType = Console.ReadLine();

                    if (tipType != "1" && tipType != "2")
                    {
                        Console.WriteLine("Error: Invalid input. Please enter 1 for percent or 2 for fixed amount.");
                        continue;
                    }

                    Console.Write("Enter value: ");
                    string tipInput = Console.ReadLine();

                    if (!double.TryParse(tipInput, out double val) || val <= 0)
                    {
                        Console.WriteLine("Error: Invalid input. Tip must be a positive number greater than zero.");
                        continue;
                    }

                    tip = (tipType == "1") ? netTotal * (val / 100) : val;
                    validTip = true;
                }
                else if (tipResponse == "0")
                {
                    tip = 0;
                    validTip = true;
                }
                else
                {
                    Console.WriteLine("Error: Invalid input. Please enter 1 for Yes or 0 for No.");
                }
            }

            // Сохраняем чаевые как отдельный элемент заказа
            orderNames.Add("Tip");
            orderPrices.Add(tip);
            orderQuantities.Add(1);
            Console.WriteLine($"Tip of ${tip:F2} added successfully.");
        }

        static void DisplayBill()
        {
            if (orderNames.Count == 0) { Console.WriteLine("Order is empty."); return; }

            double netTotal = 0;
            for (int i = 0; i < orderNames.Count; i++)
            {

                if (orderNames[i] != "Tip")
                    netTotal += orderPrices[i] * orderQuantities[i];
            }


            double tip = 0;
            for (int i = 0; i < orderNames.Count; i++)
            {
                if (orderNames[i] == "Tip")
                {
                    tip = orderPrices[i] * orderQuantities[i];
                    break;
                }
            }

            double gst = netTotal * 0.05;
            double totalAmount = netTotal + tip + gst;

            Console.WriteLine("\nDescription     Price");
            Console.WriteLine("---");
            for (int i = 0; i < orderNames.Count; i++)
            {
                if (orderNames[i] != "Tip")
                    Console.WriteLine($"{orderNames[i],-15} ${orderPrices[i] * orderQuantities[i],6:F2}");
            }
            Console.WriteLine("---");
            Console.WriteLine($"Net Total    ${netTotal,6:F2}");
            Console.WriteLine($"Tip Amount   ${tip,6:F2}");
            Console.WriteLine($"Total GST    ${gst,6:F2}");
            Console.WriteLine($"Total Amount ${totalAmount,6:F2}");

            Console.WriteLine("\nPress 0 to return to Main Menu.");
            while (Console.ReadLine() != "0") { }
        }

        static void DisplayOrder()
        {
            if (orderNames.Count == 0) { Console.WriteLine("Order is empty."); return; }
            Console.WriteLine("\n--- Current Order ---");
            int displayIndex = 1;
            for (int i = 0; i < orderNames.Count; i++)
            {
                if (orderNames[i] != "Tip")
                {
                    Console.WriteLine($"{displayIndex}. {orderNames[i],-15} ({orderQuantities[i]}) {orderPrices[i] * orderQuantities[i],8:F2}$");
                    displayIndex++;
                }
            }
        }

        static void ClearOrder()
        {
            if (orderNames.Count == 0) { Console.WriteLine("Error: Order is already empty."); return; }
            orderNames.Clear(); orderPrices.Clear(); orderQuantities.Clear();
            Console.WriteLine("Order cleared successfully.");
        }

        static void SaveToFile()
        {
            if (orderNames.Count == 0) { Console.WriteLine("Error: Cannot save empty order."); return; }
            File.WriteAllLines($"Menu-{currentOrderNumber}.txt", orderNames.Select((n, i) => $"{n}|{orderQuantities[i]}|{orderPrices[i]}"));
            Console.WriteLine($"Saved to Menu-{currentOrderNumber}.txt");
        }

        static void LoadFromFile()
        {
            string num = GetInput("Enter order number to load: ", true);
            string filename = $"Menu-{num}.txt";
            if (File.Exists(filename))
            {
                orderNames.Clear(); orderPrices.Clear(); orderQuantities.Clear();
                var lines = File.ReadAllLines(filename);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    orderNames.Add(parts[0]);
                    orderQuantities.Add(int.Parse(parts[1]));
                    orderPrices.Add(double.Parse(parts[2]));
                }
                Console.WriteLine("\nFile loaded successfully:");
                DisplayOrder();
            }
            else Console.WriteLine("Error: File not found. You haven't saved that order yet.");
        }

        static string GetInput(string prompt, bool isOrderNum)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (isOrderNum && (string.IsNullOrEmpty(input) || input.Length > 10 || !input.All(char.IsDigit)))
                {
                    Console.WriteLine("Invalid input. Use digits only (max 10).");
                    continue;
                }
                return input;
            }
        }
    }
}