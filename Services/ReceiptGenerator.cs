using System.Collections.Generic;
using System.Text;
using OrderingSystem.Models; // Adjust based on your actual namespace

public class ReceiptGenerator
{
    public string GenerateReceipt(IEnumerable<OrderedTShirt> orderedTShirts)
    {
        StringBuilder receipt = new StringBuilder();
        decimal grandTotal = 0;

        // Header
        receipt.AppendLine("T-Shirt Order Receipt");
        receipt.AppendLine("-------------------------------------------------");
        receipt.AppendLine("Product\t\tQuantity\tPrice\t\tTotal");
        receipt.AppendLine("-------------------------------------------------");

        // Iterate through ordered T-shirts and add them to the receipt
        foreach (var tShirt in orderedTShirts)
        {
            decimal lineTotal = tShirt.Quantity * tShirt.TotalPrice;
            grandTotal += lineTotal;

            receipt.AppendLine($"{tShirt.Product}\t\t{tShirt.Quantity}\t\t{tShirt.TotalPrice:C}\t\t{lineTotal:C}");
        }

        // Footer with grand total
        receipt.AppendLine("-------------------------------------------------");
        receipt.AppendLine($"Grand Total:\t\t\t\t\t{grandTotal:C}");

        return receipt.ToString();
    }
}