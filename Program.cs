using System;
using System.IO;
using System.Text;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

class Program
{
    static void Main(string[] args)
    {
        // HLAVNÍ FIX: Registrace kódování. Bez balíčku 'System.Text.Encoding.CodePages' tohle spadne.
        try
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }
        catch (Exception e)
        {
            Console.WriteLine("CHYBA: Nemáš nainstalovaný balíček 'System.Text.Encoding.CodePages'!");
            Console.WriteLine(e.Message);
            Console.ReadKey();
            return;
        }

        string slozka = "C:\\Users\\kvese\\Downloads\\UDPP";//Directory.GetCurrentDirectory();
        string nazevVystupu = "!KOMPLET_SLOUCENO.pdf";
        string cestaVystupu = Path.Combine(slozka, nazevVystupu);

        Console.WriteLine($"Pracuji ve složce: {slozka}");

        try
        {
            var pdfSoubory = Directory.GetFiles(slozka, "*.pdf");
            Array.Sort(pdfSoubory);

            if (pdfSoubory.Length == 0)
            {
                Console.WriteLine("VAROVÁNÍ: Ve složce nejsou žádná PDFka!");
            }
            else
            {
                Console.WriteLine($"Nalezeno {pdfSoubory.Length} souborů.");

                using (PdfDocument outputDocument = new PdfDocument())
                {
                    int pocet = 0;
                    foreach (string file in pdfSoubory)
                    {
                        if (Path.GetFileName(file).Equals(nazevVystupu, StringComparison.OrdinalIgnoreCase)) continue;

                        try
                        {
                            using (PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import))
                            {
                                int count = inputDocument.PageCount;
                                for (int i = 0; i < count; i++)
                                {
                                    outputDocument.AddPage(inputDocument.Pages[i]);
                                }
                            }
                            Console.WriteLine($"[OK] {Path.GetFileName(file)}");
                            pocet++;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[CHYBA] {Path.GetFileName(file)}: {ex.Message}");
                        }
                    }

                    if (pocet > 0)
                    {
                        outputDocument.Save(cestaVystupu);
                        Console.WriteLine("\n-----------------------------");
                        Console.WriteLine("HOTOVO! Soubor byl vytvořen.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nKRITICKÁ CHYBA: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }

        // TOTO JE DŮLEŽITÉ: Aby se okno hned nezavřelo
        Console.WriteLine("\nStiskni libovolnou klávesu pro ukončení...");
        Console.ReadKey();
    }
}